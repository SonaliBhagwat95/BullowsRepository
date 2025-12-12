using Bullows.Database.Migrations;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;



namespace Bullows.Controllers
{
    public class EnquiryController : BaseController
    {
        static int EID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;

        private readonly IWebHostEnvironment Environment;
        public EnquiryController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment _environment) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;

            Environment = _environment;
        }

        public IActionResult Enquiry(int id = 0, IFormFile imagefile = null)
        {
            ViewBag.ActivePage = "Enquiry";
            ViewBag.GridData = _uow.enquiryRepository.PopulateGrid();            
            if (id > 0)
            {
                SaveFlag = 1;
                SetPanelHeading("Edit Enquiry Details");
                var data = _uow.enquiryRepository.Editmodel(id);
                if (data == null)
                    return HttpNotFound();
                else
                    return View(data);
            }
            else
            {
                SetPanelHeading("Enquiry Details");
                SaveFlag = 0;
                if (EID == 1)
                    SetSuccessMessage("Enquiry has been saved successfully");
                else if (EID == 2)
                    SetErrorMessage("Enquiry has been deleted successfully");
                else if (EID < 0)
                    SetErrorMessage("Something went wrong while saving Enquiry");
                EID = 0;
            }
            return View(new EnquiryModel());
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        //[HttpGet]
        //public IActionResult CheckEnquiryNo(string enquiryNo)
        //{

        //    var existingEnquiry = _uow.enquiryRepository.GetEnquiryByNo(enquiryNo);
        //    if (existingEnquiry != null)
        //    {
        //        return Json(new { exists = true });
        //    }
        //    return Json(new { exists = false });
        //}
        
        [HttpPost]
        public IActionResult SaveEnquiryDetails(EnquiryModel model, Bullows.Database.EnquiryMaster tblenq, Bullows.Database.ComponentTable tblComp, int flag)
        {
            try
            {
               
                // MotorTypesval = model.MotorType;
                var webRootPath = string.Empty;
                var filelist = Request.Form.Files;
                flag = SaveFlag;

                if (Environment.IsDevelopment())
                {
                    webRootPath = "E:/Sonali/Upload File";
                }
                else
                {
                    webRootPath = this.Environment.WebRootPath;
                }
                List<string> filePaths = new List<string>();

                foreach (var file in filelist)
                {
                    if (file != null && file.Length > 0)
                    {
                        var folderPath = Path.Combine(webRootPath, "Bullows File");
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var path = Path.Combine(folderPath, file.FileName);
                        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(stream);
                        }
                        filePaths.Add(path);
                    }
                }
                // Join file paths as comma-separated string
                model.Image_Path = string.Join(",", filePaths);
                var CompanyDetails = _uow.CustomerMasterRepository.GetCompanyDetails(model.CompanyName);
                if (CompanyDetails != null)
                {
                    // Set the CustomerID in the tblenq object
                    model.CustomerID = CompanyDetails.CustomerID;
                }
                EID = _uow.enquiryRepository.SaveComponent(model, tblComp, flag);
                EID = _uow.enquiryRepository.SaveEnquiry(model, tblenq, flag);

                _uow.enquiryRepository.SaveMotorTypes(model, flag);
                SaveFlag = 1;
                return RedirectToAction("Enquiry");
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("EnquiryController", "SaveEnquiryDetails",ex.Message);
                return RedirectToAction("Enquiry");


            }
        }
       

        [HttpGet]       
        public IActionResult SearchCompanies(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return Json(new { success = false, message = "Search term is required.", companies = new List<string>() });
                }
                var companies = _uow.CustomerMasterRepository.GetCompaniesStartingWith(term);

                return Json(companies);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("EnquiryController", "SearchCompanies", ex.Message);

                throw;
            }
        }
        [HttpPost]
        public JsonResult SearchByCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return Json(null); // Or handle as needed
            }
            var CompanyDetails = _uow.CustomerMasterRepository.GetCompanyDetails(companyName);

            return Json(CompanyDetails);
        }

        public IActionResult Delete(int id = 0)
        {
            EID = _uow.enquiryRepository.Delete(id);
            EID = 2;
            return RedirectToAction("Enquiry");
           

        }
    }
}
