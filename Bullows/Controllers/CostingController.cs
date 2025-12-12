
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;

using Microsoft.AspNetCore.Mvc;


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

        [HttpPost]
        //public IActionResult GetEnquiryCodes(string enquiryCode)
        //{
        //    var enquiryCodes = 
        //    return Json(enquiryCodes); // Return the list of matching enquiry codes
        //}

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult SearchByEnquiryID(string EnquiryCode)
        {
            try
            {
                var model = new CostingModel();
                model.DetailsList = _uow.costingRepository.GetAllByEnquiryID(EnquiryCode);

                var baffleList = _uow.costingRepository.GetAllMetalBafflesDetails(EnquiryCode);
                model.metalBaffles = baffleList;
                if(baffleList!= null && baffleList.Any())
                {
                    foreach (var baffles in baffleList)
                    {
                        baffles.PanelPosition = "MetalBaffle";
                        baffles.SheetThickness = model.DetailsList[0].SheetThickness;
                    }
                    model.DetailsList.AddRange(baffleList);
                }
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
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("CostingController", "SearchByEnquiryID", ex.Message);
                throw;
            }
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

        public IActionResult SaveCostOFFrameAndPanels(CostingModel model)
        {
            string enquiryCode = HttpContext.Session.GetString("EnquiryCode");
            _uow.costingRepository.SaveCostOFFrame(enquiryCode, model);
            return RedirectToAction("BOCalculations");
        }

        public IActionResult Delete(int id = 0)
        {
            MID = _uow.materialRepository.Delete(id);
            MID = 2;
            return RedirectToAction("Material");

        }
        #endregion

        public IActionResult BOCalculations(string enquiryCode )
        {
             //string enquiryCode = HttpContext.Session.GetString("EnquiryCode");
            (decimal totalPriceOfFrame,decimal totalPriceOfPanels)=_uow.costingRepository.GetCostOFFrame(enquiryCode);

            try
            {
                var fetchpanelsDetails = _uow.costingRepository.UpdateTotalPriceOfFrame(enquiryCode, totalPriceOfFrame, totalPriceOfPanels);
                var LightDetails = _uow.costingRepository.GetAlllightDetails(enquiryCode);
                var frameDetails = _uow.costingRepository.GetAllFrameDetails(enquiryCode);
                var BlowerDetails = _uow.costingRepository.GetAllBlowerDetails(enquiryCode);
                var data = _uow.materialRepository.GetAllMaterialForBO();
                var ExhaustDuct = _uow.costingRepository.GetAllExhaustDuctingDetails(enquiryCode);
                var model = new BOCalculationModel
                {
                    TotalPriceOfFrame = totalPriceOfFrame,
                    TotalPriceOfPanels = totalPriceOfPanels,
                    Materials = data,
                    FrameDetails = frameDetails,
                    PaintBooth = BlowerDetails,
                    TubeLightDetails = LightDetails,
                    ExhaustDuctDetails = ExhaustDuct
                };
                ViewBag.ActivePage = "BO Calculations";
                return View(model);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("CostingController", "BOCalculations", ex.Message);
                throw;
            }
        }
        public IActionResult savePriceDetails(BOCalculationModel model)
        {
            try
            {
                //string EnquiryCode = Session.GetString("EnquiryCode");
                _uow.costingRepository.SavePriceDetailsValues(model, model.EnquiryCode);
                return RedirectToAction("PriceBID");
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("CostingController", "savePriceDetails", ex.Message);
                return RedirectToAction("PriceBID");

            }
        }
        public IActionResult ProposalForm()
        {
            ViewBag.ActivePage = "ProposalForm";
            return View();
        }

        [HttpPost]
        public IActionResult ShowProposalForm(string SalesNO)
        {

            try
            {
                ProposalPriceBIDViewModel viewModel = new();

                if (SalesNO != null)
                {
                    viewModel.ProposalDetails = _uow.costingRepository.GetAllDetails(SalesNO);
                    viewModel.PriceBIDDetails = _uow.costingRepository.FetchPriceBidRecoreds(SalesNO);
                    viewModel.MaterialList = _uow.costingRepository.GetMaterialsValue(SalesNO);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                _uow.exceptionHandlerRepository.SaveException("CostingController", "ShowProposalForm", ex.Message);

                return View(new ProposalPriceBIDViewModel());
            }
        }

        public IActionResult PriceBID()
        {
            ViewBag.ActivePage = "PriceBID";
            return View();
        }

        [HttpPost]
        public IActionResult FetchPriceBIDData(string enquiryCode)
        {
            try
            {
                // Use the repository to fetch data
                var priceBIDData = _uow.costingRepository.GetPriceBIDData(enquiryCode);
                var totalWeights = _uow.costingRepository.CalculateTotalWeightByEnquiryCode(enquiryCode);
                if (priceBIDData == null || !priceBIDData.Any())
                {
                    return Json(new { success = false, message = "Costing Approval Pending...." });
                }
                return Json(new { success = true, data = priceBIDData, totalWeights });
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("CostingController", "FetchPriceBIDData", ex.Message);

                throw;
            }
        }
        [HttpPost]
        public IActionResult UpdatePriceDetails(PriceBIDModel model)
        {
            try
            {

                var updatedata = _uow.costingRepository.UpdatePriceDetails(model);
                // Assuming SalesNO is part of the model or available elsewhere
                string salesNo = model.SalesNO; // Adjust if SalesNO is obtained differently

                return RedirectToAction("PriceBID", "Costing", new { SalesNO = salesNo });
                //return RedirectToAction("ShowProposalForm");
            }
            catch (Exception ex)
            {
                return RedirectToAction("PriceBID", new { SalesNO = model?.SalesNO });
                //return RedirectToAction("ShowProposalForm");
            }
        }

    
    }

}


