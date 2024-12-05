//using Bullows.Database;
//using Bullows.Model;
//using Bullows.Repositories.Contracts;
//using Bullows.Repositories.Repositories;
//using devDept.Geometry.ConstraintSolver;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Cryptography;


//namespace Bullows.Controllers
//{
//    public class PanelSettingController : BaseController
//    {
//        private readonly UnitOfWorks _uow;
//        static int SID = 0;
//        private readonly ISession Session;
//        private readonly BullowsDbContext _context;

//        static int SaveFlag = 0;

//        public PanelSettingController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, BullowsDbContext context) : base(httpContextAccessor)
//        {
//            this._uow = uow as UnitOfWorks;
//            this.Session = httpContextAccessor.HttpContext.Session;
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }
//        public IActionResult panelSetting(int id=0)
//        {
//            ViewBag.ActivePage = "PanelSetting";
//            ViewBag.GridData = _uow.settingDetailsRepository.GetAllData();
//            if (id > 0)
//            {
//                SaveFlag = 1;
//                SetPanelHeading("Edit Setting Details");
//                var data = _uow.settingDetailsRepository.EditModel(id);
//                if (data == null)
//                    return HttpNotFound();
//                else
//                    return View(data);
//            }
//            else
//            {
//                SetPanelHeading("Setting Details");
//                SaveFlag = 0;
//                if (SID == 1)
//                    SetSuccessMessage("Setting Details has been saved successfully");
//                else if (SID == 2)
//                    SetErrorMessage("Setting Details has been deleted successfully");
//                else if (SID < 0)
//                    SetErrorMessage("Something went wrong while saving Setting Details");
//                SID = 0;
//            }
//            var model = new SettingModel();
//            return View(model);           
//        }

//        private IActionResult HttpNotFound()
//        {
//            throw new NotImplementedException();
//        }

//        [HttpPost]
//        public IActionResult GetEnquiryCodes(string term)
//        {
//            var enquiryCodes = _uow.PaintBoothRepository.GetEnquiryCodes(term);
//            var result = new
//            {
//                success = enquiryCodes.Any(),
//                results = enquiryCodes
//            };
//            return Json(result);
//        }

//        public IActionResult SaveSetting(SettingModel model,int flag,SettingDetails objtbl,int id,EnquiryModel enquiry)
//        {
//            var EnquiryDetails = _uow.settingDetailsRepository.GetEnquiryID(model.SalesNo);
//            if (EnquiryDetails != null)
//            {
//                model.EnquiryID = EnquiryDetails.EnquiryId;
//            }
//            flag = 0;
//            flag = SaveFlag;
//            SID = _uow.settingDetailsRepository.SaveSettingsDetails(model, flag, objtbl);
//            SaveFlag = 1;
//            return RedirectToAction("panelSetting");
//        }

//        public IActionResult Delete(int id = 0)
//        {
//            SID = _uow.settingDetailsRepository.Delete(id);
//            return RedirectToAction("panelSetting");
//            SID = 2;

//        }
//    }
//}
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using devDept.Geometry.ConstraintSolver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;


namespace Bullows.Controllers
{
    public class PanelSettingController : BaseController
    {
        private readonly UnitOfWorks _uow;
        static int SID = 0;
        private readonly ISession Session;
        private readonly BullowsDbContext _context;

        static int SaveFlag = 0;

        public PanelSettingController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, BullowsDbContext context) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IActionResult panelSetting(int id = 0)
        {
           
            ViewBag.GridData = _uow.settingDetailsRepository.GetAllData();
            if (id > 0)
            {
                SaveFlag = 1;
                SetPanelHeading("Edit Setting Details");
                var data = _uow.settingDetailsRepository.EditModel(id);
                if (data == null)
                    return HttpNotFound();
                else
                    return View(data);
            }
            else
            {
                SetPanelHeading("Setting Details");
                SaveFlag = 0;
                if (SID == 1)
                    SetSuccessMessage("Setting Details has been saved successfully");
                else if (SID == 2)
                    SetErrorMessage("Setting Details has been deleted successfully");
                else if (SID < 0)
                    SetErrorMessage("Something went wrong while saving Setting Details");
                SID = 0;
            }
            var model = new SettingModel();
            ViewBag.ActivePage = "PanelSetting";
            return View(model);
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult GetEnquiryCodes(string term)
        {
            var enquiryCodes = _uow.PaintBoothRepository.GetEnquiryCodes(term);
            var result = new
            {
                success = enquiryCodes.Any(),
                results = enquiryCodes
            };
            return Json(result);
        }

        public IActionResult SaveSetting(SettingModel model, int flag, SettingDetails objtbl, int id, EnquiryModel enquiry, PaintBoothModel pmodel)
        {
            var EnquiryDetails = _uow.settingDetailsRepository.GetEnquiryID(model.SalesNo);
            if (EnquiryDetails != null)
            {
                model.EnquiryID = EnquiryDetails.EnquiryId;
            }
            flag = 0;
            flag = SaveFlag;
            SID = _uow.settingDetailsRepository.SaveSettingsDetails(model, flag, objtbl);
            _uow.settingDetailsRepository.SaveTubeLightDetails(model, flag);
            SaveFlag = 1;
            return RedirectToAction("panelSetting");
        }

        public IActionResult Delete(int id = 0)
        {
            SID = _uow.settingDetailsRepository.Delete(id);
            SID = 2;
            return RedirectToAction("panelSetting");
           

        }
    }
}
