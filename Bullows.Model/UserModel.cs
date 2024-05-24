using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class UserModel:BaseModel
    {
        public int EmpId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        [Required]
        public int UserRoleID { get; set; }
        public List<int> Roles { get; set; }
        public string FullName { get; set; }
    }
}
