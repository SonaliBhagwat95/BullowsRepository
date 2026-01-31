using Bullows.Database;
using Bullows.Model;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using static ODA.Kernel.TD_RootIntegrated.OdGiLinetypeDash;
using devregion = devDept.Eyeshot.Entities.Region;
namespace Bullows.Business
{
    public class PaintBoothDesign
    {
        private readonly BullowsDbContext _DbContext;
        private readonly IConfiguration _configuration;
        public PaintBoothDesign(BullowsDbContext dbContext, IConfiguration configuration)
        {
            _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration;
        }
        #region Variables
        public DesignDocument drawing { get; set; }
        public DesignDocument developmentdrawing { get; set; }
        public int PaintBoothID { get; set; }
        public double ScaleFactor { get; set; }
        public int EnquiryID { get; set; }

        public double D1 { get; set; }

        public double JobSize { get; set; }

        public double D2 { get; set; }

        public double D3 { get; set; }

        public double W1 { get; set; }

        public double W2 { get; set; }

        public double W3 { get; set; }

        public double D { get; set; }


        public double H1 { get; set; }

        public double H2 { get; set; }

        public double W { get; set; }
        public double H { get; set; }
        public string material = string.Empty;
        public double PanelWidth { get; set; }

        public double PanelLength { get; set; }
        public double PanelLengthForSideDoors { get; set; }
        public double PanelWidthForSideDoors { get; set; }
        public double PanelHeightForSideDoors { get; set; }
        public double PanelLengthForTopPanels { get; set; }
        public double PanelHeight { get; set; }
        public double SheetThickness { get; set; }
        public double SettingStandardBend1 { get; set; }
        public double SettingStandardBend2 { get; set; }
        public double PitchDistance { get; set; }
        public int NoofPanels { get; set; }
        public double BackPanelLength { get; set; }
        public double FrontPanelLength { get; set; }
        public double FrontPanelHeight { get; set; }
        static double totalPanelsforD; static double totalPanelsforW; static double totalPanelsforH;
        static double smallPanelWidthforD; static double noOfFramesW;
        static int noOfPanelsforD; static int noOfPanelsforW; static int noOfPanelsforH;
        public string Weight = string.Empty;

        public double SettingH { get; set; }
        public double SettingW { get; set; }
        public double SettingT { get; set; }
        public string Materials { get; set; }
        public string SlotDimention { get; set; }

        public string Section { get; set; }
        public ISession Session { get; }
        public static double RightPanels_Weight = 0;
        public static double LeftPanels_Weight = 0;
        public static double D3Panels_weight = 0;
        public static double TopPanel_Weight = 0;
        public static double BackPanel_Weight = 0;
        public static double TopStructureFrame_Weight = 0;
        public static double BaseFrame_Weight = 0;
        public static decimal TubeLightCalculations = 0;
        public int[] panels;
        private int[] Panelinputs;
        public int[] stiff;
        Point2D Pmax = new Point2D();
        DrawingDocument drawingdoc = new DrawingDocument();
        private Sheet somesheet;
        double UntrimmedWidth = 625;
        double UntrimmedHeight = 450;
        double trimmedWidth = 594;
        double trimmedHeight = 420;
        double titleBoxWidth = 185;
        double titleBoxHeight = 65;
        public static int selectedBaffleHeight = 1140;
        public static int bafflePanelCount;
        public double StandardBend1 { get; set; }
        public double StandardBend2 { get; set; }
        public string LightTypes { get; set; }
        public decimal LuxLevel { get; set; }
        public decimal Lumens { get; set; }

        public static int roundScalefactor = 0; public static double totalWeightOfbaffle = 0; public static double totalWeight = 0; // Variable to store the total weight
        #endregion

        # region Right and Panels drawing
        public PaintBoothclass PanelsInPaintBooth(int j, PaintBoothModel model, int k)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;

            var rectangle1 = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, PanelHeight),
                    new Point3D(PanelWidth, PanelLength, PanelHeight),
                    new Point3D(PanelWidth, PanelLength, 0)
            });
            Brep brep = rectangle1.ExtrudeAsBrep(SheetThickness);

            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            drawing.Entities.Add(brep, Color.Green);

            LinearPath rail = new LinearPath(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, PanelHeight),
                    new Point3D(PanelWidth, PanelLength, PanelHeight),
                    new Point3D(PanelWidth, PanelLength, 0),
                    new Point3D(0, PanelLength, 0)
            });
            devregion section = CreatePolygon(k);
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLength, SheetThickness);
            // Generate holes on YZ and XY planes
            frame = GenerateHoles(frame);
            Material mat1 = Material.StructuralSteel;
            mat1 = new Material(Materials);

            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);

            if (k == 0)
            {
                var existingRecord = _DbContext.PanelDetails
                   .FirstOrDefault(p =>
                       p.EnquiryId == EnquiryID &&
                       p.PanelPosition == "RightSide" &&
                       p.StandardPanelDepth == PanelWidth &&
                       p.StandardPanelHeight == PanelHeight);

                // If the record does not exist, save it
                if (existingRecord == null || existingRecord.NoOfPanels == 0)
                {
                    RightPanels_Weight = Rectangle_Weight + Frame_Weight;
                    SavePanelDetails(model, "RightSide", k, RightPanels_Weight);
                }
                else
                {
                    // Increment NoOfPanels
                    existingRecord.NoOfPanels += 1;

                    // Save the changes for the updated Quantity
                    _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
                }

            }
            else
            {

                // Check if a record with the same details already exists
                var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "LeftSide" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == PanelHeight);

                // If the record does not exist, save it
                if (existingRecord == null || existingRecord.NoOfPanels == 0)
                {
                    LeftPanels_Weight = Rectangle_Weight + Frame_Weight;
                    SavePanelDetails(model, "LeftSide", k, LeftPanels_Weight);
                }
                else
                {
                    // Increment NoOfPanels
                    existingRecord.NoOfPanels += 1;

                    // Save the changes for the updated Quantity
                    _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
                }
            }


            drawing.Entities.Add(frame, Color.Yellow);
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath = "";
            if (k == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/RightPanel {j} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/LeftPanel {j} {DateTime.Now:hh - mm}.dwg";
            }
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();

            string pdfFilePath = $"{path}/PaintBooth drawing/RightPanel {j} {DateTime.Now:hh - mm}.pdf";
            //pdf file

            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, pdfFilePath);
            pdf1.DoWork();
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };

        }
        public PaintBoothclass DoorsOnBothSide(PaintBoothModel model, int k)
        {
            drawing = new();
            //double z = model.PanelHeight;
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle1 = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(0, PanelLengthForSideDoors, 0),
                    new Point3D(0, PanelLengthForSideDoors, PanelHeightForSideDoors),
                    new Point3D(PanelWidthForSideDoors, PanelLengthForSideDoors, PanelHeightForSideDoors),
                    new Point3D(PanelWidthForSideDoors, PanelLengthForSideDoors, 0)
            });
            Brep brep = rectangle1.ExtrudeAsBrep(SheetThickness);

            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            drawing.Entities.Add(brep, Color.Green);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                    new Point3D(0, PanelLengthForSideDoors, 0),
                    new Point3D(0, PanelLengthForSideDoors, PanelHeightForSideDoors),
                    new Point3D(PanelWidthForSideDoors, PanelLengthForSideDoors,PanelHeightForSideDoors),
                    new Point3D(PanelWidthForSideDoors, PanelLengthForSideDoors, 0),
                    new Point3D(0, PanelLengthForSideDoors, 0)
            });
            devregion section = CreatePolygon(k);
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLengthForSideDoors, SheetThickness);
            // Generate holes on YZ and XY planes
            frame = GenerateHoles(frame);
            Material mat1 = Material.StructuralSteel;
            mat1 = new Material(Materials);

            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);

            drawing.Entities.Add(frame, Color.Yellow);
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath = "";
            if (k == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/RightPanelDoors {k} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/LeftPanelDoors {k} {DateTime.Now:hh - mm}.dwg";
            }
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };

        }
        public PaintBoothclass PanelsInPaintBoothForType5(int j, PaintBoothModel model, int k, int PlenumHeight)
        {
            drawing = new();

            drawing.Units = linearUnitsType.Millimeters;
            double z = PlenumHeight;
            var rectangle1 = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, z),
                    new Point3D(PanelWidth, PanelLength, z),
                    new Point3D(PanelWidth, PanelLength, 0)
            });
            Brep brep = rectangle1.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            drawing.Entities.Add(brep, Color.AntiqueWhite);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, z),
                    new Point3D(PanelWidth, PanelLength, z),
                    new Point3D(PanelWidth, PanelLength, 0),
                    new Point3D(0, PanelLength, 0)
            });
            devregion section = CreatePolygon(k);
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLength, SheetThickness);
            // Generate holes on YZ and XY planes
            frame = GenerateHoles(frame);
            Material mat1 = Material.StructuralSteel;
            mat1 = new Material(Materials);

            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);

            if (k == 0)
            {
                var existingRecord = _DbContext.PanelDetails
                   .FirstOrDefault(p =>
                       p.EnquiryId == EnquiryID &&
                       p.PanelPosition == "RightSideForFAS" &&
                       p.StandardPanelDepth == PanelWidth &&
                       p.StandardPanelHeight == PanelHeight);

                // If the record does not exist, save it
                if (existingRecord == null || existingRecord.NoOfPanels == 0)
                {
                    RightPanels_Weight = Rectangle_Weight + Frame_Weight;
                    SavePanelDetails(model, "RightSideforFAS", k, RightPanels_Weight);
                }
                else
                {
                    // Increment NoOfPanels
                    existingRecord.NoOfPanels += 1;

                    // Save the changes for the updated Quantity
                    _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
                }

            }
            else
            {

                // Check if a record with the same details already exists
                var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "LeftSideForFAS" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == PanelHeight);

                // If the record does not exist, save it
                if (existingRecord == null || existingRecord.NoOfPanels == 0)
                {
                    LeftPanels_Weight = Rectangle_Weight + Frame_Weight;
                    SavePanelDetails(model, "LeftSideForFAS", k, LeftPanels_Weight);
                }
                else
                {
                    // Increment NoOfPanels
                    existingRecord.NoOfPanels += 1;

                    // Save the changes for the updated Quantity
                    _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
                }
            }


            drawing.Entities.Add(frame, Color.Yellow);
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath = "";
            if (k == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/RightPanelForFAS {j} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/LeftPanelForFAS {j} {DateTime.Now:hh - mm}.dwg";
            }
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };

        }
        #endregion
        #region 3D D3 Panels Drawings
        public PaintBoothclass D3Panels(int i, PaintBoothModel model, double ExtractionChemberHeight)
        {
            //string panelPosition;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            const string Layer = "OuterDoorFrame";
            drawing.Layers.Add(new Layer(Layer, Color.Gray));
            
            double x = model.D;
            PanelLength = (i == 0) ? 0 : model.W;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, ExtractionChemberHeight),
                    new Point3D(D3, PanelLength, ExtractionChemberHeight),
                    new Point3D(D3, PanelLength, 0)
            });
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            drawing.Entities.Add(brep, Color.Green);

            LinearPath rail = new LinearPath(new Point3D[]
           {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, ExtractionChemberHeight),
                    new Point3D(D3, PanelLength, ExtractionChemberHeight),
                    new Point3D(D3, PanelLength, 0),
                    new Point3D(0, PanelLength, 0)
           });
            devregion section = CreatePolygon(i);

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLength, SheetThickness);

            // Generate holes on YZ and XY planes
            GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new Material(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            D3Panels_weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            if (i == 0)
            {
                SavePanelDetails(model, "D3Panels Right side", i, D3Panels_weight);
            }
            else
            {
                SavePanelDetails(model, "D3Panels Left Side", i, D3Panels_weight);
            }

            //Added serviceDoor code here 
            double doorWidth = model.DoorWidth; // Width of the service door
            double doorHeight = model.DoorHeight; // Height of the service door
            double doorPositionX = (D3 - doorWidth) / 2;
            double doorStartHeight = 100; // Starting height above the base
            double sectionDoorTHK = 1.6;
            double sectionDoorHeight = 76;
            double sectionDoorWidth = 38;


            double frameY = PanelLength;   // same Y you used for door cutout
            double z0 = doorStartHeight;
            double z1 = doorStartHeight + doorHeight;
            double x0 = doorPositionX;
            double T = sectionDoorTHK;
            double L = sectionDoorWidth - (2 * T);
            double outerLineDistance = 22.5;
            if (model.ServiceDoorLocation == "RightSide" && i == 0)
            {
                {
                    
                    Point3D[] cutout = new Point3D[]
                    {
                        new Point3D(doorPositionX, PanelLength, doorStartHeight),
                        new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                    };
                    var cutout1 = devregion.CreatePolygon(cutout);

                    brep.ExtrudeRemove(cutout1, SheetThickness);
                    drawing.Entities.Add(brep);

                    var Door = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(doorPositionX, PanelLength, doorStartHeight),
                        new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                    });
                    Brep DoorBrep = Door.ExtrudeAsBrep(-1.6);//1.6 mm Thickness
                    drawing.Entities.Add(DoorBrep,Color.Aquamarine);

                    #region outer frame
                    #region top frame section
                    //YZ Plane
                    var topsectionframe = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0-sectionDoorWidth,  -frameY-T, z1),                     // 1
                        new Point3D(x0-sectionDoorWidth,  -frameY-T, z1-sectionDoorHeight+T), // 2
                        new Point3D(x0-sectionDoorWidth,   0,        z1-sectionDoorHeight+T), // 3
                        new Point3D(x0-sectionDoorWidth,   0,        z1+T),      // 4

                        new Point3D(x0 - sectionDoorWidth,  -L-T-T,   z1+T), // 5
                        new Point3D(x0 - sectionDoorWidth,  -L-T-T,   z1-sectionDoorHeight+T), // 6
                        new Point3D(x0 - sectionDoorWidth,  -L-T,     z1-sectionDoorHeight+T),  // 7
                        new Point3D(x0 - sectionDoorWidth,  -L-T,     z1),        // 8


                    });

                    Brep topsectionframeBrep = topsectionframe.ExtrudeAsBrep(doorWidth+(2* sectionDoorWidth));
                    drawing.Entities.Add(topsectionframeBrep, Layer);
                    #endregion

                    #region Base Frame Section
                    var basesectionframe = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0 - sectionDoorWidth ,    -frameY-T,  z0-L+T),        // 8
                        new Point3D(x0 - sectionDoorWidth ,    -frameY-T,  z0),                     // 1
                        new Point3D(x0 - sectionDoorWidth,     0,          z0), // 2
                        new Point3D(x0 - sectionDoorWidth ,    0,          z0-L), // 3

                        new Point3D(x0 - sectionDoorWidth ,    -L-T-T,     z0-L),      // 4
                        new Point3D(x0 - sectionDoorWidth ,   -L-T-T,     z0), // 5
                        new Point3D(x0 - sectionDoorWidth ,   -L-T,       z0), // 6
                        new Point3D(x0 - sectionDoorWidth ,    -L-T,       z0-L+T), // 7
                    });
                    Brep basesectionframeBrep = basesectionframe.ExtrudeAsBrep(-doorWidth- (2 * sectionDoorWidth));
                    drawing.Entities.Add(basesectionframeBrep, Layer);
                    #endregion

                    #region left L section
                    var leftLFrameH = devregion.CreatePolygon(new Point3D[]
                    {
                         new Point3D(x0- sectionDoorWidth ,    0,      z1+T),
                         new Point3D(x0- sectionDoorWidth ,    -L-T-T,  z1+T),
                         new Point3D(x0-T- sectionDoorWidth ,  -L-T-T, z1+T),
                         new Point3D(x0-T- sectionDoorWidth ,   0,     z1+T),
                    });

                    Brep leftLFrameHBrep = leftLFrameH.ExtrudeAsBrep(doorHeight + L + T);
                    drawing.Entities.Add(leftLFrameHBrep, Layer);


                    var leftLFrameW = devregion.CreatePolygon(new Point3D[]
                    {
                         new Point3D(x0- sectionDoorWidth+1.6,  T-T, z1-sectionDoorHeight),
                         new Point3D(x0+L- sectionDoorWidth+1.6,T-T, z1-sectionDoorHeight),
                         new Point3D(x0 + L - sectionDoorWidth+1.6 ,0-T,   z1-sectionDoorHeight),
                         new Point3D(x0 - sectionDoorWidth+1.6 ,  0-T, z1-sectionDoorHeight),
                    });
                    Brep leftLFrameWBrep = leftLFrameW.ExtrudeAsBrep(doorHeight - sectionDoorHeight-1.6);
                    drawing.Entities.Add(leftLFrameWBrep, Layer);

                    #endregion

                    #region Right L section
                    Brep rightLFrameHBrep = (Brep)leftLFrameHBrep.Clone();
                    rightLFrameHBrep.Translate(doorWidth + ( 2 * sectionDoorWidth), 0, 0);
                    drawing.Entities.Add(rightLFrameHBrep, Layer);

                    Brep rightleftLFrameWBrep = (Brep)leftLFrameWBrep.Clone();
                    rightleftLFrameWBrep.Translate(doorWidth + sectionDoorWidth , 0, 0);
                    drawing.Entities.Add(rightleftLFrameWBrep, Layer);

                    #endregion
                    #endregion

                    #region Outer Rectangle
                    const string OuterRectangleLayer = "OuterRectangleLayer";
                    drawing.Layers.Add(new Layer(OuterRectangleLayer, Color.BlueViolet));
                    #region Left Outer Rectangle
                    var LeftOuterRectangle = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0-22.5,            -frameY-T+ 1.6, z1+22.5),//Left Line
                        new Point3D(x0-22.5,            -frameY-T+ 1.6, z0-22.5),
                        new Point3D(x0+27.5,  -frameY-T+ 1.6, z0-22.5),
                        new Point3D(x0+27.5,  -frameY-T+ 1.6, z1+22.5),//Right line

                    });

                    Brep LeftOuterRectanglebrep = LeftOuterRectangle.ExtrudeAsBrep(-3);/*3 is Thickness*/
                    drawing.Entities.Add(LeftOuterRectanglebrep, OuterRectangleLayer);




                    #endregion

                    #region Right Outer Rectangle
                    var RightOuterRectangle = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0+doorWidth-22.5,            -frameY - T + 1.6, z1+22.5),//Left Line
                        new Point3D(x0+doorWidth-22.5,            -frameY - T + 1.6, z0-22.5),
                        new Point3D(x0+doorWidth+27.5,  -frameY - T + 1.6, z0-22.5),
                        new Point3D(x0+doorWidth+27.5,  -frameY-T+1.6, z1+22.5),//Right line

                    });

                    Brep RightOuterRectanglebrep = RightOuterRectangle.ExtrudeAsBrep(-3);/*3 is Thickness*/
                    drawing.Entities.Add(RightOuterRectanglebrep, OuterRectangleLayer);
                    #endregion

                    #region Top Outer Rectangle
                    var TopOuterRectangle = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0+27.5,            -frameY-T+1.6, z1+22.5),//Left Line 1.6 is Thickness
                        new Point3D(x0+27.5,            -frameY-T+1.6, z1-22.5),
                        new Point3D(x0+doorWidth-22.5,  -frameY-T+1.6, z1-22.5),
                        new Point3D(x0+doorWidth-22.5,  -frameY-T+1.6, z1+22.5),//Right line

                    });
                    Brep TopOuterRectanglebrep = TopOuterRectangle.ExtrudeAsBrep(-3);/*3 is Thickness*/
                    drawing.Entities.Add(TopOuterRectanglebrep, OuterRectangleLayer);
                    #endregion

                    #region Bottom Outer Rectangle
                    var BottomOuterRectangle = devregion.CreatePolygon(new Point3D[]
                    {
                        new Point3D(x0+27.5,            -frameY-T+1.6, z0+22.5),//Left Line
                        new Point3D(x0+27.5,            -frameY-T+1.6, z0-22.5),
                        new Point3D(x0+doorWidth-22.5,  -frameY-T+1.6, z0-22.5),
                        new Point3D(x0+doorWidth-22.5,  -frameY-T+1.6, z0+22.5),//Right line

                    });
                    Brep BottomOuterRectanglebrep = BottomOuterRectangle.ExtrudeAsBrep(-3);/*3 is Thickness*/
                    drawing.Entities.Add(BottomOuterRectanglebrep, OuterRectangleLayer);
                    #endregion 
                    #endregion
                }

            }
            else if (model.ServiceDoorLocation == "LeftSide" && i == 1)
            {
                {
                    Point3D[] cutout = new Point3D[]
                {
                    new Point3D(doorPositionX, PanelLength, doorStartHeight),
                    new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                };
                    var cutout1 = devregion.CreatePolygon(cutout);
                    brep.ExtrudeRemove(cutout1, SheetThickness);
                    drawing.Entities.Add(brep);

                }
            }
            else if (model.ServiceDoorLocation == "Both Side")
            {
                {
                    Point3D[] cutout = new Point3D[]
                    {
                        new Point3D(doorPositionX, PanelLength, doorStartHeight),
                        new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                    };
                    var cutoutPolygon = devregion.CreatePolygon(cutout);
                    brep.ExtrudeRemove(cutoutPolygon, SheetThickness);
                    drawing.Entities.Add(brep);
                }
            }
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath;
            if (i == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3Panels Right side  {i} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3Panels Left side {i} {DateTime.Now:hh - mm}.dwg";
            }
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();

            #endregion
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };

        }

        public PaintBoothclass D3PanelsBeforeD(int k, int i, string Doorlocation, PaintBoothModel model)
        {
            string panelPosition;

            PanelInputModel pmodel = new PanelInputModel();
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, PanelHeight),
                    new Point3D(D3, PanelLength, PanelHeight),
                    new Point3D(D3, PanelLength, 0)
            });
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            drawing.Entities.Add(brep, Color.Green);

            LinearPath rail = new LinearPath(new Point3D[]
            {
                    new Point3D(0, PanelLength, 0),
                    new Point3D(0, PanelLength, PanelHeight),
                    new Point3D(D3, PanelLength, PanelHeight),
                    new Point3D(D3, PanelLength, 0),
                    new Point3D(0, PanelLength, 0)
            });
            devregion section = CreatePolygon(i);

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLength, SheetThickness);

            // Generate holes on YZ and XY planes
            GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new Material(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            D3Panels_weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            if (i == 0)
            {
                SavePanelDetails(model, "D3Panels Right side", i, D3Panels_weight);
            }
            else
            {
                SavePanelDetails(model, "D3Panels Left Side", i, D3Panels_weight);
            }

            //Added serviceDoor code here 
            double doorWidth = model.DoorWidth; // Width of the service door
            double doorHeight = model.DoorHeight; // Height of the service door
            double doorPositionX = (D3 - doorWidth) / 2;
            double doorStartHeight = 100; // Starting height above the base

            if (model.ServiceDoorLocation == "RightSide" && k == 1 && i == 0)
            {
                // if (k == 1 && i==0 && i != 1) // RightAssembly
                {
                    //GenerateDoor(model);
                    Point3D[] cutout = new Point3D[]
                    {
                    new Point3D(doorPositionX, PanelLength, doorStartHeight),
                    new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                    };
                    var cutout1 = devregion.CreatePolygon(cutout);

                    brep.ExtrudeRemove(cutout1, SheetThickness);
                    drawing.Entities.Add(brep);

                }

            }
            else if (model.ServiceDoorLocation == "LeftSide" && k == 1 && i == 1)
            {
                // if (k == 1 && i == 1 && i != 0) // LeftAssembly
                {
                    Point3D[] cutout = new Point3D[]
                {
                    new Point3D(doorPositionX, PanelLength, doorStartHeight),
                    new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                    new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                };
                    var cutout1 = devregion.CreatePolygon(cutout);
                    brep.ExtrudeRemove(cutout1, SheetThickness);
                    drawing.Entities.Add(brep);
                }
            }
            else if (model.ServiceDoorLocation == "Both Side" && k == 1)
            {
                // if(k==1) // 0 for Right, 1 for Left
                {
                    Point3D[] cutout = new Point3D[]
                    {
                        new Point3D(doorPositionX, PanelLength, doorStartHeight),
                        new Point3D(doorPositionX, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight + doorHeight),
                        new Point3D(doorPositionX + doorWidth, PanelLength, doorStartHeight)
                    };
                    var cutoutPolygon = devregion.CreatePolygon(cutout);
                    brep.ExtrudeRemove(cutoutPolygon, SheetThickness);
                    drawing.Entities.Add(brep);
                }
            }
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath;
            if (i == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3PanelsBeforeD Right side  {k} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3PanelsBeforeD Left side {k} {DateTime.Now:hh - mm}.dwg";
            }


            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion           
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };
        }

        #endregion
        #region 3D top panels Drawings
        public PaintBoothclass TopSidePanels(int j, PaintBoothModel model, int m/*,string DraftSubType*/)
        {
            #region old code

            string panelPosition;
            string dwgFilePath = "";
            int k = 0;
            double PaintboothHeight = model.H;
            double ExtraHeightforTopPanels = 70;//Extra Height for Top Panels
            PaintboothHeight = PaintboothHeight + ExtraHeightforTopPanels;
            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;

            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            W = PanelLengthForTopPanels;

            double ComponentInDSize = model.D1 + (model.Depth / 2);
            double TubelightLocation = Math.Ceiling(ComponentInDSize / PanelWidth);
            double ComponentInWSize = model.W1 + (model.Width / 2);
            double TubelightLocationinW = Math.Ceiling(ComponentInWSize / PanelLength);
            //if (j == TubelightLocation && m == TubelightLocationinW)
            //{
            //    PaintBoothclass cutoutTubelight = TubelightCutout(j, model);
            //    drawing = cutoutTubelight.drawing;
            //}
            //else
            //{
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,PaintboothHeight),
                new Point3D(PanelWidth,0,PaintboothHeight),
                new Point3D(PanelWidth,W,PaintboothHeight),
                new Point3D(0,W,PaintboothHeight),
            });
            drawing.Entities.Add(rectangle, Color.AntiqueWhite);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);

            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,PaintboothHeight),
                new Point3D(PanelWidth,0,PaintboothHeight),
                new Point3D(PanelWidth,W,PaintboothHeight),
                new Point3D(0,W,PaintboothHeight),
                new Point3D(0,0,PaintboothHeight),
            });

            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, PaintboothHeight),
                new Point3D(0,SheetThickness,PaintboothHeight),
                new Point3D(0,SheetThickness,(PaintboothHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1,(PaintboothHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1, (PaintboothHeight+StandardBend2)),
                new Point3D(0,0, (PaintboothHeight+StandardBend2))
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.AntiqueWhite);

            #region TubeLight
            int TubeLightQuantity = 1;
            //double PaintBoothArea = (model.W / 1000) * (model.D / 1000);
            decimal PaintBoothArea = ((decimal)(model.W * model.D)) / 1000000m;//Area is in SquareMeters
            TubeLightCalculations = Math.Ceiling(((decimal)PaintBoothArea * LuxLevel) / (Lumens * TubeLightQuantity * 0.7m));//0.7m is scaleing factor

            model.Lights = (int)TubeLightCalculations;




            #region fetch Tublight Table and save Quantity of TB in database
            // Step 2: Retrieve the saved record based on a unique identifier
            var savedRecord = _DbContext.TubeLightDetails
                                        .FirstOrDefault(x => x.EnquiryID == model.EnquiryId || x.SalesNo == model.SalesNO);

            // Step 3: Update the Quantity value
            if (savedRecord != null)
            {
                savedRecord.Quantity = (int)TubeLightCalculations;

                // Step 4: Save the changes for the updated Quantity
                _DbContext.SaveChanges();  // Only the Quantity column will be updated
            }
            #endregion
            #endregion
            #endregion

            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            dwgFilePath = $"{path}/PaintBooth drawing/TopPanel {j} {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            string pdfFilePath = $"{path}/PaintBooth drawing/TopPanel {j} {DateTime.Now:hh - mm}.pdf";

            Write3DPdfParams pdfauto = new Write3DPdfParams(drawing);
            Write3DPDF pdf12 = new Write3DPDF(pdfauto, pdfFilePath);
            pdf12.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                   .FirstOrDefault(p =>
                       p.EnquiryId == EnquiryID &&
                       p.PanelPosition == "TopPanels" &&
                       p.StandardPanelDepth == PanelWidth &&
                       p.StandardPanelHeight == PanelHeight);

            // If the record does not exist, save it
            if (existingRecord == null || existingRecord.NoOfPanels == 0)
            {
                SavePanelDetails(model, "TopPanels", k, TopPanel_Weight);
            }
            else
            {
                // Increment NoOfPanels
                existingRecord.NoOfPanels += 1;
                _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            }
            //}
            // TubelightCutout(model);
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass TopSidePanelsForFAS(int j, PaintBoothModel model, int m, int PlenumHeight)
        {
            #region old code

            string panelPosition;
            string dwgFilePath = "";
            int k = 0;
            double PaintboothHeight = model.H + PlenumHeight;
            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;

            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            W = PanelLengthForTopPanels;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,PaintboothHeight),
                new Point3D(PanelWidth,0,PaintboothHeight),
                new Point3D(PanelWidth,W,PaintboothHeight),
                new Point3D(0,W,PaintboothHeight),
            });
            drawing.Entities.Add(rectangle, Color.AntiqueWhite);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);

            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,PaintboothHeight),
                new Point3D(PanelWidth,0,PaintboothHeight),
                new Point3D(PanelWidth,W,PaintboothHeight),
                new Point3D(0,W,PaintboothHeight),
                new Point3D(0,0,PaintboothHeight),
            });

            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, PaintboothHeight),
                new Point3D(0,SheetThickness,PaintboothHeight),
                new Point3D(0,SheetThickness,(PaintboothHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1,(PaintboothHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1, (PaintboothHeight+StandardBend2)),
                new Point3D(0,0, (PaintboothHeight+StandardBend2))
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.AntiqueWhite);

            #region TubeLight
            int TubeLightQuantity = 1;
            //double PaintBoothArea = (model.W / 1000) * (model.D / 1000);
            decimal PaintBoothArea = ((decimal)(model.W * model.D)) / 1000000m;//Area is in SquareMeters
            TubeLightCalculations = Math.Ceiling(((decimal)PaintBoothArea * LuxLevel) / (Lumens * TubeLightQuantity * 0.7m));//0.7m is scaleing factor

            model.Lights = (int)TubeLightCalculations;




            #region fetch Tublight Table and save Quantity of TB in database
            // Step 2: Retrieve the saved record based on a unique identifier
            var savedRecord = _DbContext.TubeLightDetails
                                        .FirstOrDefault(x => x.EnquiryID == model.EnquiryId || x.SalesNo == model.SalesNO);

            // Step 3: Update the Quantity value
            if (savedRecord != null)
            {
                savedRecord.Quantity = (int)TubeLightCalculations;

                // Step 4: Save the changes for the updated Quantity
                _DbContext.SaveChanges();  // Only the Quantity column will be updated
            }
            #endregion
            #endregion
            #endregion

            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            dwgFilePath = $"{path}/PaintBooth drawing/TopPanelForFAS {j} {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            string pdfFilePath = $"{path}/PaintBooth drawing/TopPanelForFAS {j} {DateTime.Now:hh - mm}.pdf";

            Write3DPdfParams pdfauto = new Write3DPdfParams(drawing);
            Write3DPDF pdf12 = new Write3DPDF(pdfauto, pdfFilePath);
            pdf12.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                   .FirstOrDefault(p =>
                       p.EnquiryId == EnquiryID &&
                       p.PanelPosition == "TopPanelForFAS" &&
                       p.StandardPanelDepth == PanelWidth &&
                       p.StandardPanelHeight == PanelHeight);

            // If the record does not exist, save it
            if (existingRecord == null || existingRecord.NoOfPanels == 0)
            {
                SavePanelDetails(model, "TopPanelForFAS", k, TopPanel_Weight);
            }
            else
            {
                // Increment NoOfPanels
                existingRecord.NoOfPanels += 1;
                _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            }
            //}
            // TubelightCutout(model);
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        #endregion
        #region 3D Back panels 
        public PaintBoothclass BackPanelsAfterExtractionChamber(PaintBoothModel model/*,string DraftSubType*/, double EXtractionChemberHeight)
        {

            model.D = model.D;
            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double x = 0;
            //if (DraftSubType == "5"|| DraftSubType == "7"||DraftSubType=="4"|| DraftSubType == "6")
            //     x = model.D + D3 + D3;
            //else
            x = model.D + D3;
            var rectangle = devregion.CreatePolygon(new Point3D[]
                {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,EXtractionChemberHeight),//Changed PanelHeight to EXtractionChemberHeight 
                new Point3D(x,0,EXtractionChemberHeight)
                });
            drawing.Entities.Add(rectangle, Color.Green);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,EXtractionChemberHeight),
                new Point3D(x,0,EXtractionChemberHeight),
                new Point3D(x,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(x, 0, 0),
                new Point3D(((x)+36.8),0,0),
                new Point3D(((x)+36.8),0,1.2),
                new Point3D(((x)+SheetThickness),0,SheetThickness),
                new Point3D(((x)+SheetThickness),0,36.8),
                new Point3D(x,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/RearPanel {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "BackPanels" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == PanelHeight);

            // If the record does not exist, save it
            if (existingRecord == null || existingRecord.NoOfPanels == 0)
            {
                SavePanelDetails(model, "BackPanels", k, BackPanel_Weight);
            }
            else
            {
                // Increment NoOfPanels
                existingRecord.NoOfPanels += 1;
                _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            }


            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass BackPanels(PaintBoothModel model/*, string DraftSubType*/, double EXtractionChemberHeight)
        {

            model.D = model.D;
            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double x = 0;
            double z = model.H - EXtractionChemberHeight;
            z = 200;
            //if (DraftSubType == "5" || DraftSubType == "7" || DraftSubType == "4" || DraftSubType == "6")
            //    x = model.D + D3 + D3;
            //else
            x = model.D + D3;
            var rectangle = devregion.CreatePolygon(new Point3D[]
                {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,z),
                new Point3D(x,0,z)
                });
            drawing.Entities.Add(rectangle, Color.Green);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,z),
                new Point3D(x,0,z),
                new Point3D(x,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(x, 0, 0),
                new Point3D(((x)+36.8),0,0),
                new Point3D(((x)+36.8),0,1.2),
                new Point3D(((x)+SheetThickness),0,SheetThickness),
                new Point3D(((x)+SheetThickness),0,36.8),
                new Point3D(x,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/RearPanelBeforeExtractionC {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "BackPanels" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == PanelHeight);

            // If the record does not exist, save it
            if (existingRecord == null || existingRecord.NoOfPanels == 0)
            {
                SavePanelDetails(model, "BackPanels", k, BackPanel_Weight);
            }
            else
            {
                // Increment NoOfPanels
                existingRecord.NoOfPanels += 1;
                _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            }


            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass BackPanelsForType5(PaintBoothModel model/*, string DraftSubType*/, int PlenumHeight)
        {


            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            //double x = model.D + D3;
            double x = model.D;
            //if (DraftSubType == "5" || DraftSubType == "7"|| DraftSubType=="3" || DraftSubType == "4" || DraftSubType == "6")
            //    x  -= D3;

            var rectangle = devregion.CreatePolygon(new Point3D[]
                {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,PlenumHeight),
                new Point3D(x,0,PlenumHeight)
                });
            drawing.Entities.Add(rectangle, Color.AntiqueWhite);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(x,0,0),
                new Point3D(x,BackPanelLength,0),
                new Point3D(x,BackPanelLength,PlenumHeight),
                new Point3D(x,0,PlenumHeight),
                new Point3D(x,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(x, 0, 0),
                new Point3D(((x)+36.8),0,0),
                new Point3D(((x)+36.8),0,1.2),
                new Point3D(((x)+SheetThickness),0,SheetThickness),
                new Point3D(((x)+SheetThickness),0,36.8),
                new Point3D(x,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.AntiqueWhite);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/RearPanel {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "BackPanels" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == 1000);

            // If the record does not exist, save it
            if (existingRecord == null || existingRecord.NoOfPanels == 0)
            {
                SavePanelDetails(model, "BackPanels", k, BackPanel_Weight);
            }
            else
            {
                // Increment NoOfPanels
                existingRecord.NoOfPanels += 1;
                _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            }
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };
        }

        public PaintBoothclass FrontPanels(PaintBoothModel model)
        {
            model.D = model.D;
            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,FrontPanelLength,0),
                new Point3D(0,FrontPanelLength,FrontPanelHeight),
                new Point3D(0,0,FrontPanelHeight)
            });
            drawing.Entities.Add(rectangle, Color.Green);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,FrontPanelLength,0),
                new Point3D(0,FrontPanelLength,FrontPanelHeight),
                new Point3D(0,0,FrontPanelHeight),
                new Point3D(0,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D((0+36.8),0,0),
                new Point3D((0+36.8),0,1.2),
                new Point3D((0+SheetThickness),0,SheetThickness),
                new Point3D((0+SheetThickness),0,36.8),
                new Point3D(0,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/FrontPanel {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            var existingRecord = _DbContext.PanelDetails
                    .FirstOrDefault(p =>
                        p.EnquiryId == EnquiryID &&
                        p.PanelPosition == "FrontPanels" &&
                        p.StandardPanelDepth == PanelWidth &&
                        p.StandardPanelHeight == PanelHeight);

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass FrontPanelsWithDoors(double y, PaintBoothModel model)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double z = (double)model.HingedDoorHeight;
            //double z = model.Width + 700 + 250;//Component Width+700+250=total panel Height 
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,y,0),
                new Point3D(0,y,z),
                new Point3D(0,0,z)
            });
            drawing.Entities.Add(rectangle, Color.BurlyWood);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,y,0),
                new Point3D(0,y,z),
                new Point3D(0,0,z),
                new Point3D(0,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D((0+36.8),0,0),
                new Point3D((0+36.8),0,1.2),
                new Point3D((0+SheetThickness),0,SheetThickness),
                new Point3D((0+SheetThickness),0,36.8),
                new Point3D(0,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/FrontPanelWithDoors {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            //var existingRecord = _DbContext.PanelDetails
            //        .FirstOrDefault(p =>
            //            p.EnquiryId == EnquiryID &&
            //            p.PanelPosition == "BackPanels" &&
            //            p.StandardPanelDepth == PanelWidth &&
            //            p.StandardPanelHeight == PanelHeight);

            //// If the record does not exist, save it
            //if (existingRecord == null || existingRecord.NoOfPanels == 0)
            //{
            //    SavePanelDetails(model, pmodel, "BackPanels", k, BackPanel_Weight);
            //}
            //else
            //{
            //    // Increment NoOfPanels
            //    existingRecord.NoOfPanels += 1;
            //    _DbContext.SaveChanges(); // Only the NoOfPanels column will be updated
            //}
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass FrontPanelsForType5(PaintBoothModel model, int PlenumHeight)
        {
            //double BackPanel_Weight = 0;
            PanelInputModel pmodel = new PanelInputModel();
            if (model.PanelTypes == "1")
                model.D = model.StandardPanelWidthForD;

            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,BackPanelLength,0),
                new Point3D(0,BackPanelLength,PlenumHeight),
                new Point3D(0,0,PlenumHeight)
            });
            drawing.Entities.Add(rectangle, Color.AntiqueWhite);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(0,BackPanelLength,0),
                new Point3D(0,BackPanelLength,PlenumHeight),
                new Point3D(0,0,PlenumHeight),
                new Point3D(0,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D((0+36.8),0,0),
                new Point3D((0+36.8),0,1.2),
                new Point3D((0+SheetThickness),0,SheetThickness),
                new Point3D((0+SheetThickness),0,36.8),
                new Point3D(0,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            BackPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.AntiqueWhite);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/FrontPanelInTop {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion


            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        #endregion
        #region Structureframe
        public PaintBoothclass TopStructureFrame(PaintBoothModel model)
        {
            //if (model.PanelTypesforH == "1")
            //    PanelHeight = model.PanelHeightforH;
            //else
            PanelHeight = model.H;

            drawing = new();
            int k = 0;
            drawing.Units = linearUnitsType.Millimeters;


            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,-SettingStandardBend2,PanelHeight),
                new Point3D(model.D,-SettingStandardBend2,PanelHeight),
                new Point3D(model.D,(model.W-SettingStandardBend2),PanelHeight),
                new Point3D(0,(model.W-SettingStandardBend2),PanelHeight),
            });

            LinearPath rail = new LinearPath(new Point3D[]
           {
               new Point3D(0,0,PanelHeight+75),
                new Point3D(model.D,0,PanelHeight+75),
                new Point3D(model.D,model.W,PanelHeight+75),
                new Point3D(0,model.W,PanelHeight+75),
                 new Point3D(0,0,PanelHeight+75),
           });

            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, PanelHeight),
                new Point3D(0,0,(SettingH+PanelHeight)),
                new Point3D(0,-SettingW,(SettingH+PanelHeight)),
                new Point3D(0,-SettingW,((SettingH-SettingT)+PanelHeight)),
                new Point3D(0,-SettingT,((SettingH-SettingT)+PanelHeight)),
                new Point3D(0,-SettingT,(SettingT+PanelHeight)),
                new Point3D(0,-SettingW,(SettingT+PanelHeight)),
                new Point3D(0,-SettingW,PanelHeight)
            });

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);
            //Calculate weight of TopStructureFrame
            Material mt = Material.StructuralSteel;
            mt = new(Materials);
            frame = GenerateHoles(frame);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mt, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopStructureFrame_Weight = Frame_Weight;
            SavePanelDetails(model, "TopStructureFrame", k, TopStructureFrame_Weight);

            drawing.Entities.Add(frame, Color.Yellow);
            model.CChannelHeight = SettingH;
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/TopStructure {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
        }

        public PaintBoothclass BaseStructure(PaintBoothModel model/*,string DraftSubType*/)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            int k = 0;

            //W = model.W;

            double x = 0;
            //if(DraftSubType=="7"|| DraftSubType == "5" || DraftSubType == "4"||DraftSubType=="6")
            //    x = model.D + D3 +D3;
            //else 
            x = model.D + D3;
            var rectangle = devregion.CreatePolygon(new Point3D[]
                {
                new Point3D(0,0,0),
                new Point3D(x,0,0),
                new Point3D(x,model.W,0),
                new Point3D(0,model.W,0),
                });
            // drawing.Entities.Add(rectangle, Color.Pink);
            #region Frame Calculations
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(x,0,0),
                new Point3D(x,model.W,0),
                new Point3D(0,model.W,0),
                // new Point3D(0,0,0),
            });
            devregion section;
            if (Section == "Bend Section")
            {
                section = devregion.CreatePolygon(new Point3D[]
               {

                new Point3D(0, 0, 0),
                new Point3D(0,0,-SettingH),
                new Point3D(0,-SettingW,-SettingH),
                new Point3D(0,-SettingW,-(SettingH-SettingT)),
                new Point3D(0,-SettingT,-(SettingH-SettingT)),
                new Point3D(0,-SettingT,-SettingT),
                new Point3D(0,-SettingW,-SettingT),
                new Point3D(0,-SettingW,0)
                });
            }
            else
            {
                section = devregion.CreatePolygon(new Point3D[]
               {

                new Point3D(0, 0, 0),
                new Point3D(0,0,-75),
                new Point3D(0,-40,-75),
                new Point3D(0,-40,-72),
                new Point3D(0,-3,-72),
                new Point3D(0,-3,-3),
                new Point3D(0,-40,-3),
                new Point3D(0,-40,0)
               });
            }

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);
            //Calculate Weight of BaseStructureFrame
            Material mt = Material.StructuralSteel;
            frame.Regen(0.1);
            double MassofBaseFrame = frame.GetMass(mt, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            BaseFrame_Weight = Math.Round(MassofBaseFrame, 3);
            #endregion
            SavePanelDetails(model, "BaseStructureFrame", k, BaseFrame_Weight);

            GenerateHoles(frame);
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/BaseStructure {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
        }
        #endregion
        private Solid GenerateHoles(Solid frame)
        {
            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;
            double divisionresult = PanelHeight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);
            double multipliedresult = wholenumberpart * PitchDistance;
            double samespacedivide = PanelHeight - multipliedresult;

            for (int i = 0; i < wholenumberpart; i++)
            {
                double centerz = i == 0 ? samespacedivide / 2 : PanelHeight - (i + 1) * PitchDistance + (samespacedivide / 2);
                devregion ssr2 = devregion.CreateSlot(Plane.YZ, (StandardBend2 / 2), centerz, 5, 2, 1.5708);
                ssr2.Translate(0, 0, samespacedivide / 2);

                frame.ExtrudeRemove(ssr2, PanelWidth, 0);
                drawing.Entities.Add(frame);
            }
            //for generating holes on xy Plane
            double divisionresult1 = PanelWidth / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);
            double multipliedresult1 = wholenumberpart1 * PitchDistance;
            double samespacedivide1 = PanelWidth - multipliedresult1;

            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centerx = i == 0 ? samespacedivide1 / 2 : PanelWidth - (i + 1) * PitchDistance + (samespacedivide / 2);
                devregion circle1 = devregion.CreateSlot(Plane.XY, centerx, (StandardBend2 / 2), 5, 2);

                frame.ExtrudeRemove(circle1, PanelHeight, 0);
            }
            return frame;
        }
        private devregion CreatePolygon(int k)
        {
            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;
            if (k == 1)
            {
                return devregion.CreatePolygon(new Point3D[]
                {
                    new Point3D(0, 0, 0),
                    new Point3D(SheetThickness, 0, 0),
                    new Point3D(SheetThickness, StandardBend2 - SheetThickness, 0),
                    new Point3D(StandardBend1, StandardBend2 - SheetThickness, 0),
                    new Point3D(StandardBend1, StandardBend2, 0),
                    new Point3D(0, StandardBend2, 0)
                });
            }
            else
            {
                return devregion.CreatePolygon(new Point3D[]
                {
                    new Point3D(0, 0, 0),
                    new Point3D(SheetThickness, 0, 0),
                    new Point3D(SheetThickness, -(StandardBend2 - SheetThickness), 0),
                    new Point3D(StandardBend1, -(StandardBend2 - SheetThickness), 0),
                    new Point3D(StandardBend1, -StandardBend2, 0),
                    new Point3D(0, -StandardBend2, 0)
                });
            }
        }
        public List<PaintBoothclass> OuterFilterFrame(PaintBoothModel model/*,string DraftSubType,*/, double extractionChamberHeight)
        {
            List<PaintBoothclass> metalBaffleDrawings = new List<PaintBoothclass>();

            #region Setup Parameters
            model.W = model.PanelTypesforW == "1" ? model.PanelHeightforW : model.W;
            model.H = model.PanelTypesforH == "1" ? model.PanelHeightforH : model.H;
            model.D = model.PanelTypes == "1" ? model.StandardPanelWidthForD : model.D;


            double frameWidth = 600; // Width of each baffle
                                     // double frameHeight = 1000; // Height of each baffle
            model.FrameWidth = frameWidth;
            noOfFramesW = Math.Floor(model.W / frameWidth);
            bafflePanelCount = (int)noOfFramesW;
            double offsetX = 0;

            offsetX = model.D;
            double equalFilterSpace = (model.W - (noOfFramesW * frameWidth)) / 2;
            int BaffleHeight = 1140;

            model.FilterHeight = BaffleHeight;
            int totalBafflesinHeight = (int)extractionChamberHeight / BaffleHeight;
            #endregion

            #region Generate and Save Separate Drawings for Each Metal Baffle
            for (int j = 0; j < totalBafflesinHeight; j++)
            {

                for (int i = 0; i < noOfFramesW; i++)
                {
                    // Create a new drawing document for each baffle instance
                    DesignDocument drawing = new DesignDocument();
                    drawing.Units = linearUnitsType.Millimeters;

                    // Calculate position for the current baffle
                    double posX = offsetX;

                    // Create an instance of MetalBaffle for the current position
                    PaintBoothclass metalBaffleInstance = MetalBaffle(BaffleHeight);

                    foreach (Entity entity in metalBaffleInstance.drawing.Entities)
                    {
                        Entity clonedEntity = (Entity)entity.Clone();

                        // Rotate and translate to position each metal baffle
                        clonedEntity.Rotate(Math.PI / 2, Vector3D.AxisZ, new Point3D(0, 0, 0));
                        clonedEntity.Translate(posX, equalFilterSpace, 0);

                        drawing.Entities.Add(clonedEntity, entity.Color);
                    }
                    // Call the FilterFrame1 method to generate the current frame
                    PaintBoothclass filterFrameInstance = FilterFrame1(BaffleHeight, model);

                    // Position each frame at an offset
                    posX = offsetX + 100; // Add 100 units to X for each iteration               

                    foreach (Entity entity in filterFrameInstance.drawing.Entities)
                    {
                        Entity clonedEntity = (Entity)entity.Clone();
                        clonedEntity.Translate(posX, equalFilterSpace, 0); // Translate to new X position                   
                        drawing.Entities.Add(clonedEntity, entity.Color);
                    }


                    // Save the individual drawing
                    var path = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                  .Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

                    if (!Directory.Exists(path + "/PaintBooth drawing/OuterFilterFrame"))
                        Directory.CreateDirectory(path + "/PaintBooth drawing/OuterFilterFrame");

                    string dwgFilePath = $"{path}/PaintBooth drawing/OuterFilterFrame/FilterFrame_{i + 1}_{DateTime.Now:hh-mm}.dwg";

                    WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
                    WriteAutodesk dwgWriter = new WriteAutodesk(auto, dwgFilePath);
                    dwgWriter.DoWork();

                    // Add the individual drawing info to the list
                    metalBaffleDrawings.Add(new PaintBoothclass
                    {
                        drawing = drawing,
                        lstpath = dwgFilePath
                    });
                }

            }
            #endregion
            SaveMetalBaffleDetails(model);
            SaveFilterDetails(model);
            return metalBaffleDrawings;
        }

        //this method for when down draft With FAS without Civil paintbooth type=5 then call 
        public List<PaintBoothclass> OuterFilterFrameAtZeroX(PaintBoothModel model, string DraftSubType)
        {
            List<PaintBoothclass> metalBaffleDrawings = new List<PaintBoothclass>();

            #region Setup Parameters
            model.W = model.PanelTypesforW == "1" ? model.PanelHeightforW : model.W;
            model.H = model.PanelTypesforH == "1" ? model.PanelHeightforH : model.H;
            model.D = model.PanelTypes == "1" ? model.StandardPanelWidthForD : model.D;

            double frameWidth = 600;
            model.FrameWidth = frameWidth;
            noOfFramesW = Math.Floor(model.W / frameWidth);
            bafflePanelCount = (int)noOfFramesW;

            double equalFilterSpace = (model.W - (noOfFramesW * frameWidth)) / 2;
            int BaffleHeight = 1000;
            int baffleHeight1 = 1200;
            int paintBoothHeight = (int)model.H;

            int remainingHeight1 = paintBoothHeight % BaffleHeight;
            int remainingHeight2 = paintBoothHeight % baffleHeight1;

            if (remainingHeight1 < remainingHeight2)
            {
                selectedBaffleHeight = BaffleHeight;
            }
            else
            {
                selectedBaffleHeight = baffleHeight1;
            }

            model.FilterHeight = selectedBaffleHeight;
            int totalBafflesinHeight = paintBoothHeight / selectedBaffleHeight;
            #endregion

            #region Generate and Save Separate Drawings for Each Metal Baffle
            for (int j = 0; j < totalBafflesinHeight; j++)
            {
                for (int i = 0; i < noOfFramesW; i++)
                {
                    DesignDocument drawing = new DesignDocument();
                    drawing.Units = linearUnitsType.Millimeters;


                    double posX = D3; // Updated position instead of 0
                    double framePosX = D3;

                    // Add metal baffle
                    PaintBoothclass metalBaffleInstance = MetalBaffle(selectedBaffleHeight);
                    foreach (Entity entity in metalBaffleInstance.drawing.Entities)
                    {
                        Entity clonedEntity = (Entity)entity.Clone();
                        clonedEntity.Rotate(Math.PI / 2, Vector3D.AxisZ, new Point3D(0, 0, 0));
                        clonedEntity.Translate(posX, equalFilterSpace, 0);
                        drawing.Entities.Add(clonedEntity, entity.Color);
                    }

                    // Add filter frame
                    PaintBoothclass filterFrameInstance = FilterFrame1(selectedBaffleHeight, model);
                    foreach (Entity entity in filterFrameInstance.drawing.Entities)
                    {
                        Entity clonedEntity = (Entity)entity.Clone();
                        clonedEntity.Translate(framePosX, equalFilterSpace, 0);
                        drawing.Entities.Add(clonedEntity, entity.Color);
                    }

                    // Regenerate entity bounds once at the end
                    drawing.Entities.Regen();

                    // Validate entity presence
                    if (drawing.Entities.Count == 0)
                        throw new Exception("No entities were added to the drawing. Cannot export empty DWG.");

                    // Prepare path
                    var path = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build()
                        .GetSection("FolderPathConfig")["AbsolutePath"];

                    string folderPath = Path.Combine(path, "PaintBooth drawing", "OuterFilterFrameZeroX");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string dwgFilePath = Path.Combine(folderPath, $"FilterFrame_{i + 1}_{DateTime.Now:HH-mm-ss}.dwg");

                    // Write DWG file
                    try
                    {
                        WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
                        WriteAutodesk dwgWriter = new WriteAutodesk(auto, dwgFilePath);
                        dwgWriter.DoWork(); // <-- Main export
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"DWG export failed for frame {i + 1}: {ex.Message}", ex);
                    }

                    metalBaffleDrawings.Add(new PaintBoothclass
                    {
                        drawing = drawing,
                        lstpath = dwgFilePath
                    });
                }
            }
            #endregion

            //SaveMetalBaffleDetails(model);
            //SaveFilterDetails(model);
            return metalBaffleDrawings;
        }
        public PaintBoothclass FAS(/*int j,*/ PaintBoothModel model, int m)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;

            #region Setup Parameters
            model.W = model.PanelTypesforW == "1" ? model.PanelHeightforW : model.W;
            model.H = model.PanelTypesforH == "1" ? model.PanelHeightforH : model.H;
            model.D = model.PanelTypes == "1" ? model.StandardPanelWidthForD : model.D;
            PanelHeight = model.H;

            double totalFilterAreaSqM = model.FilterArea;
            double frameWidthMm = 1200;
            double frameHeightMm = 600;

            // Convert frame area from mm² to m²
            double frameAreaSqM = (frameWidthMm * frameHeightMm) / 1_000_000.0;

            // Calculate number of frames needed
            int numberOfFrames = (int)Math.Floor(totalFilterAreaSqM / frameAreaSqM);
            #endregion
            //double xOffset = m * frameWidthMm; // `m` is the index passed in

            //var rectangle = devregion.CreatePolygon(new Point3D[]
            //{
            //    new Point3D(xOffset, 0, PanelHeight),
            //    new Point3D(xOffset + frameWidthMm, 0, PanelHeight),
            //    new Point3D(xOffset + frameWidthMm, frameHeightMm, PanelHeight),
            //    new Point3D(xOffset, frameHeightMm, PanelHeight),
            //});
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, PanelHeight),
                new Point3D(frameWidthMm, 0, PanelHeight),
                new Point3D(frameWidthMm, frameHeightMm, PanelHeight),
                new Point3D(0,frameHeightMm, PanelHeight),
            });
            drawing.Entities.Add(rectangle, Color.AntiqueWhite);

            // Save the individual drawing
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                          .Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing/FAS"))
                Directory.CreateDirectory(path + "/PaintBooth drawing/FAS");

            string dwgFilePath = $"{path}/PaintBooth drawing/FAS/Filters_{DateTime.Now:hh-mm-ss}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgWriter = new WriteAutodesk(auto, dwgFilePath);
            dwgWriter.DoWork();

            // Add the individual drawing info to the list
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };

        }
        public int SaveFilterDetails(PaintBoothModel model)
        {
            FilterFrameDetails obj = new FilterFrameDetails();
            obj.IsDeleted = false;
            obj.EnquiryID = EnquiryID;
            obj.SalesNO = model.SalesNO;
            obj.FrameWidth = (decimal)model.FrameWidth;
            obj.FrameHeight = (decimal)model.FilterHeight;
            obj.Quantity = (int)(noOfFramesW * 2);
            obj.FrameWeight = (decimal)(totalWeight) * (int)(noOfFramesW * 2);
            _DbContext.FilterFrameDetails.Add(obj);
            _DbContext.SaveChanges();
            return 1;
        }
        public int SaveMetalBaffleDetails(PaintBoothModel model)
        {
            MetalBaffleDetails obj = new MetalBaffleDetails();
            obj.IsDeleted = false;
            obj.EnquiryID = EnquiryID;
            obj.SalesNo = model.SalesNO;
            obj.BaffleWidth = (decimal)model.FrameWidth;
            obj.BaffleHeight = (decimal)model.FilterHeight;
            obj.Quantity = (int)(noOfFramesW * 2);
            obj.BaffleWeight = (decimal)(totalWeightOfbaffle * (int)(noOfFramesW * 2));
            _DbContext.MetalBaffleDetails.Add(obj);
            _DbContext.SaveChanges();
            return 1;
        }
        public int SavePanelDetails(PaintBoothModel model, string panelPosition, int k, double PanelWeight)
        {
            PanelDetails tblobj = new PanelDetails();
            tblobj.IsDeleted = false;
            tblobj.EnquiryId = EnquiryID;
            tblobj.SalesNo = model.SalesNO;
            tblobj.SlotDimention = SlotDimention;
            tblobj.PanelPosition = panelPosition;

            //tblobj.EqualPanelDepth = 0;
            //tblobj.EqualPanelHeight = 0;
            //tblobj.EqualPanelWidth = 0;
            tblobj.NoOfPanels = 0;

            if (model.PanelTypes == "1" || model.PanelTypesforW == "1" || model.PanelTypesforH == "1")
            {
                tblobj.StandardPanelWidth = model.PanelHeightforW;
                tblobj.StandardPanelDepth = PanelWidth;
                tblobj.StandardPanelHeight = model.PanelHeightforH;
            }
            else
            {
                tblobj.StandardPanelWidth = PanelHeight;
                tblobj.StandardPanelDepth = PanelWidth;
                tblobj.StandardPanelHeight = PanelHeight;

                //if (model.PanelTypes != "1")
                //    tblobj.EqualPanelDepth = model.RemainingPanels;

                //if (model.PanelTypesforW != "1")
                //    tblobj.EqualPanelWidth = model.RemainingPanelsByW;

                //if (model.PanelTypesforH != "1")
                //    tblobj.EqualPanelHeight = model.RemainingPanelsByH;
            }
            if (panelPosition == "RightSide")
            {
                tblobj.NoOfPanels = 1;
                tblobj.PanelWeight = PanelWeight;

            }
            if (panelPosition == "LeftSide")
            {
                tblobj.NoOfPanels = 1;
                tblobj.PanelWeight = PanelWeight;
            }
            if (panelPosition == "BackPanels")
            {
                tblobj.NoOfPanels = 1;
                tblobj.PanelWeight = PanelWeight;
            }

            if (panelPosition == "BaseStructureFrame")
            {
                tblobj.PanelWeight = PanelWeight;
                tblobj.FrameHeight = model.W;
                tblobj.FrameWidth = model.D + D3;
                tblobj.NoOfPanels = 0;
            }
            if (panelPosition == "D3Panels Right side")
            {
                tblobj.PanelWeight = PanelWeight;
                tblobj.StandardPanelWidth = PanelHeight;
                tblobj.StandardPanelDepth = D3;
                tblobj.StandardPanelHeight = PanelHeight;
                tblobj.NoOfPanels = 1;
                if (model.PanelTypes == "1" || model.PanelTypesforW == "1" || model.PanelTypesforH == "1")
                {
                    //tblobj.EqualPanelDepth = 0;
                    //tblobj.EqualPanelHeight = 0;
                }
                else
                {
                    tblobj.StandardPanelWidth = PanelHeight;
                    tblobj.StandardPanelHeight = PanelHeight;
                }
            }
            if (panelPosition == "D3Panels Left Side")
            {
                tblobj.PanelWeight = PanelWeight;
                tblobj.StandardPanelWidth = PanelHeight;
                tblobj.StandardPanelDepth = D3;
                tblobj.StandardPanelHeight = PanelHeight;
                tblobj.NoOfPanels = 1;
                if (model.PanelTypes == "1" || model.PanelTypesforW == "1" || model.PanelTypesforH == "1")
                {
                    //tblobj.EqualPanelDepth = 0;
                    //tblobj.EqualPanelHeight = 0;
                }
                else
                {
                    tblobj.StandardPanelWidth = PanelHeight;
                    tblobj.StandardPanelHeight = PanelHeight;
                }
            }
            if (panelPosition == "TopPanels")
            {

                tblobj.PanelWeight = PanelWeight;
                tblobj.NoOfPanels = 1;
                if (model.PanelTypes == "2" || model.PanelTypesforW == "2" || model.PanelTypesforH == "2")
                {
                    tblobj.StandardPanelWidth = PanelLength;
                    tblobj.StandardPanelHeight = PanelHeight;
                    tblobj.StandardPanelDepth = PanelWidth;
                }
                else
                {
                    tblobj.StandardPanelWidth = model.PanelHeightforW;
                }
            }

            if (panelPosition == "TopStructureFrame")
            {
                tblobj.PanelWeight = PanelWeight;
                tblobj.FrameHeight = model.W;
                tblobj.FrameWidth = model.D + D3;
                tblobj.NoOfPanels = 0;
            }

            tblobj.StandardBend1 = (decimal)SettingStandardBend1;
            tblobj.StandardBend2 = (decimal)SettingStandardBend2;
            tblobj.SheetThickness = (decimal)SheetThickness;
            tblobj.PitchDistance = (decimal)PitchDistance;
            tblobj.CostingStatus = true;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.ModifiedBy = 0;
            _DbContext.PanelDetails.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }

        public PaintBoothclass detailsdrawing(DesignDocument model, PaintBoothModel pmodel)
        {

            model.Entities.Regen();
            model.Entities.UpdateBoundingBox();
            string EnquiryCode = pmodel.SalesNO;
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            string partname = "CombineAssembly";
            model.Units = linearUnitsType.Millimeters;
            AddSheet asfp = new AddSheet();

            drawingdoc = asfp.AddSheets(partname);

            #region INSERTION OF VIEWS
            // Get the model bounding box (Eyeshot axis: X = width, Z = depth, Y = height)
            Point3D box = model.Entities.BoxSize;
            double modelWidth = Math.Max(box.X, box.Z); // largest horizontal span (width or depth)
            double modelHeight = box.Y + 4900;                  // vertical height (front view height)

            #endregion

            #region SCALING SHEET

            // Define usable drawing area (A2 size minus margins)
            double usableWidth = 594 - 40;     // Adjust to match StandardFrame margins
            double usableHeight = 420 - 80;    // Adjust to match titlebox height

            // Safety check
            if (usableWidth <= 0) usableWidth = 1;
            if (usableHeight <= 0) usableHeight = 1;

            // Add margins
            double marginX = 40;
            double marginY = 40;

            // Calculate the required scale
            double scaleX = (modelWidth + marginX) / usableWidth;
            double scaleY = (modelHeight + marginY) / usableHeight;

            double requiredScale = Math.Max(scaleX, scaleY);

            int finalScaleFactor = (int)Math.Ceiling(requiredScale);
            if (finalScaleFactor < 1) finalScaleFactor = 1;

            #endregion


            #region addviews

            MySheet mysheet;
            pmodel.standardbend2 = (decimal)SettingStandardBend2;
            var panelDetails = GetAllPanels(EnquiryCode);
            mysheet = asfp.StandardFrame(finalScaleFactor, (MySheet)drawingdoc.ActiveSheet, new Point2D(0, 0), pmodel, panelDetails);
            const string Dim = "Dimension";

            drawingdoc.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            ViewBuilder vb = new ViewBuilder(model, drawingdoc);
            vb.DoWork();
            vb.AddTo(drawingdoc);
            drawingdoc.ActiveSheet = mysheet;
            #endregion addviews          

            #region writefiles
            if (!Directory.Exists(path + "/MFG Drawing"))
                Directory.CreateDirectory(path + "/MFG Drawing");
            string dwgFilePath = $"{path}/MFG Drawing/GA {DateTime.Now:hh-mm}.dwg";
            WriteAutodeskParams auto = new WriteAutodeskParams(drawingdoc);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion writefiles
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath

            };
        }
        public List<PanelDetails> GetAllPanels(string enquiryCode)
        {
            var panels = _DbContext.PanelDetails.Where(c => c.IsDeleted == false && c.SalesNo == enquiryCode)
                .Select(c => new PanelDetails
                {
                    PanelID = c.PanelID,
                    PanelPosition = c.PanelPosition,
                    PanelWeight = c.PanelWeight

                }).ToList();

            return panels;
        }
        public PaintBoothclass MetalBaffle(double FilterframeHeight)
        {
            DesignDocument drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;
            double W = 66;
            double H = 20;
            double ST = 1.2; //SheetThickness
            double offsetDistance = 36;
            double startPoint = 11.5;
            double ExtrudeHeight = FilterframeHeight;

            Material mat = new Material(Materials);
            List<double> partWeights = new List<double>();

            // Helper function to calculate weight
            double CalculateWeight(Brep brep, Material mat)
            {
                brep.Regen(0.1);
                double mass = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
                return Math.Abs(Math.Round(mass, 3));
            }

            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(startPoint,0),
                new Point3D(startPoint,H),
                new Point3D((W+startPoint),H),
                new Point3D((W+startPoint),0),
                new Point3D(((W+startPoint)-ST),0),
                new Point3D(((W+startPoint)-ST),H-ST),
                new Point3D(ST+startPoint,H-ST),
                new Point3D(ST+startPoint,0),
                new Point3D(startPoint,0),
            });
            Brep brep = rail.ExtrudeAsBrep(Vector3D.AxisZ * ExtrudeHeight, 0);

            double railWeight = CalculateWeight(brep, mat);
            partWeights.Add(railWeight);


            drawing.Entities.Add(brep, Color.Yellow);
            int i = 1;
            LinearPath railClone = rail;
            LinearPath oppositeRail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0),
                new Point3D(0,H),
                new Point3D(W,H),
                new Point3D(W,0),
                new Point3D(W-ST,0),
                new Point3D(W-ST,H-ST),
                new Point3D(ST,H-ST),
                new Point3D(ST,0),
                new Point3D(0,0),
            });
            while (i < 6)
            {
                railClone = (LinearPath)railClone.Clone();
                railClone.Translate(W + offsetDistance, 0, 0);
                Brep brepClone = railClone.ExtrudeAsBrep(Vector3D.AxisZ * ExtrudeHeight, 0); // Extrude each clone
                drawing.Entities.Add(brepClone, Color.White);
                // drawing.Entities.Add(railClone, Color.Yellow);
                i++;
            }
            i = 1;
            Point3D centerPoint = new Point3D(W / 2, 0); // Adjusted to be in the middle of the shape
            oppositeRail.Rotate(Math.PI, Vector3D.AxisZ, centerPoint);
            oppositeRail.Translate(54.5, 0);
            Brep oppositeBrepClone = oppositeRail.ExtrudeAsBrep(Vector3D.AxisZ * ExtrudeHeight, 0); // Extrude each opposite clone
            double oppositeBreplWeight = CalculateWeight(oppositeBrepClone, mat);
            partWeights.Add(oppositeBreplWeight);

            drawing.Entities.Add(oppositeBrepClone, Color.White);
            while (i < 5)
            {
                oppositeRail = (LinearPath)oppositeRail.Clone();
                oppositeRail.Translate(W + offsetDistance, 0);
                oppositeBrepClone = oppositeRail.ExtrudeAsBrep(Vector3D.AxisZ * ExtrudeHeight, 0); // Extrude each opposite clone
                drawing.Entities.Add(oppositeBrepClone, Color.White);

                i++;
            }

            // Define the Bottom Plate with dimensions 572.6 x 40 and position it 150 units from the bottom
            double bottomPlateWidth = 600;
            double bottomPlateHeight = 40;
            double bottomPlateYPosition = 150;

            LinearPath bottomPlate = new LinearPath(new Point3D[]
            {
                new Point3D(0, 0),
                new Point3D(bottomPlateWidth, 0),
                new Point3D(bottomPlateWidth, bottomPlateHeight),
                new Point3D(0, bottomPlateHeight),
                new Point3D(0, 0)
            });

            bottomPlate.Translate(0, 20, bottomPlateYPosition);
            // Define the hole radius and positions (20 units from each side)
            double holeRadius = 5; // Adjust hole radius as needed
            double holeOffsetFromEdge = 20;

            // Left Hole position
            Circle leftHole = new Circle(new Point3D(holeOffsetFromEdge, bottomPlateHeight, bottomPlateYPosition), holeRadius);

            // Right Hole position
            Circle rightHole = new Circle(new Point3D(bottomPlateWidth - holeOffsetFromEdge, bottomPlateHeight, bottomPlateYPosition), holeRadius);
            //// Extrude the Bottom Plate
            Brep bottomPlateBrep = bottomPlate.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0); // Extrude by 5 units
            double bottomPlateBrepWeight = CalculateWeight(bottomPlateBrep, mat);
            partWeights.Add(bottomPlateBrepWeight);

            Brep leftHoleBrep = leftHole.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);
            double leftHoleBrepWeight = CalculateWeight(leftHoleBrep, mat);
            partWeights.Add(leftHoleBrepWeight);

            Brep rightHoleBrep = rightHole.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);
            double rightHoleBreplWeight = CalculateWeight(rightHoleBrep, mat);
            partWeights.Add(rightHoleBreplWeight);

            // Add the Bottom Plate with holes to the drawing

            drawing.Entities.Add(leftHoleBrep, Color.Green);
            drawing.Entities.Add(rightHoleBrep, Color.Green);
            drawing.Entities.Add(bottomPlateBrep, Color.Green);

            LinearPath bottomPlateClone = (LinearPath)bottomPlate.Clone();
            // Adjust the Y translation for the cloned Bottom Plate
            bottomPlateClone.Translate(0, 0, 612);


            // Clone and translate the hole circles to the second Bottom Plate position
            Circle leftHoleClone = (Circle)leftHole.Clone();
            leftHoleClone.Translate(0, 0, 612);
            Circle rightHoleClone = (Circle)rightHole.Clone();
            rightHoleClone.Translate(0, 0, 612);

            // Extrude and subtract holes from the second Bottom Plate
            Brep leftHoleBrep2 = leftHoleClone.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);
            Brep rightHoleBrep2 = rightHoleClone.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);
            // Extrude and add the second Bottom Plate
            Brep bottomPlateBrep2 = bottomPlateClone.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0); // Extrude by 5 units
            double bottomPlateBrep2Weight = CalculateWeight(bottomPlateBrep2, mat);
            partWeights.Add(bottomPlateBrep2Weight);
            double totalWeight = 0;
            foreach (var weight in partWeights)
            {
                totalWeight += weight;

            }
            totalWeightOfbaffle = totalWeight;

            drawing.Entities.Add(leftHoleClone, Color.Green);
            drawing.Entities.Add(rightHoleClone, Color.Green);

            drawing.Entities.Add(bottomPlateBrep2, Color.Green);



            #region Write file         
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "MetalBaffle" + DateTime.Now.ToString("hh-mm") + ".dwg";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            ExportAllViewsWithSection(drawing, path);

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
            #endregion
        }
        public PaintBoothclass FilterFrame1(double filterHeight, PaintBoothModel model)
        {
            // Initialize drawing and dimensions
            drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;

            double frameWidth = 600;
            double frameHeight = filterHeight;
            double railWidth = 30;
            double railHeight = 40;
            double sheetThickness = 1.2;
            double holeRadius = 7 / 2.0;

            Material mat = new Material(Materials); // Define material properties


            // Helper function to calculate weight
            double CalculateWeight(Solid frame, Material mat)
            {
                frame.Regen(0.1);
                double mass = frame.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
                return Math.Abs(Math.Round(mass, 3));
            }

            // Frame Outline
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, frameWidth, 0),
                new Point3D(0, frameWidth, frameHeight),
                new Point3D(0, 0, frameHeight),
                new Point3D(0, 0, 0)
            });
            drawing.Entities.Add(rail);

            #region Frame Section and Weight Calculation
            // Frame Section for Extrusion
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D(railWidth, 0, 0),
                new Point3D(railWidth, 0, sheetThickness),
                new Point3D(sheetThickness, 0, sheetThickness),
                new Point3D(sheetThickness, 0, railHeight),
                new Point3D(0, 0, railHeight),
            });

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, sheetThickness);

            // Calculate weight for the frame rail
            double railWeight = CalculateWeight(frame, mat);
            totalWeight += railWeight; // Add rail weight to total weight

            // Add holes and calculate weight reduction for each hole
            double totalHoleWeight = 0;

            List<devregion> holes = new List<devregion>
    {
        devregion.CreateCircle(Plane.XY, new Point3D(frameWidth / 2, railHeight / 2), holeRadius),
        devregion.CreateCircle(Plane.XY, new Point3D((frameWidth / 2) - 285, railHeight / 2), holeRadius),
        devregion.CreateCircle(Plane.XY, new Point3D((frameWidth / 2) + 285, railHeight / 2), holeRadius),
        devregion.CreateCircle(Plane.XY, new Point3D(railWidth / 2, frameHeight / 3), holeRadius),
        devregion.CreateCircle(Plane.XY, new Point3D(railWidth / 2, frameHeight - frameHeight / 3), holeRadius)
    };

            foreach (var hole in holes)
            {
                Solid tempFrame = (Solid)frame.Clone();
                tempFrame.ExtrudeRemove(hole, 50, 0);
                double holeWeight = CalculateWeight(tempFrame, mat) - railWeight; // Calculate weight reduction
                totalHoleWeight += holeWeight;
            }

            totalWeight -= totalHoleWeight; // Subtract hole weights from total weight
            #endregion

            #region Save Drawing and Return
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "FilterFrame" + DateTime.Now.ToString("hh-mm") + ".dwg";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
            #endregion
        }
        //Enclosed with FAS
        public PaintBoothclass EnclosedWithFAS(PaintBoothModel model)
        {
            // Initialize drawing and dimensions
            drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;

            double frameWidth = 600;
            double frameHeight = 1000;

            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,PanelHeight),
                new Point3D(frameWidth,0,PanelHeight),
                new Point3D(frameWidth,frameHeight,PanelHeight),
                new Point3D(0,frameHeight,PanelHeight),
            });

            drawing.Entities.Add(rectangle, Color.Blue);


            #region Save Drawing and Return
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "FilterFrame" + DateTime.Now.ToString("hh-mm") + ".dwg";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
            #endregion
        }
        public void StandardFrame(int scaleFator, DesignDocument drawing)
        {

            double UntrimmedWidth = 625 * scaleFator;
            double UntrimmedHeight = 450 * scaleFator;
            double trimmedWidth = 594 * scaleFator;
            double trimmedHeight = 420 * scaleFator;

            double outerX0 = 0, outerX1 = UntrimmedWidth, outerY0 = -300, outerY1 = UntrimmedHeight - 300;

            double iX0 = (UntrimmedWidth - trimmedWidth) / 2;
            double iY0 = (UntrimmedHeight - trimmedHeight) / 2;

            double innerX0 = iX0, innerX1 = trimmedWidth + iX0, innerY0 = iY0 - 300, innerY1 = trimmedHeight + iY0 - 300;
            var outerRectangle = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(outerX0,outerY0),
                new Point2D(outerX1,outerY0),
                new Point2D(outerX1,outerY1),
                new Point2D(outerX0,outerY1),

            });
            drawing.Entities.Add(outerRectangle, Color.Yellow);

            var innerRectangle = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(innerX0,innerY0),
                new Point2D(innerX1,innerY0),
                new Point2D(innerX1,innerY1),
                new Point2D(innerX0,innerY1),

            });
            drawing.Entities.Add(innerRectangle, Color.Yellow);

            #region TitleBox
            double titleBoxWidth = 185 * scaleFator;
            double titleBoxHeight = 65 * scaleFator;
            double titleBoxX = innerX1 - titleBoxWidth, titleBoxY = innerY0 + titleBoxHeight;

            var titleBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(titleBoxX,titleBoxY),
                new Point2D(innerX1,titleBoxY),
                new Point2D(innerX1,innerY0),
                new Point2D(titleBoxX,innerY0),

            });
            drawing.Entities.Add(titleBox, Color.Yellow);
            drawing.Entities.AddRange(new Entity[]
            {
                  //left
                new Line(titleBoxX,innerY0+(10*scaleFator),titleBoxX+(25*scaleFator),innerY0+(10*scaleFator)),
                new Line(titleBoxX,innerY0+(20 * scaleFator),innerX1,innerY0+(20*scaleFator)),
                new Line(titleBoxX+(130 * scaleFator),titleBoxY,titleBoxX+(130 * scaleFator),innerY0),

                new Line(titleBoxX+(25 * scaleFator),innerY0+(20 * scaleFator),titleBoxX+(25 * scaleFator),innerY0),

                new Text(titleBoxX+((25/2)*scaleFator),innerY0+((20-5)*scaleFator),0,"SCALE",(3*scaleFator),Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+((25+(25/2))*scaleFator),innerY0+((20-5)*scaleFator),0,"TITLE",(3*scaleFator),Text.alignmentType.MiddleCenter),
                //Right
                //new Line(titleBoxX+130+10,innerY0+20,titleBoxX+130+10,titleBoxY),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7)*scaleFator,innerX1,innerY0+(20+7)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+(7/2))*scaleFator),0,"APPROVED BY",(2*scaleFator),Text.alignmentType.MiddleCenter),

                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*2)*scaleFator,innerX1,innerY0+(20+7*2)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7+(7/2))*scaleFator),0,"STANDARD",(2*scaleFator),Text.alignmentType.MiddleCenter),



                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*3)*scaleFator,innerX1,innerY0+(20+7*3)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*2+(7/2))*scaleFator),0,"CHECKED BY",(2*scaleFator),Text.alignmentType.MiddleCenter),


                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*4)*scaleFator,innerX1,innerY0+(20+7*4)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*3+(7/2))*scaleFator),0,"DRAWN",(2*scaleFator),Text.alignmentType.MiddleCenter),

                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*5)*scaleFator,innerX1,innerY0+(20+7*5)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*4+(7/2))*scaleFator),0,"DESIGNED",(2*scaleFator),Text.alignmentType.MiddleCenter),

                new Text(titleBoxX+(130+20+12.5)*scaleFator,titleBoxY-(5*scaleFator),0,"NAME",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+(130+20+25+5)*scaleFator,titleBoxY-(5*scaleFator),0,"DATE",(2*scaleFator),Text.alignmentType.MiddleCenter),

              //  new Line(titleBoxX+(130 * scaleFator),innerY0+20+7*5,innerX1,innerY0+20+7*5),
                new Line(innerX1-(10*scaleFator),innerY0+(20*scaleFator),innerX1-(10 * scaleFator),titleBoxY),
                new Line(innerX1-((10+25)*scaleFator),innerY0+(20 * scaleFator),innerX1-((10+25)*scaleFator),titleBoxY),


                new Text(titleBoxX+(130+5)*scaleFator,innerY0 + (20 - 5)*scaleFator,0,"DRAWING NO.",(2*scaleFator),Text.alignmentType.MiddleLeft),


            }, Color.Yellow);
            drawing.Entities.Add(new Text(titleBoxX + (10 * scaleFator), titleBoxY - (10 * scaleFator), "Bullows Paint Equipment Pvt.Ltd", (5 * scaleFator)), Color.Red);
            string ApprovedBy = "ABCD";
            string CreatedBy = "ABCD";
            string checkedBy = "ABCD";
            string drawnBy = "ABCD";


            drawing.Entities.AddRange(new Entity[]
            {
               new Text(titleBoxX+(130+20+12.5)*scaleFator,innerY0+((20+(7/2))*scaleFator),0,ApprovedBy,(2*scaleFator),Text.alignmentType.MiddleCenter),

                new Text(titleBoxX+(130+20+12.5)*scaleFator,innerY0+((20+7*2+(7/2))*scaleFator),0,checkedBy,(2*scaleFator),Text.alignmentType.MiddleCenter),
                  new Text(titleBoxX+(130+20+12.5)*scaleFator,innerY0+((20+7*3+(7/2))*scaleFator),0,drawnBy,(2*scaleFator),Text.alignmentType.MiddleCenter),
            }, Color.Green);

            #region BOM         
            //var BOMBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            //{
            //    new Point2D(titleBoxX,titleBoxY),
            //    new Point2D(titleBoxX,titleBoxY+15),
            //    new Point2D(innerX1,titleBoxY+15),
            //    new Point2D(innerX1,titleBoxY),

            //});
            //drawing.Entities.Add(BOMBox, Color.White);
            //List<double> XCoordinate = new List<double>()
            //{
            //    titleBoxX+20+10,
            //    titleBoxX+20+40+10,
            //    titleBoxX+20+40+80+10,
            //    titleBoxX+20+40+80+40+10,
            //    titleBoxX+20+40+80+40+60+10,
            //    titleBoxX+20+40+80+60+40+40+10,
            //    titleBoxX+20+40+80+60+40+40+40+10,
            //};

            //drawing.Entities.AddRange(new Entity[]
            //{
            //      new Text(titleBoxX+10,(titleBoxY+15-(15/2)),0,"SR.NO",3,Text.alignmentType.MiddleCenter),
            //      new Text(XCoordinate[0],(titleBoxY+15-(15/2)),0,"PART NO",3,Text.alignmentType.MiddleCenter),
            //       new Text(XCoordinate[1],(titleBoxY+15-(15/2)),0,"PART NAME",3,Text.alignmentType.MiddleLeft),
            //        new Text(XCoordinate[2],(titleBoxY+15-(15/2)),0,"MATERIAL",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[3],(titleBoxY+15-(15/2)),0,"T.SPECIFICATION",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[4], (titleBoxY+15-(15/2)),0,"QUANTITY",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[5], (titleBoxY+15-(15/2)),0,"UMO",3,Text.alignmentType.MiddleCenter),
            //      new Text(XCoordinate[6], (titleBoxY+15-(15/2)),0,"WEIGHT",3,Text.alignmentType.MiddleCenter),
            //}, Color.Pink);
            //double y = 0;
            //for (int i = 0; i < 10; i++)
            //{
            //    y = titleBoxY + (15 * (i + 2));
            //    Line l = new Line(titleBoxX, titleBoxY + (15 * (i + 2)), innerX1, titleBoxY + (15 * (i + 2)));

            //    drawing.Entities.Add(l, Color.White);

            //}
            //drawing.Entities.AddRange(new Entity[]
            //{    new Line(titleBoxX,titleBoxY,titleBoxX,y),
            //     new Line(XCoordinate[0] - 10,y,XCoordinate[0]-10,titleBoxY),  //sr.no           
            //     new Line(XCoordinate[1]-10,y,XCoordinate[1]-10,titleBoxY),//part no
            //     new Line(XCoordinate[2] - 10,y,XCoordinate[2] - 10,titleBoxY),//part name
            //     new Line(XCoordinate[3] - 10,y,XCoordinate[3] - 10,titleBoxY),//material
            //     new Line(XCoordinate[4] - 10,y,XCoordinate[4] - 10,titleBoxY),//SPECIFICATION
            //     new Line(XCoordinate[5] - 10,y,XCoordinate[5]-10,titleBoxY),//QUANTITY
            //     new Line(XCoordinate[6] - 10,y,XCoordinate[6] - 10,titleBoxY),
            //}, Color.White); 
            #endregion
            #endregion

            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/Standard Frame"))
                Directory.CreateDirectory(path + "/Standard Frame");

            string dwgFilePath = path + "/Standard Frame/" + "Frame" + DateTime.Now.ToString("hh-mm") + ".dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
        }
        public PaintBoothclass StandardFrameWithoutDrawing(int scaleFator, string enquirycode)
        {
            drawing = new();
            var panels = GetAllPanels(enquirycode);
            double UntrimmedWidth = 625 * scaleFator;
            double UntrimmedHeight = 450 * scaleFator;
            double trimmedWidth = 594 * scaleFator;
            double trimmedHeight = 420 * scaleFator;

            double outerX0 = 0, outerX1 = UntrimmedWidth, outerY0 = 0, outerY1 = UntrimmedHeight;

            double iX0 = (UntrimmedWidth - trimmedWidth) / 2;
            double iY0 = (UntrimmedHeight - trimmedHeight) / 2;

            double innerX0 = iX0, innerX1 = trimmedWidth + iX0, innerY0 = iY0, innerY1 = trimmedHeight + iY0;
            var outerRectangle = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(outerX0,outerY0),
                new Point2D(outerX1,outerY0),
                new Point2D(outerX1,outerY1),
                new Point2D(outerY0,outerY1),

            });
            drawing.Entities.Add(outerRectangle, Color.Yellow);

            var innerRectangle = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(innerX0,innerY0),
                new Point2D(innerX1,innerY0),
                new Point2D(innerX1,innerY1),
                new Point2D(innerX0,innerY1),

            });
            drawing.Entities.Add(innerRectangle, Color.Yellow);

            #region TitleBox
            double titleBoxWidth = 185 * scaleFator;
            double titleBoxHeight = 65 * scaleFator;
            double titleBoxX = innerX1 - titleBoxWidth, titleBoxY = innerY0 + titleBoxHeight;
            double nameOfFirmX = titleBoxX + (130 * scaleFator);
            double scaleX = titleBoxX + 25 * scaleFator;
            double scaleY = titleBoxY + 20 * scaleFator;
            var titleBox = devregion.CreatePolygon(Plane.XY, new Point2D[]//titleBoxX-left top corner of title box,innerX1-right bottom corner of titlebox
            {
                new Point2D(titleBoxX,titleBoxY),
                new Point2D(innerX1,titleBoxY),
                new Point2D(innerX1,innerY0),
                new Point2D(titleBoxX,innerY0),

            });

            drawing.Entities.Add(titleBox, Color.Yellow);
            drawing.Entities.AddRange(new Entity[]
            {
                  //left
                new Line(titleBoxX,innerY0+(10*scaleFator),titleBoxX+(25*scaleFator),innerY0+(10*scaleFator)),
                new Line(titleBoxX,innerY0+(20 * scaleFator),innerX1,innerY0+(20*scaleFator)),
                new Line(titleBoxX+(130 * scaleFator),titleBoxY,titleBoxX+(130 * scaleFator),innerY0),

                new Line(titleBoxX+(25 * scaleFator),innerY0+(20 * scaleFator),titleBoxX+(25 * scaleFator),innerY0),
                new Text(titleBoxX+(10*scaleFator),titleBoxY-(10*scaleFator),"NAME OF THE FIRM",4),
                new Text(titleBoxX+((25/2)*scaleFator),innerY0+((20-5)*scaleFator),0,"SCALE",4,Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+((25+(25/2))*scaleFator),innerY0+((20-5)*scaleFator),0,"TITLE",4,Text.alignmentType.MiddleCenter),
                //Right
                //new Line(titleBoxX+130+10,innerY0+20,titleBoxX+130+10,titleBoxY),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7)*scaleFator,innerX1,innerY0+(20+7)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+(7/2))*scaleFator),0,"APPROVED",2,Text.alignmentType.MiddleCenter),

                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*2)*scaleFator,innerX1,innerY0+(20+7*2)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7+(7/2))*scaleFator),0,"STANDARD",2,Text.alignmentType.MiddleCenter),



                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*3)*scaleFator,innerX1,innerY0+(20+7*3)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*2+(7/2))*scaleFator),0,"CHECKED",2,Text.alignmentType.MiddleCenter),


                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*4)*scaleFator,innerX1,innerY0+(20+7*4)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*3+(7/2))*scaleFator),0,"DRAWN",2,Text.alignmentType.MiddleCenter),

                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*5)*scaleFator,innerX1,innerY0+(20+7*5)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*4+(7/2))*scaleFator),0,"DESIGNED",2,Text.alignmentType.MiddleCenter),

                new Text(titleBoxX+(130+20+12.5)*scaleFator,titleBoxY-(5*scaleFator),0,"NAME",2,Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+(130+20+25+5)*scaleFator,titleBoxY-(5*scaleFator),0,"DATE",2,Text.alignmentType.MiddleCenter),

              //  new Line(titleBoxX+(130 * scaleFator),innerY0+20+7*5,innerX1,innerY0+20+7*5),
                new Line(innerX1-(10*scaleFator),innerY0+(20*scaleFator),innerX1-(10 * scaleFator),titleBoxY),
                new Line(innerX1-((10+25)*scaleFator),innerY0+(20 * scaleFator),innerX1-((10+25)*scaleFator),titleBoxY),


                new Text(titleBoxX+(130+5)*scaleFator,innerY0 + (20 - 5)*scaleFator,0,"DRAWING NO.",4,Text.alignmentType.MiddleLeft),


            }, Color.Yellow);

            #region BOM old        
            var BOMBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(titleBoxX,titleBoxY),
                new Point2D(titleBoxX,titleBoxY+15),
                new Point2D(innerX1,titleBoxY+15),
                new Point2D(innerX1,titleBoxY),

            });
            drawing.Entities.Add(BOMBox);
            List<double> XCoordinate = new List<double>()
            {
                (titleBoxX+20+10),
                (titleBoxX+20+40+10),
                (titleBoxX+20+40+80+10),
                (titleBoxX+20+40+80+40+10),
                (titleBoxX+20+40+80+40+60+10),
                (titleBoxX+20+40+80+60+40+40+10),
                (titleBoxX+20+40+80+60+40+40+40+10),
            };

            drawing.Entities.AddRange(new Entity[]
            {
                  new Text(titleBoxX+10,(titleBoxY+15-(15/2)),0,"SR.NO",3,Text.alignmentType.MiddleCenter),
                  new Text(XCoordinate[0],(titleBoxY+15-(15/2)),0,"PART NO",3,Text.alignmentType.MiddleCenter),
                   new Text(XCoordinate[1],(titleBoxY+15-(15/2)),0,"PART NAME",3,Text.alignmentType.MiddleLeft),
                    new Text(XCoordinate[2],(titleBoxY+15-(15/2)),0,"MATERIAL",3,Text.alignmentType.MiddleLeft),
                   new Text(XCoordinate[3],(titleBoxY+15-(15/2)),0,"T.SPECIFICATION",3,Text.alignmentType.MiddleLeft),
                   new Text(XCoordinate[4], (titleBoxY+15-(15/2)),0,"QUANTITY",3,Text.alignmentType.MiddleLeft),
                   new Text(XCoordinate[5], (titleBoxY+15-(15/2)),0,"UMO",3,Text.alignmentType.MiddleCenter),
                  new Text(XCoordinate[6], (titleBoxY+15-(15/2)),0,"WEIGHT",3,Text.alignmentType.MiddleCenter),
            });
            double y = 0;
            for (int i = 0; i < panels.Count; i++)
            {
                y = titleBoxY + (15 * (i + 2));
                Line l = new Line(titleBoxX, titleBoxY + (15 * (i + 2)), innerX1, titleBoxY + (15 * (i + 2)));
                drawing.Entities.Add(l);
                //Passed value to BOM
                drawing.Entities.AddRange(new Entity[]
                {
                new Text(titleBoxX+10,(y-(15/2)),0,(i+1).ToString(),3,Text.alignmentType.MiddleCenter),
                new Text(XCoordinate[0],(y-(15/2)),0,"PART NO",3,Text.alignmentType.MiddleCenter),
                new Text(XCoordinate[1],(y-(15/2)),0,panels[i].PanelPosition,3,Text.alignmentType.MiddleLeft),
                new Text(XCoordinate[2],(y-(15/2)),0,"MATERIAL",3,Text.alignmentType.MiddleLeft),
                new Text(XCoordinate[3],(y-(15/2)),0,"T.SPECIFICATION",3,Text.alignmentType.MiddleLeft),
                new Text(XCoordinate[4], (y-(15/2)),0,"QUANTITY",3,Text.alignmentType.MiddleLeft),
                new Text(XCoordinate[5], (y-(15/2)),0,"UMO",3,Text.alignmentType.MiddleCenter),
                new Text(XCoordinate[6], (y-(15/2)),0,panels[i].PanelWeight.ToString(),3,Text.alignmentType.MiddleCenter),
                });

            }
            drawing.Entities.AddRange(new Entity[]
            {    new Line(titleBoxX,titleBoxY,titleBoxX,y),
                 new Line(XCoordinate[0] - 10,y,XCoordinate[0]-10,titleBoxY),  //sr.no           
                 new Line(XCoordinate[1]-10,y,XCoordinate[1]-10,titleBoxY),//part no
                 new Line(XCoordinate[2] - 10,y,XCoordinate[2] - 10,titleBoxY),//part name
                 new Line(XCoordinate[3] - 10,y,XCoordinate[3] - 10,titleBoxY),//material
                 new Line(XCoordinate[4] - 10,y,XCoordinate[4] - 10,titleBoxY),//SPECIFICATION
                 new Line(XCoordinate[5] - 10,y,XCoordinate[5]-10,titleBoxY),//QUANTITY
                 new Line(XCoordinate[6] - 10,y,XCoordinate[6] - 10,titleBoxY),
            });


            #endregion

            #endregion

            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/Standard Frame"))
                Directory.CreateDirectory(path + "/Standard Frame");

            string dwgFilePath = path + "/Standard Frame/" + "Frame" + DateTime.Now.ToString("hh-mm") + ".dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath

            };
        }
        public PaintBoothclass ComponantryEntrySideDoorold(DoorDimensionsModel doorModel)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
           
            double T = 3;
            double sectionWidth = 25;
            double sectionHeight = 25;

            #region Door Outer Frame

            LinearPath doorPath = new LinearPath(new Point3D[]
            {
                new Point3D(doorModel.xOffeset, doorModel.yOffeset, 0),
                new Point3D(doorModel.xOffeset, doorModel.yOffeset, doorModel.doorHeight),
                new Point3D(doorModel.doorWidth + doorModel.xOffeset, doorModel.yOffeset, doorModel.doorHeight),
                new Point3D(doorModel.doorWidth + doorModel.xOffeset, doorModel.yOffeset, 0),
                new Point3D(doorModel.xOffeset, doorModel.yOffeset, 0)
            });

            drawing.Entities.Add(doorPath, Color.Aquamarine);

            var outerSection = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(doorModel.xOffeset,                0, 0),
                new Point3D(doorModel.xOffeset + sectionWidth, 0, 0),
                new Point3D(doorModel.xOffeset + sectionWidth, -T, 0),
                new Point3D(doorModel.xOffeset + T,            -T, 0),
                new Point3D(doorModel.xOffeset + T,            -sectionHeight, 0),
                new Point3D(doorModel.xOffeset,               -sectionHeight, 0)
            });

            Solid outerFrame = outerSection.SweepAsSolid(doorPath, 0);
            drawing.Entities.Add(outerFrame, Color.RosyBrown);

            #endregion

            #region Door Leaf Calculation

            int doorLeafCount;
            double innerDoorWidth;

            if (doorModel.doorSubType == "Split Doors")
            {
                doorLeafCount = 2;
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 2;
            }
            else
            {
                doorLeafCount = 4;
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 4;
            }

            double innerDoorHeight = doorModel.doorHeight - (2 * T);
            double innerSectionSize = 40;

            #endregion

            #region Inner Door Frames & Sheets

            LinearPath innerDoorPath = new LinearPath(new Point3D[]
            {
        new Point3D(doorModel.xOffeset + T, doorModel.yOffeset, 0),
        new Point3D(doorModel.xOffeset + T, doorModel.yOffeset, innerDoorHeight),
        new Point3D(doorModel.xOffeset + T + innerDoorWidth, doorModel.yOffeset, innerDoorHeight),
        new Point3D(doorModel.xOffeset + T + innerDoorWidth, doorModel.yOffeset, 0),
        new Point3D(doorModel.xOffeset + T, doorModel.yOffeset, 0)
            });

            var innerSection = devregion.CreatePolygon(new Point3D[]
            {
        new Point3D(doorModel.xOffeset + T,                   0, 0),
        new Point3D(doorModel.xOffeset + T + innerSectionSize,0, 0),
        new Point3D(doorModel.xOffeset + T + innerSectionSize,-innerSectionSize,0),
        new Point3D(doorModel.xOffeset + T,                  -innerSectionSize,0)
            });

            Solid baseInnerFrame = innerSection.SweepAsSolid(innerDoorPath, 0);
            drawing.Entities.Add(baseInnerFrame, Color.AliceBlue);

            var metalSheetSection = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(doorModel.xOffeset + T,                   doorModel.yOffeset, 0),
                new Point3D(doorModel.xOffeset + T,                   doorModel.yOffeset, innerDoorHeight),
                new Point3D(doorModel.xOffeset + T + innerDoorWidth,  doorModel.yOffeset, innerDoorHeight),
                new Point3D(doorModel.xOffeset + T + innerDoorWidth,  doorModel.yOffeset, 0)
            });

            Brep baseSheet = metalSheetSection.ExtrudeAsBrep(-SheetThickness);

            Color[] doorColors =
            {
        Color.CadetBlue,
        Color.Brown,
        Color.DarkOliveGreen,
        Color.OrangeRed
    };

            // First door leaf
            //drawing.Entities.Add(baseInnerFrame, doorColors[0]);
            //drawing.Entities.Add(baseSheet, doorColors[0]);

            // Remaining door leaves
            for (int i = 1; i < doorLeafCount; i++)
            {
                Solid frameClone = (Solid)baseInnerFrame.Clone();
                frameClone.Translate(innerDoorWidth * i, 0, 0);

                Brep sheetClone = (Brep)baseSheet.Clone();
                sheetClone.Translate(innerDoorWidth * i, 0, 0);

                //drawing.Entities.Add(frameClone, doorColors[i % doorColors.Length]);
               // drawing.Entities.Add(sheetClone, doorColors[i % doorColors.Length]);
            }
            #region Fix orientation for Left / Right side

            //if (doorModel.Side == DoorSide.Right)
            //{
            //    drawing.Entities.Rotate(
            //        Math.PI,                 // 180 degrees
            //        Vector3D.AxisZ,
            //        new Point3D(
            //            doorModel.xOffeset + doorModel.doorWidth / 2,
            //            doorModel.yOffeset,
            //            0
            //        )
            //    );
            //}

            #endregion

            #endregion

            #region Save DWG

            var path = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("FolderPathConfig")["AbsolutePath"];

            string folder = Path.Combine(path, "Bullows Panel Drawing");
            Directory.CreateDirectory(folder);

            string dwgFilePath = Path.Combine(
                folder,
                $"ComponantryEntrySideDoor_{DateTime.Now:HH-mm}.dwg"
            );

            WriteAutodeskParams writeParams = new(drawing);
            WriteAutodesk dwg = new(writeParams, dwgFilePath);
            dwg.DoWork();

            #endregion

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };
        }

        public PaintBoothclass ComponantryEntrySideDoor(DoorDimensionsModel doorModel)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;

            #region split door
            #region Door Outer frame
            LinearPath door = new LinearPath(new Point3D[]
                {
                    new Point3D(doorModel.xOffeset, doorModel.yOffeset, 0),
                    new Point3D(doorModel.xOffeset, doorModel.yOffeset, doorModel.doorHeight),
                    new Point3D(doorModel.doorWidth+doorModel.xOffeset, doorModel.yOffeset,doorModel.doorHeight),
                    new Point3D(doorModel.doorWidth+doorModel.xOffeset, doorModel.yOffeset, 0),
                    new Point3D(doorModel.xOffeset, doorModel.yOffeset, 0)
                });
            //drawing.Entities.Add(door, Color.Aquamarine);
            double T = 3;
            double sectionWidth = 25;
            double sectionHeight = 25;
            double x0 = doorModel.xOffeset;
            double L = sectionWidth - (2 * T);

            //left L Section
            var section = devregion.CreatePolygon(new Point3D[]
            {

                         new Point3D(x0+0 ,               0,      0),
                         new Point3D(x0+sectionWidth ,    0,      0),
                         new Point3D(x0+sectionWidth ,    -T,      0),
                         new Point3D(x0+T,                -T,      0),
                         new Point3D(x0+T,                -sectionHeight,      0),
                         new Point3D(x0+0,               - sectionHeight,      0),
            });

            Solid frame = section.SweepAsSolid(door, 0);
            frame.Translate(0, doorModel.yOffeset, 0);

            drawing.Entities.Add(frame, Color.RosyBrown);
            #endregion

            double innerDoorWidth = 0;
            int doorLeafCount = 0;
            if (doorModel.doorSubType== "Split Doors")
            {
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 2;
                doorLeafCount = 2; // default Split Doors
            }
            else
            {
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 4;
                doorLeafCount = 4; // Quad Doors
            }
            double innerDoorHeight = doorModel.doorHeight - (2 * T);
            double innerSectionWidth = 40;
            double innerSectionHeight = 40;

            #region Inner Door left
            LinearPath Innerdoorleft = new LinearPath(new Point3D[]
            {
                    new Point3D(doorModel.xOffeset+T,                   doorModel.yOffeset, 0),
                    new Point3D(doorModel.xOffeset+T,                   doorModel.yOffeset, innerDoorHeight),
                    new Point3D(innerDoorWidth+doorModel.xOffeset+T,    doorModel.yOffeset,innerDoorHeight),
                    new Point3D(innerDoorWidth+doorModel.xOffeset+T,    doorModel.yOffeset, 0),
                    new Point3D(doorModel.xOffeset+T,                   doorModel.yOffeset, 0)
            });
            double x1 = x0 + T;
            double innerSectionWidthWithMinusT = x1 + innerSectionWidth - (2 * T);
            var squareSection = devregion.CreatePolygon(new Point3D[]
            {
                 new Point3D(x1 ,                    0,                         0),
                 new Point3D(x1+innerSectionWidth ,  0,                         0),
                 new Point3D(x1+innerSectionWidth ,  -innerSectionHeight,      0),
                 new Point3D(x1+0 ,                  -innerSectionHeight,      0),
            });
            Solid innerframe = squareSection.SweepAsSolid(Innerdoorleft, 0);
            for (int i = 1; i < doorLeafCount; i++)
            {
                Solid cloneInnerFrame = (Solid)innerframe.Clone();
                cloneInnerFrame.Translate(innerDoorWidth * i, doorModel.yOffeset, 0);
                drawing.Entities.Add(cloneInnerFrame, Color.Brown);
            }
            var metalsheet = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(doorModel.xOffeset+T, -innerSectionWidth, 0),
                    new Point3D(doorModel.xOffeset+T,-innerSectionWidth, innerDoorHeight),
                    new Point3D(innerDoorWidth+doorModel.xOffeset+T,-innerSectionWidth,innerDoorHeight),
                    new Point3D(innerDoorWidth+doorModel.xOffeset+T, -innerSectionWidth, 0),

            });
            Color[] doorColors =
            {
                Color.CadetBlue,   // Door 1
                Color.Brown,       // Door 2
                Color.DarkOliveGreen, // Door 3
                Color.OrangeRed    // Door 4
            };
            Brep baseSheet = metalsheet.ExtrudeAsBrep(-SheetThickness);
            for (int i = 0; i < doorLeafCount; i++)
            {
                Solid doorFrame; Brep doorSheet;
                doorFrame = (Solid)innerframe.Clone();
                doorFrame.Translate(innerDoorWidth * i, doorModel.yOffeset, 0);
                doorSheet = (Brep)baseSheet.Clone();
                doorSheet.Translate(innerDoorWidth * i, doorModel.yOffeset, 0);

                drawing.Entities.Add(
                    doorFrame,
                    doorColors[i % doorColors.Length] 
                );
                drawing.Entities.Add(
                    doorSheet,
                 doorColors[i % doorColors.Length]
                );
            }
                        


            #endregion
            #endregion

            #region Writing DWG
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "ComponantryEntrySideDoor"+doorModel.Side + DateTime.Now.ToString("hh-mm") + ".dwg";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath

            };
        }
        public PaintBoothclass ComponantryEntryFrontDoor(DoorDimensionsModel doorModel)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double yOffeset = doorModel.xOffesetForFrontDoor;
        
            #region front Door Outer frame
            LinearPath door = new LinearPath(new Point3D[]
            {
                    new Point3D(0,  yOffeset,                         0),
                    new Point3D(0,  yOffeset,                         doorModel.doorHeight),
                    new Point3D(0,  doorModel.doorWidth+ yOffeset,     doorModel.doorHeight),
                    new Point3D(0,  doorModel.doorWidth+ yOffeset,     0),
                    new Point3D(0,  yOffeset,                         0)
            });
            //drawing.Entities.Add(door, Color.LightBlue);
            double T = 3;
            double sectionWidth = 25;
            double sectionHeight = 25;
            double L = sectionWidth - (2 * T);
            //L section
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,             yOffeset,   0),
                new Point3D(sectionWidth,  yOffeset,   0),
                new Point3D(sectionWidth,  yOffeset+T,  0),
                new Point3D(T,             yOffeset+ T,  0),
                new Point3D(T,             yOffeset+sectionHeight, 0 ),
                new Point3D(0,             yOffeset+ sectionHeight,  0),
            });
            Solid frame = section.SweepAsSolid(door, 0);
            drawing.Entities.Add(frame, Color.RosyBrown);
            #endregion

            #region Door condition values
            double innerDoorWidth = 0;
            int doorLeafCount = 0;
            if (doorModel.doorSubType == "Split Doors")
            {
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 2;
                doorLeafCount = 2; // default Split Doors
            }
            else
            {
                innerDoorWidth = (doorModel.doorWidth - (2 * T)) / 4;
                doorLeafCount = 4; // Quad Doors
            }
            double innerDoorHeight = doorModel.doorHeight - (2 * T);
            double innerSectionWidth = 40;
            double innerSectionHeight = 40; 
            #endregion

            LinearPath Innerdoor = new LinearPath(new Point3D[]
            {
                    new Point3D(0,   yOffeset+T,                   0),
                    new Point3D(0,   yOffeset+T,                   innerDoorHeight),
                    new Point3D(0,   innerDoorWidth+yOffeset+T,    innerDoorHeight),
                    new Point3D(0,   innerDoorWidth+yOffeset+T,    0),
                    new Point3D(0,   yOffeset+T,                   0)
            });
            drawing.Entities.Add(Innerdoor, Color.AliceBlue);

            double y1 = yOffeset + T;
           
            var squareSection = devregion.CreatePolygon(new Point3D[]
            {
                 new Point3D(sectionWidth,                      y1 ,                        0),
                 new Point3D(sectionWidth,                      y1+innerSectionWidth ,      0),
                 new Point3D(sectionWidth-innerSectionHeight,    y1+innerSectionWidth ,      0),
                 new Point3D(sectionWidth-innerSectionHeight,    y1 ,                        0),
            });
            Solid innerframe = squareSection.SweepAsSolid(Innerdoor, 0);
            for (int i = 1; i < doorLeafCount; i++)
            {
                Solid cloneInnerFrame = (Solid)innerframe.Clone();
                cloneInnerFrame.Translate(0, innerDoorWidth * i, 0);
                drawing.Entities.Add(cloneInnerFrame, Color.Brown);
            }

            var metalsheet = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(sectionWidth-innerSectionHeight,      yOffeset+T,                      0),
                    new Point3D(sectionWidth-innerSectionHeight,      yOffeset+T,                    innerDoorHeight),
                    new Point3D(sectionWidth-innerSectionHeight,     innerDoorWidth + yOffeset+T,     innerDoorHeight),
                    new Point3D(sectionWidth-innerSectionHeight,     innerDoorWidth + yOffeset+T,     0),

            });
            Color[] doorColors =
            {
                Color.CadetBlue,   // Door 1
                Color.Brown,       // Door 2
                Color.DarkOliveGreen, // Door 3
                Color.OrangeRed    // Door 4
            };
            Brep baseSheet = metalsheet.ExtrudeAsBrep(SheetThickness);
            for (int i = 0; i < doorLeafCount; i++)
            {
                Solid doorFrame; Brep doorSheet;
                doorFrame = (Solid)innerframe.Clone();
                doorFrame.Translate(0,innerDoorWidth * i, 0);
                doorSheet = (Brep)baseSheet.Clone();
                doorSheet.Translate(0,innerDoorWidth * i, 0);

                drawing.Entities.Add(
                    doorFrame,
                    doorColors[i % doorColors.Length]
                );
                drawing.Entities.Add(
                    doorSheet,Color.HotPink
                 //doorColors[i % doorColors.Length]
                );
            }







            #region Writing DWG
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "ComponantryEntryFrontDoor" + DateTime.Now.ToString("hh-mm") + ".dwg";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath

            };
        }
        public PaintBoothclass CreateLoftOld(PaintBoothModel model, double extractionChemberHeight)
        {
            var path = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            DesignDocument drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;

            #region loft
            Plane p1 = Plane.XY;
            Plane p2 = p1.Offset(extractionChemberHeight);

            W = model.W;
            double x = model.D;
            double z1 = extractionChemberHeight;
            double z2 = extractionChemberHeight + 300;


            LinearPath rectangle = new LinearPath(p2, new Point3D[]
            {
                new Point3D(x, 0, extractionChemberHeight),
                new Point3D(x + model.D3, 0, extractionChemberHeight),
                new Point3D(x + model.D3, W, extractionChemberHeight),
                new Point3D(x, W, extractionChemberHeight),
                new Point3D(x, 0, extractionChemberHeight)
            });
            drawing.Entities.Add(rectangle, Color.Pink);

            LinearPath UpperRectangle = new LinearPath(new Point3D[]
            {
                new Point3D(x, (W - model.D3) / 2, extractionChemberHeight + 300),
                new Point3D(x + model.D3, (W - model.D3) / 2, extractionChemberHeight + 300),
                new Point3D(x + model.D3, ((W - model.D3) / 2) + model.D3, extractionChemberHeight + 300),
                new Point3D(x, ((W - model.D3) / 2) + model.D3, extractionChemberHeight + 300),
                new Point3D(x, (W - model.D3) / 2, extractionChemberHeight + 300)
            });
            drawing.Entities.Add(UpperRectangle, Color.Pink);

            Brep loft1 = Brep.Loft(new ICurve[] { rectangle, UpperRectangle }, 2);
            drawing.Entities.Add(loft1, Color.Yellow);
            #endregion

            //// --- STEP import (COMMENTED OUT) ---
            //var stepFilePath = Path.Combine(path, "CasingModel.step");
            //if (!File.Exists(stepFilePath))
            //    throw new FileNotFoundException($"STEP file not found: {stepFilePath}");
            //
            //ReadSTEP stepReader = new ReadSTEP(stepFilePath);
            //stepReader.DoWork();
            //stepReader.AddTo(drawing);
            //
            //double stepPositionX = x + (model.D3 / 2);
            //double stepPositionY = W / 2;
            //double stepPositionZ = extractionChemberHeight + 600; // centrifugal fan height
            //
            //double angle = 0;
            //if (model.BlowerOrientation == "90") angle = Math.PI / 2;
            //else if (model.BlowerOrientation == "180") angle = Math.PI;
            //else if (model.BlowerOrientation == "270") angle = (Math.PI * 3) / 2;
            //
            //foreach (Entity ent in drawing.Entities.Skip(origEntitiesCount).ToArray())
            //{
            //    ent.Translate(stepPositionX, stepPositionY, stepPositionZ);
            //    Point3D centerPoint = new Point3D(stepPositionX, stepPositionY, stepPositionZ);
            //    ent.Rotate(Math.PI / 2, Vector3D.AxisX, centerPoint);
            //    ent.Rotate(angle, Vector3D.AxisZ, centerPoint);
            //}

            //// --- BendRevolve + Exhaust duct (COMMENTED OUT) ---
            //#region BendRevolve
            //LinearPath l1 = new LinearPath(Plane.XY, new Point3D[]
            //{
            //    new Point3D(400, 0, extractionChemberHeight + 1000),
            //    new Point3D(400 + model.ExhaustWidth, 0, extractionChemberHeight + 1000),
            //    new Point3D(400 + model.ExhaustWidth, model.ExhaustDuctHeight, extractionChemberHeight + 1000),
            //    new Point3D(400, model.ExhaustDuctHeight, extractionChemberHeight + 1000),
            //    new Point3D(400, 0, extractionChemberHeight + 1000)
            //});
            //// ... rest of BendRevolve & Exhaust duct code remains the same
            //#endregion

            if (!Directory.Exists(Path.Combine(path, "Bullows Panel Drawing")))
                Directory.CreateDirectory(Path.Combine(path, "Bullows Panel Drawing"));

            string dwgFilePathforLoft = Path.Combine(path, "Bullows Panel Drawing",
                "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".dwg");
            string dwgFilePathforLoftpdf = Path.Combine(path, "Bullows Panel Drawing",
                "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".pdf");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathforLoft);
            dwgg1.DoWork();

            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, dwgFilePathforLoftpdf);
            pdf1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePathforLoft
            };
        }

        public PaintBoothclass CreateLoft(PaintBoothModel model, double extractionChemberHeight)
        {
            var path = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            DesignDocument drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;

            #region loft
            Plane p1 = Plane.XY;
            Plane p2 = p1.Offset(extractionChemberHeight);

            W = model.W;
            double x = model.D;
            double z1 = extractionChemberHeight;
            double z2 = extractionChemberHeight + 300;


            Line l1 = new Line(new Point3D(x, 0, z1), new Point3D(x + model.D3, 0, z1));
            Line l2 = new Line(new Point3D(x + model.D3, 0, z1), new Point3D(x + model.D3, W, z1));
            Line l3 = new Line(new Point3D(x + model.D3, W, z1), new Point3D(x, W, z1));
            Line l4 = new Line(new Point3D(x, W, z1), new Point3D(x, 0, z1));


            CompositeCurve rect = new CompositeCurve(new ICurve[] { l1, l2, l3, l4 });
            drawing.Entities.Add(rect, Color.AliceBlue);
            //circle for upper side
            double centerX = x + model.D3 / 2;
            double centerY = (W - model.D3) / 2 + model.D3 / 2;
            double radius = model.D3 / 2;



            Point3D center = new Point3D(centerX, centerY, z2);

            // Circle using 4 arcs rotated by 45°
            Arc arc1 = new Arc(Plane.XY, center, radius, Utility.DegToRad(45), Utility.DegToRad(135));
            Arc arc2 = new Arc(Plane.XY, center, radius, Utility.DegToRad(135), Utility.DegToRad(225));
            Arc arc3 = new Arc(Plane.XY, center, radius, Utility.DegToRad(225), Utility.DegToRad(315));
            Arc arc4 = new Arc(Plane.XY, center, radius, Utility.DegToRad(315), Utility.DegToRad(405));

            // Join arcs into a single ICurve (CompositeCurve)
            CompositeCurve circleByArcs = new CompositeCurve(new ICurve[] { arc1, arc2, arc3, arc4 });

            Surface[] loftA = Surface.Loft(new ICurve[] { arc1, l3 });
            Surface[] loftB = Surface.Loft(new ICurve[] { arc2, l4 });
            Surface[] loftC = Surface.Loft(new ICurve[] { arc3, l1 });
            Surface[] loftD = Surface.Loft(new ICurve[] { arc4, l2 });

            devregion circle = new devregion(circleByArcs);
            PlanarSurface circleSurf = circle.ConvertToSurface();
            //drawing.Entities.Add(circleSurf);


            devregion rectangle = new devregion(rect);
            PlanarSurface rectSurf = rectangle.ConvertToSurface();
            rectSurf.FlipNormal();

            Solidifier solid = new Solidifier(new Surface[] { loftA[0], loftB[0], loftC[0], loftD[0], circleSurf, rectSurf });
            solid.DoWork();

            drawing.Entities.Add(solid.Result, Color.Yellow);
            #endregion

            if (!Directory.Exists(Path.Combine(path, "Bullows Panel Drawing")))
                Directory.CreateDirectory(Path.Combine(path, "Bullows Panel Drawing"));

            string dwgFilePathforLoft = Path.Combine(path, "Bullows Panel Drawing",
                "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".dwg");
            string dwgFilePathforLoftpdf = Path.Combine(path, "Bullows Panel Drawing",
                "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".pdf");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathforLoft);
            dwgg1.DoWork();

            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, dwgFilePathforLoftpdf);
            pdf1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePathforLoft
            };
        }

        public PaintBoothclass CreateLoftForPaintboothType5(PaintBoothModel model, string DraftSubType)
        {
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            DesignDocument drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;
            #region loft
            // Define base planes for the shapes
            Plane p1 = Plane.XY;
            Plane p2 = p1.Offset(model.H);


            if (model.PanelTypes == "2" || model.PanelTypesforW == "2" || model.PanelTypesforH == "2")
            {
                model.D = model.D;
                W = model.W;
            }
            else
            {
                model.D = model.StandardPanelWidthForD;
                if (model.StandardPanelsByW == 0)
                    W = model.RemainingPanelsByW;
            }
            double x = 0;
            if (DraftSubType == "7" || DraftSubType == "5" || DraftSubType == "4" || DraftSubType == "6")
                x = -D3 - (SettingStandardBend2 + SettingStandardBend1);
            else
                x = model.D;
            // Create the loft base 
            LinearPath rectangle = new LinearPath(p2, new Point3D[]
                {
                new Point3D(x, 0, model.H),
                new Point3D(x + model.D3, 0, model.H),
                new Point3D(x + model.D3, W, model.H),
                new Point3D(x, W, model.H),
                new Point3D(x, 0, model.H)
                });

            drawing.Entities.Add(rectangle, Color.Pink);

            LinearPath rectangle1 = new LinearPath(new Point3D[]// top rectangles
            {
                new Point3D(x, (W - D3) / 2, model.H + 300),
                new Point3D((x + model.D3), ((W - D3) / 2), model.H + 300),
                new Point3D((x + model.D3), ((W - D3) / 2) + D3, model.H + 300),
                new Point3D(x, ((W - D3) / 2) + D3, model.H + 300),
                new Point3D(x, (W - D3) / 2, model.H + 300)
            });
            drawing.Entities.Add(rectangle1, Color.Pink);

            // Create the loft between two rectangles
            Brep loft1 = Brep.Loft(new ICurve[] { rectangle, rectangle1 }, 2);
            drawing.Entities.Add(loft1, Color.Yellow);
            #endregion
            //code for reading step file 
            var stepFilePath = path + "/CasingModel.step";

            ReadSTEP stepReader = new ReadSTEP(stepFilePath);
            stepReader.DoWork();

            // Position of the imported STEP model on the loft
            double stepPositionX = x + (model.D3 / 2);
            double stepPositionY = W / 2;
            double stepPositionZ = model.H + 300 + 300;//centrifugal fan height
            double angle = 0;
            int i = 1;
            foreach (Entity stepEntity in stepReader.Entities)
            {
                // Check if the entity is a BlockReference and if its name is in the list of names to skip
                if (stepEntity is BlockReference blockRef)
                {
                    string blockName = blockRef.BlockName;
                    if (blockName == "Block_0" || blockName == "Block_1" || blockName == "Block_2" || blockName == "Block_3" || blockName == "Block_4")
                    {
                        continue;
                    }
                }

                // Translate the entity before adding it to the drawing
                stepEntity.Translate(stepPositionX, stepPositionY, stepPositionZ);
                // Rotate the entity 90 degrees (π/2 radians) around the X-axis at the same position
                Point3D centerPoint = new Point3D(stepPositionX, stepPositionY, stepPositionZ);
                // stepEntity.Rotate(Math.PI / 2, Vector3D.AxisX, centerPoint);

                // Rotate another 90 degrees around the Y-axis if needed
                if (model.BlowerOrientation == "0")
                    angle = 0;
                else if (model.BlowerOrientation == "90")
                    angle = Math.PI / 2;
                else if (model.BlowerOrientation == "180")
                    angle = Math.PI;
                else if (model.BlowerOrientation == "270")
                    angle = (Math.PI * 3) / 2;

                stepEntity.Rotate(Math.PI / 2, Vector3D.AxisX, centerPoint);
                stepEntity.Rotate(angle, Vector3D.AxisZ, centerPoint);
                drawing.Entities.Add(stepEntity);
                i++;
            }

            #region BendRevolve
            LinearPath l1 = new LinearPath(Plane.XY, new Point3D[]
               {
                    new Point3D(400, 0, stepPositionZ+400),
                    new Point3D(400+model.ExhaustWidth, 0, stepPositionZ+400),
                    new Point3D(400+model.ExhaustWidth, model.ExhaustDuctHeight, stepPositionZ+400),
                    new Point3D(400,model.ExhaustDuctHeight, stepPositionZ+400),
                    new Point3D(400,0, stepPositionZ+400)
               });
            LinearPath l2 = new LinearPath(Plane.XY, new Point3D[]
            {
                    new Point3D(400+model.ExhaustThickness, model.ExhaustThickness, stepPositionZ+400),
                    new Point3D(400+(model.ExhaustWidth-model.ExhaustThickness),model.ExhaustThickness, stepPositionZ+400),
                    new Point3D(400+(model.ExhaustWidth-model.ExhaustThickness), (model.ExhaustDuctHeight-model.ExhaustThickness), stepPositionZ+400),
                    new Point3D(400+model.ExhaustThickness,(model.ExhaustDuctHeight-model.ExhaustThickness), stepPositionZ+400),
                    new Point3D(400+model.ExhaustThickness, model.ExhaustThickness, stepPositionZ+400)
            });


            devregion region2 = new devregion(l1);
            devregion region3 = new devregion(l2);
            devregion region = devregion.Difference(region2, region3)[0];
            Brep brep1 = region.RevolveAsBrep(Utility.DegToRad(90), Vector3D.AxisY, new Point3D(0, 200, 0));
            brep1.Translate(stepPositionX + 300, stepPositionY - 365, stepPositionZ + 400);
            //drawing.Entities.Add(brep1, Color.LightGray);

            Material matBend = Material.StructuralSteel;
            matBend = new Material(Materials);
            brep1.Regen(0.1);
            double massofBend = brep1.GetMass(matBend, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Bend_Weight = Math.Round(massofBend, 3);
            model.BendWeight = Bend_Weight;



            #region  Exhaust Duct
            LinearPath d1 = new LinearPath(Plane.XY, new Point3D[]
                {
                    new Point3D(400, 0, stepPositionZ+400),
                    new Point3D(400+model.ExhaustWidth, 0, stepPositionZ+400),
                    new Point3D(400+model.ExhaustWidth, model.ExhaustDuctHeight, stepPositionZ+400),
                    new Point3D(400,model.ExhaustDuctHeight, stepPositionZ+400),
                    new Point3D(400,0, stepPositionZ+400)
                });

            LinearPath d2 = new LinearPath(Plane.XY, new Point3D[]
            {
                    new Point3D(400+model.ExhaustThickness, model.ExhaustThickness, stepPositionZ+400),
                    new Point3D(400+(model.ExhaustWidth-model.ExhaustThickness),model.ExhaustThickness, stepPositionZ+400),
                    new Point3D(400+(model.ExhaustWidth-model.ExhaustThickness), (model.ExhaustDuctHeight-model.ExhaustThickness), stepPositionZ+400),
                    new Point3D(400+model.ExhaustThickness,(model.ExhaustDuctHeight-model.ExhaustThickness), stepPositionZ+400),
                    new Point3D(400+model.ExhaustThickness, model.ExhaustThickness, stepPositionZ+400)
            });

            devregion regionduct1 = new devregion(d1);
            devregion regionduct2 = new devregion(d2);
            devregion regionduct = devregion.Difference(regionduct1, regionduct2)[0];
            Brep ductbrep = regionduct.ExtrudeAsBrep(300);
            ductbrep.Translate(stepPositionX + 300, stepPositionY - 365, stepPositionZ + 400);
            //drawing.Entities.Add(ductbrep, Color.Green);

            Material matduct = Material.StructuralSteel;
            matduct = new Material(Materials);
            ductbrep.Regen(0.1);
            double massofFrame = ductbrep.GetMass(matduct, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity12);
            double Frame_Weight = Math.Round(massofFrame, 3);
            model.DuctWeight = Frame_Weight;


            #endregion


            if (model.BlowerOrientation != "90")
            {
                Brep clonebrep1 = (Brep)brep1.Clone();
                clonebrep1.Rotate(-Math.PI / 2, Vector3D.AxisZ);
                clonebrep1.Rotate(Math.PI / 2, Vector3D.AxisY);
                clonebrep1.Translate(-300, 3000, stepPositionZ + 400);
                drawing.Entities.Add(clonebrep1, Color.LightGray);
            }
            #endregion



            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePathforLoft = path + "/Bullows Panel Drawing/" + "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".dwg";
            string dwgFilePathforLoftpdf = path + "/Bullows Panel Drawing/" + "loft_with_centrifugal_fan_" + DateTime.Now.ToString("hh-mm") + ".pdf";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathforLoft);
            dwgg1.DoWork();

            // Save as PDF
            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, dwgFilePathforLoftpdf);
            pdf1.DoWork();

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePathforLoft
            };
        }

        public PaintBoothclass TubelightCutout(int j, PaintBoothModel model)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double tublightHeight = 265;
            double tublightWidth = 1340;
            double PaintBoothHeight = model.H;
            double ComponentInDSize = PanelWidth / 2;//model.D1 + (model.Depth/2);
            double ComponentInWSize = W / 2;//model.W1 + (model.Width / 2);double StandardBend1 = SettingStandardBend1;
            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;

            devregion tubelightCut = devregion.CreatePolygon(new Point3D[]
            {
                    new Point3D(ComponentInDSize-(tublightHeight/2), ComponentInWSize-(tublightWidth/2), PaintBoothHeight-10),
                    new Point3D(ComponentInDSize+(tublightHeight/2), (ComponentInWSize-(tublightWidth/2)), PaintBoothHeight-10),
                    new Point3D(ComponentInDSize+(tublightHeight/2), ComponentInWSize+(tublightWidth/2),PaintBoothHeight-10),
                    new Point3D(ComponentInDSize-(tublightHeight/2),ComponentInWSize+(tublightWidth/2), PaintBoothHeight-10)
            });
            drawing.Entities.Add(tubelightCut);
            #region Old Code
            //  var tubelightCut1 = devregion.CreatePolygon(new Point3D[]
            //{
            //          new Point3D(ComponentInDSize-(tublightHeight/2)-100, ComponentInWSize-(tublightWidth/2)-100, PaintBoothHeight+10),
            //          new Point3D(ComponentInDSize+(tublightHeight/2)+100, (ComponentInWSize-(tublightWidth/2))-100, PaintBoothHeight+10),
            //          new Point3D(ComponentInDSize+(tublightHeight/2)+100, ComponentInWSize+(tublightWidth/2)+100,PaintBoothHeight+10),
            //          new Point3D(ComponentInDSize-(tublightHeight/2)-100,ComponentInWSize+(tublightWidth/2)+100, PaintBoothHeight+10)
            //});
            //  Brep brep = tubelightCut1.ExtrudeAsBrep(5);

            // brep.ExtrudeRemove(tubelightCut, 300);
            //drawing.Entities.Add(brep,Color.Red);
            //var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            //if (!Directory.Exists(path + "/Bullows Panel Drawing"))
            //    Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            //string dwgFilePathforTubelight = path + "/Bullows Panel Drawing/" + "TubelightCutout_" + DateTime.Now.ToString("hh-mm") + ".dwg";          
            //// Save as DWG
            //WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            //WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathforTubelight);
            //dwgg1.DoWork();

            //return new PaintBoothclass
            //{
            //    drawing = drawing,
            //    lstpath = dwgFilePathforTubelight
            //};
            #endregion
            var tubelightCut1 = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,PanelHeight),
                new Point3D(PanelWidth,0,PanelHeight),
                new Point3D(PanelWidth,W,PanelHeight),
                new Point3D(0,W,PanelHeight),
            });
            Brep brep = tubelightCut1.ExtrudeAsBrep(SheetThickness);

            brep.ExtrudeRemove(tubelightCut, 300);
            drawing.Entities.Add(brep, Color.Green);
            //calculate Weight of panels 
            Material mat = Material.StructuralSteel;
            mat = new Material(Materials);
            brep.Regen(0.1);
            double massofRectangle = brep.GetMass(mat, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity);
            double Rectangle_Weight = Math.Round(massofRectangle, 3);

            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,PanelHeight),
                new Point3D(PanelWidth,0,PanelHeight),
                new Point3D(PanelWidth,W,PanelHeight),
                new Point3D(0,W,PanelHeight),
                new Point3D(0,0,PanelHeight),
            });

            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, PanelHeight),
                new Point3D(0,SheetThickness,PanelHeight),
                new Point3D(0,SheetThickness,(PanelHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1,(PanelHeight +(StandardBend2 - SheetThickness))),
                new Point3D(0,StandardBend1, (PanelHeight+StandardBend2)),
                new Point3D(0,0, (PanelHeight+StandardBend2))
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);

            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/TopPanel {j} {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath
            };

        }
        public PaintBoothclass DevelopmentForAllPanels(PaintBoothModel model, int side, int j)
        {
            #region Development
            drawing = new DesignDocument();

            SettingStandardBend1 = (double)model.standardbend1;
            SettingStandardBend2 = (double)model.standardbend2;
            SheetThickness = (double)model.SheetThickness;
            PitchDistance = (double)model.PitchDistance;
            SlotDimention = model.SlotDimention;

            StandardBend1 = SettingStandardBend1 - SheetThickness;
            StandardBend2 = SettingStandardBend2 - (SheetThickness * 2);
            int bendlineLength = 50;
            if (PanelWidth > 1000)
                bendlineLength = 160;
            else
                bendlineLength = 110;


            const string Dim = "Dimension";
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);

            #region Bottom Left
            #region Horizontal
            #region Inner
            //Add InnerRectangle for Left Bottom
            LinearPath Innerrectangleleft = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)+10, 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+bendlineLength, 0)
            });
            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.Yellow;
            drawing.Layers.Add(mylayer);
            Innerrectangleleft.LayerName = "bendlayer";
            drawing.Entities.Add(Innerrectangleleft);
            #region  upwords Dimensions
            // Calculate the midpoint X position
            double midX = (PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + bendlineLength + 10; // Shift 10 units right

            // Calculate the midpoint Y position
            double midY = 0;  // Keep 

            // Create the Text Entity
            Text UpwordsText = new Text(
                new Point3D(midX, midY, 0), // Position at the midpoint
                "90 Up",  // The text content
               10  // Font size
            )
            {
                LayerName = Dim,  // Assign the same layer as the dimension               
            };
            // Add Text to the Drawing
            drawing.Entities.Add(UpwordsText);
            #endregion


            #endregion
            #region Outer
            LinearPath bendlineBottom = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))+10,-(StandardBend2)),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+bendlineLength),-(StandardBend2)),
            });
            bendlineBottom.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom);
            #region  upwords Dimensions
            // Calculate the midpoint X position
            double bendlineBottomX = (PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + bendlineLength + 10; // Shift 10 units right           
            double bendlineBottomY = -StandardBend2;  // Keep 

            // Create the Text Entity
            Text bendlineBottomText = new Text(
                new Point3D(bendlineBottomX, bendlineBottomY, 0), // Position at the midpoint
                "90 Up",  // The text content
               10  // Font size
            )
            {
                LayerName = Dim,  // Assign the same layer as the dimension               
            };
            // Add Text to the Drawing
            drawing.Entities.Add(bendlineBottomText);
            #endregion
            #endregion
            #endregion

            #region Vertical
            #region Inner
            // inner rectangle left bottom
            LinearPath InnerrectangleleftSide = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 10),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)), bendlineLength)
            });
            InnerrectangleleftSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSide);
            #region Dimensions
            // Calculate the midpoint X position
            double InnerRectangleLeftSideX = (PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + 5; // Shift 10 units right

            // Calculate the midpoint Y position
            double InnerRectangleLeftSideY = bendlineLength + 10;

            // Create the Text Entity
            Text InnerRectangleText = new Text(verticalPlane,
                new Point3D(InnerRectangleLeftSideX, InnerRectangleLeftSideY, 0), // Position at the midpoint
                "90 Up",
               10  // Font size
            )
            {
                LayerName = Dim,
            };
            // Add Text to the Drawing
            drawing.Entities.Add(InnerRectangleText);
            #endregion 
            #endregion
            #region Outer
            LinearPath bendlineLeft = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),10),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),bendlineLength),
            });
            bendlineLeft.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft);
            #region Dimensions
            //Adding 90 Up text for this bendline here
            double bendlineLeftX = (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + (StandardBend2)) - 65;
            double bendlineLeftY = bendlineLength + 10;
            Text bendlineLeftText = new Text(verticalPlane, new Point3D(bendlineLeftX, bendlineLeftY, 0), "90 Up", 10) { LayerName = Dim };
            drawing.Entities.Add(bendlineLeftText);
            #endregion
            #endregion
            #endregion

            #endregion

            #region Bottom Right

            #region Horizontal
            #region Inner
            //for innerRectangle Right Bottom
            LinearPath InnerrectangleRightBottom = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness)-10, 0),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness))-bendlineLength, 0),
            });
            InnerrectangleRightBottom.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightBottom);
            #region   Text Dimensions
            // Calculate the midpoint X position
            double BottonRight = (PanelWidth * 2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness) - bendlineLength - 50; // Shift 10 units right

            // Calculate the midpoint Y position
            double BottonRightmidY = 0;  // Keep 

            // Create the Text Entity
            Text BottonRightUpwordsText = new Text(
                new Point3D(BottonRight, BottonRightmidY, 0), // Position at the midpoint
                "90 Up",  // The text content
               10  // Font size
            )
            {
                LayerName = Dim,  // Assign the same layer as the dimension               
            };
            // Add Text to the Drawing
            drawing.Entities.Add(BottonRightUpwordsText);
            #endregion
            #endregion

            #region Outer
            LinearPath bendlineBottomRight = new LinearPath(new Point3D[]
            {
                  new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-10,-(StandardBend2)),
              new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-bendlineLength,-(StandardBend2)),
            });
            bendlineBottomRight.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRight);
            #region Text Dimensions
            // Calculate the midpoint X position
            double BottonRightX = (PanelWidth * 2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness) - bendlineLength - 50; // Shift 10 units right           
            double BottonRightY = -(StandardBend2);
            Text BottonRightText = new Text(
                new Point3D(BottonRightX, BottonRightY, 0), "90 Up", 10)
            {
                LayerName = Dim,
            };
            drawing.Entities.Add(BottonRightText);
            #endregion
            #endregion
            #endregion

            #region Vertical
            //  inner rectangle Rightside bottom
            #region Inner
            LinearPath InnerrectangleRightSide = new LinearPath(new Point3D[]
               {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), 10),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), bendlineLength),
               });
            InnerrectangleRightSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSide);
            #region Dimensions
            // Calculate the midpoint X position
            double InnerRectangleRightSideTopX = (PanelWidth * 2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)); // Shift 10 units right

            // Calculate the midpoint Y position
            double InnerRectangleRightSideTopY = bendlineLength + 10;

            // Create the Text Entity
            Text InnerRectangleRightTopText1 = new Text(verticalPlane,
                new Point3D(InnerRectangleRightSideTopX, InnerRectangleRightSideTopY, 0), // Position at the midpoint
                "90 Up",
               10  // Font size
            )
            {
                LayerName = Dim,
            };
            // Add Text to the Drawing
            drawing.Entities.Add(InnerRectangleRightTopText1);
            #endregion 
            #endregion

            #region Outer 
            LinearPath bendlineright = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,10),
                  new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,bendlineLength),
           });
            bendlineright.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright);
            //Adding 90 Up text for this bendline here
            double bendlinerightX = (PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + (StandardBend2)) - 2 * SheetThickness;
            double bendlinerightY = bendlineLength + 10;
            Text bendlinerightText = new Text(verticalPlane, new Point3D(bendlinerightX, bendlinerightY, 0), "90 Up", 10) { LayerName = Dim };
            drawing.Entities.Add(bendlinerightText);
            #endregion

            #endregion

            #endregion

            #region Top Left
            #region Horizontal
            #region Inner
            //Add InnerRectangle for Left Top
            LinearPath InnerrectangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)+10,  PanelHeight - 2 * SheetThickness),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)+bendlineLength), ( PanelHeight - 2 * SheetThickness))
            });
            InnerrectangleleftTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftTop);
            #region   Left Top upwords Dimensions
            // Calculate the midpoint X position
            double LeftTopbendX = (PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + bendlineLength + 10; // Shift 10 units right

            // Calculate the midpoint Y position
            double LeftTopbendinY = PanelHeight - 2 * SheetThickness;  // Keep 

            // Create the Text Entity
            Text leftTopUpwordsText = new Text(
                new Point3D(LeftTopbendX, LeftTopbendinY, 0), // Position at the midpoint
                "90 Up",  // The text content
               10  // Font size
            )
            {
                LayerName = Dim,  // Assign the same layer as the dimension               
            };
            // Add Text to the Drawing
            drawing.Entities.Add(leftTopUpwordsText);
            #endregion
            #endregion

            #region Outer

            Line bendlineBottom1 = new Line(Plane.XY,

                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + 10, (PanelHeight - 2 * SheetThickness) + (StandardBend2)),
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + bendlineLength), (PanelHeight - 2 * SheetThickness) + StandardBend2)
           );
            bendlineBottom1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom1);

            #region Dimentions
            double x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + bendlineLength))) / 2;
            LinearDim bendlineBottom1Dim = new LinearDim(Plane.XY,

            bendlineBottom1.StartPoint,
            bendlineBottom1.EndPoint,
            new Point3D(bendlineBottom1.MidPoint.X, bendlineBottom1.MidPoint.Y + 30), 10
                )
            { ExtLineOffset = 0 };
            drawing.Entities.Add(bendlineBottom1Dim, Dim);
            Text bendlineBottom1Text = new Text(bendlineBottom1.EndPoint.X + 30, bendlineBottom1.EndPoint.Y, 0, "90 Up", 10)
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            drawing.Entities.Add(bendlineBottom1Text);
            #endregion
            #endregion
            #endregion

            #region Vertical
            #region Inner
            // inner rectangle left Top
            LinearPath InnerrectangleleftSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), (PanelHeight - 2 * SheetThickness)-10),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)),((PanelHeight - 2 * SheetThickness)-bendlineLength) )
            });
            InnerrectangleleftSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSideTop);
            #region Dimensions
            //// Calculate the midpoint X position
            //double InnerRectangleLeftSideTopX = (PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - 2; // Shift 10 units right

            //// Calculate the midpoint Y position
            //double InnerRectangleLeftSideTopY = ((PanelHeight - 2 * SheetThickness) - bendlineLength) - 20;

            // Create the Text Entity
            Text InnerRectangleLeftTopText = new Text(verticalPlane,
                new Point3D(InnerrectangleleftSideTop.EndPoint.X, InnerrectangleleftSideTop.EndPoint.Y - 30, 0), // Position at the midpoint
                "90 Up",
               10  // Font size
            )
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            // Add Text to the Drawing
            drawing.Entities.Add(InnerRectangleLeftTopText);
            #endregion
            #endregion

            #region Outer
            LinearPath bendlineLeft1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)-10),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)-bendlineLength)
            });
            bendlineLeft1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft1);
            #region Dimensions
            //Adding 90 Up text for this bendline here
            //double bendlineLeft1X = (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + (StandardBend2)) - 65;
            //double bendlineLeft1Y = PanelHeight - 2 * SheetThickness - bendlineLength - 35;
            //bendlineLeft1X = bendlineLeft1.EndPoint.X;
            //bendlineLeft1Y = bendlineLeft1.EndPoint.Y - 30;

            Text bendlineLeft1Text = new Text(verticalPlane, new Point3D(bendlineLeft1.EndPoint.X, bendlineLeft1.EndPoint.Y - 30, 0), "90 Up", 10)
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            drawing.Entities.Add(bendlineLeft1Text);
            #endregion
            #endregion

            #endregion
            #endregion

            #region Top Right

            #region Horizontal
            #region Inner
            //for innerRectangle Right Top
            LinearPath InnerrectangleRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness)-10,  PanelHeight - 2 * SheetThickness),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness))-bendlineLength,  PanelHeight - 2 * SheetThickness),
            });
            InnerrectangleRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightTop);
            #region   Right Top upwords Dimensions
            // Calculate the midpoint X position
            double RightTopbendX = (PanelWidth * 2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness) - bendlineLength - 50; // Shift 10 units right

            // Calculate the midpoint Y position
            double RightTopbendinY = PanelHeight - 2 * SheetThickness;  // Keep 

            // Create the Text Entity
            Text RightTopUpwordsText = new Text(
                new Point3D(RightTopbendX, RightTopbendinY, 0), // Position at the midpoint
                "90 Up",
               10  // Font size
            )
            {
                LayerName = Dim,
            };
            // Add Text to the Drawing
            drawing.Entities.Add(RightTopUpwordsText);
            #endregion
            #endregion

            #region Outer

            LinearPath bendlineBottomRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-10,(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-bendlineLength,(PanelHeight - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottomRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRightTop);
            Text bendlineBottomRightTopText = new Text(bendlineBottomRightTop.EndPoint.X - 30, bendlineBottomRightTop.EndPoint.Y, "90 Up", 10)
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            drawing.Entities.Add(bendlineBottomRightTopText);

            #endregion
            #endregion

            #region Vertical
            #region Inner
            LinearPath InnerrectangleRightSideTop = new LinearPath(new Point3D[]
  {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), (PanelHeight - 2 * SheetThickness)-10),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)),((PanelHeight - 2 * SheetThickness)-bendlineLength) )
  });
            InnerrectangleRightSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSideTop);
            #region Dimensions
            // Calculate the midpoint X position
            double RectangleRightSideTopX = (PanelWidth * 2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness); // Shift 10 units right

            // Calculate the midpoint Y position
            double RectangleRightSideTopY = ((PanelHeight - 2 * SheetThickness) - bendlineLength) - 30;

            // Create the Text Entity
            Text RectangleRightTopText1 = new Text(verticalPlane,
                new Point3D(RectangleRightSideTopX, RectangleRightSideTopY, 0), // Position at the midpoint
                "90 Up",
               10  // Font size
            )
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            // Add Text to the Drawing
            drawing.Entities.Add(RectangleRightTopText1);
            #endregion 
            #endregion

            #region Outer
            LinearPath bendlineright1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 * SheetThickness)-10),
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 * SheetThickness)-bendlineLength),
           });
            bendlineright1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright1);
            #region Dimensions
            Text bendlineright1Text = new Text(verticalPlane, new Point3D(bendlineright1.EndPoint.X, bendlineright1.EndPoint.Y - 30, 0), "90 Up", 10)
            {
                LayerName = Dim,
                Alignment = Text.alignmentType.MiddleCenter
            };
            drawing.Entities.Add(bendlineright1Text);
            #endregion
            #endregion

            #endregion

            #endregion

            #region Outer Bottom Line
            // //for bottom
            LinearPath linearBottom = new LinearPath(new Point3D[]
            {
                  new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+(StandardBend1),-(StandardBend2+StandardBend1)),
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - ((- SheetThickness)+StandardBend1),-(StandardBend1+StandardBend2)),
                  new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-(2*SheetThickness)),-(StandardBend2)),
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness),0),
            });
            drawing.Entities.Add(linearBottom, Color.White);
            //LinearBottomOuter
            #region Outer Dimmention
            #region Outer 1


            x = ((0) + (-(StandardBend2))) / 2;
            LinearDim linearBottomdim = new(verticalPlane,
               new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2) - 20, x), 10)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion

            #region Outer 2
            x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1))) / 2;
            linearBottomdim = new(Plane.XY,
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1), -(StandardBend2 + StandardBend1)),
                 new Point3D(x - 10, -(StandardBend2 + StandardBend1) - 40), 10)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #region Outer 3
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness)) + (((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - ((-SheetThickness) + StandardBend1))) / 2;
            linearBottomdim = new(Plane.XY,
                  new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                 new Point3D(((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) - (2 * SheetThickness)), -(StandardBend2)),
                 new Point3D(x, -(StandardBend2 + StandardBend1) - 40), 10)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #endregion
            //For Top

            double Panelheight = PanelHeight - 2 * SheetThickness;
            LinearPath linearTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), Panelheight),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),StandardBend2 + Panelheight),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+(StandardBend1),Panelheight+(StandardBend2+StandardBend1)),
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness+StandardBend1),Panelheight+(StandardBend1+StandardBend2)),
                    new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),Panelheight+(StandardBend2)),
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness),Panelheight),
            });

            drawing.Entities.Add(linearTop, Color.White);
            // For LeftSide

            LinearPath linearLeft = new LinearPath(new Point3D[]
            {
                new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),0),
                new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))-(StandardBend1+StandardBend2), StandardBend1),
                new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))-(StandardBend1+StandardBend2),Panelheight-StandardBend1),
                 new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))-(StandardBend2),Panelheight),
                new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)),Panelheight),
            });
            drawing.Entities.Add(linearLeft, Color.White);
            #region Dimension
            double y = ((StandardBend1) + (Panelheight - StandardBend1)) / 2;//calculate midpoint of y for placing dimention text
            LinearDim linearLefttDim = new(verticalPlane,
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2)), 0),
                 new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (StandardBend2), Panelheight),
                new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (StandardBend1 + StandardBend2) - 40, y), 20);
            drawing.Entities.Add(linearLefttDim, Dim);

            #endregion

            //For Right Sides

            LinearPath linearRight = new LinearPath(new Point3D[]
            {
               new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-2*SheetThickness), 0),
                new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2-2*SheetThickness)),0),
                new Point3D(((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+(StandardBend1+StandardBend2)-2*SheetThickness), StandardBend1),
                new Point3D(((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+(StandardBend1+StandardBend2)-2*SheetThickness),Panelheight-StandardBend1),

                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+(StandardBend2-2*SheetThickness),Panelheight),
                new Point3D(((PanelWidth*2+ ((PanelWidth / 2 + PanelWidth / 2) * 0.2))-2*SheetThickness),Panelheight),
            });
            drawing.Entities.Add(linearRight, Color.White);
            #endregion


            #region Notching
            LinearPath BottomLeftBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend1),-(StandardBend2+StandardBend1)),
            });
            //Point3D trimPoint = new Point3D(-StandardBend1, 0, 0);
            //BottomLeftNotching.TrimBy(trimPoint, true);
            drawing.Entities.Add(BottomLeftBottmNotching, Color.White);

            //Left bottom Notching 
            LinearPath BottomLeftBottmNotching1 = new LinearPath(new Point3D[]
            {
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))-(StandardBend2),0),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))-(StandardBend2+StandardBend1),StandardBend1),

            });
            drawing.Entities.Add(BottomLeftBottmNotching1, Color.White);
            //for right side 
            LinearPath BottomRightBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),-(StandardBend2)),


                new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-(StandardBend1-SheetThickness),-(StandardBend2+StandardBend1)),
            });

            drawing.Entities.Add(BottomRightBottmNotching, Color.White);
            LinearPath BottomRightBottmNotching1 = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2)))+(StandardBend2 -(2* SheetThickness)),0),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2+StandardBend1))-2*SheetThickness,StandardBend1),

            });
            drawing.Entities.Add(BottomRightBottmNotching1, Color.White);
            //For TopRight Side

            LinearPath BottomRightTopNotching = new LinearPath(new Point3D[]
            {
                 new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2))-2*SheetThickness,PanelHeight - 2 * SheetThickness),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2+StandardBend1)-2*SheetThickness),(PanelHeight - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(BottomRightTopNotching, Color.White);
            LinearPath BottomRightTopNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D(((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D((((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness))-StandardBend1,(PanelHeight - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

            });
            drawing.Entities.Add(BottomRightTopNotching1, Color.White);
            //For Left Top Side
            LinearPath TopleftNotching = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))-(StandardBend2),PanelHeight - 2 * SheetThickness),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))-(StandardBend2+StandardBend1),(PanelHeight - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(TopleftNotching, Color.White);
            LinearPath TopleftNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)))+StandardBend1,(PanelHeight - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

            });
            drawing.Entities.Add(TopleftNotching1, Color.White);
            #endregion
            #region Create Slots
            string[] dimensions = SlotDimention.Split('-');
            if (dimensions.Length != 2)
            {
                throw new ArgumentException("Invalid slot dimensions format. Expected format is 'width-height'.");
            }

            // Parse the width and height
            if (!double.TryParse(dimensions[0], out double slotWidth) || !double.TryParse(dimensions[1], out double slotLength))
            {
                throw new ArgumentException("Slot dimensions must be numeric values.");
            }

            #region holes on PanelHeight


            double PanelHeightSheetThickness = PanelHeight - 2 * SheetThickness;
            double divisionresult = PanelHeightSheetThickness / PitchDistance;

            // Get the number of holes
            int wholenumberpart = (int)Math.Round(divisionresult);

            // Calculate remaining space and distribute evenly at start & end
            double multipliedresult = (wholenumberpart - 1) * PitchDistance;
            double samespacedivide = (PanelHeightSheetThickness - multipliedresult) / 2;

            // **Shift first hole by 1mm closer**
            samespacedivide -= 1;

            // Create slots and add them to the drawing
            for (int i = 0; i < wholenumberpart; i++)
            {
                double centerz = samespacedivide + (i * PitchDistance);

                // Create first slot
                devregion slot = devregion.CreateSlot(Plane.XY,
                    (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2)),
                    centerz,
                    (slotLength - slotWidth),
                    slotWidth / 2,
                    1.5708);

                // Create second slot
                devregion slot1 = devregion.CreateSlot(Plane.XY,
                    (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2) + (PanelWidth + StandardBend2)),
                    centerz,
                    (slotLength - slotWidth),
                    slotWidth / 2,
                    1.5708);

                slot.Color = Color.Yellow;

                // Add slots to drawing
                drawing.Entities.Add(slot, Color.White);
                drawing.Entities.Add(slot1, Color.White);
            }
            #endregion



            #region Holes on PanelWidth

            // Calculate available panel width excluding sheet thickness
            double PanelWidthSheetThickness = PanelWidth - 2 * SheetThickness;

            // Calculate number of holes that fit within the panel width
            double divisionresult1 = PanelWidthSheetThickness / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            // Calculate the total occupied space by the slots
            double multipliedresult1 = wholenumberpart1 * PitchDistance;

            // Calculate the remaining space to distribute evenly
            double samespacedivide1 = (PanelWidthSheetThickness - multipliedresult1) / 2;

            // Adjust first slot position by shifting it 1mm closer
            samespacedivide1 -= 1;

            // Loop through each hole position and create slots
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                // Calculate the Y-coordinate for slot placement
                double centery = samespacedivide1 + (i * PitchDistance);

                // Create slot at the bottom of the panel
                devregion slot2 = devregion.CreateSlot(Plane.XY,
                    ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery),
                    (-StandardBend2 / 2),
                    (slotLength - slotWidth),
                    slotWidth / 2,
                    0);

                // Create slot at the top of the panel
                devregion slot3 = devregion.CreateSlot(Plane.XY,
                    ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery),
                    (PanelHeight + StandardBend2 / 2),
                    (slotLength - slotWidth),
                    slotWidth / 2,
                    0);

                // Adjust slot position for equal spacing
                slot2.Translate(0, 0, samespacedivide1 / 2);

                // Set color for visualization
                slot2.Color = Color.Yellow;

                // Add slots to the drawing
                drawing.Entities.Add(slot2, Color.White);
                drawing.Entities.Add(slot3, Color.White);
            }
            #endregion
            #region Dimension
            double centerz1 = (samespacedivide1 / 2 + (3) * PitchDistance) + 20;
            double centerz2 = (samespacedivide1 / 2 + (4) * PitchDistance) + 20;
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1)) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2))) / 2;
            LinearDim slotsDim = new LinearDim(Plane.XY,
              new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1), (-StandardBend2 / 2)),
               new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2), (-StandardBend2 / 2)),
              new Point3D(x, StandardBend2), 20);

            drawing.Entities.Add(slotsDim, Dim);


            #endregion
            #endregion
            #endregion development
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();


            if (!Directory.Exists(path + "/Development"))
                Directory.CreateDirectory(path + "/Development");

            var dwgFilePathfordevelopment = "";
            dwgFilePathfordevelopment = (side == 0) ?
            dwgFilePathfordevelopment = Path.Combine(path, "Development", "Right Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg") :
            dwgFilePathfordevelopment = Path.Combine(path, "Development", "Left Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");

            switch (side)
            {
                case 0:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "Right Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 1:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "Left Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 2:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "D3 Right Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 3:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "D3 Left Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 4:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "Top Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 5:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "Back Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
                case 6:
                    dwgFilePathfordevelopment = Path.Combine(path, "Development", "Front Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");
                    break;
            }


            // var dwgFilePathfordevelopment = Path.Combine(path, "Development", "Development" + DateTime.Now.ToString("hh-mm") + ".dwg");
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();
            #endregion

            return new PaintBoothclass
            {
                developmentpath = dwgFilePathfordevelopment,
            };

        }
        public string devlopmentForCSection(BendSectionModel model)
        {
            double X = (double)model.Length;
            decimal H = model.H - 2 * model.T;
            decimal W = model.W - model.T;
            decimal Y = H + (2 * W);

            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle = devregion.CreatePolygon(Plane.XY, new Point3D[]
            {
                new Point3D(0,0),
                new Point3D(X,0),
                new Point3D(X,(double)Y),
                new Point3D(0,(double)Y),
            });
            drawing.Entities.Add(rectangle, Color.White);
            LinearPath BendctangleleftBottom = new LinearPath(new Point3D[]
            {
                 new Point3D(0,(double)(model.W-model.T)),
                  new Point3D(50,(double)(model.W-model.T))
            });
            drawing.Entities.Add(BendctangleleftBottom, Color.Yellow);
            LinearPath BendctangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(0,(double)(Y-(model.W-model.T))),

                  new Point3D(50,(double)(Y-(model.W-model.T)))

            });
            drawing.Entities.Add(BendctangleleftTop, Color.Yellow);
            LinearPath BendctangleRightBottom = new LinearPath(new Point3D[]
            {
                 new Point3D(X,(double)(model.W-model.T)),
                  new Point3D(X-50,(double)(model.W-model.T))
            });
            drawing.Entities.Add(BendctangleRightBottom, Color.Yellow);
            LinearPath BendctangleRightTop = new LinearPath(new Point3D[]
            {
                 new Point3D(X,(double)(Y-(model.W-model.T))),
                  new Point3D(X-50,(double)(Y-(model.W-model.T)))
            });
            drawing.Entities.Add(BendctangleRightTop, Color.Yellow);


            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/Bend Development"))
                Directory.CreateDirectory(path + "/Bend Development");
            var dwgFilePathfordevelopment = Path.Combine(path, "Bend Development", "BendDevelopment" + DateTime.Now.ToString("hh-mm") + ".dwg");
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();
            return dwgFilePathfordevelopment;
            #endregion
        }
        public PaintBoothclass Civil(PaintBoothModel model)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            double x = model.D + D3 + D3;
            double y = model.W;
            double z = -70;

            var civilTank = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0, 0, z),
                new Point3D(x, 0, z),
                new Point3D(x, y, z),
                new Point3D(0, y, z)
            });

            Brep brep = civilTank.ExtrudeAsBrep(-1000);
            drawing.Entities.Add(brep, Color.White);
            string path = _configuration["FolderPathConfig:AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/CivilTank"))
                Directory.CreateDirectory(path + "/CivilTank");

            string dwgFilePathforCivil = path + "/CivilTank/" + "CivilTank" + DateTime.Now.ToString("hh-mm") + ".dwg";
            string pdfFilePathforCivil = path + "/CivilTank/" + "CivilTank" + DateTime.Now.ToString("hh-mm") + ".pdf";

            // Save as DWG
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathforCivil);
            dwgg1.DoWork();

            // Save as PDF
            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, pdfFilePathforCivil);
            pdf1.DoWork();


            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePathforCivil
            };

        }
        #region Export Views
        public static List<string> ExportAllViewsWithSection(DesignDocument designDoc, string outputFolder)// Y-coordinate for the XZ-plane section
        {
            var exportedFiles = new List<string>();
            // ---  Export Top, Front, Side Views ---
            exportedFiles.Add(ExportView(designDoc, outputFolder, viewType.Top, "Top"));
            exportedFiles.Add(ExportView(designDoc, outputFolder, viewType.Front, "Front"));
            exportedFiles.Add(ExportView(designDoc, outputFolder, viewType.Left, "Side"));
            return exportedFiles;
        }
        private static string ExportView(DesignDocument designDoc, string outputFolder, viewType vType, string suffix)
        {
            string filePath = Path.Combine($"Bullows_{suffix}_{DateTime.Now:yyyyMMdd_HHmmss}.dwg");

            var settings = new HiddenLinesViewSettingsEx(vType, designDoc)
            {
                KeepEntityColor = true,
                KeepHiddenSegments = true,   //  hidden lines visible
                KeepEntityLineWeight = true,

            };

            var export = new HiddenLinesViewOnFileAutodesk(settings, filePath, 0);
            export.DoWork();

            return filePath;
        }
        #endregion

    }

    public class PaintBoothclass
    {
        public DesignDocument drawing { get; set; }
        public string lstpath { get; set; }
        public string developmentpath { get; set; }

    }
}
