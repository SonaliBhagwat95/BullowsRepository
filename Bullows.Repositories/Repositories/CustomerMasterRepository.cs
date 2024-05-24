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
    public class CustomerMasterRepository: GenericRepository<CustomerMaster>
    {
        private readonly ISession Session;
        public CustomerMasterRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public int saveCustomerDetails(EnquiryModel model, CustomerMaster tblobj, int flag)
        {
                    tblobj = new CustomerMaster();
                    tblobj.CompanyName = model.CompanyName;
                    tblobj.CustomerAddress = model.CustomerAddress;
                    tblobj.Designation = model.Designation;
                    tblobj.StateId = model.StateId;
                    tblobj.DistrictId = model.DistrictId;
                    tblobj.CityId = model.CityId;
                    tblobj.Pin = model.Pin;
                    tblobj.PAN = model.PAN;
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.IsDeleted = false;
                    tblobj.ModifiedBy = 0;
                    _DbContext.CustomerMasters.Add(tblobj);
                _DbContext.SaveChanges();
            return 1;
        }
        private int GetCustomerId()
        {
            var customerid = _DbContext.CustomerMasters.OrderByDescending(x => x.CustomerID).Select(x => x.CustomerID).FirstOrDefault();
            return customerid;
        }
        public int SaveAddPerson(tblAddContactPerson tblobj,EnquiryModel model)
        {
            int CustomerId = GetCustomerId();
            tblobj = new tblAddContactPerson();
            tblobj.CustomerID = CustomerId;
            tblobj.ContactPerson = model.Contactperson;
            tblobj.MobileNo = model.MobileNo;
            tblobj.EmailId = model.EmailId;
            tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
           
            tblobj.IsDeleted = false;
            _DbContext.tblAddContactPersons.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;

        }

        public List<ContactPersonModel> GetContacts()
        {
            List<ContactPersonModel> lstlist = _DbContext.tblAddContactPersons.Where(x => x.IsDeleted == false)
                .Select(item => new ContactPersonModel()
                {
                    ContactId = item.ContactId,
                    ContactPerson = item.ContactPerson,
                    MobileNo = item.MobileNo,
                    EmailId = item.EmailId

                }).ToList();
            return lstlist;
            
        }
        public List<EnquiryModel> GetAllCustomer()
        {
            var data =(from c in _DbContext.CustomerMasters join s in _DbContext.tblStates on c.StateId equals s.StateId where 
           c.IsDeleted==false
                       select new EnquiryModel()
                       {
                           CustomerID = c.CustomerID,
                           CompanyName = c.CompanyName,
                           PAN = c.PAN,
                           State = s.State
                       }).ToList();
            
            return data;

        }

        //public EnquiryModel GetContactTemp(ContactPersonModel model)
        //{
        //    EnquiryModel enquiryModels = new EnquiryModel();
        //    enquiryModels.ContactPersons.Add(model);
        //    return enquiryModels;
            
        //}
    }
}
