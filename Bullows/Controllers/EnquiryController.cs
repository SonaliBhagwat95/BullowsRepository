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
        public EnquiryController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor,  IWebHostEnvironment _environment) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
           
            Environment = _environment;
        }
       
        public IActionResult Enquiry(int id =0,IFormFile imagefile= null)

        {
            ViewBag.GridData = _uow.enquiryRepository.PopulateGrid();
            // ViewBag.Company = new SelectList(_uow.enquiryRepository.FillComanyDropDown(), "CustomerID", "CompanyName");

            if (id>0)
            {
                SaveFlag=1;
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
                if(EID==1)
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

        [HttpPost]
        public IActionResult SaveEnquiryDetails(EnquiryModel model,Bullows.Database.EnquiryMaster tblenq, Bullows.Database.CustomerMaster tblCust, Bullows.Database.ComponentTable tblComp,int flag)
        {
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

            if (model.imageFile != null && model.imageFile.Length > 0)
            {
                var folderPath = Path.Combine(webRootPath,"Bullows File");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var path = Path.Combine(folderPath,model.imageFile.FileName);

                var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                model.imageFile.CopyTo(stream);
                model.Image_Path = path;

            }
            EID = _uow.enquiryRepository.SaveComponent(model, tblComp, flag);
            EID = _uow.enquiryRepository.SaveEnquiry(model, tblenq, flag);
           
            SaveFlag = 1;
            return RedirectToAction("Enquiry");
        }

        public IActionResult Delete(int id=0)
        {
            EID = _uow.enquiryRepository.Delete(id);
            return RedirectToAction("Enquiry");
            EID = 2;

        }
    }
}
