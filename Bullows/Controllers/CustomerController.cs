using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Web.Mvc;

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

        public IActionResult Customer(int id, EnquiryModel model)
        {
           
            ViewBag.GridCustomer = _uow.CustomerMasterRepository.GetAllCustomer();
            var stateList = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(_uow.CustomerMasterRepository.FillStateDropDown(), "StateId", "State");
            ViewBag.State = stateList;

          
            

            if (id > 0)
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
            ViewBag.ActivePage = "Customer";
            return View();
        }
        public IActionResult SaveCustomerDetails(tblAddContactPerson tbl, EnquiryModel model, CustomerMaster tblobj, int flag, int selectedStateId,int selectedCityId)
        {
            flag = SaveFlag;
            model.StateId = selectedStateId;
            model.CityId = selectedCityId;
            CID = _uow.CustomerMasterRepository.saveCustomerDetails(model, tblobj, flag);
            CID = _uow.CustomerMasterRepository.SaveAddPerson(tbl, model);
            // ViewBag.Grid = _uow.CustomerMasterRepository.GetContacts();
            SaveFlag = 1;
            return RedirectToAction("Customer");
        }

        public IActionResult GetCitiesByState(int stateId)
        {
            // Assuming you have a method in your repository that fetches cities based on stateId
            var cities = _uow.CustomerMasterRepository.GetCitiesByState(stateId);

            var cityList = cities.Select(c => new { value = c.CityId, text = c.Description }).ToList();

            return Json(cityList);
        }
        public IActionResult Delete(int id = 0)
        {
            CID = _uow.CustomerMasterRepository.Delete(id);
            CID = 2;
            return RedirectToAction("Customer");
           
        }
    }
}
