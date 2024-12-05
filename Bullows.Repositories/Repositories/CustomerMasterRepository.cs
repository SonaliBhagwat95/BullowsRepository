using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bullows.Repositories.Repositories
{
    public class CustomerMasterRepository : GenericRepository<CustomerMaster>
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
           // tblobj.DistrictId = model.DistrictId;
            tblobj.CityId = model.CityId;
            tblobj.Pin = model.Pin;
            //tblobj.PAN = model.PAN;
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
        public int SaveAddPerson(tblAddContactPerson tblobj, EnquiryModel model)
        {
            int CustomerId = GetCustomerId();
            tblobj = new tblAddContactPerson();
            tblobj.CustomerID = CustomerId;
            tblobj.ContactPerson = model.Contactperson;
            tblobj.MobileNo = model.MobileNo;
            tblobj.EmailId = model.EmailId;
            tblobj.PAN = model.PAN;
            tblobj.Designation= model.Designation;
            tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.IsDeleted = false;
            _DbContext.tblAddContactPersons.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }
        public List<string> GetCompaniesStartingWith(string term)
        {
            var companies = _DbContext.CustomerMasters
                 .Where(c => c.CompanyName.StartsWith(term))
                 .Select(c => c.CompanyName)
                 .ToList();
            return companies;
        }

        public EnquiryModel GetCompanyDetails(string companyName)
        {
            var companyDetails = (from customer in _DbContext.CustomerMasters
                                  join contact in _DbContext.tblAddContactPersons
                                  on customer.CustomerID equals contact.CustomerID
                                  where customer.CompanyName == companyName
                                  select new EnquiryModel()
                                  {
                                      CustomerID = customer.CustomerID,
                                      CustomerAddress = customer.CustomerAddress,
                                      Contactperson = contact.ContactPerson,
                                      Designation = contact.Designation,
                                      MobileNo = contact.MobileNo,
                                      EmailId = contact.EmailId
                                  }).FirstOrDefault();
            return companyDetails;
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
            var AllCustomer = _DbContext.CustomerMasters.Where(x => x.IsDeleted == false).Select(s => new
            {
                s.CustomerID,
                s.CompanyName,
                s.CustomerAddress
            });
            List<EnquiryModel> enquiryModels = AllCustomer.Select(item => new EnquiryModel()
            {
                CustomerID = item.CustomerID,
                CompanyName = item.CompanyName,
                CustomerAddress = item.CustomerAddress
            }).ToList();
            return enquiryModels;
        }
        public List<EnquiryModel> GetCitiesByState(int stateId)
        {
            // to get cities based on stateId
            var cities = _DbContext.tblCity
                                   .Where(x => x.StateID == stateId && x.IsDeleted == false)
                                   .Select(item => new EnquiryModel
                                   {
                                       CityId = item.CityID,
                                       Description = item.Description
                                   })
                                   .ToList();

            return cities;
        }

        
        public List<EnquiryModel> FillStateDropDown()
        {
            List<EnquiryModel> lstdata = _DbContext.tblState.Where(x => x.IsDeleted == false).Select(item => new EnquiryModel()
            {
                StateId = item.StateId,
                State = item.State,
            }).ToList();
            return lstdata;
        }
        public List<EnquiryModel> FillCityDropdown()
        {
            List<EnquiryModel> lstcity = _DbContext.tblCity.Where(x => x.IsDeleted == false).Select(item => new EnquiryModel()
            {
                CityId=item.CityID,
                Description=item.Description,
            }).ToList();
            return lstcity;
        }

        public int Delete(int id)
        {
            CustomerMaster objtbl = _DbContext.CustomerMasters.Find(id);
            objtbl.IsDeleted = true;
            _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _DbContext.SaveChanges();
            return 2;
        }
    }
}
