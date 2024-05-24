using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Bullows.Controllers
{
    public class CustomerController : BaseController
    {
        static int CID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        public CustomerController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
           
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Customer(int id,EnquiryModel model)
        {
            //if (model.ContactPersons == null)
            //{
            //    model.ContactPersons = new List<ContactPersonModel>();
            //}

            ViewBag.GridCustomer = _uow.CustomerMasterRepository.GetAllCustomer();
            if (id>0)
            {

            }
            else
            {
                SetPanelHeading("CustomerMaster Details");
                SaveFlag = 0;
                if (CID == 1)
                    SetSuccessMessage("CustomerDetails has been saved successfully");
                else if (CID == 2)
                    SetErrorMessage("CustomerDetails has been deleted successfully");
                else if (CID < 0)
                    SetErrorMessage("Something went wrong while saving CustomerDetails");
                CID = 0;

            }
            return View();
        }

        //public IActionResult ContactPerson(EnquiryModel enquiryModel, ContactPersonModel contactPerson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        enquiryModel.ContactPersons.Add(contactPerson);
        //        return View(enquiryModel);
        //    }
        //    return View(enquiryModel);
        //}

        public IActionResult SaveCustomerDetails(tblAddContactPerson tbl,EnquiryModel model,CustomerMaster tblobj, int flag)
        {
            
           
            flag = SaveFlag;
            CID=_uow.CustomerMasterRepository.SaveAddPerson( tbl,model);
            CID =_uow.CustomerMasterRepository.saveCustomerDetails(model, tblobj, flag);
            ViewBag.Grid = _uow.CustomerMasterRepository.GetContacts();
           
            SaveFlag = 1;
            return RedirectToAction("Customer");
        }
        
    }
}
