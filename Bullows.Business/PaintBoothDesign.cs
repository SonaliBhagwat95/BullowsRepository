


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
using System.Linq;
using devregion = devDept.Eyeshot.Entities.Region;


namespace Bullows.Business
{
    public class PaintBoothDesign
    {
        private readonly BullowsDbContext _DbContext;

        public PaintBoothDesign(BullowsDbContext dbContext)
        {
            _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
        public double PanelHeight { get; set; }
        public double SheetThickness { get; set; }
        public double SettingStandardBend1 { get; set; }
        public double SettingStandardBend2 { get; set; }
        public double PitchDistance { get; set; }
        public int NoofPanels { get; set; }
        public double BackPanelLength { get; set; }
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
        public static int selectedBaffleHeight;
        public static int bafflePanelCount;
        public string LightTypes { get; set; }
        public decimal LuxLevel { get; set; }
        public decimal Lumens { get; set; }
        public static int roundScalefactor = 0;
        #endregion

        #region  Right left Panels and Development drawings
        public PaintBoothclass PanelsInPaintBooth(int j, double yaxis, PanelInputModel pmodel, PaintBoothModel model, int k)
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
            //calculate Weight of panels 
            //if(Materials=="Al")
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
            devDept.Eyeshot.Entities.Region section = CreatePolygon(k);
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, PanelLength, SheetThickness);

            Material mat1 = Material.StructuralSteel;
            mat = new Material(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            if (k == 0)
            {
                RightPanels_Weight = Rectangle_Weight + Frame_Weight;
                SavePanelDetails(model, pmodel, "RightSide", k, RightPanels_Weight);
            }
            else
            {
                LeftPanels_Weight = Rectangle_Weight + Frame_Weight;
                SavePanelDetails(model, pmodel, "LeftSide", k, LeftPanels_Weight);
            }

            // Generate holes on YZ and XY planes
            frame = GenerateHoles(frame);
            drawing.Entities.Add(frame, Color.Yellow);
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/" + pmodel.ProjectID;
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
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,
            };

        }
        public PaintBoothclass DevelopmentforRightLeftPanels(PaintBoothModel model, int side, int j)
        {
            drawing = new DesignDocument();
            //PanelWidth = model.StandardPanelWidthForD;
            //PanelHeight = model.PanelHeightforH;
            SettingStandardBend1 = (double)model.standardbend1;
            SettingStandardBend2 = (double)model.standardbend2;
            SheetThickness = (double)model.SheetThickness;
            PitchDistance = (double)model.PitchDistance;
            SlotDimention = model.SlotDimention;
            #region Development

            double StandardBend1 = SettingStandardBend1 - SheetThickness;
            double StandardBend2 = SettingStandardBend2 - (SheetThickness * 2);

            const string Dim = "Dimension";
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);
            #region inner Rectangle Dimentions
            #region Inner Rectangle Bottom
            //Add InnerRectangle for Left Bottom
            LinearPath Innerrectangleleft = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+50, 0)
            });

            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            drawing.Layers.Add(mylayer);
            Innerrectangleleft.LayerName = "bendlayer";
            drawing.Entities.Add(Innerrectangleleft, Color.Yellow);
            //for innerRectangle Right Bottom
            LinearPath InnerrectangleRightBottom = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), 0),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness))-50, 0),
            });
            InnerrectangleRightBottom.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightBottom, Color.Yellow);
            #endregion
            #region Inner Rectangle Top
            //Add InnerRectangle for Left Top
            LinearPath InnerrectangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), PanelHeight - 2 * SheetThickness),
                  new Point3D((PanelWidth + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)+50), ( PanelHeight - 2 * SheetThickness))
            });
            InnerrectangleleftTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftTop, Color.Yellow);
            //for innerRectangle Right Top
            LinearPath InnerrectangleRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), PanelHeight - 2 * SheetThickness),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness))-50,  PanelHeight - 2 * SheetThickness),
            });
            InnerrectangleRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightTop, Color.Yellow);
            #endregion
            #region Inner Rectangle Left
            //left bottom
            LinearPath InnerrectangleleftSide = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)), 50)
            });
            InnerrectangleleftSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSide, Color.Yellow);
            //legt Top
            LinearPath InnerrectangleleftSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), (PanelHeight - 2 * SheetThickness)),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)),((PanelHeight - 2 * SheetThickness)-50) )
            });
            InnerrectangleleftSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSideTop, Color.Yellow);
            #endregion
            #region Inner Rectangle Right
            //left bottom
            LinearPath InnerrectangleRightSide = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)-(2*SheetThickness)), 0),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), 50),
            });
            InnerrectangleRightSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSide, Color.Yellow);
            //left Top
            LinearPath InnerrectangleRightSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), (PanelHeight - 2 * SheetThickness)),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)),((PanelHeight - 2 * SheetThickness)-50) )
            });
            InnerrectangleRightSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSideTop, Color.Yellow);
            #endregion
            #endregion
            #region bend
            #region RightSideBendLine
            //for bendline right side
            LinearPath bendlineright = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,0),
                  new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,50),
            });
            bendlineright.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright, Color.Yellow);
            LinearPath bendlineright1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 * SheetThickness)),
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 *SheetThickness)-50),

           });
            bendlineright1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright1, Color.Yellow);
            #endregion
            #region LeftSideBendLine
            //for bendline right side
            LinearPath bendlineLeft = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),0),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),50),
            });
            bendlineLeft.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft, Color.Yellow);
            LinearPath bendlineLeft1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)-50)
            });
            bendlineLeft1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft1, Color.Yellow);
            #endregion
            #region BottomBend Line
            //for bendline Bottom side
            LinearPath bendlineBottom = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),-(StandardBend2)),
            });
            bendlineBottom.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom, Color.Yellow);
            LinearPath bendlineBottom1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),(PanelHeight - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottom1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom1, Color.Yellow);
            #region Dimensions
            double x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50))) / 2;
            LinearDim bendlineBottom1Dim = new LinearDim(Plane.XY,
            new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), (PanelHeight - 2 * SheetThickness) + (StandardBend2)),
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50), (PanelHeight - 2 * SheetThickness) + StandardBend2),
                new Point3D(x, (PanelHeight - 2 * SheetThickness) + (StandardBend2) + 50), 20);
            drawing.Entities.Add(bendlineBottom1Dim, Dim);
            #endregion
            #endregion
            #region BottomBend Right Line
            //for bendline Bottom side
            LinearPath bendlineBottomRight = new LinearPath(new Point3D[]
             {
                  new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),-(StandardBend2)),
              new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,-(StandardBend2)),
            });
            bendlineBottomRight.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRight, Color.Yellow);

            LinearPath bendlineBottomRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,(PanelHeight - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottomRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRightTop, Color.Yellow);
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
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness),0),
            });
            drawing.Entities.Add(linearBottom, Color.White);
            #region Outer Dimmention
            #region Outer 1
            x = ((0) + (-(StandardBend2))) / 2;
            LinearDim linearBottomdim = new(verticalPlane,
               new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2) - 40, x), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion

            #region Outer 2
            x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness))) / 2;
            linearBottomdim = new(Plane.XY,
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1), -(StandardBend2 + StandardBend1)),
                 new Point3D(x - 10, -(StandardBend2 + StandardBend1) - 40), 20)
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
                new Point3D(((PanelWidth*2+ ((PanelWidth / 2 +PanelWidth / 2) * 0.2))-2*SheetThickness),Panelheight),
            });
            drawing.Entities.Add(linearRight, Color.White);
            #endregion
            #endregion bend
            #region Notching
            LinearPath BottomLeftBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend1-SheetThickness),-(StandardBend2+StandardBend1)),
            });

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
                 new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2),PanelHeight - 2 * SheetThickness),
                new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2+StandardBend1),(PanelHeight - 2 * SheetThickness)-StandardBend1),
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
            //  CreateSlots(model);
            //creating holes on xy direction 
            double newpanelHeight = PanelHeight - 2 * SheetThickness;
            double divisionresult = newpanelHeight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            // calculate the remaining space after creating whole slots
            double multipliedresult = (wholenumberpart) * PitchDistance;
            double samespacedivide = newpanelHeight - multipliedresult;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart; i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else

                    centerz = samespacedivide / 2 + (i) * PitchDistance;
                // create a slot
                devregion slot = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);

                devregion slot1 = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2) + (PanelWidth + StandardBend2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);
                slot.Translate(0, 0, samespacedivide / 2);
                slot.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot, Color.White);
                drawing.Entities.Add(slot1, Color.White);
            }

            //create slots on width
            double newPanelWidth = PanelWidth - 2 * SheetThickness;

            double divisionresult1 = newPanelWidth / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            // calculate the remaining space after creating whole slots
            double multipliedresult1 = wholenumberpart1 * PitchDistance;
            double samespacedivide1 = newPanelWidth - multipliedresult1;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centery;
                if (i == 0)
                    centery = samespacedivide1 / 2;
                else
                    //centery = /*(PanelWidth) - (((i + 0) * PitchDistance) - (samespacedivide1 / 2));*/
                    centery = samespacedivide1 / 2 + (i) * PitchDistance;

                // create a slot
                devregion slot2 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (-StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);

                devregion slot3 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (PanelHeight + StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                slot2.Translate(0, 0, samespacedivide1 / 2);
                slot2.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot2, Color.White);
                drawing.Entities.Add(slot3, Color.White);
            }
            #region Dimension
            double centerz1 = samespacedivide1 / 2 + (2) * PitchDistance;
            double centerz2 = samespacedivide1 / 2 + (3) * PitchDistance;
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1)) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2))) / 2;
            LinearDim slotsDim = new LinearDim(Plane.XY,
              new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1), (-StandardBend2 / 2)),
               new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2), (-StandardBend2 / 2)),
              new Point3D(x, (-StandardBend2 / 2) + 50), 20);

            drawing.Entities.Add(slotsDim, Dim);

            #endregion
            #endregion

            #region ScaleFactor                    
            double xd = PanelWidth;
            double yd = PanelHeight;

            double scaleX = xd / (trimmedWidth - titleBoxWidth);
            double scaleY = yd / (trimmedHeight - titleBoxHeight);
            double scaleFactor = Math.Max(scaleX, scaleY);
            roundScalefactor = (int)Math.Ceiling(scaleFactor);
            StandardFrame(roundScalefactor, drawing);
            #endregion
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/";

            if (!Directory.Exists(path + "/Development"))
                Directory.CreateDirectory(path + "/Development");
            var dwgFilePathfordevelopment = "";
            dwgFilePathfordevelopment = (side == 0) ?
            dwgFilePathfordevelopment = Path.Combine(path, "Development", "Right Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg") :
            dwgFilePathfordevelopment = Path.Combine(path, "Development", "Left Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();

            #endregion

            #endregion development
            return new PaintBoothclass
            {
                //developmentdrawing = developmentdrawing,
                developmentpath = dwgFilePathfordevelopment,
            };
        }
        #endregion
        #region 3D D3 Panels And Development Drawings
        public PaintBoothclass D3Panels(int k, int i, string Doorlocation, PaintBoothModel model)
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
            devDept.Eyeshot.Entities.Region section = CreatePolygon(i);

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
                SavePanelDetails(model, pmodel, "D3Panels Right side", i, D3Panels_weight);
            }
            else
            {
                SavePanelDetails(model, pmodel, "D3Panels Left Side", i, D3Panels_weight);
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
            #region EriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");
            string dwgFilePath;
            if (i == 0)
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3Panels Right side  {k} {DateTime.Now:hh - mm}.dwg";
            }
            else
            {
                dwgFilePath = $"{path}/PaintBooth drawing/D3Panels Left side {k} {DateTime.Now:hh - mm}.dwg";
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
        public PaintBoothclass DevelopmentforD3Panels(PaintBoothModel model, int side, int j)
        {
            drawing = new DesignDocument();
            // D3 = model.StandardPanelWidthForD;
            //  PanelHeight = model.PanelHeightforH;
            SettingStandardBend1 = (double)model.standardbend1;
            SettingStandardBend2 = (double)model.standardbend2;
            SheetThickness = (double)model.SheetThickness;
            PitchDistance = (double)model.PitchDistance;
            SlotDimention = model.SlotDimention;
            #region Development
            double StandardBend1 = SettingStandardBend1 - SheetThickness;
            double StandardBend2 = SettingStandardBend2 - (SheetThickness * 2);

            const string Dim = "Dimension";
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);

            #region inner Rectangle Dimentions
            #region Inner Rectangle Bottom
            //Add InnerRectangle for Left Bottom
            LinearPath Innerrectangleleft = new LinearPath(new Point3D[]
            {
                 new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), 0),
                  new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2))+50, 0)
            });

            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.Yellow;
            drawing.Layers.Add(mylayer);
            Innerrectangleleft.LayerName = "bendlayer";
            drawing.Entities.Add(Innerrectangleleft);
            //for innerRectangle Right Bottom
            LinearPath InnerrectangleRightBottom = new LinearPath(new Point3D[]
            {
                new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 * SheetThickness), 0),
                new Point3D((((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 *SheetThickness))-50, 0),
            });
            InnerrectangleRightBottom.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightBottom);
            #endregion
            #region Inner Rectangle Top
            //Add InnerRectangle for Left Top
            LinearPath InnerrectangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), PanelHeight - 2 * SheetThickness),
                  new Point3D((D3 + ((D3 / 2 +D3 / 2) * 0.2)+50), ( PanelHeight - 2 * SheetThickness))
            });
            InnerrectangleleftTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftTop);
            //for innerRectangle Right Top
            LinearPath InnerrectangleRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 * SheetThickness), PanelHeight - 2 * SheetThickness),
                new Point3D((((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 * SheetThickness))-50,  PanelHeight - 2 * SheetThickness),
            });
            InnerrectangleRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightTop);
            #endregion
            #region Inner Rectangle Left
            //left bottom
            LinearPath InnerrectangleleftSide = new LinearPath(new Point3D[]
            {
                 new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), 0),
                  new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2)), 50)
            });
            InnerrectangleleftSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSide);
            //legt Top
            LinearPath InnerrectangleleftSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), (PanelHeight - 2 * SheetThickness)),
                  new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2)),((PanelHeight - 2 * SheetThickness)-50) )
            });
            InnerrectangleleftSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSideTop);
            #endregion
            #region Inner Rectangle Right
            //left bottom
            LinearPath InnerrectangleRightSide = new LinearPath(new Point3D[]
            {
                 new Point3D((D3*2 + ((D3 / 2 +D3 / 2) * 0.2)-(2*SheetThickness)), 0),
                  new Point3D((D3*2 + ((D3 / 2 + D3 / 2) * 0.2)-(2*SheetThickness)), 50),
            });
            InnerrectangleRightSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSide, Color.Yellow);
            //legt Top
            LinearPath InnerrectangleRightSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D((D3*2 + ((D3 / 2 + D3 / 2) * 0.2)-(2*SheetThickness)), (PanelHeight - 2 * SheetThickness)),
                  new Point3D((D3*2 + ((D3 / 2 + D3 / 2) * 0.2)-(2*SheetThickness)),((PanelHeight - 2 * SheetThickness)-50) )
            });
            InnerrectangleRightSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSideTop, Color.Yellow);
            #endregion
            #endregion
            #region bend
            #region RightSideBendLine
            //for bendline right side
            LinearPath bendlineright = new LinearPath(new Point3D[]
             {
                 new Point3D((D3*2)+((D3/2+D3/2)*(0.2)+(StandardBend2))-2*SheetThickness,0),
                  new Point3D((D3*2)+((D3/2+D3/2)*(0.2)+(StandardBend2))-2*SheetThickness,50),
            });
            bendlineright.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright);
            LinearPath bendlineright1 = new LinearPath(new Point3D[]
            {
                 new Point3D((D3*2)+((D3/2+D3/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 * SheetThickness)),
                 new Point3D((D3*2)+((D3/2+D3/2)*(0.2)+(StandardBend2))-2*SheetThickness,(PanelHeight - 2 *SheetThickness)-50),

           });
            bendlineright1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright1);
            #endregion
            #region LeftSideBendLine
            //for bendline right side
            LinearPath bendlineLeft = new LinearPath(new Point3D[]
             {
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)-(StandardBend2)),0),
                  new Point3D((D3)+((D3/2+D3/2)*(0.2)-(StandardBend2)),50),
            });
            bendlineLeft.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft);
            LinearPath bendlineLeft1 = new LinearPath(new Point3D[]
            {
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)),
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)-(StandardBend2)),(PanelHeight - 2 * SheetThickness)-50)
            });
            bendlineLeft1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft1);
            #endregion
            #region BottomBend Line
            //for bendline Bottom side
            LinearPath bendlineBottom = new LinearPath(new Point3D[]
             {
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)),-(StandardBend2)),
                  new Point3D((D3)+((D3/2+D3/2)*(0.2)+50),-(StandardBend2)),
            });
            bendlineBottom.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom);
            LinearPath bendlineBottom1 = new LinearPath(new Point3D[]
            {
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)+50),(PanelHeight - 2 * SheetThickness)+StandardBend2)

           });
            bendlineBottom1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom1);

            #region Dimensions
            double x = (((D3) + ((D3 / 2 + D3 / 2) * (0.2))) + ((D3) + ((D3 / 2 + D3 / 2) * (0.2) + 50))) / 2;
            LinearDim bendlineBottom1Dim = new LinearDim(Plane.XY,
           new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2)), (PanelHeight - 2 * SheetThickness) + (StandardBend2)),
                 new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2) + 50), (PanelHeight - 2 * SheetThickness) + StandardBend2),
                new Point3D(x, (PanelHeight - 2 * SheetThickness) + (StandardBend2) + 50), 20);
            drawing.Entities.Add(bendlineBottom1Dim, Dim);
            #endregion
            #endregion
            #region BottomBend Right Line
            //for bendline Bottom side
            LinearPath bendlineBottomRight = new LinearPath(new Point3D[]
             {
                  new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-2*SheetThickness),-(StandardBend2)),
                  new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-2*SheetThickness)-50,-(StandardBend2)),

            });
            bendlineBottomRight.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRight);

            LinearPath bendlineBottomRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-2*SheetThickness),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-2*SheetThickness)-50,(PanelHeight - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottomRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRightTop);
            #endregion
            #region Outer Bottom Line
            // //for bottom

            LinearPath linearBottom = new LinearPath(new Point3D[]
            {
                  new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), 0),
                  new Point3D((D3)+((D3/2+D3/2)*(0.2)),-(StandardBend2)),
                  new Point3D(((D3) + ((D3 / 2 + D3 / 2) * 0.2))+(StandardBend1),-(StandardBend2+StandardBend1)),
                  new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - ((- SheetThickness)+StandardBend1),-(StandardBend1+StandardBend2)),
                  new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-(2*SheetThickness)),-(StandardBend2)),
                  new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 *SheetThickness),0),
            });
            drawing.Entities.Add(linearBottom, Color.White);
            #region Outer Dimmention
            #region Outer 1
            x = ((0) + (-(StandardBend2))) / 2;
            LinearDim linearBottomdim = new(verticalPlane,
               new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), 0),
                  new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2) - 40, x), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion

            #region Outer 2
            x = (((D3) + ((D3 / 2 + D3 / 2) * (0.2))) + (((D3) + ((D3 / 2 + D3 / 2) * 0.2)) + (StandardBend1 - SheetThickness))) / 2;
            linearBottomdim = new(Plane.XY,
                new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((D3) + ((D3 / 2 + D3 / 2) * 0.2)) + (StandardBend1), -(StandardBend2 + StandardBend1)),
                 new Point3D(x - 10, -(StandardBend2 + StandardBend1) - 40), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #region Outer 3
            x = ((((D3) + ((D3 / 2 + D3 / 2) * 0.2)) + (StandardBend1 - SheetThickness)) + (((D3 * 2) + ((D3 / 2 + D3 / 2) * 0.2)) - ((-SheetThickness) + StandardBend1))) / 2;
            linearBottomdim = new(Plane.XY,
                 new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((D3 * 2) + ((D3 / 2 + D3 / 2) * (0.2)) - (2 * SheetThickness)), -(StandardBend2)),
                 new Point3D(x, -(StandardBend2 + StandardBend1) - 20), 10)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #endregion

            //For Top

            double Panelheight = PanelHeight - 2 * SheetThickness;
            LinearPath linearTop = new LinearPath(new Point3D[]
            {
                 new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), Panelheight),
                  new Point3D((D3)+((D3/2+D3/2)*(0.2)),StandardBend2 + Panelheight),
                  new Point3D(((D3) + ((D3 / 2 + D3 / 2) * 0.2))+(StandardBend1),Panelheight+(StandardBend2+StandardBend1)),
                  new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 * SheetThickness+StandardBend1),Panelheight+(StandardBend1+StandardBend2)),
                    new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))-2*SheetThickness),Panelheight+(StandardBend2)),
                  new Point3D(((D3*2) + ((D3 / 2 + D3 / 2) * 0.2)) - (2 * SheetThickness),Panelheight),
            });

            drawing.Entities.Add(linearTop, Color.White);
            // For LeftSide

            LinearPath linearLeft = new LinearPath(new Point3D[]
            {
                new Point3D(D3 + ((D3 / 2 + D3 / 2) * 0.2), 0),
                new Point3D((D3)+((D3/2+D3/2)*(0.2)-(StandardBend2)),0),
                new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2))-(StandardBend1+StandardBend2), StandardBend1),
                new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2))-(StandardBend1+StandardBend2),Panelheight-StandardBend1),
                 new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2))-(StandardBend2),Panelheight),
                new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2)),Panelheight),
            });
            drawing.Entities.Add(linearLeft, Color.White);
            #region Dimension
            double y = ((StandardBend1) + (Panelheight - StandardBend1)) / 2;//calculate midpoint of y for placing dimention text
            LinearDim linearLefttDim = new(verticalPlane,
                new Point3D((D3) + ((D3 / 2 + D3 / 2) * (0.2) - (StandardBend2)), 0),
                new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2)) - (StandardBend2), Panelheight),
                new Point3D((D3 + ((D3 / 2 + D3 / 2) * 0.2)) - (StandardBend1 + StandardBend2) - 40, y), 20);
            drawing.Entities.Add(linearLefttDim, Dim);

            #endregion

            //For Right Sides

            LinearPath linearRight = new LinearPath(new Point3D[]
                {
                   new Point3D((D3*2 + ((D3 / 2 + D3 / 2) * 0.2)-2*SheetThickness), 0),
                    new Point3D((D3*2)+((D3/2+D3/2)*(0.2)+(StandardBend2-2*SheetThickness)),0),
                    new Point3D(((D3*2 + ((D3 / 2 + D3 / 2) * 0.2))+(StandardBend1+StandardBend2)-2*SheetThickness), StandardBend1),
                    new Point3D(((D3*2 + ((D3 / 2 + D3 / 2) * 0.2))+(StandardBend1+StandardBend2)-2*SheetThickness),Panelheight-StandardBend1),

                     new Point3D((D3*2 + ((D3 / 2 + D3 / 2) * 0.2))+(StandardBend2-2*SheetThickness),Panelheight),
                    new Point3D(((D3*2+ ((D3 / 2 +D3 / 2) * 0.2))-2*SheetThickness),Panelheight),
                });
            drawing.Entities.Add(linearRight, Color.White);
            #endregion


            #endregion bend

            #region Notching
            LinearPath BottomLeftBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D((D3)+((D3/2+D3/2)*(0.2)),-(StandardBend2)),
                new Point3D((D3)+((D3/2+D3/2)*(0.2))+(StandardBend1-SheetThickness),-(StandardBend2+StandardBend1)),
            });

            drawing.Entities.Add(BottomLeftBottmNotching, Color.White);

            //Left bottom Notching 
            LinearPath BottomLeftBottmNotching1 = new LinearPath(new Point3D[]
            {
                new Point3D((D3)+((D3/2+D3/2)*(0.2))-(StandardBend2),0),
                new Point3D((D3)+((D3/2+D3/2)*(0.2))-(StandardBend2+StandardBend1),StandardBend1),

            });
            drawing.Entities.Add(BottomLeftBottmNotching1, Color.White);
            //for right side 
            LinearPath BottomRightBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D(((D3*2)+((D3/2+D3/2)*(0.2))-2*SheetThickness),-(StandardBend2)),


                new Point3D((D3*2)+((D3/2+D3/2)*(0.2))-(StandardBend1-SheetThickness),-(StandardBend2+StandardBend1)),
            });

            drawing.Entities.Add(BottomRightBottmNotching, Color.White);
            LinearPath BottomRightBottmNotching1 = new LinearPath(new Point3D[]
            {
                new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2)))+(StandardBend2 -(2* SheetThickness)),0),
                new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))+(StandardBend2+StandardBend1))-2*SheetThickness,StandardBend1),

            });
            drawing.Entities.Add(BottomRightBottmNotching1, Color.White);
            //For TopRight Side

            LinearPath BottomRightTopNotching = new LinearPath(new Point3D[]
            {
                 new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))+(StandardBend2))-2*SheetThickness,PanelHeight - 2 * SheetThickness),
                new Point3D(((D3 * 2)+((D3/2+D3/2)*(0.2))+(StandardBend2+StandardBend1)-2*SheetThickness),(PanelHeight - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(BottomRightTopNotching, Color.White);
            LinearPath BottomRightTopNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D(((D3*2)+((D3/2+D3/2)*(0.2))-2*SheetThickness),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D((((D3*2)+((D3/2+D3/2)*(0.2))-2*SheetThickness))-StandardBend1,(PanelHeight - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

            });
            drawing.Entities.Add(BottomRightTopNotching1, Color.White);
            //For Left Top Side
            LinearPath TopleftNotching = new LinearPath(new Point3D[]
            {
                 new Point3D((D3) +((D3 / 2 + D3 / 2) *(0.2))-(StandardBend2),PanelHeight - 2 * SheetThickness),
                new Point3D((D3) +((D3 / 2 + D3 / 2) *(0.2))-(StandardBend2+StandardBend1),(PanelHeight - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(TopleftNotching, Color.White);
            LinearPath TopleftNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D((D3)+((D3/2+D3/2)*(0.2)),(PanelHeight - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((D3)+((D3/2+D3/2)*(0.2)))+StandardBend1,(PanelHeight - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

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
            //  CreateSlots(model);
            //creating holes on xy direction 
            double newPanelHeight = PanelHeight - 2 * SheetThickness;
            double divisionresult = newPanelHeight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            // calculate the remaining space after creating whole slots
            double multipliedresult = (wholenumberpart) * PitchDistance;
            double samespacedivide = newPanelHeight - multipliedresult;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart; i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else

                    centerz = samespacedivide / 2 + (i) * PitchDistance;
                // create a slot
                devregion slot = devregion.CreateSlot(Plane.XY, (D3) + ((D3 / 2 + D3 / 2) * (0.2) - (StandardBend2 / 2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);

                devregion slot1 = devregion.CreateSlot(Plane.XY, (D3) + ((D3 / 2 + D3 / 2) * (0.2) - (StandardBend2 / 2) + (D3 + StandardBend2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);
                slot.Translate(0, 0, samespacedivide / 2);
                slot.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot, Color.White);
                drawing.Entities.Add(slot1, Color.White);
            }

            //create slots on width
            double divisionresult1 = D3 / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            // calculate the remaining space after creating whole slots
            double multipliedresult1 = wholenumberpart1 * PitchDistance;
            double samespacedivide1 = D3 - multipliedresult1;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centery;
                if (i == 0)
                    centery = samespacedivide1 / 2;
                else
                    centery = samespacedivide1 / 2 + (i) * PitchDistance;

                // create a slot
                devregion slot2 = devregion.CreateSlot(Plane.XY, ((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centery), (-StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);

                devregion slot3 = devregion.CreateSlot(Plane.XY, ((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centery), (PanelHeight + StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                slot2.Translate(0, 0, samespacedivide1 / 2);
                slot2.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot2, Color.White);
                drawing.Entities.Add(slot3, Color.White);
            }
            #region Dimension
            double centerz1 = samespacedivide1 / 2 + (2) * PitchDistance;
            double centerz2 = samespacedivide1 / 2 + (3) * PitchDistance;
            x = ((((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centerz1)) + (((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centerz2))) / 2;
            LinearDim slotsDim = new LinearDim(Plane.XY,
              new Point3D(((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centerz1), (-StandardBend2 / 2)),
               new Point3D(((D3) + ((D3 / 2 + D3 / 2) * (0.2)) + centerz2), (-StandardBend2 / 2)),
              new Point3D(x, (-StandardBend2 / 2) + 50), 20);

            drawing.Entities.Add(slotsDim, Dim);
            #endregion

            #endregion
            #region ScaleFactor                    
            double xd = D3;
            double yd = PanelHeight;

            double scaleX = xd / (trimmedWidth - titleBoxWidth);
            double scaleY = yd / (trimmedHeight - titleBoxHeight);
            double scaleFactor = Math.Max(scaleX, scaleY);
            int roundScalefactor = (int)Math.Ceiling(scaleFactor);
            //StandardFrame(roundScalefactor);
            StandardFrame(roundScalefactor, drawing);
            #endregion
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/";

            if (!Directory.Exists(path + "/Development"))
                Directory.CreateDirectory(path + "/Development");
            var dwgFilePathfordevelopment = "";
            dwgFilePathfordevelopment = (side == 0) ?
            dwgFilePathfordevelopment = Path.Combine(path, "Development", " D3 Right Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg") :
            dwgFilePathfordevelopment = Path.Combine(path, "Development", " D3 Left Panel Development" + j + DateTime.Now.ToString("hh-mm") + ".dwg");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();


            #endregion

            #endregion development
            return new PaintBoothclass
            {
                //developmentdrawing = developmentdrawing,
                developmentpath = dwgFilePathfordevelopment,
            };
        }
        #endregion
        #region 3D top panels and development Drawings
        public PaintBoothclass TopSidePanels(int j, PanelInputModel pmodel, PaintBoothModel model)
        {

            string panelPosition;
            int k = 0;

            double StandardBend1 = SettingStandardBend1;
            double StandardBend2 = SettingStandardBend2;

            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            W = PanelLength;

            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,PanelHeight),
                new Point3D(PanelWidth,0,PanelHeight),
                new Point3D(PanelWidth,W,PanelHeight),
                new Point3D(0,W,PanelHeight),
            });
            drawing.Entities.Add(rectangle, Color.Green);
            Brep brep = rectangle.ExtrudeAsBrep(SheetThickness);
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

            GenerateHoles(frame);
            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
            mat1 = new(Materials);
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mat1, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopPanel_Weight = Rectangle_Weight + Frame_Weight;
            drawing.Entities.Add(frame, Color.Yellow);

            #region TubeLight
            int TubeLightQuantity = 1;
            //double PaintBoothArea = (model.W / 1000) * (model.D / 1000);
            decimal PaintBoothArea = ((decimal)(model.W * model.D)) / 1000000m;//Area is in SquareMeters
            TubeLightCalculations = Math.Ceiling(((decimal)PaintBoothArea * LuxLevel) / (Lumens * TubeLightQuantity * 0.7m));//0.7m is scaleing factor

            model.Lights = (int)TubeLightCalculations;
            #region fetch Tulight Table and save Quantity of TB in database
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

            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/" + pmodel.ProjectID;

            if (!Directory.Exists(path + "/PaintBooth drawing"))
                Directory.CreateDirectory(path + "/PaintBooth drawing");

            string dwgFilePath = $"{path}/PaintBooth drawing/TopPanel {j} {DateTime.Now:hh - mm}.dwg";

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePath);
            dwgg1.DoWork();
            #endregion
            SavePanelDetails(model, pmodel, "TopPanels", k, TopPanel_Weight);
            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass DevelopmentforTopPanels(PaintBoothModel model)
        {
            drawing = new DesignDocument();

            PanelWidth = model.StandardPanelWidthForD;
            PanelHeight = model.PanelHeightforW;
            W = PanelHeight;

            SettingStandardBend1 = (double)model.standardbend1;
            SettingStandardBend2 = (double)model.standardbend2;
            SheetThickness = (double)model.SheetThickness;
            PitchDistance = (double)model.PitchDistance;
            SlotDimention = model.SlotDimention;

            const string Dim = "Dimension";
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);

            #region Development
            double StandardBend1 = SettingStandardBend1 - SheetThickness;
            double StandardBend2 = SettingStandardBend2 - (SheetThickness * 2);
            #region inner Rectangle Dimentions
            #region Inner Rectangle Bottom
            //Add InnerRectangle for Left Bottom
            LinearPath Innerrectangleleft = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+50, 0)
            });

            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.Yellow;
            drawing.Layers.Add(mylayer);
            Innerrectangleleft.LayerName = "bendlayer";
            drawing.Entities.Add(Innerrectangleleft);

            //for innerRectangle Right Bottom
            LinearPath InnerrectangleRightBottom = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), 0),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness))-50, 0),
            });
            InnerrectangleRightBottom.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightBottom);
            #endregion
            #region Inner Rectangle Top
            //Add InnerRectangle for Left Top
            LinearPath InnerrectangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), W - 2 * SheetThickness),
                  new Point3D((PanelWidth + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)+50), ( W - 2 * SheetThickness))
            });
            InnerrectangleleftTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftTop);
            //for innerRectangle Right Top
            LinearPath InnerrectangleRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), W - 2 * SheetThickness),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness))-50,  W - 2 * SheetThickness),
            });
            InnerrectangleRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightTop);
            #endregion
            #region Inner Rectangle Left
            //left bottom
            LinearPath InnerrectangleleftSide = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)), 50)
            });
            InnerrectangleleftSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSide);
            //legt Top
            LinearPath InnerrectangleleftSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), (W - 2 * SheetThickness)),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)),((W - 2 * SheetThickness)-50) )
            });
            InnerrectangleleftSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSideTop);
            #endregion
            #region Inner Rectangle Right
            //left bottom
            LinearPath InnerrectangleRightSide = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)-(2*SheetThickness)), 0),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), 50),
            });
            InnerrectangleRightSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSide);
            //legt Top
            LinearPath InnerrectangleRightSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), (W - 2 * SheetThickness)),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)),((W - 2 * SheetThickness)-50) )
            });
            InnerrectangleRightSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSideTop);
            #endregion
            #endregion
            #region bend
            #region RightSideBendLine
            //for bendline right side
            LinearPath bendlineright = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,0),
                  new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,50),
            });
            bendlineright.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright);
            LinearPath bendlineright1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(W - 2 * SheetThickness)),
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(W - 2 *SheetThickness)-50),

           });
            bendlineright1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright1);
            #endregion
            #region LeftSideBendLine
            //for bendline right side
            LinearPath bendlineLeft = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),0),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),50),
            });
            bendlineLeft.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft);
            LinearPath bendlineLeft1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(W - 2 * SheetThickness)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(W - 2 * SheetThickness)-50)
            });
            bendlineLeft1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft1);
            #endregion
            #region BottomBend Line
            //for bendline Bottom side
            LinearPath bendlineBottom = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),-(StandardBend2)),
            });
            bendlineBottom.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom);
            LinearPath bendlineBottom1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(W - 2 * SheetThickness)+(StandardBend2)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),(W - 2 * SheetThickness)+StandardBend2)

           });
            bendlineBottom1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom1);

            #region Dimensions
            double x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50))) / 2;
            LinearDim bendlineBottom1Dim = new LinearDim(Plane.XY,
            new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), (PanelHeight - 2 * SheetThickness) + (StandardBend2)),
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50), (PanelHeight - 2 * SheetThickness) + StandardBend2),
                new Point3D(x, (PanelHeight - 2 * SheetThickness) + (StandardBend2) + 50), 20);
            drawing.Entities.Add(bendlineBottom1Dim, Dim);
            #endregion
            #endregion
            #region BottomBend Right Line
            //for bendline Bottom side
            LinearPath bendlineBottomRight = new LinearPath(new Point3D[]
             {
                  new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),-(StandardBend2)),
              new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,-(StandardBend2)),
            });
            bendlineBottomRight.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRight);

            LinearPath bendlineBottomRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,(W - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottomRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRightTop);
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
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness),0),
            });
            drawing.Entities.Add(linearBottom, Color.White);

            #region Outer Dimmention
            #region Outer 1
            x = ((0) + (-(StandardBend2))) / 2;
            LinearDim linearBottomdim = new(verticalPlane,
               new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2) - 60, x), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion

            #region Outer 2
            x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness))) / 2;
            linearBottomdim = new(Plane.XY,
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1), -(StandardBend2 + StandardBend1)),
                 new Point3D(x - 10, -(StandardBend2 + StandardBend1) - 40), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #region Outer 3
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness)) + (((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - ((-SheetThickness) + StandardBend1))) / 2;
            linearBottomdim = new(Plane.XY,
                //new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness), -(StandardBend2 + StandardBend1)),
                // new Point3D(((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - ((-SheetThickness) + StandardBend1), -(StandardBend1 + StandardBend2)),
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                 new Point3D(((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) - (2 * SheetThickness)), -(StandardBend2)),
                 new Point3D(x, -(StandardBend2 + StandardBend1) - 20), 10)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #endregion


            //For Top
            W = PanelHeight;
            double Panelheight = W - 2 * SheetThickness;
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
                 //new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (StandardBend1 + StandardBend2), StandardBend1),
                 //new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (StandardBend1 + StandardBend2), Panelheight - StandardBend1),
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
                new Point3D(((PanelWidth*2+ ((PanelWidth / 2 +PanelWidth / 2) * 0.2))-2*SheetThickness),Panelheight),
            });
            drawing.Entities.Add(linearRight, Color.White);
            #endregion


            #endregion bend

            #region Notching
            LinearPath BottomLeftBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend1),-(StandardBend2+StandardBend1)),
            });

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
                 new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2))-2*SheetThickness,W - 2 * SheetThickness),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2+StandardBend1)-2*SheetThickness),(W - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(BottomRightTopNotching, Color.White);
            LinearPath BottomRightTopNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D(((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D((((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness))-StandardBend1,(W - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

            });
            drawing.Entities.Add(BottomRightTopNotching1, Color.White);
            //For Left Top Side
            LinearPath TopleftNotching = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2),W - 2 * SheetThickness),
                new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2+StandardBend1),(W - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(TopleftNotching, Color.White);
            LinearPath TopleftNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)))+StandardBend1,(W - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

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
            //  CreateSlots(model);

            double divisionresult = Panelheight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            // calculate the remaining space after creating whole slots
            double multipliedresult = (wholenumberpart) * PitchDistance;
            double samespacedivide = Panelheight - multipliedresult;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart; i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else

                    centerz = samespacedivide / 2 + (i + 0) * PitchDistance;
                // create a slot
                devregion slot = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + (StandardBend2 / 2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);

                devregion slot1 = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2) + (PanelWidth - StandardBend2 / 2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);
                slot.Translate(0, 0, samespacedivide / 2);
                slot.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot, Color.White);
                drawing.Entities.Add(slot1, Color.White);
            }

            //create slots on width(Horizontal slots)
            double newPanelWidth = PanelWidth - 2 * SheetThickness;

            double divisionresult1 = newPanelWidth / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            // calculate the remaining space after creating whole slots
            double multipliedresult1 = wholenumberpart1 * PitchDistance;
            double samespacedivide1 = newPanelWidth - multipliedresult1;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centery;
                if (i == 0)
                    centery = samespacedivide1 / 2;
                else
                    //centery = /*(PanelWidth) - (((i + 0) * PitchDistance) - (samespacedivide1 / 2));*/
                    centery = samespacedivide1 / 2 + (i) * PitchDistance;

                // create a slot
                //devregion slot2 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (-StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                devregion slot2 = devregion.CreateSlot(Plane.XY, ((newPanelWidth) + ((newPanelWidth / 2 + newPanelWidth / 2) * (0.2)) + centery), (StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                devregion slot3 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (W - StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                slot2.Translate(0, 0, samespacedivide1 / 2);
                slot2.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot2, Color.White);
                drawing.Entities.Add(slot3, Color.White);
            }

            #region Dimension for  PitchDistance slots
            double centerz1 = samespacedivide1 / 2 + (2) * PitchDistance;
            double centerz2 = samespacedivide1 / 2 + (3) * PitchDistance;
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1)) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2))) / 2;
            LinearDim slotsDim = new LinearDim(Plane.XY,
              new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1), (+StandardBend2 / 2)),
               new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2), (+StandardBend2 / 2)),
              new Point3D(x, (+StandardBend2 / 2) + 50), 20);

            drawing.Entities.Add(slotsDim, Dim);
            #endregion
            #endregion
            #region ScaleFactor                    
            double xd = PanelWidth;
            double yd = PanelHeight;

            double scaleX = xd / (trimmedWidth - titleBoxWidth);
            double scaleY = yd / (trimmedHeight - titleBoxHeight);
            double scaleFactor = Math.Max(scaleX, scaleY);
            int roundScalefactor = (int)Math.Ceiling(scaleFactor);
            StandardFrame(roundScalefactor, drawing);
            #endregion
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/";

            if (!Directory.Exists(path + "/Development"))
                Directory.CreateDirectory(path + "/Development");

            var dwgFilePathfordevelopment = Path.Combine(path, "Development", "Top Panel Development" + DateTime.Now.ToString("hh-mm") + ".dwg");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();

            #endregion

            #endregion development
            return new PaintBoothclass
            {
                //developmentdrawing = developmentdrawing,
                developmentpath = dwgFilePathfordevelopment,
            };
        }
        #endregion
        #region 3D Back panels and development Drawings
        public PaintBoothclass BackPanels(PaintBoothModel model)
        {
            //double BackPanel_Weight = 0;
            PanelInputModel pmodel = new PanelInputModel();
            if (model.PanelTypes == "1")
                model.D = model.StandardPanelWidthForD;
            else
                model.D = model.D;
            int k = 0;
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(model.D+D3,0,0),
                new Point3D(model.D+D3,BackPanelLength,0),
                new Point3D(model.D+D3,BackPanelLength,PanelHeight),
                new Point3D(model.D+D3,0,PanelHeight)
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
                new Point3D(model.D+D3,0,0),
                new Point3D(model.D+D3,BackPanelLength,0),
                new Point3D(model.D+D3,BackPanelLength,PanelHeight),
                new Point3D(model.D+D3,0,PanelHeight),
                new Point3D(model.D+D3,0,0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(model.D+D3, 0, 0),
                new Point3D(((model.D+D3)+36.8),0,0),
                new Point3D(((model.D+D3)+36.8),0,1.2),
                new Point3D(((model.D+D3)+SheetThickness),0,SheetThickness),
                new Point3D(((model.D+D3)+SheetThickness),0,36.8),
                new Point3D(model.D+D3,0,36.8)
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);

            frame = GenerateHoles(frame);

            //Calculate weight of panels
            Material mat1 = Material.StructuralSteel;
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
            SavePanelDetails(model, pmodel, "BackPanels", k, BackPanel_Weight);

            return new PaintBoothclass
            {
                drawing = drawing,
                lstpath = dwgFilePath,

            };
        }
        public PaintBoothclass DevelopmentforBackPanels(PaintBoothModel model, int j)
        {
            drawing = new DesignDocument();
            //PanelWidth = model.StandardPanelWidthForD;
            //PanelHeight = model.PanelHeightforH;
            SettingStandardBend1 = (double)model.standardbend1;
            SettingStandardBend2 = (double)model.standardbend2;
            SheetThickness = (double)model.SheetThickness;
            PitchDistance = (double)model.PitchDistance;
            SlotDimention = model.SlotDimention;
            #region Development
            double StandardBend1 = SettingStandardBend1 - SheetThickness;
            double StandardBend2 = SettingStandardBend2 - (SheetThickness * 2);

            const string Dim = "Dimension";
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);
            #region inner Rectangle Dimentions
            #region Inner Rectangle Bottom
            //Add InnerRectangle for Left Bottom
            LinearPath Innerrectangleleft = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2))+50, 0)
            });

            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.Yellow;
            drawing.Layers.Add(mylayer);
            Innerrectangleleft.LayerName = "bendlayer";
            drawing.Entities.Add(Innerrectangleleft);
            //for innerRectangle Right Bottom
            LinearPath InnerrectangleRightBottom = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), 0),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness))-50, 0),
            });
            InnerrectangleRightBottom.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightBottom);
            #endregion
            #region Inner Rectangle Top
            //Add InnerRectangle for Left Top
            LinearPath InnerrectangleleftTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), W - 2 * SheetThickness),
                  new Point3D((PanelWidth + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)+50), ( W - 2 * SheetThickness))
            });
            InnerrectangleleftTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftTop);
            //for innerRectangle Right Top
            LinearPath InnerrectangleRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness), W - 2 * SheetThickness),
                new Point3D((((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 * SheetThickness))-50,  W - 2 * SheetThickness),
            });
            InnerrectangleRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightTop);
            #endregion
            #region Inner Rectangle Left
            //left bottom
            LinearPath InnerrectangleleftSide = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)), 50)
            });
            InnerrectangleleftSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSide);
            //legt Top
            LinearPath InnerrectangleleftSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), (W - 2 * SheetThickness)),
                  new Point3D((PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)),((W - 2 * SheetThickness)-50) )
            });
            InnerrectangleleftSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleleftSideTop);
            #endregion
            #region Inner Rectangle Right
            //left bottom
            LinearPath InnerrectangleRightSide = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 +PanelWidth / 2) * 0.2)-(2*SheetThickness)), 0),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), 50),
            });
            InnerrectangleRightSide.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSide);
            //legt Top
            LinearPath InnerrectangleRightSideTop = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)), (W - 2 * SheetThickness)),
                  new Point3D((PanelWidth*2 + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)-(2*SheetThickness)),((W - 2 * SheetThickness)-50) )
            });
            InnerrectangleRightSideTop.LayerName = "bendlayer";
            drawing.Entities.Add(InnerrectangleRightSideTop);
            #endregion
            #endregion
            #region bend
            #region RightSideBendLine
            //for bendline right side
            LinearPath bendlineright = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,0),
                  new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,50),
            });
            bendlineright.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright, Color.Yellow);
            LinearPath bendlineright1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(W - 2 * SheetThickness)),
                 new Point3D((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2)+(StandardBend2))-2*SheetThickness,(W - 2 *SheetThickness)-50),

           });
            bendlineright1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineright1);
            #endregion
            #region LeftSideBendLine
            //for bendline right side
            LinearPath bendlineLeft = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),0),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),50),
            });
            bendlineLeft.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft);
            LinearPath bendlineLeft1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(W - 2 * SheetThickness)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)-(StandardBend2)),(W - 2 * SheetThickness)-50)
            });
            bendlineLeft1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineLeft1);
            #endregion
            #region BottomBend Line
            //for bendline Bottom side
            LinearPath bendlineBottom = new LinearPath(new Point3D[]
             {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                  new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),-(StandardBend2)),
            });
            bendlineBottom.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom);
            LinearPath bendlineBottom1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(W - 2 * SheetThickness)+(StandardBend2)),
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)+50),(W - 2 * SheetThickness)+StandardBend2)

           });
            bendlineBottom1.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottom1);

            #region Dimensions
            double x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50))) / 2;
            LinearDim bendlineBottom1Dim = new LinearDim(Plane.XY,
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), (W - 2 * SheetThickness) + (StandardBend2)),
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) + 50), (W - 2 * SheetThickness) + StandardBend2),
                 new Point3D(x, ((W - 2 * SheetThickness) + StandardBend2) + 50), 20);
            drawing.Entities.Add(bendlineBottom1Dim, Dim);
            #endregion
            #endregion
            #region BottomBend Right Line
            //for bendline Bottom side
            LinearPath bendlineBottomRight = new LinearPath(new Point3D[]
             {
                  new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),-(StandardBend2)),
              new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,-(StandardBend2)),
            });
            bendlineBottomRight.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRight);

            LinearPath bendlineBottomRightTop = new LinearPath(new Point3D[]
            {
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness)-50,(W - 2 * SheetThickness)+StandardBend2)

            });
            bendlineBottomRightTop.LayerName = "bendlayer";
            drawing.Entities.Add(bendlineBottomRightTop);
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
                  new Point3D(((PanelWidth*2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - (2 *SheetThickness),0),
            });
            drawing.Entities.Add(linearBottom, Color.White);

            #region Outer Dimmention

            #region Outer 1
            x = ((0) + (-(StandardBend2))) / 2;
            LinearDim linearBottomdim = new(verticalPlane,
               new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2), 0),
                  new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(PanelWidth + ((PanelWidth / 2 + PanelWidth / 2) * 0.2) - 40, x), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion

            #region Outer 2
            x = (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2))) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness))) / 2;
            linearBottomdim = new(Plane.XY,
                new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                  new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1), -(StandardBend2 + StandardBend1)),
                 new Point3D(x - 10, -(StandardBend2 + StandardBend1) - 40), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #region Outer 3
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) + (StandardBend1 - SheetThickness)) + (((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * 0.2)) - ((-SheetThickness) + StandardBend1))) / 2;
            linearBottomdim = new(Plane.XY,
                 new Point3D((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)), -(StandardBend2)),
                 new Point3D(((PanelWidth * 2) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) - (2 * SheetThickness)), -(StandardBend2)),
                 new Point3D(x, -(StandardBend2 + StandardBend1) - 20), 20)
            { ArrowheadSize = 10 };
            drawing.Entities.Add(linearBottomdim, Dim);
            #endregion
            #endregion
            //For Top
            double Panelheight = W - 2 * SheetThickness;
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
                new Point3D(((PanelWidth*2+ ((PanelWidth / 2 +PanelWidth / 2) * 0.2))-2*SheetThickness),Panelheight),
            });
            drawing.Entities.Add(linearRight, Color.White);
            #endregion
            #endregion bend

            #region Notching
            LinearPath BottomLeftBottmNotching = new LinearPath(new Point3D[]
            {
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),-(StandardBend2)),
                new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend1-SheetThickness),-(StandardBend2+StandardBend1)),
            });

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
                 new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2))-2*SheetThickness,W - 2 * SheetThickness),
                new Point3D(((PanelWidth * 2)+((PanelWidth/2+PanelWidth/2)*(0.2))+(StandardBend2+StandardBend1)-2*SheetThickness),(W - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(BottomRightTopNotching, Color.White);
            LinearPath BottomRightTopNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D(((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D((((PanelWidth*2)+((PanelWidth/2+PanelWidth/2)*(0.2))-2*SheetThickness))-StandardBend1,(W - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

            });
            drawing.Entities.Add(BottomRightTopNotching1, Color.White);
            //For Left Top Side
            LinearPath TopleftNotching = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2),W - 2 * SheetThickness),
                new Point3D((PanelWidth) +((PanelWidth / 2 + PanelWidth / 2) *(0.2))-(StandardBend2+StandardBend1),(W - 2 * SheetThickness)-StandardBend1),
            });

            drawing.Entities.Add(TopleftNotching, Color.White);
            LinearPath TopleftNotching1 = new LinearPath(new Point3D[]
            {
                 new Point3D((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)),(W - 2 * SheetThickness)+(StandardBend2)),
                new Point3D(((PanelWidth)+((PanelWidth/2+PanelWidth/2)*(0.2)))+StandardBend1,(W - 2 * SheetThickness)+(StandardBend2+StandardBend1)),

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
            //  CreateSlots(model);
            //creating holes on xy direction 
            double divisionresult = Panelheight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            // calculate the remaining space after creating whole slots
            double multipliedresult = (wholenumberpart) * PitchDistance;
            double samespacedivide = Panelheight - multipliedresult;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart; i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else

                    centerz = samespacedivide / 2 + (i) * PitchDistance;
                // create a slot
                devregion slot = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);

                devregion slot1 = devregion.CreateSlot(Plane.XY, (PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2) - (StandardBend2 / 2) + (PanelWidth + StandardBend2)), centerz, (slotLength - slotWidth), slotWidth / 2, 1.5708);
                slot.Translate(0, 0, samespacedivide / 2);
                slot.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot, Color.White);
                drawing.Entities.Add(slot1, Color.White);
            }

            //create slots on width
            double newPanelWidth = PanelWidth - 2 * SheetThickness;
            double divisionresult1 = newPanelWidth / PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            // calculate the remaining space after creating whole slots
            double multipliedresult1 = wholenumberpart1 * PitchDistance;
            double samespacedivide1 = newPanelWidth - multipliedresult1;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centery;
                if (i == 0)
                    centery = samespacedivide1 / 2;
                else
                    centery = samespacedivide1 / 2 + (i) * PitchDistance;
                // create a slot
                devregion slot2 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (-StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);

                devregion slot3 = devregion.CreateSlot(Plane.XY, ((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centery), (W + StandardBend2 / 2), (slotLength - slotWidth), slotWidth / 2, 0);
                slot2.Translate(0, 0, samespacedivide1 / 2);
                slot2.Color = Color.Yellow;
                // add the slot to the drawing
                drawing.Entities.Add(slot2, Color.White);
                drawing.Entities.Add(slot3, Color.White);
            }
            #region Dimension
            double centerz1 = samespacedivide1 / 2 + (2) * PitchDistance;
            double centerz2 = samespacedivide1 / 2 + (3) * PitchDistance;
            x = ((((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1)) + (((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2))) / 2;
            LinearDim slotsDim = new LinearDim(Plane.XY,
              new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz1), (-StandardBend2 / 2)),
               new Point3D(((PanelWidth) + ((PanelWidth / 2 + PanelWidth / 2) * (0.2)) + centerz2), (-StandardBend2 / 2)),
              new Point3D(x, (-StandardBend2 / 2) + 50), 20);

            drawing.Entities.Add(slotsDim, Dim);
            #endregion

            #endregion
            #region ScaleFactor                    
            double xd = PanelWidth;
            double yd = PanelHeight;

            double scaleX = xd / (trimmedWidth - titleBoxWidth);
            double scaleY = yd / (trimmedHeight - titleBoxHeight);
            double scaleFactor = Math.Max(scaleX, scaleY);
            int roundScalefactorforRare = (int)Math.Ceiling(scaleFactor);
            StandardFrame(roundScalefactorforRare, drawing);
            #endregion
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/";
            if (!Directory.Exists(path + "/Development"))
                Directory.CreateDirectory(path + "/Development");
            var dwgFilePathfordevelopment = Path.Combine(path, "Development", "Rear Panel Development" + DateTime.Now.ToString("hh-mm") + ".dwg");
            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, dwgFilePathfordevelopment);
            dwgg1.DoWork();
            #endregion
            #endregion development
            return new PaintBoothclass
            {
                developmentpath = dwgFilePathfordevelopment,
            };
        }
        #endregion
        public PaintBoothclass TopStructureFrame(PanelInputModel pmodel, PaintBoothModel model)
        {
            if (model.PanelTypesforH == "1")
                PanelHeight = model.PanelHeightforH;
            else
                PanelHeight = model.H;

            drawing = new();
            int k = 0;
            drawing.Units = linearUnitsType.Millimeters;

            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,-SettingStandardBend2,PanelHeight),
                new Point3D(model.D+D3,-SettingStandardBend2,PanelHeight),
                new Point3D(model.D+D3,(model.W-SettingStandardBend2),PanelHeight),
                new Point3D(0,(model.W-SettingStandardBend2),PanelHeight),
            });
            LinearPath rail = new LinearPath(new Point3D[]
            {
               new Point3D(0,0,PanelHeight+75),
                new Point3D(model.D+D3,0,PanelHeight+75),
                new Point3D(model.D + D3,model.W,PanelHeight+75),
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
            frame.Regen(0.1);
            double massofFrame = frame.GetMass(mt, linearUnitsType.Millimeters, massUnitsType.Kilograms, out double convertedDensity1);
            double Frame_Weight = Math.Round(massofFrame, 3);
            TopStructureFrame_Weight = Frame_Weight;
            SavePanelDetails(model, pmodel, "TopStructureFrame", k, TopStructureFrame_Weight);

            GenerateHoles(frame);
            drawing.Entities.Add(frame, Color.Yellow);

            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/" + pmodel.ProjectID;

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
        public PaintBoothclass BaseStructure(PanelInputModel pmodel, PaintBoothModel model)
        {
            drawing = new();
            drawing.Units = linearUnitsType.Millimeters;
            int k = 0;
            if (model.MakeItEqualByW)
            {
                W = W = model.EqualPanelWidthByW;

            }
            else
            {
                W = model.W;
            }

            var rectangle = devregion.CreatePolygon(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(model.D+D3,0,0),
                new Point3D(model.D + D3,model.W,0),
                new Point3D(0,model.W,0),
            });
            // drawing.Entities.Add(rectangle, Color.Pink);
            #region Frame Calculations
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0,0),
                new Point3D(model.D + D3,0,0),
                new Point3D(model.D+D3,model.W,0),
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
            SavePanelDetails(model, pmodel, "BaseStructureFrame", k, BaseFrame_Weight);

            GenerateHoles(frame);
            drawing.Entities.Add(frame, Color.Yellow);
            #region WriteFile
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            path += "/" + pmodel.ProjectID;

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
        private devDept.Eyeshot.Entities.Region CreatePolygon(int k)
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

        public List<PaintBoothclass> OuterFilterFrame(PaintBoothModel model)
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

            double offsetX = model.D; // Starting at depth model.D
            double equalFilterSpace = (model.W - (noOfFramesW * frameWidth)) / 2;
            int BaffleHeight = 1000;
            int baffleHeight1 = 1200;
            int paintBoothHeight = (int)model.H;
            //int totalBafflesForBaffleHeight = paintBoothHeight / BaffleHeight;
            //int totalBafflesForBaffleHeight1 = paintBoothHeight / baffleHeight1;

            // Calculate the remaining heights for comparison
            int remainingHeight1 = paintBoothHeight % BaffleHeight;
            int remainingHeight2 = paintBoothHeight % baffleHeight1;

            // Compare remaining heights and select the best baffle height
            if (remainingHeight1 < remainingHeight2)
            {
                selectedBaffleHeight = BaffleHeight;
                //totalBafflesinHeight = totalBafflesForBaffleHeight;
            }
            else
            {
                selectedBaffleHeight = baffleHeight1;
                // totalBafflesinHeight = totalBafflesForBaffleHeight1;
            }
            model.FilterHeight = selectedBaffleHeight;
            int totalBafflesinHeight = paintBoothHeight / selectedBaffleHeight;
            SaveFilterDetails(model);

            #endregion
            //// Convert FilterArea from m² to mm²
            //double filterAreaInMm2 = model.FilterArea * 1000000; // 1 m² = 1,000,000 mm²
            //// Calculate FilterFrameHeight in mm
            //double FilterFrameHeight = filterAreaInMm2 / model.W;
            //double noOfFrameForheight = Math.Floor(FilterFrameHeight / frameHeight);

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
                    PaintBoothclass metalBaffleInstance = MetalBaffle(selectedBaffleHeight);

                    foreach (Entity entity in metalBaffleInstance.drawing.Entities)
                    {
                        Entity clonedEntity = (Entity)entity.Clone();

                        // Rotate and translate to position each metal baffle
                        clonedEntity.Rotate(Math.PI / 2, Vector3D.AxisZ, new Point3D(0, 0, 0));
                        clonedEntity.Translate(posX, equalFilterSpace, 0);

                        drawing.Entities.Add(clonedEntity, entity.Color);
                    }
                    // Call the FilterFrame1 method to generate the current frame
                    PaintBoothclass filterFrameInstance = FilterFrame1(selectedBaffleHeight, model);

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

            return metalBaffleDrawings;
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
            _DbContext.FilterFrameDetails.Add(obj);
            _DbContext.SaveChanges();
            return 1;
        }
        public int SavePanelDetails(PaintBoothModel model, PanelInputModel pmmodel, string panelPosition, int k, double PanelWeight)
        {
            PanelDetails tblobj = new PanelDetails();
            tblobj.IsDeleted = false;
            tblobj.EnquiryId = EnquiryID;
            tblobj.SalesNo = model.SalesNO;
            tblobj.SlotDimention = SlotDimention;
            tblobj.PanelPosition = panelPosition;

            tblobj.EqualPanelDepth = 0;
            tblobj.EqualPanelHeight = 0;
            tblobj.EqualPanelWidth = 0;
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

                if (model.PanelTypes != "1")
                    tblobj.EqualPanelDepth = model.RemainingPanels;

                if (model.PanelTypesforW != "1")
                    tblobj.EqualPanelWidth = model.RemainingPanelsByW;

                if (model.PanelTypesforH != "1")
                    tblobj.EqualPanelHeight = model.RemainingPanelsByH;
            }
            if (panelPosition == "RightSide")
            {
                totalPanelsforD = model.D / PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                tblobj.NoOfPanels = noOfPanelsforD;
                tblobj.PanelWeight = PanelWeight;

            }

            if (panelPosition == "LeftSide")
            {
                totalPanelsforD = model.D / PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
                tblobj.PanelWeight = PanelWeight;
            }
            if (panelPosition == "BackPanels")
            {
                totalPanelsforD = model.D / PanelWidth;
                noOfPanelsforD = (int)Math.Floor(totalPanelsforD);
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
                if (model.PanelTypes == "1" || model.PanelTypesforW == "1" || model.PanelTypesforH == "1")
                {
                    tblobj.EqualPanelDepth = 0;
                    tblobj.EqualPanelHeight = 0;
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
                if (model.PanelTypes == "1" || model.PanelTypesforW == "1" || model.PanelTypesforH == "1")
                {
                    tblobj.EqualPanelDepth = 0;
                    tblobj.EqualPanelHeight = 0;
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

            }
            //tblobj.PanelWeight = PanelWeight;
            if (model.MakeItEqualByH == true)
            {
                tblobj.EqualPanelHeight = model.EqualPanelWidthByH;
                tblobj.RemainingPanelHeight = model.RemainingPanelsByH;

            }
            else
            {
                tblobj.EqualPanelHeight = 0;
                tblobj.RemainingPanelHeight = 0;
            }
            if (model.MakeItEqual == true)
            {
                tblobj.EqualPanelDepth = model.EqualPanelWidthForD;
                tblobj.RemainingPanelDepth = model.RemainingPanels;

            }
            else
            {
                tblobj.EqualPanelDepth = 0;
                tblobj.RemainingPanelDepth = 0;
            }

            if (model.MakeItEqualByW == true)
            {
                tblobj.EqualPanelHeight = model.EqualPanelWidthByW;
                tblobj.RemainingPanelHeight = model.RemainingPanelsByH;

            }
            else
            {
                tblobj.EqualPanelHeight = 0;
                tblobj.RemainingPanelHeight = 0;
            }
            if (model.PanelTypes == "1")
                tblobj.NoOfPanels = noOfPanelsforD;
            else
            {
                tblobj.NoOfPanels = noOfPanelsforD + 1;
            }
            tblobj.StandardBend1 = (decimal)SettingStandardBend1;
            tblobj.StandardBend2 = (decimal)SettingStandardBend2;
            tblobj.SheetThickness = (decimal)SheetThickness;
            tblobj.PitchDistance = (decimal)PitchDistance;
            tblobj.CostingStatus = true;
            //  tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.ModifiedBy = 0;
            _DbContext.PanelDetails.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }
        public PaintBoothclass detailsdrawing(DesignDocument model, PaintBoothModel pmodel)
        {
            string EnquiryCode = pmodel.SalesNO;
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            string partname = "CombineAssembly";
            model.Units = linearUnitsType.Millimeters;
            AddSheet asfp = new AddSheet();
            string drivetype = ""; // = objdrivetype;
            drawingdoc = asfp.AddSheets(partname, drivetype);
            #region INSERTION OF VIEWS
            // Calculate scaling value based on box size
            Point3D bx = model.Entities.BoxSize;
            Point3D boxsize = new Point3D(bx.X, bx.Z, bx.Y);
            double sv = 0;
            if (boxsize.X <= 100 || boxsize.Y <= 100)
                sv = 20;
            else
            {
                if (boxsize.X >= boxsize.Y)
                    sv = boxsize.X / 28;
                else
                    sv = boxsize.Y / 28;
            }

            #endregion
            #region SCALING SHEET          
            double Xd = pmodel.W + pmodel.D + (pmodel.D / 2) + pmodel.D3;
            //double yd = pmodel.D + pmodel.W + ((pmodel.W / 2) + pmodel.W);
            double yd = pmodel.H + pmodel.W + (pmodel.H / 2);
            double scaleX = Xd / (trimmedWidth - titleBoxWidth);
            double scaleY = yd / (trimmedHeight - titleBoxHeight);
            double scaleFactor = Math.Max(scaleX, scaleY);
            double finalScaleFactor = (int)Math.Ceiling(scaleFactor);

            #endregion
            #region addviews
            // Adds some views
            panels = new int[model.Entities.Count];
            Panelinputs = new int[model.Entities.Count];
            stiff = new int[model.Entities.Count];
            for (int a = 0; a < model.Entities.Count; a++)
            {
                panels[a] = a;
                Panelinputs[a] = a;
                stiff[a] = a;
            }
            int numtoremove = 1;
            int numtoremove1 = 2;
            panels = panels.Where(val => val != numtoremove).ToArray();
            Panelinputs = Panelinputs.Where(val => val != numtoremove1).ToArray();
            int stf2 = 3;
            stiff = stiff.Where(val => val != stf2).ToArray();
            MySheet mysheet;
            pmodel.standardbend2 = (decimal)SettingStandardBend2;
            var panelDetails = GetAllPanels(EnquiryCode);
            mysheet = asfp.StandardFrame((int)finalScaleFactor, (MySheet)drawingdoc.ActiveSheet, new Point2D(0, 0), pmodel, panelDetails);
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
            Brep leftHoleBrep = leftHole.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);
            Brep rightHoleBrep = rightHole.ExtrudeAsBrep(Vector3D.AxisZ * 5, 0);

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
            drawing.Entities.Add(leftHoleClone, Color.Green);
            drawing.Entities.Add(rightHoleClone, Color.Green);

            drawing.Entities.Add(bottomPlateBrep2, Color.Green);

            #region Write file         
            var path = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();

            if (!Directory.Exists(path + "/Bullows Panel Drawing"))
                Directory.CreateDirectory(path + "/Bullows Panel Drawing");

            string dwgFilePath = path + "/Bullows Panel Drawing/" + "CSection" + DateTime.Now.ToString("hh-mm") + ".dwg";

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
        public PaintBoothclass FilterFrame1(double filterHeight, PaintBoothModel model)
        {
            drawing = new DesignDocument();
            drawing.Units = linearUnitsType.Millimeters;
            double frameWidth = 600;
            double frameHeight = filterHeight;
            double railWidth = 30;
            double railHeight = 40;
            double sheetThickness = 1.2;
            double holeRadius = 7 / 2;

            // Frame Outline
            //rail on YZ plain
            LinearPath rail = new LinearPath(new Point3D[]
            {
                new Point3D(0,0, 0),
                new Point3D(0,frameWidth, 0),
                new Point3D(0,frameWidth,frameHeight),
                new Point3D(0,0, frameHeight),
                new Point3D(0, 0,0)
            });
            drawing.Entities.Add(rail);
            #region frame
            // Frame Section for Extrusion
            //section on XZ plane
            var section = devregion.CreatePolygon(new Point3D[]
            {
                  new Point3D(0,0,0),
                  new Point3D(railWidth,0,0),
                  new Point3D(railWidth,0,sheetThickness),
                  new Point3D(sheetThickness,0, sheetThickness),
                  new Point3D(sheetThickness,0,railHeight),
                  new Point3D(0,0,railHeight),
            });
            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, sheetThickness);
            devregion centercircle = devregion.CreateCircle(Plane.XY, new Point3D(frameWidth / 2, railHeight / 2), holeRadius);
            frame.ExtrudeRemove(centercircle, 50, 0);
            devregion leftCircle = devregion.CreateCircle(Plane.XY, new Point3D((frameWidth / 2) - 285, railHeight / 2), holeRadius);
            frame.ExtrudeRemove(leftCircle, 50, 0);
            devregion rightCircle = devregion.CreateCircle(Plane.XY, new Point3D((frameWidth / 2) + 285, railHeight / 2), holeRadius);
            frame.ExtrudeRemove(rightCircle, 50, 0);

            // Cloning and positioning holes for the top side
            var topCenterCircle = (devregion)centercircle.Clone();
            topCenterCircle.Translate(0, frameHeight - railHeight, 0);
            frame.ExtrudeRemove(topCenterCircle, 50, 0);

            var topLeftCircle = (devregion)leftCircle.Clone();
            topLeftCircle.Translate(0, frameHeight - railHeight, 0);
            frame.ExtrudeRemove(topLeftCircle, 50, 0);

            var topRightCircle = (devregion)rightCircle.Clone();
            topRightCircle.Translate(0, frameHeight - railHeight, 0);
            frame.ExtrudeRemove(topRightCircle, 50, 0);

            // New holes on the left side, equally spaced from top to bottom
            double leftSideHoleSpacing = frameHeight / 3; // Dividing frame height by 3 for equal spacing

            devregion leftSideTopHole = devregion.CreateCircle(Plane.XY, new Point3D(railWidth / 2, frameHeight - leftSideHoleSpacing), holeRadius);
            frame.ExtrudeRemove(leftSideTopHole, 50, 0);

            devregion leftSideBottomHole = devregion.CreateCircle(Plane.XY, new Point3D(railWidth / 2, leftSideHoleSpacing), holeRadius);
            frame.ExtrudeRemove(leftSideBottomHole, 50, 0);
            // Clone the left side holes for the right side
            var rightSideTopHole = (devregion)leftSideTopHole.Clone();
            rightSideTopHole.Translate(frameWidth - railWidth, 0, 0); // Move to the right side
            frame.ExtrudeRemove(rightSideTopHole, 50, 0);

            var rightSideBottomHole = (devregion)leftSideBottomHole.Clone();
            rightSideBottomHole.Translate(frameWidth - railWidth, 0, 0); // Move to the right side
            frame.ExtrudeRemove(rightSideBottomHole, 50, 0);

            drawing.Entities.Add(frame, Color.Blue);
            #endregion
            #region Write file         
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
        public PaintBoothclass CreateLoft(PaintBoothModel model)
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

            // Create the loft base 
            LinearPath rectangle = new LinearPath(p2, new Point3D[]
            {
                new Point3D(model.D, 0, model.H),
                new Point3D(model.D + model.D3, 0, model.H),
                new Point3D(model.D + model.D3, W, model.H),
                new Point3D(model.D, W, model.H),
                new Point3D(model.D, 0, model.H)
            });

            drawing.Entities.Add(rectangle, Color.Pink);

            LinearPath rectangle1 = new LinearPath(new Point3D[]// top rectangles
            {
                new Point3D(model.D, (W - D3) / 2, model.H + 300),
                new Point3D((model.D + model.D3), ((W - D3) / 2), model.H + 300),
                new Point3D((model.D + model.D3), ((W - D3) / 2) + D3, model.H + 300),
                new Point3D(model.D, ((W - D3) / 2) + D3, model.H + 300),
                new Point3D(model.D, (W - D3) / 2, model.H + 300)
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
            double stepPositionX = model.D + (model.D3 / 2);
            double stepPositionY = W / 2;
            double stepPositionZ = model.H + 300 + 300;//centrifugal fan height
            double angle = 0;
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
                    angle = Math.PI/2;
                else if (model.BlowerOrientation == "180")
                    angle = Math.PI;
                else if (model.BlowerOrientation == "270")
                    angle = (Math.PI *3)/ 2;

                stepEntity.Rotate(Math.PI/2, Vector3D.AxisX, centerPoint);
                stepEntity.Rotate(angle, Vector3D.AxisZ, centerPoint);
                drawing.Entities.Add(stepEntity);
            }

            #region BendRevolve
            
            LinearPath l1 = new LinearPath(Plane.XY, new Point3D[]
                {
                    new Point3D(400, 0, stepPositionZ+400),
                    new Point3D(400+250, 0, stepPositionZ+400),
                    new Point3D(400+250, 200, stepPositionZ+400),
                    new Point3D(400,200, stepPositionZ+400),
                    new Point3D(400,0, stepPositionZ+400)
                });

            LinearPath l2 = new LinearPath(Plane.XY, new Point3D[]
            {
                    new Point3D(410, 10, stepPositionZ+400),
                    new Point3D(400+240, 10, stepPositionZ+400),
                    new Point3D(400+240, 190, stepPositionZ+400),
                    new Point3D(410, 190, stepPositionZ+400),
                    new Point3D(410, 10, stepPositionZ+400)
            });
            
            devDept.Eyeshot.Entities.Region region2 = new devDept.Eyeshot.Entities.Region(l1);
            devDept.Eyeshot.Entities.Region region3 = new devDept.Eyeshot.Entities.Region(l2);
            devDept.Eyeshot.Entities.Region region = devDept.Eyeshot.Entities.Region.Difference(region2, region3)[0];
            Brep brep1 = region.RevolveAsBrep(Utility.DegToRad(90), Vector3D.AxisY, new Point3D(0, 200, 0));
            brep1.Translate(stepPositionX+300, stepPositionY-365, stepPositionZ + 400); // Match blower position         
            drawing.Entities.Add(brep1, Color.LightGray);

            Brep ductbrep = region.ExtrudeAsBrep(300);
            ductbrep.Translate(stepPositionX + 300, stepPositionY - 365, stepPositionZ + 400);
            drawing.Entities.Add(ductbrep,Color.Green);

            #endregion

            #region CSection Support
            var CSection = devregion.CreatePolygon(new Point3D[]
            {
               new Point3D(0,0),
               new Point3D(5,0),
               new Point3D(5,(50-5)),
               new Point3D(50,(50-5)),
               new Point3D(50,50),
               new Point3D(0,50),

            });
            Brep brepCsrction = CSection.ExtrudeAsBrep(stepPositionZ + 400);
            brepCsrction.Translate(stepPositionX + 300, stepPositionY - 300, stepPositionZ + 400);
            drawing.Entities.Add(brepCsrction, Color.White);
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
    }
    public class PaintBoothclass
    {
        public DesignDocument drawing { get; set; }
        public string lstpath { get; set; }
        public string developmentpath { get; set; }

    }
}
