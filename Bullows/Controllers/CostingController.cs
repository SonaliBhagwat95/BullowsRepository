//using Bullows.Database;
//using Bullows.Model;
//using Bullows.Repositories.Contracts;
//using Bullows.Repositories.Repositories;
//using devDept.Eyeshot;
//using Microsoft.AspNetCore.Mvc;


//using System.Web.Mvc;


//namespace Bullows.Controllers
//{
//    public class CostingController : BaseController
//    {
//        static int MID = 0; static int SaveFlag = 0;
//        private readonly UnitOfWorks _uow;
//        private readonly ISession Session;
//        public CostingController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
//        {
//            this._uow = uow as UnitOfWorks;
//            this.Session = httpContextAccessor.HttpContext.Session;
//        }
//        public IActionResult Costing()
//        {
//            var model = new CostingModel();
//            ViewBag.ActivePage = "Raw Material Costing";
//            return View(model);

//        }

//        [Microsoft.AspNetCore.Mvc.HttpPost]
//        public IActionResult SearchByEnquiryID(string EnquiryCode)
//        {
//            var model = new CostingModel();          
//            model.DetailsList = _uow.costingRepository.GetAllByEnquiryID(EnquiryCode);

//            // Fetch materials list
//            model.Materials = _uow.materialRepository.GetAllMaterial();

//            // Filtered details list

//            model.FilteredDetailsList = model.DetailsList
//           .Where(d => d.PanelPosition == "TopStructureFrame" || d.PanelPosition == "BaseStructureFrame")
//           .GroupBy(d => new { d.EnquiryId, d.PanelPosition })
//           .Select(g => g.First())
//           .ToList();


//            // Calculate the total weight for all panels and frame
//            model.TotalWeight = Math.Round(model.DetailsList.Sum(d => d.PanelWeight), 3);

//            // Calculate total weight of frame
//            model.TotalWeightOfFrame = model.FilteredDetailsList.Sum(d => d.PanelWeight);

//            // Calculate RM weight of panels
//            model.RMWeightOfPanels = Math.Round(model.TotalWeight - model.TotalWeightOfFrame);

//            // Get the rate of MS Structure
//            var msStructure = model.Materials.FirstOrDefault(m => m.MOC == "MS Structure");
//            if (msStructure != null)
//            {
//                model.TotalPriceofPanels = model.RMWeightOfPanels * msStructure.Rate;
//                model.TotalPriceofFrame = Math.Round(model.TotalWeightOfFrame * msStructure.Rate);
//            }
//            else
//            {
//                model.TotalPriceofPanels = 0;
//                model.TotalPriceofFrame = 0;
//            }
//            return View("Costing", model);
//        }

//        [Microsoft.AspNetCore.Mvc.HttpPost]
//        public Microsoft.AspNetCore.Mvc.JsonResult SearchEnquiryCode(string enquiryCode)
//        {
//            var results = _uow.costingRepository.SearchEnquiryCodes(enquiryCode);
//            if (results == null || !results.Any())
//            {
//                return Json(new { success = false, message = "No Enquiry Codes found" });
//            }
//            return Json(new { success = true, results });
//        }
//        public IActionResult ProposalForm()
//        {
//            return View();
//        }

//        #region Material
//        public IActionResult Material(int id)
//        {
//            ViewBag.getmaterial = _uow.materialRepository.GetAllMaterial();
//            if(id>0)
//            {
//                SaveFlag = 1;
//                SetPanelHeading("Edit Material Details");
//                var data = _uow.materialRepository.EditModel(id);
//                if (data == null)
//                    return HttpNotFound();
//                else
//                    return View(data);
//            }
//            else
//            {
//                SetPanelHeading("Material Details");
//                SaveFlag = 0;
//                if (MID == 1)
//                    SetSuccessMessage("Material has been saved successfully");
//                else if (MID == 2)
//                    SetErrorMessage("Material has been deleted successfully");
//                else if (MID < 0)
//                    SetErrorMessage("Something went wrong while saving Material");
//                MID = 0;
//            }
//            ViewBag.ActivePage = "Material";
//            return View(new MaterialModel());
//        }

//        private IActionResult HttpNotFound()
//        {
//            throw new NotImplementedException();
//        }

//        [Microsoft.AspNetCore.Mvc.HttpPost]
//        public IActionResult saveMaterials(MaterialModel model,int flag,tblMOC tblobj)
//        {
//            flag = 0;
//            flag = SaveFlag;
//            MID = _uow.materialRepository.Save(model, flag, tblobj);
//            SaveFlag = 1;
//            return RedirectToAction("Material");
//        }

//        public IActionResult Delete(int id=0)
//        {
//            MID = _uow.materialRepository.Delete(id);
//            return RedirectToAction("Material");
//            MID = 2;
//        }
//        #endregion

//        public IActionResult BOCalculations()
//        {

//            var data = _uow.materialRepository.GetAllMaterialForBO();
//            var model = new BOCalculationModel
//            {
//                Materials = data,              
//            };
//            ViewBag.ActivePage = "Raw Material Costing";
//            return View(model);
//        }

//    }

//}

using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using devDept.Eyeshot;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

using System.Web.Mvc;


namespace Bullows.Controllers
{
    public class CostingController : BaseController
    {
        static int MID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        public CostingController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Costing()
        {
            var model = new CostingModel();
            ViewBag.ActivePage = "Raw Material Costing";
            return View(model);

        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult SearchByEnquiryID(string EnquiryCode)
        {
            var model = new CostingModel();
            model.DetailsList = _uow.costingRepository.GetAllByEnquiryID(EnquiryCode);

            // Fetch materials list
            model.Materials = _uow.materialRepository.GetAllMaterial();

            // Filtered details list

            model.FilteredDetailsList = model.DetailsList
           .Where(d => d.PanelPosition == "TopStructureFrame" || d.PanelPosition == "BaseStructureFrame")
           .GroupBy(d => new { d.EnquiryId, d.PanelPosition })
           .Select(g => g.First())
           .ToList();


            // Calculate the total weight for all panels and frame
            model.TotalWeight = Math.Round(model.DetailsList.Sum(d => d.PanelWeight), 3);

            // Calculate total weight of frame
            model.TotalWeightOfFrame = model.FilteredDetailsList.Sum(d => d.PanelWeight);

            // Calculate RM weight of panels
            model.RMWeightOfPanels = Math.Round(model.TotalWeight - model.TotalWeightOfFrame);

            // Get the rate of MS Structure
            var msStructure = model.Materials.FirstOrDefault(m => m.MOC == "MS Structure");
            if (msStructure != null)
            {
                model.TotalPriceofPanels = model.RMWeightOfPanels * msStructure.Rate;
                model.TotalPriceofFrame = Math.Round(model.TotalWeightOfFrame * msStructure.Rate);
            }
            else
            {
                model.TotalPriceofPanels = 0;
                model.TotalPriceofFrame = 0;
            }
            HttpContext.Session.SetString("EnquiryCode", EnquiryCode);
            return View("Costing", model);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.JsonResult SearchEnquiryCode(string enquiryCode)
        {
            var results = _uow.costingRepository.SearchEnquiryCodes(enquiryCode);
            if (results == null || !results.Any())
            {
                return Json(new { success = false, message = "No Enquiry Codes found" });
            }
            return Json(new { success = true, results });
        }

        #region Material
        public IActionResult Material(int id)
        {
            ViewBag.getmaterial = _uow.materialRepository.GetAllMaterial();
            if (id > 0)
            {
                SaveFlag = 1;
                SetPanelHeading("Edit Material Details");
                var data = _uow.materialRepository.EditModel(id);
                if (data == null)
                    return HttpNotFound();
                else
                    return View(data);
            }
            else
            {
                SetPanelHeading("Material Details");
                SaveFlag = 0;
                if (MID == 1)
                    SetSuccessMessage("Material has been saved successfully");
                else if (MID == 2)
                    SetErrorMessage("Material has been deleted successfully");
                else if (MID < 0)
                    SetErrorMessage("Something went wrong while saving Material");
                MID = 0;
            }
            ViewBag.ActivePage = "Material";
            return View(new MaterialModel());
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult saveMaterials(MaterialModel model, int flag, tblMOC tblobj)
        {
            flag = 0;
            flag = SaveFlag;
            MID = _uow.materialRepository.Save(model, flag, tblobj);
            SaveFlag = 1;
            return RedirectToAction("Material");
        }

        public IActionResult Delete(int id = 0)
        {
            MID = _uow.materialRepository.Delete(id);
            MID = 2;
            return RedirectToAction("Material");
           
        }
        #endregion

        public IActionResult BOCalculations()
        {
            string enquiryCode = HttpContext.Session.GetString("EnquiryCode");
            var LightDetails = _uow.costingRepository.GetAlllightDetails(enquiryCode);
            var frameDetails = _uow.costingRepository.GetAllFrameDetails(enquiryCode);
            var BlowerDetails = _uow.costingRepository.GetAllBlowerDetails(enquiryCode);
            var data = _uow.materialRepository.GetAllMaterialForBO();
            var model = new BOCalculationModel
            {
                Materials = data,
                FrameDetails = frameDetails,
                PaintBooth = BlowerDetails,
                TubeLightDetails = LightDetails
            };
            ViewBag.ActivePage = "Raw Material Costing";
            return View(model);
        }

        public IActionResult ProposalForm(string EnquiryCode)
        {
            ViewBag.ActivePage = "ProposalForm";
            return View();
        }

    }

}

