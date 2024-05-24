using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Repositories.Repositories
{
    public class UserRepository: GenericRepository<Users>
    {
        private readonly ISession Session;
        public UserRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        #region User LogIn Validating & Mapping        
        public UserModel ValidateUser(string LoginId, string Password)
        {

            string EncPassword = Encryptdata(Password);
            var user = _DbContext.User.Where(x => x.UserName == LoginId && x.Password == EncPassword && x.IsDeleted == 0).FirstOrDefault();
            if (user != null)
            {
                var userModel = Map(user);
                var roles = _DbContext.User.Where(x => x.EmpId == userModel.EmpId).ToList();
                userModel.Roles = Map(roles);
                return userModel;
            }
            return null;
        }
        public UserModel Map(Users usr)
        {
            return new UserModel()
            {
                UserName = usr.UserName,
                FullName = usr.FirstName + " " + usr.LastName,
                EmpId = usr.EmpId,
                UserRoleID = usr.UserRoleID,
            };
        }
        public List<int> Map(List<Users> roles)
        {
            return roles.Select(x => x.UserRoleID).ToList();

        }
        public string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                byte[] encode = new byte[password.Length];
                encode = Encoding.UTF8.GetBytes(password);
                strmsg = Convert.ToBase64String(encode);
            }
            return strmsg;
        }
        public string Decryptdata(string encryptpwd) //Pass encrypted password here 
        {
            string decryptpwd;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
        #endregion
    }
}
