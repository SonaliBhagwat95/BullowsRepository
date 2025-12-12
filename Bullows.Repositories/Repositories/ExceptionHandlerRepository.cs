using Bullows.Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Repositories.Repositories
{
    public class ExceptionHandlerRepository : GenericRepository<ExceptionHandler>
    {
        private readonly ISession Session;
        public ExceptionHandlerRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;

        }
        public void SaveException(string Classname,string methodName,string Error)
        {
            ExceptionHandler obj = new ExceptionHandler();
            obj.Classname = Classname;
            obj.Methodname = methodName;
            obj.Error = Error;
            obj.CreatedDate = DateTime.Now;
            obj.UserId = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            this._DbContext.ExceptionHandler.Add(obj);
            try
            {
                this._DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during save
                // You might want to log this exception or take other actions
                Console.WriteLine($"Error saving exception: {ex.Message}");
            }
        }
  
    }
}
