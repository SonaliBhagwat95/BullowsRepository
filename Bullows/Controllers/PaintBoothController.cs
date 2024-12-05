
using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
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
        public PaintBoothController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, BullowsDbContext context) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.JsonResult SearchEnquiryCode(string enquiryCode)
        {
            var results = _uow.settingDetailsRepository.SearchEnquiryCodes(enquiryCode);
            if (results == null || !results.Any())
            {
                return Json(new { success = false, message = "No Enquiry Codes found" });
            }
            return Json(new { success = true, results });
            // return Json(new { success = true, results = results.Select(r => r.enquiryCode) });
        }

        [HttpGet]
        public IActionResult GetSettingDetailsByCode(string enquiryCode)
        {
            var paintBoothModel = _uow.PaintBoothRepository.GetSettingDetailsByCode(enquiryCode);
            return Json(new { settings = paintBoothModel.Settings });
        }
        #region Save Paintbooth Details
        public static Dictionary<string, List<DesignDocument>> designdictionary = new();
        public static List<string> AllFilesPath = new List<string>();
        public static List<string> developmentpath = new List<string>();
        static double smallPanelWidthforH;
        static double totalPanelsforD;
        static int noOfPanelsforH;
        static int noOfPanelsforD;
        static double smallPanelWidthforD, smallPanelWidthForBackSide;
        static double W;
        static double smallPanelWidthforW;
        static int noOfPanelsforW, noOfPanelsForBackSide;
        static double totalPanelsforW, TotalBackPanels; static double totalPanelsforH; static string motorTypes = "";
        static PaintBoothModel paintBoothModel = new PaintBoothModel();


        List<string> filePaths = new List<string>();

        double yOffset = 0;
        #region DesignDocument List
        List<DesignDocument> rightSideDrawings = new List<DesignDocument>();
        List<DesignDocument> leftSideDrawings = new List<DesignDocument>();
        List<DesignDocument> TopAssembly = new List<DesignDocument>();
        List<DesignDocument> BaseDrawings = new List<DesignDocument>();
        List<DesignDocument> TopFrameDrawing = new List<DesignDocument>();
        List<DesignDocument> TopAssemblyYAxis = new List<DesignDocument>();
        List<DesignDocument> FilterDrawings = new List<DesignDocument>();
        List<DesignDocument> ViewsDrawing = new List<DesignDocument>();
        List<DesignDocument> loftDrawing = new List<DesignDocument>();
        List<DesignDocument> standardFrameDrawing= new List<DesignDocument>();
        public IActionResult SavePaintBoothDetails(PaintBoothModel model, PanelInputModel pmodel, int flag, EnquiryModel enquiry, int motorCatalogID)
        {
            try
            {
                HttpContext.Session.SetString("SalesNo", enquiry.SalesNO);
                string EnquiryNo = HttpContext.Session.GetString("SalesNo");

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
               // motorTypes =EnquiryController.MotorTypesval;//save this value in MotorDetails table 
                _uow.PaintBoothRepository.SaveMotorDeatils((int)enquiryId, motorCatalogID);

                var paintBoothModel = _uow.PaintBoothRepository.GetSettingDetailsByCode(EnquiryNo);
                if (paintBoothModel.Settings.Count == 0)
                {
                    return NotFound("No settings found for the given SalesNo.");
                }

                // Assuming you want the first setting for the PaintBoothDesign
                var setting = paintBoothModel.Settings.First();

                // Create and populate the PaintBoothDesign object
                PaintBoothDesign panel = new PaintBoothDesign(_context)
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

                #region  D
                if (model.PanelTypes == "1")
                {
                    var panelSizeMappings = new Dictionary<string, string>
                        {
                        { "1", "1" },
                        { "2", "1.5" },
                        { "3", "2" },
                        { "4", "2.5" },
                        { "5", "3" },
                        { "6", "3.5" },
                        { "7", "4" },
                        { "8", "4.5" },
                        { "9", "5" },
                        { "10", "5.5" },
                        { "11", "6" },
                        { "12", "6.5" },
                        { "13", "7" },
                        { "14", "7.5" }
                        };
                    panel.PanelWidth = setting.PanelWidth;

                    //noOfPanelsforD = (int)model.PanelsizeforD;
                    string selectedValue = model.PanelsizeforD;
                    string correspondingText = "";

                    // Attempt to find the corresponding text value
                    if (panelSizeMappings.TryGetValue(selectedValue, out correspondingText))
                    {
                        // Use the correspondingText as needed in your logic
                        model.PanelsizeforD = correspondingText;
                        double panelSizeDouble = double.Parse(model.PanelsizeforD);
                        noOfPanelsforD = (int)Math.Floor(panelSizeDouble);
                        totalPanelsforD = noOfPanelsforD;
                    }
                    else
                    {
                        // Handle case where value is not found in the mapping, if necessary
                        model.PanelsizeforD = "Invalid selection"; // Example fallback
                    }

                    if (model.PanelsizeforD.ToString().Contains("."))
                    {
                        smallPanelWidthforD = model.HalfPanelswidthforD;
                    }
                    else
                    {
                        // Default logic if it's not a decimal value
                        smallPanelWidthforD = 0;
                    }

                }
                else
                {
                    panel.PanelWidth = setting.PanelWidth;
                    totalPanelsforD = model.D / setting.PanelWidth;
                    noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                    smallPanelWidthforD = model.D - (noOfPanelsforD * setting.PanelWidth);
                    if (model.MakeItEqual == true)
                    {
                        noOfPanelsforD = model.TotalPanels;
                        panel.PanelWidth = model.EqualPanelWidthForD;
                        smallPanelWidthforD = 0;
                    }
                }

                #endregion
                #region W
                if (model.PanelTypesforW == "1")
                {
                    var panelSizeMappingsforW = new Dictionary<string, string>
                        {
                        { "1", "1" },
                        { "2", "1.5" },
                        { "3", "2" },
                        { "4", "2.5" },
                        { "5", "3" },
                        { "6", "3.5" },
                        { "7", "4" },
                        { "8", "4.5" },
                        { "9", "5" },
                        { "10", "5.5" },
                        { "11", "6" },
                        { "12", "6.5" },
                        { "13", "7" },
                        { "14", "7.5" }
                        };
                    panel.PanelLength = setting.PanelHeight;
                    string selectedValue = model.PanelsizeforW;
                    string correspondingText = "";

                    // Attempt to find the corresponding text value
                    if (panelSizeMappingsforW.TryGetValue(selectedValue, out correspondingText))
                    {
                        // Use the correspondingText as needed in your logic
                        model.PanelsizeforW = correspondingText;
                        double panelSizeDouble = double.Parse(model.PanelsizeforW);
                        noOfPanelsforW = (int)Math.Floor(panelSizeDouble);
                        totalPanelsforW = noOfPanelsforW;
                    }
                    else
                    {
                        // Handle case where value is not found in the mapping, if necessary
                        model.PanelsizeforW = "Invalid selection"; // Example fallback
                    }

                    if (model.PanelsizeforW.ToString().Contains("."))
                    {
                        smallPanelWidthforW = model.HalfPanelsHeightforW;
                    }
                    else
                    {
                        // Default logic if it's not a decimal value
                        smallPanelWidthforW = 0;
                    }

                }
                else
                {
                    panel.PanelLength = setting.PanelHeight;
                    totalPanelsforW = model.W / setting.PanelHeight;
                    noOfPanelsforW = (int)Math.Floor(totalPanelsforW);
                    smallPanelWidthforW = model.W - (noOfPanelsforW * setting.PanelHeight);
                    if (model.MakeItEqualByW == true)
                    {
                        noOfPanelsforW = model.TotalPanelsByW;
                        panel.PanelLength = model.EqualPanelWidthByW;
                        smallPanelWidthforW = 0;
                    }
                }

                #endregion
                #region H
                if (model.PanelTypesforH == "1")
                {
                    var panelSizeMappingsforH = new Dictionary<string, string>
                        {
                        { "1", "1" },
                        { "2", "1.5" },
                        { "3", "2" },
                        { "4", "2.5" },
                        { "5", "3" },
                        { "6", "3.5" },
                        { "7", "4" },
                        { "8", "4.5" },
                        { "9", "5" },
                        { "10", "5.5" },
                        { "11", "6" },
                        { "12", "6.5" },
                        { "13", "7" },
                        { "14", "7.5" }
                        };
                    panel.PanelHeight = setting.PanelHeight;
                    string selectedValue = model.PanelsizeforH;
                    string correspondingText = "";

                    // Attempt to find the corresponding text value
                    if (panelSizeMappingsforH.TryGetValue(selectedValue, out correspondingText))
                    {
                        // Use the correspondingText as needed in your logic
                        model.PanelsizeforH = correspondingText;
                        double panelSizeDouble = double.Parse(model.PanelsizeforH);
                        noOfPanelsforH = (int)Math.Floor(panelSizeDouble);
                        totalPanelsforH = noOfPanelsforH;
                    }
                    else
                    {
                        // Handle case where value is not found in the mapping, if necessary
                        model.PanelsizeforH = "Invalid selection"; // Example fallback
                    }

                    if (model.PanelsizeforH.ToString().Contains("."))
                    {
                        smallPanelWidthforH = model.HalfPanelsHeightforH;
                    }
                    else
                    {
                        // Default logic if it's not a decimal value
                        smallPanelWidthforH = 0;
                    }

                }
                else
                {
                    panel.PanelHeight = setting.PanelHeight;
                    totalPanelsforH = model.H / setting.PanelHeight;
                    noOfPanelsforH = (int)Math.Floor(totalPanelsforH);
                    smallPanelWidthforH = model.H - (noOfPanelsforH * setting.PanelHeight);

                    if (model.MakeItEqualByH == true)
                    {
                        noOfPanelsforH = model.TotalPanelsByH;
                        panel.PanelHeight = model.EqualPanelWidthByH;
                        smallPanelWidthforH = 0;
                    }
                }

                #endregion
                string selectedLocation = model.ServiceDoorLocation;

                // Check if the record already exists
                if (!_uow.PaintBoothRepository.Exists(enquiry.EnquiryId))
                {
                    PaintID = _uow.PaintBoothRepository.SavePaintBooth(model, pmodel, flag, enquiry);
                }
                double yOffset = 0;
                double PanelWidthTemp = panel.PanelWidth;
                double PanelLengthTemp = panel.PanelLength;
                double PanelHeightTemp = panel.PanelHeight;
                #region D * H left right
                int i = 0;
                for (i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        panel.PanelLength = 0;
                    }
                    else
                    {
                        if (model.PanelTypesforW == "1")
                        {
                            panel.PanelLength = model.PanelHeightforW;
                        }
                        else
                            panel.PanelLength = model.W;
                    }


                    List<DesignDocument> documents = new List<DesignDocument>();


                    for (int k = 0; k < noOfPanelsforH; k++)
                    {
                        for (int j = 0; j < noOfPanelsforD; j++)
                        {
                            PaintBoothclass panelDrawingPath = panel.PanelsInPaintBooth(j + 1, yOffset, pmodel, model, i);
                            documents.Add(panelDrawingPath.drawing);

                            filePaths.Add(panelDrawingPath.lstpath);


                        }
                        if (smallPanelWidthforD > 0)
                        {
                            panel.PanelWidth = smallPanelWidthforD;
                            PaintBoothclass smallPanelDrawingPath = panel.PanelsInPaintBooth(noOfPanelsforD + 1, yOffset, pmodel, model, i);
                            documents.Add(smallPanelDrawingPath.drawing);
                            filePaths.Add(smallPanelDrawingPath.lstpath);

                            // developmentfilePaths.Add(smallPanelDrawingPath.developmentpath);
                            panel.PanelWidth = PanelWidthTemp;

                        }
                        selectedLocation = model.ServiceDoorLocation;
                        PaintBoothclass panelDrawingPathD3 = panel.D3Panels(k + 1, i, selectedLocation, model);
                        documents.Add(panelDrawingPathD3.drawing);
                        filePaths.Add(panelDrawingPathD3.lstpath);
                        // developmentfilePaths.Add(panelDrawingPathD3.developmentpath);

                    }
                    if (smallPanelWidthforH > 0)
                    {
                        panel.PanelHeight = smallPanelWidthforH;

                        for (int j = 0; j < noOfPanelsforD; j++)
                        {
                            PaintBoothclass panelDrawingPath = panel.PanelsInPaintBooth(j + 1, yOffset, pmodel, model, i);
                            documents.Add(panelDrawingPath.drawing);

                            filePaths.Add(panelDrawingPath.lstpath);
                            //  developmentfilePaths.Add(panelDrawingPath.developmentpath);

                        }
                        if (smallPanelWidthforD > 0)
                        {
                            panel.PanelWidth = smallPanelWidthforD;
                            PaintBoothclass smallPanelDrawingPath = panel.PanelsInPaintBooth(noOfPanelsforD + 1, yOffset, pmodel, model, i);

                            documents.Add(smallPanelDrawingPath.drawing);

                            filePaths.Add(smallPanelDrawingPath.lstpath);
                            //  developmentfilePaths.Add(smallPanelDrawingPath.developmentpath);
                            panel.PanelWidth = PanelWidthTemp;

                        }
                        selectedLocation = "";
                        PaintBoothclass panelDrawingPathD3 = panel.D3Panels(noOfPanelsforH + 1, i, selectedLocation, model);
                        documents.Add(panelDrawingPathD3.drawing);
                        filePaths.Add(panelDrawingPathD3.lstpath);
                        //  developmentfilePaths.Add(panelDrawingPathD3.developmentpath);

                        panel.PanelHeight = PanelHeightTemp;
                    }
                    if (i == 0)
                    {
                        rightSideDrawings.AddRange(documents);

                    }
                    else if (i == 1)
                    {
                        leftSideDrawings.AddRange(documents);


                    }
                }
                panel.PanelLength = PanelLengthTemp;
                #endregion

                //panel.PanelHeight = model.H1 + model.Height + model.H2;
                if (model.PanelTypesforH == "1")
                    panel.PanelHeight = model.PanelHeightforH;
                else
                    panel.PanelHeight = model.H;
                // panel.PanelHeight = setting.PanelHeight;

                //PaintBoothclass loft = panel.CreateLoft( model);
                //string filepathforLoft =loft.lstpath;

                #region W*D Top 
                List<DesignDocument> docdrawing = new List<DesignDocument>();
                List<string> Topsidepath = new List<string>();
                for (i = 0; i < noOfPanelsforW; i++)
                {
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        PaintBoothclass Topside = panel.TopSidePanels(k + 1, pmodel, model);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        //developmentfilePaths.Add(Topside.developmentpath);
                    }
                    if (smallPanelWidthforD > 0)
                    {
                        panel.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = panel.TopSidePanels(noOfPanelsforW + 1, pmodel, model);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        // developmentfilePaths.Add(Topside.developmentpath);

                        panel.PanelWidth = PanelWidthTemp;
                    }
                }
                if (smallPanelWidthforW > 0)
                {
                    panel.PanelLength = smallPanelWidthforW;
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        PaintBoothclass Topside = panel.TopSidePanels(k + 1, pmodel, model);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                    }
                    if (smallPanelWidthforD > 0)
                    {
                        panel.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = panel.TopSidePanels(noOfPanelsforW + 1, pmodel, model);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        //  developmentfilePaths.Add(Topside.developmentpath);

                        panel.PanelWidth = PanelWidthTemp;
                    }
                    panel.PanelLength = PanelLengthTemp;
                }

                TopAssembly.AddRange(docdrawing);
                #endregion
                #region Total BackPanels
                double BackLengthTemp = 0;
                if (model.PanelTypes == "1")
                {
                    model.W = model.PanelHeightforW;
                    panel.BackPanelLength = setting.PanelWidth;
                    BackLengthTemp = panel.BackPanelLength;
                    TotalBackPanels = model.W / setting.PanelWidth;
                    noOfPanelsForBackSide = (int)Math.Floor(TotalBackPanels);
                    smallPanelWidthForBackSide = 0;
                    panel.PanelHeight = PanelHeightTemp;

                }
                else
                {
                    // model.W = model.W;
                    panel.BackPanelLength = setting.PanelWidth;
                    BackLengthTemp = panel.BackPanelLength;
                    TotalBackPanels = model.W / setting.PanelWidth;
                    noOfPanelsForBackSide = (int)Math.Floor(TotalBackPanels);
                    smallPanelWidthForBackSide = model.W - (noOfPanelsForBackSide * setting.PanelWidth);
                    panel.PanelHeight = PanelHeightTemp;
                }

                #endregion

                #region W * H back 
                List<DesignDocument> backDrawingList = new List<DesignDocument>();
                for (int k = 0; k < noOfPanelsforH; k++)
                {
                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        PaintBoothclass panelDrawingPath = panel.BackPanels(model);
                        backDrawingList.Add(panelDrawingPath.drawing);
                        filePaths.Add(panelDrawingPath.lstpath);
                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        panel.BackPanelLength = smallPanelWidthForBackSide;
                        PaintBoothclass smallPanelDrawingPath = panel.BackPanels(model);
                        backDrawingList.Add(smallPanelDrawingPath.drawing);
                        panel.BackPanelLength = BackLengthTemp;
                    }
                }
                if (smallPanelWidthforH > 0)
                {
                    panel.PanelHeight = smallPanelWidthforH;

                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        PaintBoothclass panelDrawingPath = panel.BackPanels(model);
                        backDrawingList.Add(panelDrawingPath.drawing);
                        filePaths.Add(panelDrawingPath.lstpath);
                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        panel.BackPanelLength = smallPanelWidthForBackSide;
                        PaintBoothclass smallPanelDrawingPath = panel.BackPanels(model);
                        backDrawingList.Add(smallPanelDrawingPath.drawing);
                        panel.BackPanelLength = BackLengthTemp;
                    }
                    panel.PanelHeight = model.H;
                }
                #endregion
                #region BaseStructure
                if (model.MakeItEqualByW)
                {
                    panel.W = Math.Floor(model.EqualPanelWidthByW);
                }
                else
                {
                    panel.W = model.W;
                }
                PaintBoothclass basestructure = panel.BaseStructure(pmodel, model);
                List<DesignDocument> basedrawing = new List<DesignDocument>();
                List<string> basepath = new List<string>();
                basedrawing.Add(basestructure.drawing);
                basepath.Add(basestructure.lstpath);
                BaseDrawings.AddRange(basedrawing);
                DesignDocument BaseDrawingAss = BaseStructure(basedrawing, 0);

                string BaseFilePath = "C:/Bullows/Paintbooth_Drawing";

                if (!Directory.Exists(BaseFilePath))
                    Directory.CreateDirectory(BaseFilePath);
                string BaseDwgFilePath = BaseFilePath + "/BaseStructure_Drawing.dwg";
                WriteAutodeskParams Basedwg = new WriteAutodeskParams(BaseDrawingAss);
                WriteAutodesk dwgWriterBase = new WriteAutodesk(Basedwg, BaseDwgFilePath);
                dwgWriterBase.DoWork();


                #endregion

                List<PaintBoothclass> filterFrames = panel.OuterFilterFrame(model);
                List<DesignDocument> filterDrawings = new List<DesignDocument>();
                List<string> filterPaths = new List<string>();
                foreach (PaintBoothclass filterFrame in filterFrames)
                {
                    filterDrawings.Add(filterFrame.drawing);
                    filterPaths.Add(filterFrame.lstpath);
                }

                // Optionally, if you have an existing list named FilterDrawings, you can add all drawings at once
                FilterDrawings.AddRange(filterDrawings);


                #region TotalAssembly

                #region TopFrameStructure               
                PaintBoothclass Topstructure = panel.TopStructureFrame(pmodel, model);
                List<DesignDocument> TopStructuredrawing = new List<DesignDocument>();
                List<string> TopFramepath = new List<string>();
                TopStructuredrawing.Add(Topstructure.drawing);
                TopFramepath.Add(Topstructure.lstpath);
                TopFrameDrawing.AddRange(TopStructuredrawing);
                DesignDocument TopDrawingAss = TopStructure(TopStructuredrawing, 0);
                string TopFrameDwgFilePath = BaseFilePath + "/TopStructure_Drawing.dwg";
                WriteAutodeskParams topframedwg = new WriteAutodeskParams(TopDrawingAss);
                WriteAutodesk dwgWritertopframe = new WriteAutodesk(topframedwg, TopFrameDwgFilePath);
                dwgWritertopframe.DoWork();


                #endregion


                #region RightSide                
                DesignDocument rightSideAssembly = RightSideWallAssembly(rightSideDrawings, 0, setting.PanelHeight, model);
                string rightSideDwgFilePath = BaseFilePath + "/RightPanel.dwg";
                WriteAutodeskParams autoRight = new WriteAutodeskParams(rightSideAssembly);
                WriteAutodesk dwgWriterRight = new WriteAutodesk(autoRight, rightSideDwgFilePath);
                dwgWriterRight.DoWork();

                PaintBoothclass loft = panel.CreateLoft(model);
                string filepathforLoft = loft.lstpath;
                DesignDocument loftdrawing = new DesignDocument();
                loftdrawing = loft.drawing;
                WriteAutodeskParams loftdw = new WriteAutodeskParams(loftdrawing);
                WriteAutodesk dwgWriterloft = new WriteAutodesk(loftdw, filepathforLoft);
                dwgWriterloft.DoWork();
                loftDrawing.Add(loftdrawing);


                
                #endregion
                #region LeftSide
                // Create and save LeftSideWallAssembly
                DesignDocument leftSideAssembly = LeftSideWallAssembly(leftSideDrawings, 0);
                string leftSideDwgFilePath = BaseFilePath + "/LeftPanel.dwg";
                WriteAutodeskParams autoLeft = new WriteAutodeskParams(leftSideAssembly);
                WriteAutodesk dwgWriterLeft = new WriteAutodesk(autoLeft, leftSideDwgFilePath);
                dwgWriterLeft.DoWork();
                #endregion
                #region TopDrawing
                DesignDocument TopDrawing = TopWallAssembly(TopAssembly, 0);
                string TopDwgFilePath = "C:/Bullows/Paintbooth_Drawing/TopPanel.dwg";
                WriteAutodeskParams Topdwg = new WriteAutodeskParams(TopDrawing);
                WriteAutodesk dwgWriterTop = new WriteAutodesk(Topdwg, TopDwgFilePath);
                dwgWriterTop.DoWork();
                #endregion
                #region BackPanels
                DesignDocument BackPanelsDrawing = BackPanelsAssembly(backDrawingList, 0);
                string BackPanelsDwgFilePath = "C:/Bullows/Paintbooth_Drawing/RearPanel.dwg";
                WriteAutodeskParams backdwg = new WriteAutodeskParams(BackPanelsDrawing);
                WriteAutodesk back = new WriteAutodesk(backdwg, BackPanelsDwgFilePath);
                back.DoWork();

                #endregion
                

                #region Added value in Dictionary
                designdictionary = new();
                designdictionary.Add("rightSideDrawings", rightSideDrawings);
                designdictionary.Add("leftSideDrawings", leftSideDrawings);
                designdictionary.Add("TopAssembly", TopAssembly);
                designdictionary.Add("BaseDrawings", BaseDrawings);
                designdictionary.Add("TopStructuredrawing", TopStructuredrawing);
                designdictionary.Add("backDrawingList", backDrawingList);
                designdictionary.Add("FilterDrawings", FilterDrawings);
                designdictionary.Add("ViewsDrawing", ViewsDrawing);
                designdictionary.Add("loftdrawing", loftDrawing);
                //designdictionary.Add("standardFrameDrawing", standardFrameDrawing);

                //designdictionary.Add("bendLayer", additionalLayerDrawing);
                #endregion
                #region WriteFile
                DesignDocument combinedDrawing = CombineAssemblies2(designdictionary, setting.PanelHeight, setting.PanelWidth);

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
                    rightSideDwgFilePath,
                    leftSideDwgFilePath,
                    TopDwgFilePath,
                    BaseDwgFilePath,
                    combinedDwgFilePath,
                    TopFrameDwgFilePath,
                    BackPanelsDwgFilePath,
                    StepFilePath,
                    pdfFilePath,
                   filepathforLoft,
                   filepathforstandardframe

                };
                AllFilesPath.Add(paintBoothClass.lstpath);
               // AllFilesPath.Add(standardframe.lstpath);
                #endregion

                return RedirectToAction("FilterFrameCalculations", model);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private DesignDocument RightSideWallAssembly(List<DesignDocument> drawings, double yOffset, double panelheight, PaintBoothModel model)
        {
            PanelInputModel p = new PanelInputModel();


            var assemblyDrawing = new DesignDocument();
            assemblyDrawing.Units = linearUnitsType.Millimeters;
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            assemblyDrawing.Layers.Add(mylayer);

            double currentXOffset = 0, Woffset = 0;

            int i = 0;
            int TotalPanelOfH = 0;

            foreach (var drawing in drawings)
            {

                if (smallPanelWidthforH > 0)
                    TotalPanelOfH = noOfPanelsforH + 1;

                if (i == TotalPanelOfH)
                {
                    currentXOffset = 0;
                    Woffset = panelheight;

                }
                Block blk = new Block("Block_" + Guid.NewGuid());

                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, Woffset, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
                i++;
            }
            return assemblyDrawing;
        }
        private DesignDocument TopWallAssembly(List<DesignDocument> Docdrawing, double yOffset)
        {
            var assemblyDrawing = new DesignDocument();
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            assemblyDrawing.Layers.Add(mylayer);
            assemblyDrawing.Units = linearUnitsType.Millimeters;

            double currentXOffset = 0;

            foreach (var drawing in Docdrawing)
            {

                Block blk = new Block("Block_" + Guid.NewGuid());

                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, 0, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
            }
            return assemblyDrawing;
        }
        private DesignDocument BackPanelsAssembly(List<DesignDocument> YAxisdrawing, double yOffset)
        {
            var assemblyDrawing = new DesignDocument();
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            assemblyDrawing.Layers.Add(mylayer);
            assemblyDrawing.Units = linearUnitsType.Millimeters;

            double currentXOffset = 0;

            foreach (var drawing in YAxisdrawing)
            {

                Block blk = new Block("Block_" + Guid.NewGuid());

                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, 0, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
            }
            return assemblyDrawing;
        }
        private DesignDocument BaseStructure(List<DesignDocument> basedrawing, double yOffset)
        {
            var assemblyDrawing = new DesignDocument();
            assemblyDrawing.Units = linearUnitsType.Millimeters;

            double currentXOffset = 0;

            foreach (var drawing in basedrawing)
            {

                Block blk = new Block("Block_" + Guid.NewGuid());

                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, 0, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
            }
            return assemblyDrawing;
        }
        private DesignDocument TopStructure(List<DesignDocument> TopStructuredrawing, double yOffset)
        {
            var assemblyDrawing = new DesignDocument();
            assemblyDrawing.Units = linearUnitsType.Millimeters;

            double currentXOffset = 0;

            foreach (var drawing in TopStructuredrawing)
            {

                Block blk = new Block("Block_" + Guid.NewGuid());

                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, 0, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
            }
            return assemblyDrawing;
        }
        private DesignDocument LeftSideWallAssembly(List<DesignDocument> drawings, double yOffset)
        {
            // Same as RightSideWallAssembly but with the yOffset parameter
            var assemblyDrawing = new DesignDocument();
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            assemblyDrawing.Layers.Add(mylayer);
            assemblyDrawing.Units = linearUnitsType.Millimeters;

            double currentXOffset = 0;

            foreach (var drawing in drawings)
            {
                Block blk = new Block("Block_" + Guid.NewGuid());
                blk.Entities.AddRange(drawing.Entities);
                assemblyDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, yOffset, 0, blk.Name, 0);
                assemblyDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
            }
            return assemblyDrawing;
        }
        private DesignDocument CombineAssemblies2(Dictionary<string, List<DesignDocument>> designDocument, double panelheight, double panelWidth)
        {
            int? settingPanelWidth = HttpContext.Session.GetInt32("settingPanelWidth");
            int? settingPanelHeight = HttpContext.Session.GetInt32("settingPanelHeight");

            var combinedDrawing = new DesignDocument();
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            combinedDrawing.Layers.Add(mylayer);
            combinedDrawing.Units = linearUnitsType.Millimeters;

            foreach (var kvp in designDocument)
            {
                if (kvp.Key == "rightSideDrawings" || kvp.Key == "leftSideDrawings")
                {
                    int panelcount = noOfPanelsforD;

                    double zOffeset = paintBoothModel.EqualPanelWidthByH;
                    if (Math.Ceiling(totalPanelsforD) == noOfPanelsforD + 1)
                    {

                        panelcount += 2;
                    }
                    else if (Math.Floor(totalPanelsforD) == noOfPanelsforD)
                    {

                        panelcount++;
                    }
                    if (Math.Ceiling(totalPanelsforH) == noOfPanelsforH + 1)
                    {
                        //zOffeset = 2390;
                        zOffeset = panelheight;

                    }
                    else
                        zOffeset = panelheight;

                    AddDrawingsToAssembly(combinedDrawing, kvp.Value, 0, zOffeset, panelcount);
                }
                else if (kvp.Key == "TopAssembly")
                {
                    int panelcount = noOfPanelsforD;
                    double Yoffset = paintBoothModel.EqualPanelWidthByW;
                    if (Math.Floor(totalPanelsforD) == noOfPanelsforD + 1)
                    {

                        panelcount++;
                    }

                    if (Math.Ceiling(totalPanelsforW) == noOfPanelsforW + 1)
                    {
                        //Yoffset = 2390;
                        Yoffset = panelheight;
                        panelcount++;
                    }
                    else if (Math.Ceiling(totalPanelsforW) == noOfPanelsforW)
                    {
                        //Yoffset = 2390;
                        Yoffset = panelheight;

                    }


                    AddDrawingsToAssembly(combinedDrawing, kvp.Value, Yoffset, 0, panelcount);

                }
                else if (kvp.Key == "backDrawingList")
                {
                    int panelcount = noOfPanelsForBackSide;
                    int Yoffset = 0;
                    double zOffeset = paintBoothModel.EqualPanelWidthByH;

                    if (Math.Ceiling(TotalBackPanels) == noOfPanelsForBackSide + 1)
                    {
                        Yoffset = (int)panelWidth;
                        panelcount++;
                    }
                    else if (Math.Ceiling(TotalBackPanels) == noOfPanelsForBackSide)
                    {
                        Yoffset = (int)panelWidth;
                        // panelcount++;
                    }
                    if (Math.Ceiling(totalPanelsforH) == noOfPanelsforH + 1)
                    {
                        zOffeset = panelheight;
                    }
                    else if (Math.Ceiling(totalPanelsforH) == noOfPanelsforH)
                    {


                        zOffeset = panelheight;

                    }

                    AddDrawingsToAssembly(combinedDrawing, kvp.Value, Yoffset, zOffeset, panelcount);
                }
                else if (kvp.Key == "FilterDrawings")
                {
                    int yOffset = 600;//Filterframe and metal Baffle Width
                    int zOffset = PaintBoothDesign.selectedBaffleHeight;//filterframe and Metal Baffle Height
                    int panelCount = PaintBoothDesign.bafflePanelCount;
                    AddDrawingsToAssembly(combinedDrawing, kvp.Value, yOffset, zOffset, panelCount);


                }
                else
                    AddDrawingsToAssembly(combinedDrawing, kvp.Value, 0, 0, -1);
            }
            return combinedDrawing;
        }
        private void AddDrawingsToAssembly(DesignDocument combinedDrawing, List<DesignDocument> drawings, double yOffset, double Zoffset, int panelcount)
        {
            double currentXOffset = 0;
            int i = 0, j = 1;
            double z = 0;
            double y = 0;
            foreach (var drawing in drawings)
            {
                string uniqueBlockName = "Block_" + Guid.NewGuid();

                if (Zoffset != 0 && yOffset == 0 && Zoffset != -1)//Right left panel
                {
                    if (i == panelcount * j)
                    {
                        currentXOffset = 0;
                        z = Zoffset * j;
                        j++;
                    }
                }
                else if (yOffset != 0 && Zoffset == 0 && Zoffset != -1) //Top Panel
                {

                    if (i == panelcount * j)
                    {
                        currentXOffset = 0;
                        //y = yOffset * i;
                        y = yOffset * j;
                        j++;
                    }
                }
                else if (yOffset != 0 && Zoffset != 0 && Zoffset != -1)//Back panel
                {
                    if (i == 0 || i == (panelcount * j) + 1)
                    {
                        y = 0;
                    }
                    else
                        y += yOffset;

                    if (i == panelcount * j)
                    {

                        z = Zoffset * j;
                        y = 0;
                        j++;
                    }
                    currentXOffset = 0;
                }
                else if (yOffset != 0 && Zoffset == -1)
                {

                    y = yOffset * i;
                    currentXOffset = 0;
                }


                Block blk = new Block(uniqueBlockName);
                blk.Entities.AddRange(drawing.Entities);
                combinedDrawing.Blocks.Add(blk);

                double drawingWidth = CalculateDrawingWidth(drawing);
                BlockReference blkReference = new BlockReference(currentXOffset, y, z, blk.Name, 0);
                combinedDrawing.Entities.Add(blkReference);

                currentXOffset += drawingWidth;
                i++;
            }
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
            PaintBoothDesign panel = new PaintBoothDesign(_context);


            int? settingPanelWidth = HttpContext.Session.GetInt32("settingPanelWidth");
            int? settingPanelHeight = HttpContext.Session.GetInt32("settingPanelHeight");

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

            DesignDocument combinedDrawing = CombineAssemblies2(designdictionary, PanelHeight, PanelWidth);
            string BaseFilePath = "C:/Bullows/Paintbooth_Drawing";
            string combinedDwgFilePath = BaseFilePath + "/GA3D_Drawing.dwg";
            WriteAutodeskParams autoCombined = new WriteAutodeskParams(combinedDrawing);
            WriteAutodesk dwgWriterCombined = new WriteAutodesk(autoCombined, combinedDwgFilePath);
            dwgWriterCombined.DoWork();
            string salesNo = HttpContext.Session.GetString("SalesNo");

            string zipFileName = $"{salesNo}.zip";
            string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolderPath);
            string zipFilePath = Path.Combine(tempFolderPath, zipFileName);

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var filePath in AllFilesPath)
                {
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
        //[HttpPost]
        //public IActionResult GetEnquiryCodes(string term)
        //{
        //    var enquiryCodes = _uow.PaintBoothRepository.GetEnquiryCodes(term);
        //    var result = new
        //    {
        //        success = enquiryCodes.Any(), 
        //        results = enquiryCodes 
        //    };
        //    return Json(result);
        //}

        public IActionResult GetPanelDetails()
        {
            List<string> developmentfilePaths = new List<string>();
            PaintBoothDesign panel = new PaintBoothDesign(_context);
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
                    PaintBoothclass panels = panel.DevelopmentforRightLeftPanels(item, 0, j);
                    paintbooth.Add(panels);
                    developmentfilePaths.Add(panels.developmentpath);
                }
                else if (item.PanelPosition == "LeftSide")
                {
                    panel.PanelWidth = item.StandardPanelWidthForD;
                    panel.PanelHeight = item.PanelHeightforH;
                    PaintBoothclass panels = panel.DevelopmentforRightLeftPanels(item, 1, j);
                    paintbooth.Add(panels);
                    developmentfilePaths.Add(panels.developmentpath);
                }
                else if (item.PanelPosition == "D3Panels Right side")
                {
                    panel.D3 = item.StandardPanelWidthForD;
                    panel.PanelHeight = item.PanelHeightforH;
                    PaintBoothclass panels = panel.DevelopmentforD3Panels(item, 0, j);
                    paintbooth.Add(panels);
                    developmentfilePaths.Add(panels.developmentpath);
                }
                else if (item.PanelPosition == "D3Panels Left Side")
                {
                    panel.D3 = item.StandardPanelWidthForD;
                    panel.PanelHeight = item.PanelHeightforH;
                    PaintBoothclass panels = panel.DevelopmentforD3Panels(item, 1, j);
                    paintbooth.Add(panels);
                    developmentfilePaths.Add(panels.developmentpath);
                }
                else if (item.PanelPosition.Contains("TopPanels"))
                {
                    PaintBoothclass panels = panel.DevelopmentforTopPanels(item);
                    paintbooth.Add(panels);
                    developmentfilePaths.Add(panels.developmentpath);
                }
                else if (item.PanelPosition.Contains("BackPanels"))
                {
                    panel.PanelWidth = item.StandardPanelWidthForD;
                    panel.PanelHeight = item.PanelHeightforH;
                    PaintBoothclass panels = panel.DevelopmentforBackPanels(item, j);
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
    #endregion


}
