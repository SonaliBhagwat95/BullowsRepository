using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Bullows.Service;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO.Compression;
using DesignDocument = devDept.Eyeshot.DesignDocument;


namespace Bullows.Controllers
{
    public class PaintBoothController : BaseController
    {
        static int PaintID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        private readonly BullowsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaintBoothController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, BullowsDbContext context, IConfiguration configuration) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult PaintBooth(int id)
        {
            if (id > 0)
            {

            }
            else
            {
                SetPanelHeading("Paint Booth Inputs");
                SaveFlag = 0;
                if (PaintID == 1)
                    SetSuccessMessage("PaintBooth has been saved successfully");
                else if (PaintID == 2)
                    SetErrorMessage("PaintBooth has been deleted successfully");
                else if (PaintID < 0)
                    SetErrorMessage("Something went wrong while saving PaintBooth");
                PaintID = 0;
            }
            ViewBag.Message = TempData["Message"];
            ViewBag.ActivePage = "PaintBooth";
            return View(new PaintBoothModel());
        }
        #region Search Mathods
        public IActionResult Search(string enquiryCode)
        {
            var enquiryDetails = _uow.PaintBoothRepository.GetEnquiryDetailsByCode(enquiryCode);
            if (enquiryDetails != null)
            {
                var paintBoothModel = _uow.PaintBoothRepository.GetSettingDetailsByCode(enquiryCode);
                // Assuming paintBoothModel.Settings is a list or object, extract PanelWidth and PanelHeight
                var setting = paintBoothModel?.Settings?.FirstOrDefault(); // Get the first setting or modify this to your requirement
                // Extract PanelWidth and PanelHeight from the setting
                var panelWidth = setting?.PanelWidth;
                var panelHeight = setting?.PanelHeight;
                HttpContext.Session.SetInt32("EnquiryID", enquiryDetails.EnquiryId);
                // return Json(new {EnqiryID=enquiryDetails.EnquiryId, Length = enquiryDetails.LengthSize, Width = enquiryDetails.WidthSize, Height = enquiryDetails.HeightSize  /*SettingDetails = settingDetails*/ });
                return Json(new
                {
                    EnquiryID = enquiryDetails.EnquiryId,
                    Length = enquiryDetails.LengthSize,
                    Width = enquiryDetails.WidthSize,
                    Height = enquiryDetails.HeightSize,
                    PanelWidth = panelWidth,
                    PanelHeight = panelHeight
                });
            }
            else
            {
                //return Json(null);
                return Json(new { error = "Enquiry code not found." });
            }
        }

        [HttpPost]
        public JsonResult SearchEnquiryCode(string enquiryCode)
        {
            var results = _uow.settingDetailsRepository.SearchEnquiryCodes(enquiryCode);
            if (results == null || !results.Any())
            {
                return Json(new { success = false, message = "No Enquiry Codes found" });
            }
            return Json(new { success = true, results });
            // return Json(new { success = true, results = results.Select(r => r.enquiryCode) });
        } 
        #endregion
        [HttpGet]
        public IActionResult GetSettingDetailsByCode(string enquiryCode)
        {
            var paintBoothModel = _uow.PaintBoothRepository.GetSettingDetailsByCode(enquiryCode);
            return Json(new { settings = paintBoothModel.Settings });
        }
        #region Variables declarations
        public static Dictionary<string, List<DesignDocument>> designdictionary = new();
        public static List<string> AllFilesPath = new List<string>();
        public static List<string> developmentpath = new List<string>();

        static PaintBoothModel paintBoothModel = new PaintBoothModel();
        static int noofFiltersInW = 0; static int noofFiltersInD = 0; static int XdistanceInW = 0;
        List<string> filePaths = new List<string>();
        List<DesignDocument> standardFrameDrawing = new List<DesignDocument>();
        double yOffset = 0; 
        #endregion
        public IActionResult SavePaintBoothDetails(PaintBoothModel model, PanelInputModel pmodel, int flag, EnquiryModel enquiry, int motorCatalogID)
        {
            try
            {
                HttpContext.Session.SetString("SalesNo", enquiry.SalesNO);
                string EnquiryNo = HttpContext.Session.GetString("SalesNo");
                HttpContext.Session.SetInt32("PaintboothDepth", (int)(double)model.D);
                HttpContext.Session.SetInt32("PaintboothHeight", (int)(double)model.H);
                HttpContext.Session.SetInt32("PaintboothWidth", (int)(double)model.W);
                HttpContext.Session.SetInt32("D3Panel", (int)(double)model.D3);
                int PlenumHeight = 0;
                string SubTypeofDraft = _uow.PaintBoothRepository.FetchSubTypeOfDraft(EnquiryNo);
                if (SubTypeofDraft == "3")
                {
                    PlenumHeight = int.Parse(_uow.PaintBoothRepository.FetchPlenumHeight(EnquiryNo));
                    HttpContext.Session.SetInt32("PlenumHeight", PlenumHeight);

                }

                if (model.MotorCatalogID.HasValue)
                {
                    int selectedMotorId = model.MotorCatalogID.Value;
                }
                int? enquiryId = HttpContext.Session.GetInt32("EnquiryID");
                if (enquiryId.HasValue)
                {
                    enquiry.EnquiryId = enquiryId.Value;
                }
                else
                {
                    return BadRequest("EnquiryID not found in session.");
                }
                _uow.PaintBoothRepository.SaveMotorDeatils((int)enquiryId, motorCatalogID);
                var PaintBoothTypefromEnquiry = _uow.PaintBoothRepository.FetchPaintBoothType(model.SalesNO);
                decimal RatedOutputHP = _uow.PaintBoothRepository.FetchRatedOutputHP((int)model.MotorCatalogID);
                string MotorTypes = _uow.PaintBoothRepository.GetMotorTypes((int)model.MotorCatalogID);
                model.RatedOutputHP = RatedOutputHP;
                model.MotorTypes = MotorTypes;
                HttpContext.Session.SetString("DraftSubType", PaintBoothTypefromEnquiry);

                var paintBoothModel = _uow.PaintBoothRepository.GetSettingDetailsByCode(EnquiryNo);
                if (paintBoothModel.Settings.Count == 0)
                {
                    return NotFound("No settings found for the given SalesNo.");
                }

                // Assuming you want the first setting for the PaintBoothDesign
                var setting = paintBoothModel.Settings.First();

                // Create and populate the PaintBoothDesign object
                PaintBoothDesign panel = new PaintBoothDesign(_context, _configuration)
                {
                    SheetThickness = setting.SheetThickness, // Use setting values
                    SettingStandardBend1 = setting.StandardBend1,
                    SettingStandardBend2 = setting.StandardBend2,
                    PitchDistance = setting.PitchDistance,
                    PanelWidth = setting.PanelWidth,
                    PanelHeight = setting.PanelHeight,
                    SlotDimention = setting.SlotDimention,
                    SettingH = setting.H,
                    SettingW = setting.W,
                    SettingT = setting.T,
                    Materials = setting.Materials,
                    Section = setting.Section,
                    D3 = model.D3,
                    EnquiryID = enquiry.EnquiryId,
                    LightTypes = setting.LightTypes,
                    LuxLevel = setting.LuxLevel,
                    Lumens = setting.Lumens,
                };
                model.PitchDistance = (decimal)setting.PitchDistance;

                HttpContext.Session.SetInt32("settingPanelWidth", (int)setting.PanelWidth);
                HttpContext.Session.SetInt32("settingPanelHeight", (int)setting.PanelHeight);

                string selectedLocation = model.ServiceDoorLocation;

                // Check if the record already exists
                if (!_uow.PaintBoothRepository.Exists(enquiry.EnquiryId))
                {
                    PaintID = _uow.PaintBoothRepository.SavePaintBooth(model, pmodel, flag, enquiry);
                }
                int componentID = _uow.PaintBoothRepository.FetchComponentId((int)enquiryId);
                double ExtractionC_Height = int.Parse(_uow.PaintBoothRepository.FetchExtractionHeight(componentID));

                ComponentTable DoorType = null;
                DoorType = _uow.PaintBoothRepository.FetchDoorType(componentID);


                #region DoorModel Values Assigning
                DoorDimensionsModel doorModel = new();
                doorModel.doorType = DoorType.DoorSubType;
                doorModel.doorSubType = DoorType.TypeOfHingedDoor;
                doorModel.sideDoorLocation = DoorType.SideDoorLOcation;
                doorModel.doorWidth = (double)model.HingedDoorWidth;
                doorModel.doorHeight = (double)model.HingedDoorHeight;
                doorModel.TypeOfHinges = DoorType.TypeOfHinges; 
                #endregion

                #region All Panels methods
                PaintBoothService service = new PaintBoothService(_uow, _httpContextAccessor, _context, _configuration);
                if (!DoorType.ComponentEntry.Contains("Side") || DoorType.ComponentEntry == "")
                {
                    service.LeftRightPanels(model, panel);
                }
                else
                {
                    model.PanelWidth = setting.PanelWidth;
                    model.PanelHeight = setting.PanelHeight;
                    service.LeftRightPanelsForSideDoor(model, panel, doorModel);
                }
                service.TopPanels(model, panel);
                service.D3Panels(model, panel, ExtractionC_Height);

                service.CreateLoft(model, panel, ExtractionC_Height);
                service.backPanels(model, panel, ExtractionC_Height);
                if (PaintBoothTypefromEnquiry == "3")
                {
                    service.FASPanelsFrontAndBack(model, panel, PlenumHeight);
                    service.FASPanelsRightAndLeft(model, panel, PlenumHeight);
                    service.FASPanelsForTop(model, panel, PlenumHeight);
                }
                service.FiltersAndBaffles(model, panel, ExtractionC_Height);

                service.TopStructureFrame(model, panel);
                service.BaseStructureFrame(model, panel);
                double SameSpaceBetweenDoors = 0;
                if (PaintBoothTypefromEnquiry != "1")
                {
                    if (DoorType.ComponentEntry == "Side")
                    {
                        service.FrontPanels(model, panel);
                    }
                    else
                    {
                        SameSpaceBetweenDoors = service.FrontDoorsType(model, panel,doorModel);

                    }
                }
                #endregion
                #region WriteFile
                string BaseFilePath = "C:/Bullows/Paintbooth_Drawing";
                if (!Directory.Exists(BaseFilePath))
                {
                    Directory.CreateDirectory(BaseFilePath);
                }
                DesignDocument combinedDrawing = CombineAssemblies2(service.designdictionary, setting.PanelHeight, setting.PanelWidth, PaintBoothTypefromEnquiry, ExtractionC_Height, SameSpaceBetweenDoors, doorModel);

           
                string combinedDwgFilePath = BaseFilePath + "/GA3D.dwg";
                WriteAutodeskParams autoCombined = new WriteAutodeskParams(combinedDrawing);
                WriteAutodesk dwgWriterCombined = new WriteAutodesk(autoCombined, combinedDwgFilePath);
                dwgWriterCombined.DoWork();

                //making pdf file format 
                string pdfFilePath = BaseFilePath + "/GA3D.pdf";
                Write3DPdfParams pdf = new Write3DPdfParams(combinedDrawing);
                Write3DPDF pdf1 = new Write3DPDF(pdf, pdfFilePath);
                pdf1.DoWork();

                //STEP file format
                string StepFilePath = BaseFilePath + "/GA3D_STEP.step";

                WriteSTEP wastep = new WriteSTEP(combinedDrawing, StepFilePath);
                wastep.DoWork(); 
               
                #endregion

                #region Views
                //Added Views here
                DesignDocument designDocument = combinedDrawing;

                PaintBoothclass paintBoothClass = panel.detailsdrawing(designDocument, model);

                PaintBoothclass standardframe = panel.StandardFrameWithoutDrawing(2, EnquiryNo);

                string filepathforstandardframe = standardframe.lstpath;
                DesignDocument standardframedrawing = new DesignDocument();
                standardframedrawing = standardframe.drawing;
                WriteAutodeskParams framedw = new WriteAutodeskParams(standardframedrawing);
                WriteAutodesk dwgWriterframe = new WriteAutodesk(framedw, filepathforstandardframe);
                dwgWriterframe.DoWork();
                standardFrameDrawing.Add(standardframedrawing);
                #endregion

                AllFilesPath = new List<string>(filePaths)
                {
                    combinedDwgFilePath,
                    pdfFilePath,
                   //filepathforLoft,
                   //filepathforstandardframe,
                };
                AllFilesPath.Add(paintBoothClass.lstpath);
                return RedirectToAction("FilterFrameCalculations", model);

            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothController", "SavePaintBoothDetails", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        private DesignDocument CombineAssemblies2(Dictionary<string, List<DesignDocument>> designDocument, double panelheight, double panelWidth, string DraftSubType, double extractionCHeight, double SameSpaceBetweenDoors,DoorDimensionsModel doorModel)
        {
            try
            {

                int? settingPanelWidth = HttpContext.Session.GetInt32("settingPanelWidth");
                int? settingPanelHeight = HttpContext.Session.GetInt32("settingPanelHeight");
                double? PaintboothHeight = HttpContext.Session.GetInt32("PaintboothHeight");
                double? PaintboothWidth = HttpContext.Session.GetInt32("PaintboothWidth");
                double? PaintBoothDepth = HttpContext.Session.GetInt32("PaintboothDepth");
                //int? PlenumHeight=  HttpContext.Session.GetInt32("PlenumHeight");

                double? D3Panel = HttpContext.Session.GetInt32("D3Panel");

                var combinedDrawing = new DesignDocument();
                Layer mylayer = new Layer("bendlayer");
                Layer OuterDoorFrame = new Layer("OuterDoorFrame", Color.Gray);
                Layer OuterRectangleLayer = new Layer("OuterRectangleLayer", Color.BlueViolet);
                Layer DoorFrameLayer = new Layer("DoorFrame", Color.BurlyWood);
                Layer MetalSheetLayer = new Layer("MetalSheet", Color.FloralWhite);
                Layer MainDoorFrameLayer = new Layer("MainDoorFrame", Color.DarkMagenta);
                mylayer.Color = Color.FromArgb(165, 82, 165);
                combinedDrawing.Layers.Add(mylayer);
                combinedDrawing.Layers.Add(OuterDoorFrame);
                combinedDrawing.Layers.Add(OuterRectangleLayer);
                combinedDrawing.Layers.Add(DoorFrameLayer);
                combinedDrawing.Layers.Add(MetalSheetLayer);
                combinedDrawing.Layers.Add(MainDoorFrameLayer);
                combinedDrawing.Layers.Add("HingeBackPlate", Color.BurlyWood);

                combinedDrawing.Units = linearUnitsType.Millimeters;

                foreach (var kvp in designDocument)
                {
                    if (kvp.Key == "rightSideDrawings" || kvp.Key == "leftSideDrawings")
                    {
                        int panelcount = PaintBoothService.noOfPanelsforD;

                        double zOffeset = 0;
                        if (Math.Ceiling(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD + 1)
                        {
                            if (DraftSubType == "7" || DraftSubType == "5" || DraftSubType == "4" || DraftSubType == "6")
                            {
                                panelcount += 3;
                            }
                            else
                            {
                                // panelcount += 2;
                                panelcount++;


                            }
                        }
                        else if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD)
                        {
                            if (DraftSubType == "7" || DraftSubType == "5" || DraftSubType == "4" || DraftSubType == "6")
                            {
                                panelcount += 2;
                            }
                            else
                            {
                                panelcount++;
                            }
                        }
                        if (Math.Ceiling(PaintBoothService.totalPanelsforH) == PaintBoothService.noOfPanelsforH + 1)
                        {
                            //zOffeset = 2390;
                            zOffeset = panelheight;

                        }
                        else
                            zOffeset = panelheight;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = 0,
                            Zoffset = zOffeset,
                            paintboothSide = "BottomRightLeftPanels"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }//right and left panles
                    else if (kvp.Key == "TopAssembly")
                    {
                        int panelcount = PaintBoothService.noOfPanelsforD;
                        double xOffset = 0;
                        double Yoffset = 0;
                        if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD + 1)
                        {

                            panelcount++;
                        }

                        if (Math.Ceiling(PaintBoothService.totalPanelsforW) == PaintBoothService.noOfPanelsforW + 1)
                        {
                            //Yoffset = 2390;
                            Yoffset = panelheight;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.totalPanelsforW) == PaintBoothService.noOfPanelsforW)
                        {
                            //Yoffset = 2390;
                            Yoffset = panelheight;

                        }

                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = DraftSubType == "5" || DraftSubType == "4" ? (double)D3Panel : 0,
                            yOffset = Yoffset,
                            Zoffset = 0,
                            paintboothSide = kvp.Key
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "rightSideDoorDrawings" || kvp.Key == "leftSideDoorDrawings")
                    {
                        

                        double smallPanelCount = PaintBoothService.smallPanelsWidthDoors > 0 ? 1 : 0;
                        int panelcount = (int)(PaintBoothService.noOfPanelsInSideDoor + smallPanelCount);

                        double zOffeset = 0;
                        if (Math.Ceiling(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsInSideDoor + smallPanelCount)
                        {
                            panelcount++;
                        }
                        else if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsInSideDoor)
                        {
                            panelcount++;
                        }
                        if (Math.Ceiling(PaintBoothService.totalPanelsforH) == PaintBoothService.noOfPanelsforH + 1)
                        {
                            //zOffeset = 2390;
                            zOffeset = panelheight;

                        }
                        else
                            zOffeset = panelheight;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = 0,
                            Zoffset = zOffeset,
                            paintboothSide = "rightSideDoorDrawings"
                        };
                        //double panelWidthForSideDoors = PaintBoothService.RemainingSpanceinD < panelWidth ? PaintBoothService.RemainingSpanceinD : panelWidth;
                        AddDrawingsToAssembly(assembly, panelWidth);
                    }

                    else if (kvp.Key == "rightSideDoorNearExtractionCDrawings" || kvp.Key == "leftSideDoorNearExtractionCDrawings")
                    {
                        double smallPanelCount = PaintBoothService.smallPanelsWidthDoors > 0 ? 1 : 0;
                        int panelcount = (int)(PaintBoothService.noOfPanelsInSideDoor + smallPanelCount);

                        double zOffeset = 0;
                        if (Math.Ceiling(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsInSideDoor + smallPanelCount)
                        {
                            panelcount++;
                        }
                        else if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsInSideDoor)
                        {
                            panelcount++;
                        }
                        if (Math.Ceiling(PaintBoothService.totalPanelsforH) == PaintBoothService.noOfPanelsforH + 1)
                        {
                            //zOffeset = 2390;
                            zOffeset = panelheight;

                        }
                        else
                            zOffeset = panelheight;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = (double)PaintBoothDepth,
                            yOffset = 0,
                            Zoffset = zOffeset,
                            paintboothSide = "rightAndLeftSideDoorNearExtractionCDrawings"
                        };
                        double xwidth = PaintBoothService.RemainingSpanceinD < panelWidth ? PaintBoothService.RemainingSpanceinD : panelWidth;
                        assembly.xoffset = (double)PaintBoothDepth - xwidth;
                        //panelWidth = PaintBoothService.RemainingSpanceinD < panelWidth ? PaintBoothService.RemainingSpanceinD : panelWidth;
                        AddDrawingsToAssembly(assembly, panelWidth);
                    }

                    else if (kvp.Key == "D3PanelRightDrawings" || kvp.Key == "D3PanelLeftDrawings")
                    {
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = 0,
                            xoffset = (double)PaintBoothDepth,
                            yOffset = 0,
                            Zoffset = 0,
                            paintboothSide = "D3BothSidePanels"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    
                    //Componant entry Door
                    else if (kvp.Key == "ComponantEntrySideDoor"|| kvp.Key == "ComponantEntryFrontDoor")
                    {
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = 0,
                            xoffset =0,
                            yOffset = 0,
                            Zoffset = 0,
                            paintboothSide = "ComponantEntrySideDoor"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "panelsAboveEntryDoors")
                    {
                        int panelcount = (int)PaintBoothService.noOfPanelsAboveCompEntryDoor;

                        double zOffeset = doorModel.doorHeight;
                        if (Math.Ceiling(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD + 1)
                        {
                                panelcount++;
                        }
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = 0,
                            xoffset = 0,
                            yOffset = 0,
                            Zoffset = 0,
                            paintboothSide = "panelsAboveEntryDoors"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "backDrawingList" || kvp.Key == "frontDrawingList")
                    {
                        int panelcount = PaintBoothService.noOfPanelsForBackSide;
                        int Yoffset = 0;
                        double zOffeset = 0;

                        if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide + 1)
                        {
                            Yoffset = (int)panelWidth;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide)
                        {
                            Yoffset = (int)panelWidth;
                        }
                        if (Math.Ceiling(PaintBoothService.totalPanelsforH) == PaintBoothService.noOfPanelsforH + 1)
                        {
                            zOffeset = panelheight;
                        }
                        else if (Math.Ceiling(PaintBoothService.totalPanelsforH) == PaintBoothService.noOfPanelsforH)
                        {
                            zOffeset = panelheight;
                        }
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = Yoffset,
                            Zoffset = zOffeset,
                            paintboothSide = "BackAndFrontPanels"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "frontDoorsDrawingList")
                    {
                        int panelcount = 2;
                        int Yoffset = 0;
                        double zOffeset = 0;


                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = Yoffset,
                            Zoffset = zOffeset,
                            paintboothSide = "frontDoorsDrawingList"
                        };
                        AddDrawingsToAssembly(assembly, SameSpaceBetweenDoors);
                    }
                    else if (kvp.Key == "backDrawingListBeforeExtractionC")
                    {
                        int panelcount = PaintBoothService.noOfPanelsForBackSide;
                        int Yoffset = 0;
                        double zOffeset = extractionCHeight;

                        if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide + 1)
                        {
                            Yoffset = (int)panelWidth;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide)
                        {
                            Yoffset = (int)panelWidth;
                        }
                        //zOffeset = (double)PaintboothHeight;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = (double)-D3Panel,
                            yOffset = Yoffset,
                            Zoffset = zOffeset,
                            paintboothSide = "backDrawingListInTopSide"
                        };
                        //assembly.xoffset = DraftSubType == "3" ? 0 : assembly.xoffset;
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "FilterDrawings")
                    {
                        int yOffset = 600;//Filterframe and metal Baffle Width 600
                        int zOffset = PaintBoothDesign.selectedBaffleHeight;//filterframe and Metal Baffle Height
                        int panelCount = PaintBoothDesign.bafflePanelCount;
                        //int panelCount = 1;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelCount,
                            xoffset = 0,
                            yOffset = yOffset,
                            Zoffset = zOffset,
                            paintboothSide = "BackAndFrontPanels"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "backDrawingListInTopSide")
                    {
                        int panelcount = PaintBoothService.noOfPanelsForBackSide;
                        int Yoffset = 0;
                        double zOffeset = paintBoothModel.EqualPanelWidthByH;

                        if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide + 1)
                        {
                            Yoffset = (int)panelWidth;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide)
                        {
                            Yoffset = (int)panelWidth;
                        }

                        zOffeset = (double)PaintboothHeight;

                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            //xoffset = (double)D3Panel,
                            xoffset = 0,
                            yOffset = Yoffset,
                            Zoffset = zOffeset,
                            paintboothSide = "backDrawingListInTopSide"
                        };
                        //assembly.xoffset = DraftSubType == "3" ? 0 : assembly.xoffset;
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "FrontDrawingListInTopSide")
                    {
                        int panelcount = PaintBoothService.noOfPanelsForBackSide;
                        int Yoffset = 0;
                        double zOffeset = paintBoothModel.EqualPanelWidthByH;

                        if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide + 1)
                        {
                            Yoffset = (int)panelWidth;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.TotalBackPanels) == PaintBoothService.noOfPanelsForBackSide)
                        {
                            Yoffset = (int)panelWidth;
                        }
                        zOffeset = (double)PaintboothHeight;
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = (double)D3Panel,
                            yOffset = Yoffset,
                            Zoffset = zOffeset,
                            paintboothSide = "FrontDrawingListInTopSide"
                        };
                        assembly.xoffset = DraftSubType == "3" ? 0 : assembly.xoffset;
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "LeftPanelsListInTopSideForFAS" || kvp.Key == "RightPanelsListInTopSideForFAS")
                    {
                        int panelcount = PaintBoothService.noOfPanelsforD;
                        double zOffeset = 0;
                        if (Math.Ceiling(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD + 1)
                        {
                            panelcount++;
                        }
                        else if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD)
                        {
                            panelcount++;
                        }

                        zOffeset = (double)PaintboothHeight;

                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = 0,
                            Zoffset = zOffeset,
                            paintboothSide = "TopRightLeftPanelsforFAS"
                        };

                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else if (kvp.Key == "TopAssemblyForFAS")
                    {
                        int panelcount = PaintBoothService.noOfPanelsforD;
                        double xOffset = 0;
                        double Yoffset = 0;
                        if (Math.Floor(PaintBoothService.totalPanelsforD) == PaintBoothService.noOfPanelsforD + 1)
                        {

                            panelcount++;
                        }

                        if (Math.Ceiling(PaintBoothService.totalPanelsforW) == PaintBoothService.noOfPanelsforW + 1)
                        {
                            //Yoffset = 2390;
                            Yoffset = panelheight;
                            panelcount++;
                        }
                        else if (Math.Ceiling(PaintBoothService.totalPanelsforW) == PaintBoothService.noOfPanelsforW)
                        {
                            //Yoffset = 2390;
                            Yoffset = panelheight;

                        }

                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = panelcount,
                            xoffset = 0,
                            yOffset = Yoffset,
                            Zoffset = 0,
                            paintboothSide = "TopAssemblyForFAS"
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                    else
                    {
                        AssemblyValueModel assembly = new()
                        {
                            combinedDrawing = combinedDrawing,
                            drawings = kvp.Value,
                            panelcount = -1,
                            xoffset = 0,
                            yOffset = 0,
                            Zoffset = 0
                        };
                        AddDrawingsToAssembly(assembly, 0);
                    }
                }
                return combinedDrawing;
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothController", "CombineAssemblies2", ex.Message);
                throw;
            }
        }
        #region Assembly calculations
        private void AddDrawingsToAssembly(AssemblyValueModel model, double DoorWidthForHinge)
        {
            try
            {
                // 🔹 Make sure hinge blocks exist in combined drawing
                if (!model.combinedDrawing.Blocks.Any(b => b.Name.Contains("Hinge")))
                {
                    foreach (var d in model.drawings)
                    {
                        foreach (var blk in d.Blocks)
                        {
                            if (!model.combinedDrawing.Blocks.Contains(blk.Name))
                            {
                                model.combinedDrawing.Blocks.Add(blk);
                            }
                        }
                    }
                }

                double currentXOffset = 0;
                int i = 0, j = 1;
                double z = 0;
                double y = 0;
                double? PaintboothWidth = HttpContext.Session.GetInt32("PaintboothWidth");
                double? D3Panel = HttpContext.Session.GetInt32("D3Panel");
                double? PaintBoothDepth = HttpContext.Session.GetInt32("PaintboothDepth");

                foreach (var drawing in model.drawings)
                {
                    string uniqueBlockName = "Block_" + Guid.NewGuid();

                    if (model.paintboothSide == "BottomRightLeftPanels")//Right left panel
                    {
                        if (i == model.panelcount * j)
                        {
                            currentXOffset = 0;
                            z = model.Zoffset * j;
                            j++;
                        }
                    }

                    else if (model.paintboothSide == "D3BothSidePanels")//Right left panel for type 5
                    {
                        currentXOffset = model.xoffset;

                    }
                    else if (model.paintboothSide == "frontDoorsDrawingList")//For Hinge door 
                    {
                        if (i == 0)
                        {
                            currentXOffset = 0;
                            z = 0;
                            y = 0;
                        }
                        else
                            y = (double)PaintboothWidth - DoorWidthForHinge;

                    }
                    else if (model.paintboothSide == "rightSideDoorDrawings")//For Side door 
                    {
                        if (i == 0)
                        {
                            currentXOffset = 0;
                        }
                        //model.panelcount++;
                        j = 1;
                        model.panelcount = 1;
                        if (i == model.panelcount * j)
                        {
                            currentXOffset = 0;
                            z = 2390 * j;
                            model.panelcount++;
                        }

                    }
                    else if (model.paintboothSide == "rightAndLeftSideDoorNearExtractionCDrawings")//For Side door near Extraction Chamber 
                    {
                        if (i == 0)
                        {
                            currentXOffset = model.xoffset;
                        }
                        else
                            currentXOffset = model.xoffset;
                        if (i == model.panelcount * j)
                        {
                            currentXOffset = model.xoffset;
                            z = 2390 * j;
                            model.panelcount++;
                        }
                        y = model.yOffset;
                    }
                    else if (model.paintboothSide == "TopAssembly") //Top Panel
                    {
                        if (i == 0)
                        {
                            currentXOffset = model.xoffset;
                        }
                        if (i == model.panelcount * j)
                        {
                            currentXOffset = model.xoffset;
                            y = model.yOffset * j;
                            j++;
                        }
                    }
                    else if (model.paintboothSide == "TopAssemblyForFAS") //Top Panel for FAS
                    {
                        if (i == 0)
                        {
                            currentXOffset = model.xoffset;
                        }
                        if (i == model.panelcount * j)
                        {
                            currentXOffset = model.xoffset;
                            y = model.yOffset * j;
                            j++;
                        }
                    }
                    else if (model.paintboothSide == "BackAndFrontPanels")//Back panel
                    {
                        if (i == 0 || i == (model.panelcount * j) + 1)
                        {
                            y = 0;
                        }
                        else
                            y += model.yOffset;

                        if (i == model.panelcount * j)
                        {

                            z = model.Zoffset * j;
                            y = 0;
                            j++;
                        }
                        currentXOffset = 0;
                    }

                    else if (model.paintboothSide == "FrontDrawingListInTopSide")//Back panel and Front panels in Top
                    {
                        currentXOffset = model.xoffset;
                        y = model.yOffset * i;
                        z = model.Zoffset;
                    }

                    else if (model.paintboothSide == "backDrawingListInTopSide")//Back panel in Top
                    {
                        currentXOffset = model.xoffset;
                        y = model.yOffset * i;
                        z = model.Zoffset;
                    }
                    else if (model.paintboothSide == "TopRightLeftPanelsforFAS" && i == 0)//Right left panel for FAS
                    {
                        currentXOffset = model.xoffset;
                        z = model.Zoffset;
                    }

                    //else if (model.paintboothSide == "Side Filters")
                    else if (model.yOffset != 0 && model.Zoffset == -1)
                    {
                        y = model.yOffset * i;
                        currentXOffset = 0;
                    }

                    else if (model.paintboothSide == "ComponantEntrySideDoor")
                    {
                        currentXOffset = y = z = 0;

                    }
                    else if (model.paintboothSide == "panelsAboveEntryDoors")
                    {

                        currentXOffset = y = z = 0;

                    }
                    Block blk = new Block(uniqueBlockName);
                    blk.Entities.AddRange(drawing.Entities);
                    model.combinedDrawing.Blocks.Add(blk);

                    double drawingWidth = CalculateDrawingWidth(drawing);
                    BlockReference blkReference = new BlockReference(currentXOffset, y, z, blk.Name, 0);
                    model.combinedDrawing.Entities.Add(blkReference);

                    currentXOffset += drawingWidth;
                    i++;
                }
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothController", "AddDrawingsToAssembly", ex.Message);
                throw;
            }

        }
        private void AddDrawingsToAssemblyForTopFilters(AssemblyValueModel model, string DraftSubType)
        {
            double currentXOffset = 0;
            int i = 0, j = 1;
            double z = 0;
            double y = 0;
            double? D3Panel = HttpContext.Session.GetInt32("D3Panel");
            foreach (var drawing in model.drawings)
            {
                string uniqueBlockName = "Block_" + Guid.NewGuid();
                if (i == 0)
                {
                    if (DraftSubType == "7" || DraftSubType == "5" || DraftSubType == "4" || DraftSubType == "6")
                    {
                        currentXOffset = (double)D3Panel;
                        y = XdistanceInW;
                    }
                    else
                    {
                        currentXOffset = 0;
                        y = XdistanceInW;

                    }
                }
                if (i == model.panelcount * j)
                {
                    if (DraftSubType == "7" || DraftSubType == "5" || DraftSubType == "4" || DraftSubType == "6")
                    {
                        currentXOffset = (double)D3Panel;
                        y = (model.yOffset * j) + XdistanceInW;

                    }
                    else
                    {
                        currentXOffset = 0;
                        y = (model.yOffset * j) + XdistanceInW;
                    }

                    j++;
                }
                Block blk = new Block(uniqueBlockName);
                blk.Entities.AddRange(drawing.Entities);
                model.combinedDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidthForTopFilters(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, y, z, blk.Name, 0);
                model.combinedDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
                i++;
            }
        }
        private double CalculateDrawingWidthForTopFilters(DesignDocument drawing)
        {
            if (drawing.Entities.Count == 0)
                return 0;

            double minX = double.MaxValue;
            double maxX = double.MinValue;

            foreach (Entity entity in drawing.Entities)
            {
                // Ensure the entity has bounding box data
                if (entity.BoxMin != null && entity.BoxMax != null)
                {
                    minX = Math.Min(minX, entity.BoxMin.X);
                    maxX = Math.Max(maxX, entity.BoxMax.X);
                }
            }

            // Ensure valid bounds were found
            if (minX == double.MaxValue || maxX == double.MinValue)
                return 0;

            return maxX - minX;
        }
        private double CalculateDrawingWidth(DesignDocument drawing)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;

            foreach (var entity in drawing.Entities)
            {
                if (entity.BoxMin.X < minX) minX = entity.BoxMin.X;
                else if (entity.BoxMax.X > maxX) maxX = entity.BoxMax.X;

            }

            return maxX - minX;
        } 
        #endregion
        public IActionResult FilterFrameCalculations(double FilterArea, double w, double h, double d, int lights)
        {
            var model = new PaintBoothModel
            {
                FilterArea = FilterArea,
                W = w,
                H = h,
                D = d,
                Lights = lights
            };
            ViewBag.ActivePage = "PaintBooth";
            return View(model);

        }
        public IActionResult FilterFrameBox(PaintBoothModel model)
        {
            PaintBoothDesign panel = new PaintBoothDesign(_context, _configuration);
            int? settingPanelWidth = HttpContext.Session.GetInt32("settingPanelWidth");
            int? settingPanelHeight = HttpContext.Session.GetInt32("settingPanelHeight");
            double? PaintboothHeight = HttpContext.Session.GetInt32("PaintboothHeight");

            double W = (double)settingPanelHeight;
            double H = model.H;
            int PanelWidth = (int)settingPanelWidth;
            int PanelHeight = (int)settingPanelHeight;


            double FilterAreaValue = model.FilterArea;
            double filterBaseArea = W * H;
            double D = model.D;
            if (filterBaseArea <= FilterAreaValue)
            {
                return RedirectToAction("PaintBooth");
            }
            string PaintBoothTypefromEnquiry = HttpContext.Session.GetString("DraftSubType");
            //string htmlfilepath = Path.Combine(BaseFilePath, "WebGL File");

            //if (!Directory.Exists(htmlfilepath))
            //    Directory.CreateDirectory(htmlfilepath);
            //var wgl = new devDept.Eyeshot.Translators.WriteWebGL(combinedDrawing, Path.Combine(htmlfilepath, "Scanning.html"), 0.1, false, Color.LightGray);
            //wgl.DoWork();
            // ViewBag.FilePathScanning = Path.Combine("WebGL File",  "Scanning.html");
            string salesNo = HttpContext.Session.GetString("SalesNo");

            string zipFileName = $"{salesNo}.zip";
            string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolderPath);
            string zipFilePath = Path.Combine(tempFolderPath, zipFileName);

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var filePath in AllFilesPath)
                {
                    if (string.IsNullOrWhiteSpace(filePath))
                        continue;
                    zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                }
            }
            byte[] zipBytes = System.IO.File.ReadAllBytes(zipFilePath);
            System.IO.File.Delete(zipFilePath); // Clean up temporary zip file

            return File(zipBytes, "application/zip", zipFileName);

        }
        [HttpGet]
        public JsonResult GetMotorDetails(PaintBoothModel model)
        {
            decimal BlowerHpCalculation = (decimal)model.BlowerHpCalculation;
            var records = _uow.PaintBoothRepository.GetMotorDeatils(BlowerHpCalculation);
            return Json(records);
        }

        public IActionResult GetPanelDetails()
        {
            try
            {
                List<string> developmentfilePaths = new List<string>();
                PaintBoothDesign panel = new PaintBoothDesign(_context, _configuration);
                string EnquiryCode = HttpContext.Session.GetString("SalesNo");
                var details = _uow.PaintBoothRepository.GetPanelDetailsByCode(EnquiryCode);
                List<PaintBoothclass> paintbooth = new List<PaintBoothclass>();
                int j = 0;
                foreach (var item in details)
                {

                    if (item.PanelPosition == "RightSide")
                    {
                        panel.PanelWidth = item.StandardPanelWidthForD;
                        panel.PanelHeight = item.PanelHeightforH;
                        //PaintBoothclass panels = panel.DevelopmentforRightLeftPanels(item, 0, j);
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 0, j);

                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    else if (item.PanelPosition == "LeftSide")
                    {
                        panel.PanelWidth = item.StandardPanelWidthForD;
                        panel.PanelHeight = item.PanelHeightforH;
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 1, j);
                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    else if (item.PanelPosition == "D3Panels Right side")
                    {
                        panel.PanelWidth = item.StandardPanelWidthForD;
                        panel.PanelHeight = item.PanelHeightforH;
                        //PaintBoothclass panels = panel.DevelopmentforD3Panels(item, 0, j);
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 2, j);
                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    else if (item.PanelPosition == "D3Panels Left Side")
                    {
                        panel.PanelWidth = item.StandardPanelWidthForD;
                        panel.PanelHeight = item.PanelHeightforH;
                        //PaintBoothclass panels = panel.DevelopmentforD3Panels(item, 1, j);
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 3, j);
                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    else if (item.PanelPosition.Contains("TopPanels"))
                    {
                        //PaintBoothclass panels = panel.DevelopmentforTopPanels(item);
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 4, j);
                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    else if (item.PanelPosition.Contains("BackPanels"))
                    {
                        panel.PanelWidth = item.StandardPanelWidthForD;
                        panel.PanelHeight = item.PanelHeightforH;
                        //PaintBoothclass panels = panel.DevelopmentforBackPanels(item, j);
                        PaintBoothclass panels = panel.DevelopmentForAllPanels(item, 5, j);
                        paintbooth.Add(panels);
                        developmentfilePaths.Add(panels.developmentpath);
                    }
                    j++;
                }
                // Create ZIP folder with development files and allow download
                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var filePath in developmentfilePaths)
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                string fileName = Path.GetFileName(filePath);
                                var zipEntry = zipArchive.CreateEntry(fileName, CompressionLevel.Optimal);

                                using (var entryStream = zipEntry.Open())
                                using (var fileStream = System.IO.File.OpenRead(filePath))
                                {
                                    fileStream.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                    string Developmentfile = $"Development_{EnquiryCode}.zip";
                    return File(memoryStream.ToArray(), "application/zip", Developmentfile);
                }

            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothController", "GetPanelDetails", ex.Message);
                throw;
            }

        }

        public IActionResult WaterColumnCalculations()
        {

            var pressureDropData = _uow.PaintBoothRepository.GetAallWaterColumnDetails();

            // Map database entities to the view model
            var model = pressureDropData.Select(x => new Bullows.Model.PressureDrop
            {
                ItemNumber = x.ItemNumber,
                Description = x.Description,
                PressureDrop_mm = x.PressureDrop_mm,

            }).ToList();

            return Json(new { pressureDropDetails = model }); // Return the data as JSON
        }
    }
}
public class AssemblyValueModel
{
    public DesignDocument combinedDrawing { get; set; }
    public List<DesignDocument> drawings { get; set; }
    public double xoffset { get; set; }
    public double yOffset { get; set; }
    public double Zoffset { get; set; }
    public int panelcount { get; set; }
    public string paintboothSide { get; set; }

}
