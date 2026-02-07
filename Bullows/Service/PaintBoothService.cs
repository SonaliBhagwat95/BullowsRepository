using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using System.Drawing;


namespace Bullows.Service
{
    public class PaintBoothService
    {
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        private readonly BullowsDbContext _DbContext;
        private readonly IConfiguration _configuration;

        DesignDocument drawing = new DesignDocument();
        #region List objects
        List<DesignDocument> rightSideDrawings = new List<DesignDocument>();
        List<DesignDocument> leftSideDrawings = new List<DesignDocument>();
        List<DesignDocument> TopAssembly = new List<DesignDocument>();
        List<DesignDocument> D3PanelRightDrawings = new List<DesignDocument>();
        List<DesignDocument> D3PanelLeftDrawings = new List<DesignDocument>();
        List<DesignDocument> loftDrawing = new List<DesignDocument>();
        List<DesignDocument> FilterDrawings = new List<DesignDocument>();
        List<DesignDocument> TopFrameDrawing = new List<DesignDocument>();
        List<DesignDocument> BaseDrawings = new List<DesignDocument>(); 
        #endregion

        string BaseFilePath = "C:/Bullows/Paintbooth_Drawing";
        #region Variables
        public Dictionary<string, List<DesignDocument>> designdictionary = new();
        public static int noOfPanelsforD = 0; public static double totalPanelsforD = 0;
        public static int noOfPanelsforH = 0; public static double totalPanelsforH = 0;
        public static double smallPanelWidthforH, smallPanelWidthForBackSide; public static double smallPanelWidthforD = 0;
        public static int noOfPanelsforW, noOfPanelsForBackSide; public static double totalPanelsforW = 0;
        public static double smallPanelWidthforW = 0; public static double TotalBackPanels = 0;
        public static double smallPanelsWidthDoors = 0;
        public static double noOfPanelsInSideDoor, RemainingSpanceinD;
        public static double totalPanelsAboveCompEntryDoor = 0;
        public static double noOfPanelsAboveCompEntryDoor = 0;
        List<string> filePaths = new List<string>(); 
        #endregion
        public PaintBoothService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, BullowsDbContext dbContext, IConfiguration configuration)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
            this._DbContext = dbContext;
            this._configuration = configuration;
        }

        #region Left and Right Panels
        public void LeftRightPanels(PaintBoothModel model, PaintBoothDesign paintBooth)
        {
            try
            {

                #region  D Calculate Number of panels in D
                model.PanelWidth = paintBooth.PanelWidth;
                totalPanelsforD = model.D / model.PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                smallPanelWidthforD = model.D - (noOfPanelsforD * model.PanelWidth);
                #endregion

                #region H Calculate Number of Panels In H
                model.PanelHeight = paintBooth.PanelHeight;
                totalPanelsforH = model.H / model.PanelHeight;
                noOfPanelsforH = (int)Math.Floor(totalPanelsforH);
                smallPanelWidthforH = model.H - (noOfPanelsforH * model.PanelHeight);
                #endregion
                double totalNoOfPanels = Math.Ceiling(totalPanelsforD) + Math.Ceiling(totalPanelsforH);
                #region D * H left right
                int i = 0;

                for (i = 0; i < 2; i++)
                {
                    paintBooth.PanelLength = (i == 0) ? 0 : model.W;
                    List<DesignDocument> documents = new List<DesignDocument>();

                    for (int k = 0; k < noOfPanelsforH; k++)
                    {
                        for (int j = 0; j < noOfPanelsforD; j++)
                        {
                            PaintBoothclass panelDrawingPath = paintBooth.PanelsInPaintBooth(j + 1, model, i);
                            documents.Add(panelDrawingPath.drawing);
                            filePaths.Add(panelDrawingPath.lstpath);
                        }
                        if (smallPanelWidthforD > 0)
                        {
                            paintBooth.PanelWidth = smallPanelWidthforD;

                            PaintBoothclass smallPanelDrawingPath = paintBooth.PanelsInPaintBooth(noOfPanelsforD + 1, model, i);
                            documents.Add(smallPanelDrawingPath.drawing);
                            filePaths.Add(smallPanelDrawingPath.lstpath);
                            paintBooth.PanelWidth = model.PanelWidth;
                        }

                    }
                    if (smallPanelWidthforH > 0)
                    {
                        paintBooth.PanelHeight = smallPanelWidthforH;

                        for (int j = 0; j < noOfPanelsforD; j++)
                        {
                            PaintBoothclass panelDrawingPath = paintBooth.PanelsInPaintBooth(j + 1, model, i);
                            documents.Add(panelDrawingPath.drawing);
                            filePaths.Add(panelDrawingPath.lstpath);
                        }
                        if (smallPanelWidthforD > 0)
                        {
                            paintBooth.PanelWidth = smallPanelWidthforD;

                            PaintBoothclass smallPanelDrawingPath = paintBooth.PanelsInPaintBooth(noOfPanelsforD + 1, model, i);
                            documents.Add(smallPanelDrawingPath.drawing);
                            filePaths.Add(smallPanelDrawingPath.lstpath);
                            paintBooth.PanelWidth = model.PanelWidth;

                        }

                        paintBooth.PanelHeight = model.PanelHeight;
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
                #region RightSide And LeftSide Assembly               
                DesignDocument rightSideAssembly = RightSideWallAssembly(rightSideDrawings, 0, model.PanelHeight);
                string rightSideDwgFilePath = BaseFilePath + "/RightPanel.dwg";
                WriteAutodeskParams autoRight = new WriteAutodeskParams(rightSideAssembly);
                WriteAutodesk dwgWriterRight = new WriteAutodesk(autoRight, rightSideDwgFilePath);
                dwgWriterRight.DoWork();

                //pdf file
                string rightSidepdfFilePath = BaseFilePath + "/RightPanel.pdf";
                Write3DPdfParams pdf = new Write3DPdfParams(rightSideAssembly);
                Write3DPDF pdf1 = new Write3DPDF(pdf, rightSidepdfFilePath);
                pdf1.DoWork();


                DesignDocument leftSideAssembly = LeftSideWallAssembly(leftSideDrawings, 0);
                string leftSideDwgFilePath = BaseFilePath + "/LeftPanel.dwg";
                WriteAutodeskParams autoLeft = new WriteAutodeskParams(leftSideAssembly);
                WriteAutodesk dwgWriterLeft = new WriteAutodesk(autoLeft, leftSideDwgFilePath);
                dwgWriterLeft.DoWork();
                string leftSidepdfFilePath = BaseFilePath + "/LeftPanel.pdf";

                Write3DPdfParams pdfauto = new Write3DPdfParams(leftSideAssembly);
                Write3DPDF pdf12 = new Write3DPDF(pdfauto, leftSidepdfFilePath);
                pdf12.DoWork();

                designdictionary = new();
                designdictionary.Add("rightSideDrawings", rightSideDrawings);
                designdictionary.Add("leftSideDrawings", leftSideDrawings);
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "LeftRightPanels", ex.Message);
                throw;
            }

        }
        public void LeftRightPanelsForSideDoor(PaintBoothModel model, PaintBoothDesign paintBooth, DoorDimensionsModel doorModel)
        {
            try
            {
                List<DesignDocument> rightSideDoorDrawings = new List<DesignDocument>();
                List<DesignDocument> leftSideDoorDrawings = new List<DesignDocument>();
                List<DesignDocument> rightSideDoorNearExtractionCDrawings = new List<DesignDocument>();
                List<DesignDocument> leftSideDoorNearExtractionCDrawings = new List<DesignDocument>();
                List<DesignDocument> ComponantEntrySideDoor = new List<DesignDocument>();
                List<DesignDocument> panelsAboveEntryDoors = new List<DesignDocument>();

                #region H Calculate Number of Panels In H
                model.PanelHeight = paintBooth.PanelHeight;
                totalPanelsforH = model.H / model.PanelHeight;
                noOfPanelsforH = (int)Math.Floor(totalPanelsforH);
                smallPanelWidthforH = model.H - (noOfPanelsforH * model.PanelHeight);

                int totalNoOfPanels =(int)(Math.Ceiling(totalPanelsforH))*2;
                #endregion

                if (doorModel.sideDoorLocation != "")
                {
                    RemainingSpanceinD = (Math.Floor((model.D - (model.Width + 500))) / 2);    //model.Width means component width
                    if (RemainingSpanceinD > model.PanelWidth)
                    {
                        noOfPanelsInSideDoor = Math.Floor(RemainingSpanceinD / model.PanelWidth);
                        smallPanelsWidthDoors = RemainingSpanceinD - (noOfPanelsInSideDoor * model.PanelWidth);
                        paintBooth.PanelWidthForSideDoors = model.PanelWidth;
                    }
                    else
                    {
                        noOfPanelsInSideDoor = 1;
                        smallPanelsWidthDoors = 0;
                        paintBooth.PanelWidthForSideDoors = RemainingSpanceinD;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            doorModel.yOffeset = paintBooth.PanelLengthForSideDoors = 0;
                            doorModel.Side = DoorSide.Right;

                        }
                        else
                        {
                            doorModel.yOffeset = paintBooth.PanelLengthForSideDoors = model.W;
                            doorModel.Side = DoorSide.Left;
                        }
                        List<DesignDocument> documents = new List<DesignDocument>();
                        List<DesignDocument> SideDoorNearExtractionC = new List<DesignDocument>();
                        List<DesignDocument> CoveringPanels = new List<DesignDocument>();
                        for (int k = 0; k < noOfPanelsforH; k++)
                        {
                            for (int p = 0; p < noOfPanelsInSideDoor; p++)
                            {

                                paintBooth.PanelHeightForSideDoors = model.PanelHeight;
                                PaintBoothclass DoorPath = paintBooth.DoorsOnBothSide(model, i, totalNoOfPanels);
                                documents.Add(DoorPath.drawing);
                                filePaths.Add(DoorPath.lstpath);
                                SideDoorNearExtractionC.Add(DoorPath.drawing);
                            }
                            if (smallPanelsWidthDoors > 0)
                            {
                                paintBooth.PanelWidthForSideDoors = smallPanelsWidthDoors;

                                PaintBoothclass smallDoorPanelDrawingPath = paintBooth.DoorsOnBothSide(model, i, totalNoOfPanels);
                                documents.Add(smallDoorPanelDrawingPath.drawing);
                                filePaths.Add(smallDoorPanelDrawingPath.lstpath);
                                SideDoorNearExtractionC.Add(smallDoorPanelDrawingPath.drawing);
                                paintBooth.PanelWidthForSideDoors = model.PanelWidth;
                            }
                        }
                        if (smallPanelWidthforH > 0)
                        {
                            paintBooth.PanelHeightForSideDoors = smallPanelWidthforH;

                            for (int p = 0; p < noOfPanelsInSideDoor; p++)
                            {
                                PaintBoothclass DoorPath = paintBooth.DoorsOnBothSide(model, i, totalNoOfPanels);
                                documents.Add(DoorPath.drawing);
                                filePaths.Add(DoorPath.lstpath);
                                SideDoorNearExtractionC.Add(DoorPath.drawing);
                            }
                            if (smallPanelsWidthDoors > 0)
                            {
                                paintBooth.PanelWidthForSideDoors = smallPanelsWidthDoors;

                                PaintBoothclass smallDoorPanelDrawingPath = paintBooth.DoorsOnBothSide(model, i, totalNoOfPanels);
                                documents.Add(smallDoorPanelDrawingPath.drawing);
                                filePaths.Add(smallDoorPanelDrawingPath.lstpath);
                                SideDoorNearExtractionC.Add(smallDoorPanelDrawingPath.drawing);
                                paintBooth.PanelWidthForSideDoors = model.PanelWidth;
                            }

                            paintBooth.PanelHeightForSideDoors = model.PanelHeight;
                        }


                        doorModel.xOffeset = RemainingSpanceinD;
                        PaintBoothclass entryDoor = paintBooth.ComponantryEntrySideDoor(doorModel);
                        ComponantEntrySideDoor.Add(entryDoor.drawing);
                        filePaths.Add(entryDoor.lstpath);

                        #region Panels above Componant entry doors
                        //calculations for no of panels above componant entry doors
                        totalPanelsAboveCompEntryDoor = doorModel.doorWidth / model.PanelWidth;
                        noOfPanelsAboveCompEntryDoor = Math.Floor(totalPanelsAboveCompEntryDoor);
                        double smallPanelsWidthCompEntryDoor = doorModel.doorWidth - (noOfPanelsAboveCompEntryDoor * model.PanelWidth);

                        doorModel.panelHeightforAboveCompEntryPanels = model.H - doorModel.doorHeight;
                        doorModel.panelWidthforAboveCompEntryPanels = model.PanelWidth;
                        for (int m = 0; m < noOfPanelsAboveCompEntryDoor; m++)
                        {

                            PaintBoothclass panelAboveEntryDoor = paintBooth.PanelsAboveEntryDoor(i, doorModel);
                            doorModel.xOffeset += model.PanelWidth;
                            filePaths.Add(panelAboveEntryDoor.lstpath);
                            CoveringPanels.Add(panelAboveEntryDoor.drawing);
                        }
                        if (smallPanelsWidthCompEntryDoor > 0)
                        {
                            doorModel.panelWidthforAboveCompEntryPanels = smallPanelsWidthCompEntryDoor;
                            PaintBoothclass smallPanelAboveEntryDoor = paintBooth.PanelsAboveEntryDoor(i, doorModel);
                            filePaths.Add(smallPanelAboveEntryDoor.lstpath);
                            CoveringPanels.Add(smallPanelAboveEntryDoor.drawing);
                        }
                        if (i == 0)
                        {
                            rightSideDoorDrawings.AddRange(documents);
                            rightSideDoorNearExtractionCDrawings.AddRange(SideDoorNearExtractionC);
                            panelsAboveEntryDoors.AddRange(CoveringPanels);
                        }
                        else if (i == 1)
                        {
                            leftSideDoorDrawings.AddRange(documents);
                            leftSideDoorNearExtractionCDrawings.AddRange(SideDoorNearExtractionC);
                            panelsAboveEntryDoors.AddRange(CoveringPanels);
                        }

                        #endregion
                    }
                    designdictionary.Add("rightSideDoorDrawings", rightSideDoorDrawings);
                    designdictionary.Add("leftSideDoorDrawings", leftSideDoorDrawings);
                    designdictionary.Add("rightSideDoorNearExtractionCDrawings", rightSideDoorNearExtractionCDrawings);
                    designdictionary.Add("leftSideDoorNearExtractionCDrawings", leftSideDoorNearExtractionCDrawings);
                    designdictionary.Add("ComponantEntrySideDoor", ComponantEntrySideDoor);
                    designdictionary.Add("panelsAboveEntryDoors", panelsAboveEntryDoors);
                }
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "LeftRightPanelsForSideDoor", ex.Message);

                throw;
            }
        }
        #endregion

        #region Top Panels
        public void TopPanels(PaintBoothModel model, PaintBoothDesign paintBooth)
        {
            try
            {
                #region  D Calculate Number of panels in D
                model.PanelWidth = paintBooth.PanelWidth;
                totalPanelsforD = model.D / model.PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                smallPanelWidthforD = model.D - (noOfPanelsforD * model.PanelWidth);
                #endregion
                #region W Calculate Number of panels In W
                model.PanelHeight = paintBooth.PanelHeight;
                totalPanelsforW = model.W / paintBooth.PanelHeight;
                noOfPanelsforW = (int)Math.Floor(totalPanelsforW);
                smallPanelWidthforW = model.W - (noOfPanelsforW * paintBooth.PanelHeight);
                #endregion
                #region W*D Top 
                List<DesignDocument> docdrawing = new List<DesignDocument>();
                List<string> Topsidepath = new List<string>();
                for (int i = 0; i < noOfPanelsforW; i++)
                {
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        paintBooth.PanelLengthForTopPanels = model.PanelHeight;
                        PaintBoothclass Topside = paintBooth.TopSidePanels(k + 1, model, i/*, PaintBoothTypefromEnquiry*/);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);

                    }
                    if (smallPanelWidthforD > 0)
                    {
                        paintBooth.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = paintBooth.TopSidePanels(noOfPanelsforW + 1, model, i/*, PaintBoothTypefromEnquiry*/);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        paintBooth.PanelWidth = model.PanelWidth;
                    }
                }
                if (smallPanelWidthforW > 0)
                {
                    //paintBooth.PanelLength = smallPanelWidthforW;
                    paintBooth.PanelLengthForTopPanels = smallPanelWidthforW;
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        PaintBoothclass Topside = paintBooth.TopSidePanels(k + 1, model, k /*PaintBoothTypefromEnquiry*/);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                    }
                    if (smallPanelWidthforD > 0)
                    {
                        paintBooth.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = paintBooth.TopSidePanels(noOfPanelsforW + 1, model, 0/*, PaintBoothTypefromEnquiry*/);
                        docdrawing.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        paintBooth.PanelWidth = model.PanelWidth;
                    }

                }
                TopAssembly.AddRange(docdrawing);

                DesignDocument TopDrawing = TopWallAssembly(TopAssembly, 0);
                string TopDwgFilePath = BaseFilePath + "/TopPanel.dwg";
                WriteAutodeskParams Topdwg = new WriteAutodeskParams(TopDrawing);
                WriteAutodesk dwgWriterTop = new WriteAutodesk(Topdwg, TopDwgFilePath);
                dwgWriterTop.DoWork();
                string ToppdfFilePath = BaseFilePath + "/TopPanel.pdf";

                Write3DPdfParams pdfauto = new Write3DPdfParams(TopDrawing);
                Write3DPDF pdf12 = new Write3DPDF(pdfauto, ToppdfFilePath);
                pdf12.DoWork();
                designdictionary.Add("TopAssembly", TopAssembly);

                #endregion
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "TopPanels", ex.Message);
                throw;
            }
        }
        #endregion

        #region Front Panels
        public void FrontPanels(PaintBoothModel model, PaintBoothDesign paintBooth)
        {
            try
            {
                // Front Panels
                List<DesignDocument> frontDrawingList = new List<DesignDocument>();
                //List<string> frontDrawingPath = new List<string>();
                for (int k = 0; k < noOfPanelsforH; k++)
                {
                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        paintBooth.FrontPanelLength = model.PanelWidth;
                        paintBooth.FrontPanelHeight = model.PanelHeight;
                        PaintBoothclass frontPanelsPath = paintBooth.FrontPanels(model);
                        frontDrawingList.Add(frontPanelsPath.drawing);
                        filePaths.Add(frontPanelsPath.lstpath);
                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        paintBooth.FrontPanelLength = smallPanelWidthForBackSide;

                        PaintBoothclass smallFrontPanelDrawingPath = paintBooth.FrontPanels(model);
                        frontDrawingList.Add(smallFrontPanelDrawingPath.drawing);
                        filePaths.Add(smallFrontPanelDrawingPath.lstpath);
                        paintBooth.FrontPanelLength = model.PanelWidth;
                    }
                }
                if (smallPanelWidthforH > 0)
                {
                    paintBooth.FrontPanelHeight = smallPanelWidthforH;

                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        PaintBoothclass frontPanelsPath = paintBooth.FrontPanels(model);
                        frontDrawingList.Add(frontPanelsPath.drawing);
                        filePaths.Add(frontPanelsPath.lstpath);
                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        paintBooth.FrontPanelLength = smallPanelWidthForBackSide;

                        PaintBoothclass smallFrontPanelDrawingPath = paintBooth.FrontPanels(model);
                        frontDrawingList.Add(smallFrontPanelDrawingPath.drawing);
                        filePaths.Add(smallFrontPanelDrawingPath.lstpath);

                        paintBooth.FrontPanelLength = model.PanelWidth;
                    }
                }
                designdictionary.Add("frontDrawingList", frontDrawingList);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FrontPanels", ex.Message);

                throw;
            }
        }
        public double FrontDoorsType(PaintBoothModel model, PaintBoothDesign paintBooth, DoorDimensionsModel doorModel)
        {
            try
            {
                List<DesignDocument> frontDoorsDrawingList = new List<DesignDocument>();
                List<DesignDocument> ComponantEntryFrontDoor = new List<DesignDocument>();

                double SameSpaceBetweenDoors = 0;
                #region H Calculate Number of Panels In H
                model.PanelHeight = paintBooth.PanelHeight;
                totalPanelsforH = model.H / model.PanelHeight;
                noOfPanelsforH = (int)Math.Floor(totalPanelsforH);
                smallPanelWidthforH = model.H - (noOfPanelsforH * model.PanelHeight);
                #endregion

                for (int m = 0; m < 2; m++)
                {
                    SameSpaceBetweenDoors = ((model.W1 + model.W2) - 500) / 2;
                    PaintBoothclass frontDoorPath = paintBooth.FrontPanelsWithDoors(SameSpaceBetweenDoors, model);
                    frontDoorsDrawingList.Add(frontDoorPath.drawing);
                    filePaths.Add(frontDoorPath.lstpath);
                }
                designdictionary.Add("frontDoorsDrawingList", frontDoorsDrawingList);
                doorModel.xOffesetForFrontDoor = SameSpaceBetweenDoors;
                PaintBoothclass entryDoor = paintBooth.ComponantryEntryFrontDoor(doorModel);
                ComponantEntryFrontDoor.Add(entryDoor.drawing);
                filePaths.Add(entryDoor.lstpath);
                designdictionary.Add("ComponantEntryFrontDoor", ComponantEntryFrontDoor);
                return SameSpaceBetweenDoors;
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FrontDoorsType", ex.Message);

                throw;
            }
        }
        #endregion

        #region D3 Panels
        public void D3Panels(PaintBoothModel model, PaintBoothDesign paintBooth, double ExtractionC_Height)
        {
            try
            {
                List<DesignDocument> documents = new List<DesignDocument>();
                for (int i = 0; i < 2; i++)
                {
                    PaintBoothclass panelDrawingPathD3 = paintBooth.D3Panels(i, model, ExtractionC_Height);
                    documents.Add(panelDrawingPathD3.drawing);
                    filePaths.Add(panelDrawingPathD3.lstpath);
                    if (i == 0)
                    {
                        D3PanelRightDrawings.AddRange(documents);

                    }
                    else if (i == 1)
                    {
                        D3PanelLeftDrawings.AddRange(documents);
                    }
                }
                designdictionary.Add("D3PanelRightDrawings", D3PanelRightDrawings);
                designdictionary.Add("D3PanelLeftDrawings", D3PanelLeftDrawings);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "D3Panels", ex.Message);

                throw;
            }
        } 
        #endregion

        #region Back Panels
        public void backPanels(PaintBoothModel model, PaintBoothDesign paintBooth, double ExtractionC_Height)
        {
            try
            {

                paintBooth.BackPanelLength = model.PanelWidth;
                //BackLengthTemp = panel.BackPanelLength;
                TotalBackPanels = model.W / model.PanelWidth;
                noOfPanelsForBackSide = (int)Math.Floor(TotalBackPanels);
                smallPanelWidthForBackSide = model.W - (noOfPanelsForBackSide * model.PanelWidth);
                paintBooth.PanelHeight = model.PanelHeight;

                #region H Calculate Number of Panels In H
                model.PanelHeight = paintBooth.PanelHeight;
                totalPanelsforH = model.H / model.PanelHeight;
                noOfPanelsforH = (int)Math.Floor(totalPanelsforH);
                smallPanelWidthforH = model.H - (noOfPanelsforH * model.PanelHeight);
                #endregion

                #region W * H back and front
                List<DesignDocument> backDrawingList = new List<DesignDocument>();
                List<DesignDocument> backDrawingListBeforeExtractionC = new List<DesignDocument>();
                //List<DesignDocument> FrontDrawingList = new List<DesignDocument>();
                for (int k = 0; k < noOfPanelsforH; k++)
                {
                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        PaintBoothclass panelDrawingPath = paintBooth.BackPanelsAfterExtractionChamber(model/*, PaintBoothTypefromEnquiry*/, ExtractionC_Height);
                        backDrawingList.Add(panelDrawingPath.drawing);
                        filePaths.Add(panelDrawingPath.lstpath);
                        ////back panels before extraction chembers
                        //PaintBoothclass backpanelPath = paintBooth.BackPanels(model, /*PaintBoothTypefromEnquiry,*/ ExtractionC_Height);
                        //backDrawingListBeforeExtractionC.Add(backpanelPath.drawing);
                        //filePaths.Add(backpanelPath.lstpath);

                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        paintBooth.BackPanelLength = smallPanelWidthForBackSide;
                        PaintBoothclass smallPanelDrawingPath = paintBooth.BackPanelsAfterExtractionChamber(model/*, PaintBoothTypefromEnquiry*/, ExtractionC_Height);
                        backDrawingList.Add(smallPanelDrawingPath.drawing);
                        ////back panels before extraction chembers
                        //PaintBoothclass backpanelSmallPath = paintBooth.BackPanels(model/*, PaintBoothTypefromEnquiry*/, ExtractionC_Height);
                        //backDrawingListBeforeExtractionC.Add(backpanelSmallPath.drawing);
                        //filePaths.Add(backpanelSmallPath.lstpath);
                        paintBooth.BackPanelLength = model.PanelWidth;
                    }
                }
                designdictionary.Add("backDrawingList", backDrawingList);
                designdictionary.Add("backDrawingListBeforeExtractionC", backDrawingListBeforeExtractionC);
                #endregion
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "backPanels", ex.Message);
                throw;
            }
        }
        #endregion

        #region Loft and Filters
        public void CreateLoft(PaintBoothModel model, PaintBoothDesign paintBooth, double ExtractionC_Height)
        {
            try
            {
                PaintBoothclass loft = paintBooth.CreateLoft(model/*, PaintBoothTypefromEnquiry*/, ExtractionC_Height);
                string filepathforLoft = loft.lstpath;
                DesignDocument loftdrawing = new DesignDocument();
                loftdrawing = loft.drawing;

                WriteAutodeskParams loftdw = new WriteAutodeskParams(loftdrawing);
                WriteAutodesk dwgWriterloft = new WriteAutodesk(loftdw, filepathforLoft);
                dwgWriterloft.DoWork();
                loftDrawing.Add(loftdrawing);
                designdictionary.Add("loftDrawing", loftDrawing);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "CreateLoft", ex.Message);
                throw;
            }
        }
        public void FiltersAndBaffles(PaintBoothModel model, PaintBoothDesign paintBooth, double ExtractionC_Height)
        {
            try
            {
                List<PaintBoothclass> filterFrames = paintBooth.OuterFilterFrame(model, ExtractionC_Height);
                List<DesignDocument> filterDrawings = new List<DesignDocument>();
                List<string> filterPaths = new List<string>();
                foreach (PaintBoothclass filterFrame in filterFrames)
                {
                    filterDrawings.Add(filterFrame.drawing);
                    filterPaths.Add(filterFrame.lstpath);
                }
                FilterDrawings.AddRange(filterDrawings);
                designdictionary.Add("FilterDrawings", FilterDrawings);

            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FiltersAndBaffles", ex.Message);

                throw;
            }
        } 
        #endregion

        #region Structure frames
        public void TopStructureFrame(PaintBoothModel model, PaintBoothDesign paintBooth)
        {
            try
            {
                PaintBoothclass Topstructure = paintBooth.TopStructureFrame(model);
                List<DesignDocument> TopStructuredrawing = new List<DesignDocument>();
                List<string> TopFramepath = new List<string>();
                TopStructuredrawing.Add(Topstructure.drawing);
                TopFramepath.Add(Topstructure.lstpath);
                TopFrameDrawing.AddRange(TopStructuredrawing);
                designdictionary.Add("TopFrameDrawing", TopFrameDrawing);

                DesignDocument TopDrawingAss = TopStructure(TopStructuredrawing, 0);
                string TopFrameDwgFilePath = BaseFilePath + "/TopStructure_Drawing.dwg";
                WriteAutodeskParams topframedwg = new WriteAutodeskParams(TopDrawingAss);
                WriteAutodesk dwgWritertopframe = new WriteAutodesk(topframedwg, TopFrameDwgFilePath);
                dwgWritertopframe.DoWork();
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "TopStructureFrame", ex.Message);

                throw;
            }
        }

        public void BaseStructureFrame(PaintBoothModel model, PaintBoothDesign paintBooth)
        {
            try
            {
                PaintBoothclass basestructure = paintBooth.BaseStructure(model/*, PaintBoothTypefromEnquiry*/);
                List<DesignDocument> basedrawing = new List<DesignDocument>();
                List<string> basepath = new List<string>();
                basedrawing.Add(basestructure.drawing);
                basepath.Add(basestructure.lstpath);
                BaseDrawings.AddRange(basedrawing);
                designdictionary.Add("BaseDrawings", BaseDrawings);
                DesignDocument BaseDrawingAss = BaseStructure(basedrawing, 0);
                string BaseDwgFilePath = BaseFilePath + "/BaseStructure_Drawing.dwg";
                WriteAutodeskParams Basedwg = new WriteAutodeskParams(BaseDrawingAss);
                WriteAutodesk dwgWriterBase = new WriteAutodesk(Basedwg, BaseDwgFilePath);
                dwgWriterBase.DoWork();
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "BaseStructureFrame", ex.Message);

                throw;
            }

        }
        #endregion

        #region FAS
        public void FASPanelsFrontAndBack(PaintBoothModel model, PaintBoothDesign paintBooth, int PlenumHeight)
        {
            try
            {
                #region W * H back and front(optional) 
                string BackPanelsDwgFilePathForTop = "";
                string FrontPanelsDwgFilePathInTop = "";
                List<DesignDocument> backDrawingListInTopSide = new List<DesignDocument>();
                List<DesignDocument> FrontDrawingListInTopSide = new List<DesignDocument>();
                //var PaintBoothTypefromEnquiry = _uow.PaintBoothRepository.FetchPaintBoothType(model.SalesNO);

                for (int k = 0; k < noOfPanelsforH; k++)
                {
                    for (int j = 0; j < noOfPanelsForBackSide; j++)
                    {
                        PaintBoothclass panelDrawingInTopSide = paintBooth.BackPanelsForType5(model, PlenumHeight);
                        backDrawingListInTopSide.Add(panelDrawingInTopSide.drawing);
                        filePaths.Add(panelDrawingInTopSide.lstpath);

                        PaintBoothclass frontPanelsPathInTopSide = paintBooth.FrontPanelsForType5(model, PlenumHeight);
                        FrontDrawingListInTopSide.Add(frontPanelsPathInTopSide.drawing);
                        filePaths.Add(frontPanelsPathInTopSide.lstpath);
                    }
                    if (smallPanelWidthForBackSide > 0)
                    {
                        paintBooth.BackPanelLength = smallPanelWidthForBackSide;
                        PaintBoothclass smallPanelDrawingInTopSide = paintBooth.BackPanelsForType5(model, PlenumHeight);
                        backDrawingListInTopSide.Add(smallPanelDrawingInTopSide.drawing);

                        PaintBoothclass smallFrontPanelDrawingInTopSide = paintBooth.FrontPanelsForType5(model, PlenumHeight);
                        FrontDrawingListInTopSide.Add(smallFrontPanelDrawingInTopSide.drawing);
                        filePaths.Add(smallFrontPanelDrawingInTopSide.lstpath);
                        paintBooth.BackPanelLength = model.PanelWidth;
                    }
                }

                DesignDocument BackPanelsDrawingforTop = BackPanelsAssembly(backDrawingListInTopSide, 0);
                BackPanelsDwgFilePathForTop = BaseFilePath + "/RearPanelInTop.dwg";
                WriteAutodeskParams backdwg1 = new WriteAutodeskParams(BackPanelsDrawingforTop);
                WriteAutodesk backinTop = new WriteAutodesk(backdwg1, BackPanelsDwgFilePathForTop);
                backinTop.DoWork();

                //front panels
                DesignDocument FrontPanelsDrawingforTop = FrontPanelsAssembly(FrontDrawingListInTopSide, 0);
                FrontPanelsDwgFilePathInTop = BaseFilePath + "/FrontPanelInTop.dwg";
                WriteAutodeskParams frontdwgInTop = new WriteAutodeskParams(FrontPanelsDrawingforTop);
                WriteAutodesk FrontTop = new WriteAutodesk(frontdwgInTop, FrontPanelsDwgFilePathInTop);
                FrontTop.DoWork();
                designdictionary.Add("backDrawingListInTopSide", backDrawingListInTopSide);
                designdictionary.Add("FrontDrawingListInTopSide", FrontDrawingListInTopSide);

                #endregion
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FASPanelsFrontAndBack", ex.Message);

                throw;
            }
        }
        public void FASPanelsRightAndLeft(PaintBoothModel model, PaintBoothDesign paintBooth, int PlenumHeight)
        {
            try
            {
                List<DesignDocument> RightPanelsListInTopSideForFAS = new List<DesignDocument>();
                List<DesignDocument> LeftPanelsListInTopSideForFAS = new List<DesignDocument>();
                #region  D Calculate Number of panels in D
                model.PanelWidth = paintBooth.PanelWidth;
                totalPanelsforD = model.D / model.PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                smallPanelWidthforD = model.D - (noOfPanelsforD * model.PanelWidth);
                #endregion
                for (int l = 0; l < 2; l++)
                {
                    paintBooth.PanelLength = (l == 0) ? 0 : model.W;
                    List<DesignDocument> documents = new List<DesignDocument>();

                    for (int k = 0; k < noOfPanelsforH; k++)
                    {

                        for (int j = 0; j < noOfPanelsforD; j++)
                        {

                            PaintBoothclass panelDrawingPathForType5 = paintBooth.PanelsInPaintBoothForType5(j + 1, model, l, PlenumHeight);
                            documents.Add(panelDrawingPathForType5.drawing);
                            filePaths.Add(panelDrawingPathForType5.lstpath);
                        }
                        if (smallPanelWidthforD > 0)
                        {
                            paintBooth.PanelWidth = smallPanelWidthforD;

                            PaintBoothclass smallPanelDrawingPathForType5 = paintBooth.PanelsInPaintBoothForType5(noOfPanelsforD + 1, model, l, PlenumHeight);
                            documents.Add(smallPanelDrawingPathForType5.drawing);
                            filePaths.Add(smallPanelDrawingPathForType5.lstpath);

                            paintBooth.PanelWidth = model.PanelWidth;
                        }
                    }
                    if (l == 0)
                    {
                        RightPanelsListInTopSideForFAS.AddRange(documents);

                    }
                    else if (l == 1)
                    {
                        LeftPanelsListInTopSideForFAS.AddRange(documents);
                    }
                }
                designdictionary.Add("RightPanelsListInTopSideForFAS", RightPanelsListInTopSideForFAS);
                designdictionary.Add("LeftPanelsListInTopSideForFAS", LeftPanelsListInTopSideForFAS);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FASPanelsRightAndLeft", ex.Message);

                throw;
            }
            //paintBooth.PanelLength = PanelLengthTemp;


        }
        public void FASPanelsForTop(PaintBoothModel model, PaintBoothDesign paintBooth, int PlenumHeight)
        {
            try
            {
                #region W*D Top 
                List<DesignDocument> TopPanelsForFAS = new List<DesignDocument>();
                List<DesignDocument> TopAssemblyForFAS = new List<DesignDocument>();

                List<string> Topsidepath = new List<string>();
                for (int i = 0; i < noOfPanelsforW; i++)
                {
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        paintBooth.PanelLengthForTopPanels = model.PanelHeight;
                        PaintBoothclass Topside = paintBooth.TopSidePanelsForFAS(k + 1, model, i, PlenumHeight/*, PaintBoothTypefromEnquiry*/);
                        TopPanelsForFAS.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);

                    }
                    if (smallPanelWidthforD > 0)
                    {
                        paintBooth.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = paintBooth.TopSidePanelsForFAS(noOfPanelsforW + 1, model, i, PlenumHeight/*, PaintBoothTypefromEnquiry*/);
                        TopPanelsForFAS.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        paintBooth.PanelWidth = model.PanelWidth;
                    }
                }
                if (smallPanelWidthforW > 0)
                {
                    paintBooth.PanelLengthForTopPanels = smallPanelWidthforW;
                    for (int k = 0; k < noOfPanelsforD; k++)
                    {
                        PaintBoothclass Topside = paintBooth.TopSidePanelsForFAS(k + 1, model, k, PlenumHeight /*PaintBoothTypefromEnquiry*/);
                        TopPanelsForFAS.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                    }
                    if (smallPanelWidthforD > 0)
                    {
                        paintBooth.PanelWidth = smallPanelWidthforD;
                        PaintBoothclass Topside = paintBooth.TopSidePanelsForFAS(noOfPanelsforW + 1, model, 0, PlenumHeight/*, PaintBoothTypefromEnquiry*/);
                        TopPanelsForFAS.Add(Topside.drawing);
                        Topsidepath.Add(Topside.lstpath);
                        paintBooth.PanelWidth = model.PanelWidth;
                    }
                }
                TopAssemblyForFAS.AddRange(TopPanelsForFAS);

                designdictionary.Add("TopAssemblyForFAS", TopAssemblyForFAS);

                #endregion
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("PaintBoothService", "FASPanelsForTop", ex.Message);
                throw;
            }
        } 
        #endregion

        #region Assembly Calculations
        private DesignDocument RightSideWallAssembly(List<DesignDocument> drawings, double yOffset, double panelheight)
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
        private DesignDocument TopWallAssembly(List<DesignDocument> Docdrawing, double yOffset)
        {
            #region old code
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
            #endregion
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
        private DesignDocument FrontPanelsAssembly(List<DesignDocument> YAxisdrawing, double yOffset)
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

        private double CalculateDrawingWidth(DesignDocument drawing)
        {
            if (drawing.Entities.Count == 0)
                return 0;

            double minX = double.MaxValue;
            double maxX = double.MinValue;

            foreach (var entity in drawing.Entities)
            {
                if (entity.BoxMin.X < minX)
                    minX = entity.BoxMin.X;

                if (entity.BoxMax.X > maxX)
                    maxX = entity.BoxMax.X;
            }

            return maxX - minX;
        }

        #endregion
    }
}
