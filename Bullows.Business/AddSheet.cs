
#region
using Bullows.Database;
using Bullows.Model;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using Microsoft.Extensions.Configuration;
using ODA.Publish.PdfPublish;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using devregion = devDept.Eyeshot.Entities.Region;

namespace Bullows.Business
{
    public class AddSheet
    {
        private readonly Dictionary<string, string> _formatBlockNames = new Dictionary<string, string>();

        private double margin = 25;                  // Frame margin
        private double viewSpacing = 80;             // Spacing between views


        public DrawingDocument AddSheets(string name)
        {
            DrawingDocument dr = new DrawingDocument();
            bool sheetExists = dr.Sheets.Any(s => s.Name == name);
            if (sheetExists)
            {
                // Generate a unique name for the new sheet
                name = GenerateUniqueSheetName(name);
            }
            MySheet sheet = new MySheet(new Sheet(linearUnitsType.Millimeters, 1000, 1000, name, angleProjectionType.ThirdAngle), name);
            dr = AddLayers(dr, sheet);
            return dr;
        }
        private string GenerateUniqueSheetName(string baseName)
        {
            // Generate a unique sheet name by appending a timestamp or a counter
            return $"{baseName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }

        public DrawingDocument AddLayers(DrawingDocument drawings1, MySheet sheet)
        {
            drawings1.Sheets.Add(sheet);
            drawings1.ActiveSheet = sheet;
            drawings1.LineTypes.Add("Hidden", new float[] { 5F, -3F, 5F, -2F });
            drawings1.Layers[drawings1.HiddenSilhouettesLayerName].LineTypeName = "Hidden";
            drawings1.Layers[drawings1.HiddenSilhouettesLayerName].Color = System.Drawing.Color.Yellow;
            drawings1.Layers[drawings1.HiddenEdgesLayerName].LineTypeName = "Hidden";
            drawings1.Layers[drawings1.HiddenEdgesLayerName].Color = System.Drawing.Color.Yellow;
            drawings1.Layers[drawings1.HiddenWiresLayerName].LineTypeName = "Hidden";
            drawings1.Layers[drawings1.HiddenWiresLayerName].Color = System.Drawing.Color.White;
            //drawings1.Layers[drawings1.SilhouettesLayerName].LineTypeName = "Hidden";
            //drawings1.Layers[drawings1.SilhouettesLayerName].Color = System.Drawing.Color.White;
            drawings1.Layers[drawings1.WiresLayerName].LineTypeName = "Hidden";
            drawings1.Layers[drawings1.WiresLayerName].Color = System.Drawing.Color.White;
            drawings1.Layers[drawings1.EdgesLayerName].LineTypeName = "Hidden";
            drawings1.Layers[drawings1.EdgesLayerName].Color = System.Drawing.Color.White;
            drawings1.TextStyles["Default"].FontFamilyName = "Arial";

            drawings1.Layers[0].Color = System.Drawing.Color.Green;
            drawings1.Layers[0].LineTypeName = "Default";
            drawings1.Layers["Default"].Color = System.Drawing.Color.White;
            drawings1.Layers["Silhouettes"].Color = System.Drawing.Color.White;
            drawings1.Layers["HiddenSilhouettes"].Color = System.Drawing.Color.Yellow;
            drawings1.Layers[7].Color = System.Drawing.Color.Red;
            const string Dim = "Dim";
            drawings1.Layers.Add(Dim);
            drawings1.Layers[8].LineWeight = 0.2f;
            drawings1.Layers[8].Color = Color.Lime;
            return drawings1;
        }
        //public MySheet AddTopViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        //{
        //    // Adjust placement coordinates to fit within the frame
        //    //TODO devDept 2025: A non-optional viewportSize parameter has been added to the constructor of VectorView with the window parameter.
        //    var top = new VectorView(p.X, p.Y - ((model.H / 2) + model.W), viewType.Top, 1, "Top");


        //    top.HiddenSegments = true;
        //    top.Color = Color.Green;

        //    sheet.Entities.Add(top);
        //    return sheet;
        //}

        //public MySheet AddFrontViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        //{
        //    // Adjust placement coordinates to fit within the frame
        //    //TODO devDept 2025: A non-optional viewportSize parameter has been added to the constructor of VectorView with the window parameter.
        //    var front = new VectorView(p.X, p.Y, viewType.Front, 1, "Front");
        //    front.HiddenSegments = hiddenseg;
        //    sheet.Entities.Add(front);
        //    return sheet;
        //}
        
        //public MySheet AddSideViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        //{

            
        //    var side = new VectorView(p.X - ((model.D / 2) + model.W), p.Y, viewType.Left, 1, "Left");
        //    side.HiddenSegments = hiddenseg;
        //    sheet.Entities.Add(side);
        //    return sheet;
        //}

        public MySheet AddTopViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            // Adjust placement coordinates to fit within the frame
            //TODO devDept 2025: A non-optional viewportSize parameter has been added to the constructor of VectorView with the window parameter.
            var top = new VectorView(p.X, p.Y, viewType.Top, 1, "Top");
            top.HiddenSegments = hiddenseg;
            sheet.Entities.Add(top);
            return sheet;
        }

        public MySheet AddFrontViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            // Adjust placement coordinates to fit within the frame
            //TODO devDept 2025: A non-optional viewportSize parameter has been added to the constructor of VectorView with the window parameter.
            var front = new VectorView(p.X, p.Y, viewType.Front, 1, "Front");
            front.HiddenSegments = hiddenseg;
            sheet.Entities.Add(front);
            return sheet;
        }

        public MySheet AddSideViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            //var side = new VectorView(p.X - ((model.D / 2) + model.H), p.Y, viewType.Left, 1, "Left");
            //TODO devDept 2025: A non-optional viewportSize parameter has been added to the constructor of VectorView with the window parameter.
            var side = new VectorView(p.X, p.Y, viewType.Left, 1, "Left");
            side.HiddenSegments = hiddenseg;
            sheet.Entities.Add(side);
            return sheet;
        }

        public MySheet AddIsometricViews(MySheet sheet, Point2D p, bool hiddenseg)
        {

            var isometric = new VectorView(p.X, p.Y, viewType.Isometric, 0.75, "Isometric");
            isometric.HiddenSegments = hiddenseg;
            isometric.Shaded = true;
            isometric.FillRegions = true;
            sheet.Entities.Add(isometric);
            return sheet;
        }
        public MySheet StandardFrameOld(int scaleFator, MySheet sheet, Point2D p, PaintBoothModel model, List<PanelDetails> panels)
        {
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
                new Point2D(outerX0,outerY1),
            });
            sheet.Entities.Add(outerRectangle);

            var innerRectangle = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(innerX0,innerY0),
                new Point2D(innerX1,innerY0),
                new Point2D(innerX1,innerY1),
                new Point2D(innerX0,innerY1),

            });
            sheet.Entities.Add(innerRectangle);

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
            sheet.Entities.Add(titleBox);
            double textHeight = 5;
            double textH = textHeight * scaleFator;
            sheet.Entities.AddRange(new Entity[]
            {
                  //left
                new Line(titleBoxX,innerY0+(10*scaleFator),titleBoxX+(25*scaleFator),innerY0+(10*scaleFator)),
                new Line(titleBoxX,innerY0+(20 * scaleFator),innerX1,innerY0+(20*scaleFator)),
                new Line(titleBoxX+(130 * scaleFator),titleBoxY,titleBoxX+(130 * scaleFator),innerY0),
                new Line(titleBoxX+(25 * scaleFator),innerY0+(20 * scaleFator),titleBoxX+(25 * scaleFator),innerY0),
                new Text(titleBoxX+(10*scaleFator),titleBoxY-(10*scaleFator),"Bullows Paint Equipment Pvt.Ltd",textH),
                new Text(titleBoxX+((25/2)*scaleFator),innerY0+((20-5)*scaleFator),0,"SCALE",(4*scaleFator),Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+((25+(25/2))*scaleFator),innerY0+((20-5)*scaleFator),0,"TITLE",(3*scaleFator),Text.alignmentType.MiddleCenter),
                //Right
               
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7)*scaleFator,innerX1,innerY0+(20+7)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+(7/2))*scaleFator),0,"APPROVED",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*2)*scaleFator,innerX1,innerY0+(20+7*2)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7+(7/2))*scaleFator),0,"STANDARD",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*3)*scaleFator,innerX1,innerY0+(20+7*3)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*2+(7/2))*scaleFator),0,"CHECKED",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*4)*scaleFator,innerX1,innerY0+(20+7*4)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*3+(7/2))*scaleFator),0,"DRAWN",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Line(titleBoxX+(130 * scaleFator),innerY0+(20+7*5)*scaleFator,innerX1,innerY0+(20+7*5)*scaleFator),
                new Text(titleBoxX+(130+10)*scaleFator,innerY0+((20+7*4+(7/2))*scaleFator),0,"DESIGNED",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+(130+20+12.5)*scaleFator,titleBoxY-(5*scaleFator),0,"NAME",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Text(titleBoxX+(130+20+25+5)*scaleFator,titleBoxY-(5*scaleFator),0,"DATE",(2*scaleFator),Text.alignmentType.MiddleCenter),
                new Line(innerX1-(10*scaleFator),innerY0+(20*scaleFator),innerX1-(10 * scaleFator),titleBoxY),
                new Line(innerX1-((10+25)*scaleFator),innerY0+(20 * scaleFator),innerX1-((10+25)*scaleFator),titleBoxY),
                new Text(titleBoxX+(130+5)*scaleFator,innerY0 + (20 - 5)*scaleFator,0,"DRAWING NO.",(2*scaleFator),Text.alignmentType.MiddleLeft),
            });

            #endregion
            // Create a block for the BOM

            #region BOM old        
            //var BOMBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            //{
            //    new Point2D(titleBoxX,titleBoxY),
            //    new Point2D(titleBoxX,titleBoxY+15),
            //    new Point2D(innerX1,titleBoxY+15),
            //    new Point2D(innerX1,titleBoxY),

            //});
            //sheet.Entities.Add(BOMBox);
            //List<double> XCoordinate = new List<double>()
            //{
            //    (titleBoxX+20+10),
            //    (titleBoxX+20+40+10),
            //    (titleBoxX+20+40+80+10),
            //    (titleBoxX+20+40+80+40+10),
            //    (titleBoxX+20+40+80+40+60+10),
            //    (titleBoxX+20+40+80+60+40+40+10),
            //    (titleBoxX+20+40+80+60+40+40+40+10),
            //};

            //sheet.Entities.AddRange(new Entity[]
            //{
            //      new Text(titleBoxX+10,(titleBoxY+15-(15/2)),0,"SR.NO",3,Text.alignmentType.MiddleCenter),
            //      new Text(XCoordinate[0],(titleBoxY+15-(15/2)),0,"PART NO",3,Text.alignmentType.MiddleCenter),
            //       new Text(XCoordinate[1],(titleBoxY+15-(15/2)),0,"PART NAME",3,Text.alignmentType.MiddleLeft),
            //        new Text(XCoordinate[2],(titleBoxY+15-(15/2)),0,"MATERIAL",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[3],(titleBoxY+15-(15/2)),0,"T.SPECIFICATION",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[4], (titleBoxY+15-(15/2)),0,"QUANTITY",3,Text.alignmentType.MiddleLeft),
            //       new Text(XCoordinate[5], (titleBoxY+15-(15/2)),0,"UMO",3,Text.alignmentType.MiddleCenter),
            //      new Text(XCoordinate[6], (titleBoxY+15-(15/2)),0,"WEIGHT",3,Text.alignmentType.MiddleCenter),
            //});
            //double y = 0;
            //for (int i = 0; i < panels.Count; i++)
            //{
            //    y = titleBoxY + (15 * (i + 2));
            //    Line l = new Line(titleBoxX, titleBoxY + (15 * (i + 2)), innerX1, titleBoxY + (15 * (i + 2)));
            //    sheet.Entities.Add(l);
            //    //Passed value to BOM
            //    sheet.Entities.AddRange(new Entity[]
            //    {
            //    new Text(titleBoxX+10,(y-(15/2)),0,(i+1).ToString(),3,Text.alignmentType.MiddleCenter),
            //    new Text(XCoordinate[0],(y-(15/2)),0,"PART NO",3,Text.alignmentType.MiddleCenter),
            //    new Text(XCoordinate[1],(y-(15/2)),0,panels[i].PanelPosition,3,Text.alignmentType.MiddleLeft),
            //    new Text(XCoordinate[2],(y-(15/2)),0,"MATERIAL",3,Text.alignmentType.MiddleLeft),
            //    new Text(XCoordinate[3],(y-(15/2)),0,"T.SPECIFICATION",3,Text.alignmentType.MiddleLeft),
            //    new Text(XCoordinate[4], (y-(15/2)),0,"QUANTITY",3,Text.alignmentType.MiddleLeft),
            //    new Text(XCoordinate[5], (y-(15/2)),0,"UMO",3,Text.alignmentType.MiddleCenter),
            //    new Text(XCoordinate[6], (y-(15/2)),0,panels[i].PanelWeight.ToString(),3,Text.alignmentType.MiddleCenter),
            //    });

            //}
            //sheet.Entities.AddRange(new Entity[]
            //{    new Line(titleBoxX,titleBoxY,titleBoxX,y ),
            //     new Line(XCoordinate[0] - 10,y,XCoordinate[0]-10,titleBoxY),  //sr.no           
            //     new Line(XCoordinate[1]-10,y,XCoordinate[1]-10,titleBoxY),//part no
            //     new Line(XCoordinate[2] - 10,y,XCoordinate[2] - 10,titleBoxY),//part name
            //     new Line(XCoordinate[3] - 10,y,XCoordinate[3] - 10,titleBoxY),//material
            //     new Line(XCoordinate[4] - 10,y,XCoordinate[4] - 10,titleBoxY),//SPECIFICATION
            //     new Line(XCoordinate[5] - 10,y,XCoordinate[5]-10,titleBoxY),//QUANTITY
            //     new Line(XCoordinate[6] - 10,y,XCoordinate[6] - 10,titleBoxY),
            //});


            #endregion


            sheet = Views(sheet, innerX0, innerX1, titleBoxX, titleBoxY, innerY1, model, scaleFator);
            return sheet;

        }

        public MySheet StandardFrame(int scaleFator, MySheet sheet, Point2D p, PaintBoothModel model, List<PanelDetails> panels)
        {
            double UntrimmedWidth = 625 * scaleFator;
            double UntrimmedHeight = 450 * scaleFator;
            double trimmedWidth = 594 * scaleFator;
            double trimmedHeight = 420 * scaleFator;

            double outerX0 = 0, outerX1 = UntrimmedWidth, outerY0 = 0, outerY1 = UntrimmedHeight;

            double iX0 = (UntrimmedWidth - trimmedWidth) / 2;
            double iY0 = (UntrimmedHeight - trimmedHeight) / 2;

            double innerX0 = iX0, innerX1 = trimmedWidth + iX0, innerY0 = iY0, innerY1 = trimmedHeight + iY0;

            // ---------- Outer border ----------
            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(outerX0, outerY0),
                new Point2D(outerX1, outerY0),
                new Point2D(outerX1, outerY1),
                new Point2D(outerX0, outerY1),
            }));

            // ---------- Inner border ----------
            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(innerX0, innerY0),
                new Point2D(innerX1, innerY0),
                new Point2D(innerX1, innerY1),
                new Point2D(innerX0, innerY1),
            }));

            // ==========================================================
            //  TITLE BOX (bottom-right)
            // ==========================================================


            #region Title box
            double titleBoxWidth = 185 * scaleFator;
            double titleBoxHeight = 65 * scaleFator;
            double titleBoxX = innerX1 - titleBoxWidth;
            double titleBoxY = innerY0 + titleBoxHeight; // top of title box

            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(titleBoxX, titleBoxY),
                new Point2D(innerX1, titleBoxY),
                new Point2D(innerX1, innerY0),
                new Point2D(titleBoxX, innerY0)
            }));

            // Title box internal lines & text (converted to supported constructors)
            double textHeight = 5;
            double textH = textHeight * scaleFator;

            // Lines and left text
            sheet.Entities.Add(new Line(titleBoxX, innerY0 + (10 * scaleFator), titleBoxX + (25 * scaleFator), innerY0 + (10 * scaleFator)));
            sheet.Entities.Add(new Line(titleBoxX, innerY0 + (20 * scaleFator), innerX1, innerY0 + (20 * scaleFator)));
            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), titleBoxY, titleBoxX + (130 * scaleFator), innerY0));
            sheet.Entities.Add(new Line(titleBoxX + (25 * scaleFator), innerY0 + (20 * scaleFator), titleBoxX + (25 * scaleFator), innerY0));

            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (10 * scaleFator), titleBoxY - (10 * scaleFator), 0),
                "Bullows Paint Equipment Pvt.Ltd", textH, Text.alignmentType.TopLeft));

            // SCALE (centered in the little box)
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + ((25.0 / 2.0) * scaleFator), innerY0 + ((20 - 5) * scaleFator), 0),
                "SCALE", 4 * scaleFator, Text.alignmentType.MiddleCenter));

            // TITLE label
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + ((25 + (25.0 / 2.0)) * scaleFator), innerY0 + ((20 - 5) * scaleFator), 0),
                "TITLE", 3 * scaleFator, Text.alignmentType.MiddleCenter));

            // Right side rows & labels
            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), innerY0 + ((20 + 7) * scaleFator), innerX1, innerY0 + ((20 + 7) * scaleFator)));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 10) * scaleFator, innerY0 + ((20 + (7.0 / 2.0)) * scaleFator), 0),
                "APPROVED", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), innerY0 + ((20 + 7 * 2) * scaleFator), innerX1, innerY0 + ((20 + 7 * 2) * scaleFator)));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 10) * scaleFator, innerY0 + ((20 + 7 + (7.0 / 2.0)) * scaleFator), 0),
                "STANDARD", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), innerY0 + ((20 + 7 * 3) * scaleFator), innerX1, innerY0 + ((20 + 7 * 3) * scaleFator)));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 10) * scaleFator, innerY0 + ((20 + 7 * 2 + (7.0 / 2.0)) * scaleFator), 0),
                "CHECKED", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), innerY0 + ((20 + 7 * 4) * scaleFator), innerX1, innerY0 + ((20 + 7 * 4) * scaleFator)));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 10) * scaleFator, innerY0 + ((20 + 7 * 3 + (7.0 / 2.0)) * scaleFator), 0),
                "DRAWN", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            sheet.Entities.Add(new Line(titleBoxX + (130 * scaleFator), innerY0 + ((20 + 7 * 5) * scaleFator), innerX1, innerY0 + ((20 + 7 * 5) * scaleFator)));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 10) * scaleFator, innerY0 + ((20 + 7 * 4 + (7.0 / 2.0)) * scaleFator), 0),
                "DESIGNED", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            // NAME / DATE labels on title box
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 20 + 12.5) * scaleFator, titleBoxY - (5 * scaleFator), 0),
                "NAME", 2 * scaleFator, Text.alignmentType.MiddleCenter));
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 20 + 25 + 5) * scaleFator, titleBoxY - (5 * scaleFator), 0),
                "DATE", 2 * scaleFator, Text.alignmentType.MiddleCenter));

            // vertical divider lines in title box
            sheet.Entities.Add(new Line(innerX1 - (10 * scaleFator), innerY0 + (20 * scaleFator), innerX1 - (10 * scaleFator), titleBoxY));
            sheet.Entities.Add(new Line(innerX1 - ((10 + 25) * scaleFator), innerY0 + (20 * scaleFator), innerX1 - ((10 + 25) * scaleFator), titleBoxY));

            // DRAWING NO label
            sheet.Entities.Add(new Text(new Point3D(titleBoxX + (130 + 5) * scaleFator, innerY0 + (20 - 5) * scaleFator, 0),
                "DRAWING NO.", 2 * scaleFator, Text.alignmentType.MiddleLeft)); 
            #endregion


            // ==========================================================
            // HEIGHT CALCULATIONS FOR STACK ORDER (keep stacking so it matches PDF)
            // ==========================================================
            double bomRowHeight = 16 * scaleFator;      // slightly reduced so content fits
            double bomHeaderHeight = 20 * scaleFator;
            double bomTotalHeight = bomHeaderHeight + (bomRowHeight * 6);

            double notesHeight = 160 * scaleFator;     // reduced to fit inside inner frame
            double tolHeight = 30 * scaleFator;

            // Stack from bottom (title box) upward:
            double titleTopY = titleBoxY;                 // top of title box
            double tolBottomY = titleTopY;                // tolerance bottom touches title top
            double tolTopY = tolBottomY + tolHeight;

            double notesBottomY = tolTopY;                // notes bottom touches tolerance top
            double notesTopY = notesBottomY + notesHeight;

            double bomBottomY = notesTopY;                // BOM bottom touches notes top
            double bomTopY = bomBottomY + bomTotalHeight;


            // ==========================================================
            // BOM BLOCK (F1 – F6) — positioned based on calculated bomTopY/bomBottomY
            // ==========================================================
            //--------------------------------------------------------------
            //  BOM (Bill of Materials) BLOCK
            //--------------------------------------------------------------

            // BOM POSITION BASED ON PREVIOUS CALCULATIONS
            double bomX0 = titleBoxX;
            double bomX1 = innerX1;
            double bomY0 = bomTopY;        // TOP of BOM
            double bomY1 = bomBottomY;     // BOTTOM of BOM

            // HEADER HEIGHT & ROW HEIGHT
             //bomHeaderHeight = 20 * scaleFator;
            // bomRowHeight = 18 * scaleFator;

            //--------------------------------------------------------------
            //  HEADER RECTANGLE
            //--------------------------------------------------------------
            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(bomX0, bomY0),
                new Point2D(bomX1, bomY0),
                new Point2D(bomX1, bomY0 - bomHeaderHeight),
                new Point2D(bomX0, bomY0 - bomHeaderHeight)
            }));

            //--------------------------------------------------------------
            //  Sr.No COLUMN WIDTH + VERTICAL LINE
            //--------------------------------------------------------------
            double srNoWidth = 20 * scaleFator;

            // Vertical divider line in header
            sheet.Entities.Add(new Line(bomX0 + srNoWidth, bomY0, bomX0 + srNoWidth, bomY0 - bomHeaderHeight));

            //--------------------------------------------------------------
            //  HEADER TEXT
            //--------------------------------------------------------------
            sheet.Entities.Add(new Text(
                new Point3D(bomX0 + srNoWidth / 2, bomY0 - bomHeaderHeight / 2, 0),
                "Sr. No.", 4 * scaleFator, Text.alignmentType.MiddleCenter));

            sheet.Entities.Add(new Text(
                new Point3D(bomX0 + srNoWidth + (bomX1 - (bomX0 + srNoWidth)) / 2,
                            bomY0 - bomHeaderHeight / 2, 0),
                "DESCRIPTION OF PAINT BOOTH",
                4 * scaleFator,
                Text.alignmentType.MiddleCenter));

            //--------------------------------------------------------------
            //  BOM ROWS (F1 – F6)
            //--------------------------------------------------------------
            string[] rowText = { "F1", "F2", "F3", "F4", "F5", "F6" };

            string[] desc =
            {
                $"DIMENSION : {model.W} MM W X {model.D} MM D X {model.H} MM H",
                $"CENTRIFUGAL EXHAUST BLOWER CAPACITY: {model.CapacityofBlowerinH} M3/hr  Qty : 1 NO.",
                $"MOTOR FOR EXHAUST BLOWER CAPACITY: {model.RatedOutputHP} HP {model.MotorTypes} Qty : 1 NO.",
                $"LED LAMP FLAME PROOF  Qty : {model.Lights} No.",
                "DUCTING  QTY : 1 SET.",
                "BOOTH MOC : M.S. 1.2MM THICK"
            };

            double rowY = bomY0 - bomHeaderHeight;

            // NEW CORRECT TEXT HEIGHT
            double bomTextH = 3 * scaleFator;

            for (int i = 0; i < 6; i++)
            {
                // Row rectangle
                sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
                {
                    new Point2D(bomX0, rowY),
                    new Point2D(bomX1, rowY),
                    new Point2D(bomX1, rowY - bomRowHeight),
                    new Point2D(bomX0, rowY - bomRowHeight)
                }));

                // Sr. No.
                sheet.Entities.Add(new Text(
                    new Point3D(bomX0 + srNoWidth / 2, rowY - bomRowHeight / 2, 0),
                    rowText[i], 4 * scaleFator, Text.alignmentType.MiddleCenter));

                // WRAP TEXT
                double descLeftX = bomX0 + srNoWidth + 3 * scaleFator;
                double maxWidth = (bomX1 - descLeftX) - (5 * scaleFator);

                List<string> wrapped = WrapText(desc[i], maxWidth, bomTextH);

                double textY = rowY - (4 * scaleFator);

                foreach (string line in wrapped)
                {
                    sheet.Entities.Add(new Text(
                        new Point3D(descLeftX, textY, 0),
                        line, bomTextH, Text.alignmentType.TopLeft));

                    textY -= (bomTextH + 2 * scaleFator);
                }

                rowY -= bomRowHeight;
            }
            //--------------------------------------------------------------
            //  FULL HEIGHT Sr.No VERTICAL LINE
            //--------------------------------------------------------------
            sheet.Entities.Add(new Line(bomX0 + srNoWidth, bomY0, bomX0 + srNoWidth, bomY1));



        
            // NOTES BLOCK (positioned below BOM)
            #region Notes
            double notesX0 = titleBoxX;
            double notesX1 = innerX1;

            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(notesX0, notesTopY),
                new Point2D(notesX1, notesTopY),
                new Point2D(notesX1, notesBottomY),
                new Point2D(notesX0, notesBottomY)
            }));

            double ntH = 3 * scaleFator;
            double curY = notesTopY - (8 * scaleFator);

            sheet.Entities.Add(new Text(new Point3D(notesX0 + 5 * scaleFator, curY, 0),
                "NOTES", ntH + 1, Text.alignmentType.TopLeft));
            curY -= 12 * scaleFator;

            string[] notesLines =
            {
                "1) DURING EQUIPMENT INSTALLATION, WALL/ROOF",
                "   CUT OUT & SEALING FOR EXHAUST DUCT ROUTING",
                "   IS IN CUSTOMER SCOPE.",
                "",
                "2) IF REQUIRED, BLOWER MOUNTING STRUCTURE WILL BE IN CUSTOMER SCOPE.",
                "",
                "3) MAIN UTILITY IS IN CUSTOMER SCOPE.",
                "   COMPRESSED AIR OIL & MOISTURE FREE AT INLET",
                "   PRESSURE REGULATOR @ 6 BAR.",
                "   ELECTRICAL POWER - @ 11.5 KW (APPROX.)",
                "",
                "4) GROUND LEVEL SHOULD BE CONCRETED FLAT &",
                "   LEVELED, NOT TO HAVE VARIATION MORE THAN +/- 3MM."
            };

            foreach (var line in notesLines)
            {
                sheet.Entities.Add(new Text(new Point3D(notesX0 + 5 * scaleFator, curY, 0), line, ntH, Text.alignmentType.TopLeft));
                curY -= 6 * scaleFator;
            } 
            #endregion

            // UTILITIES

            sheet.Entities.Add(new Text(new Point3D(notesX0 + 5 * scaleFator, curY, 0),
                "UTILITY IS IN CUSTOMER SCOPE", ntH + 1, Text.alignmentType.TopLeft));
            curY -= 10 * scaleFator;

            sheet.Entities.Add(new Text(new Point3D(notesX0 + 10 * scaleFator, curY, 0), "• AIR", ntH, Text.alignmentType.TopLeft));
            curY -= 8 * scaleFator;
            sheet.Entities.Add(new Text(new Point3D(notesX0 + 10 * scaleFator, curY, 0), "• ELECTRIC", ntH, Text.alignmentType.TopLeft));


            // ==========================================================
            // TOLERANCE BOX (just above title box)
            // ==========================================================
            sheet.Entities.Add(devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(notesX0, tolTopY),
                new Point2D(notesX1, tolTopY),
                new Point2D(notesX1, tolBottomY),
                new Point2D(notesX0, tolBottomY)
            }));

            sheet.Entities.Add(new Text(new Point3D(notesX0 + 5 * scaleFator, tolBottomY + tolHeight / 2, 0),
                "GENERAL TOLERANCE ± 3 MM", ntH, Text.alignmentType.MiddleLeft));


            // ==========================================================
            // VIEWS and return
            // ==========================================================
            sheet = Views(sheet, innerX0, innerX1, titleBoxX, titleBoxY, innerY1, model, scaleFator);
            return sheet;
        }
        private List<string> WrapText(string text, double maxWidth, double fontHeight)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');

            string current = "";
            foreach (string w in words)
            {
                string test = (current.Length == 0 ? w : current + " " + w);

                // Estimated character width (Eyeshot uses about 0.6 * height)
                double estimatedWidth = test.Length * (fontHeight * 0.6);

                if (estimatedWidth > maxWidth)
                {
                    lines.Add(current);
                    current = w;
                }
                else
                {
                    current = test;
                }
            }

            if (current.Length > 0)
                lines.Add(current);

            return lines;
        }

        public MySheet Views(MySheet drawingdoc, double x0,double innerX1, double x1, double y0, double y1, PaintBoothModel model,int scaleFator)
        {
            DesignDocument drawing = new DesignDocument();
            const string Dim = "Dimension";
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            double YValue = y1 - (model.H)/2 - (model.CChannelHeight * 2) - (double)(model.standardbend2 * 2);
            Line ly = new Line(Plane.XY, Point2D.Origin, new Point2D(x1 - (model.D / 2 + model.D3), YValue));
            //drawingdoc.Entities.Add(ly);

            MySheet mysheet = AddTopViews(drawingdoc, new Point2D(x1 - ((model.D / 2 + model.D3)), (y0 + model.W / 2) + (double)(2 * model.standardbend2)), true);
            mysheet = AddFrontViews(drawingdoc, new Point2D(x1 - ((model.D / 2 + model.D3)), YValue), true);
            mysheet = AddSideViews(drawingdoc, new Point2D(x0 + (model.W / 2) + (model.W / 3), YValue), true);
            mysheet = AddIsometricViews(drawingdoc, new Point2D(x0 + (model.W), (y0 + model.W / 2) + (double)(2 * model.standardbend2)), true);

            #region Top view Dimensions
            double topViewX = (x1 - ((model.D / 2 + model.D3)) - ((model.D / 2) + (model.D3 / 2)));
           

            Text topViewText = new Text(
                new Point3D(x1 - (model.D / 2 + model.D3) - 100, (y0 - 300 - (double)(2 * model.standardbend2)), 0),
                "Top View", 40// Font size for the text
            )
            {
                LayerName = Dim,  // Set the layer for the text               
            };
            drawingdoc.Entities.Add(topViewText);




            //total width of Paintbooth
            LinearDim totalWidth = new LinearDim(Plane.XY,
                new Point2D(topViewX, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(topViewX +model.D+model.D3, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(x1 - (model.D / 2 + model.D3),y0-200), 40)
            {
                LayerName = Dim
            };
          
            drawingdoc.Entities.Add(totalWidth);

            //total Depth of Paintbooth
            LinearDim DepthOfPaintbooth = new LinearDim(Plane.XY,
                new Point2D(topViewX, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(topViewX + model.D, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(((topViewX) +(topViewX + model.D))/2, y0-50), 40)
            { LayerName = Dim,
              TextOverride="DEPTH = <>"
            };

            drawingdoc.Entities.Add(DepthOfPaintbooth);

            // D3 width of Paintbooth
            LinearDim D3Width = new LinearDim(Plane.XY,
                new Point2D(topViewX + model.D, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(topViewX + model.D + model.D3, (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(((topViewX + model.D) +(topViewX + model.D + model.D3))/2, y0-50), 40)
            { LayerName = Dim,
            TextOverride="D3 = <>"
            };
           
            drawingdoc.Entities.Add(D3Width);



            LinearDim topviewYDim = new LinearDim(verticalPlane,
                new Point2D((y0 + (double)(2 * model.standardbend2)), -x1 + (model.D3 / 2)),
               new Point2D((y0 + model.W + (double)(2 * model.standardbend2)), -x1 + (model.D3 / 2)),
               new Point2D(y0 + model.W / 2, -x1 + (model.D3 / 2) - 175), 40)
            { LayerName = Dim,
              TextOverride="WIDTH = <>"
            };
            // Add the dimension entity to the drawing document
           
            drawingdoc.Entities.Add(topviewYDim);


            Line l1 = new Line(Plane.XY, Point2D.Origin, new Point2D(x1 - (model.D / 2 + model.D3) - 100, (y0)));
            //drawingdoc.Entities.Add(l1);


            #endregion

            #region Front view Dimensions
            
            Line l = new Line(Plane.XY, Point2D.Origin, new Point2D(x1,YValue));
            //drawingdoc.Entities.Add(l);
            double FrontViewY = YValue - model.H / 2 ;

            // Add "FrontView" Text above the front view dimensions
            Text frontViewText = new Text(
                new Point3D(x1 - (model.D / 2 + model.D3) - 100, FrontViewY - 300, 0),  // Adjusted Y to move it higher
                "Front View",  // The text content
                40  // Font size for the text
            )
            {
                LayerName = Dim,  // Set the layer for the text              
            };
            drawingdoc.Entities.Add(frontViewText);

            //total Depth of Paintbooth
            LinearDim DepthOfPaintboothFrontView = new LinearDim(Plane.XY,
                new Point2D(topViewX, FrontViewY),
                new Point2D(topViewX + model.D, FrontViewY),
                new Point2D(((topViewX) + (topViewX + model.D)) / 2, FrontViewY - 150), 40)
            {
                LayerName = Dim,
                TextOverride = "DEPTH = <>"
            };
            drawingdoc.Entities.Add(DepthOfPaintboothFrontView);

            // D3 width of Paintbooth
            LinearDim D3WidthFrontView = new LinearDim(Plane.XY,
                new Point2D(topViewX + model.D, FrontViewY),
                new Point2D(topViewX + model.D + model.D3, FrontViewY),
                new Point2D(((topViewX + model.D) + (topViewX + model.D + model.D3)) / 2, FrontViewY - 150), 40)
            {
                LayerName = Dim,
                TextOverride = "D3 = <>"
            };
            drawingdoc.Entities.Add(D3WidthFrontView);

            LinearDim FrontviewXDim = new LinearDim(Plane.XY,
                 new Point2D((x1 - ((model.D / 2 + model.D3)) - ((model.D / 2) + (model.D3 / 2))), FrontViewY),
                 new Point2D((x1 - ((model.D / 2 + model.D3)) + (model.D / 2) + model.D3 / 2), FrontViewY),
                new Point2D(x1 - ((model.D / 2 + model.D3)), FrontViewY-230), 40)
            {
               LayerName = Dim
            };
               
            drawingdoc.Entities.Add(FrontviewXDim);

            LinearDim FrontviewYDim = new LinearDim(verticalPlane,
                new Point2D(FrontViewY, -x1 + (model.D3 / 2)),
              new Point2D(FrontViewY+model.H, -x1 + (model.D3 / 2)),
              new Point2D(((FrontViewY) +(FrontViewY+ model.H))/2, -x1 + (model.D3 / 2)-150),40)
            { LayerName = Dim,
              TextOverride="HEIGHT = <>"
            };
            drawingdoc.Entities.Add(FrontviewYDim);

            #endregion

            #region Side view Dimensions

            // Add "SideView" Text above the side view dimensions
            Text sideViewText = new Text(
                new Point3D(x0 + ((model.W / 2) + model.W / 3) - 100, FrontViewY - 300, 0),  // Adjusted Y to move the text above
                "Side View",  // The text content
                40  // Font size for the text
            )
            {
                LayerName = Dim,  // Set the layer for the text               
            };
            drawingdoc.Entities.Add(sideViewText);



            LinearDim SideviewXDimWidth = new LinearDim(Plane.XY,
                 new Point2D((x0 + model.W / 3), FrontViewY),
                 new Point2D((x0 + model.W + model.W / 3), FrontViewY),

                new Point2D(x0 + ((model.W / 2) + model.W / 3), FrontViewY-150), 40)
            {
                LayerName = Dim,
                TextOverride="WIDTH = <>"
            };
            drawingdoc.Entities.Add(SideviewXDimWidth);

            double sideViewX0 = x0 + model.W / 3 - (double)model.standardbend2;
            double sideViewX1 = x0 + model.W + model.W / 3 + (double)model.standardbend2;

            LinearDim SideviewXDimTotalWidth = new LinearDim(Plane.XY,
                 new Point2D(sideViewX0, FrontViewY),
                 new Point2D(sideViewX1, FrontViewY),

                new Point2D(((sideViewX0) +(sideViewX1))/2, FrontViewY - 230), 40)
            {
                LayerName = Dim,
            };
            drawingdoc.Entities.Add(SideviewXDimTotalWidth);

            LinearDim SideviewHeight = new LinearDim(verticalPlane,
                new Point2D(FrontViewY, -sideViewX0),
              new Point2D(FrontViewY + model.H, -sideViewX0),
               new Point2D(((FrontViewY + model.H) +(FrontViewY))/2, -sideViewX0 + 150), 40)
            {
                LayerName = Dim,
                TextOverride = "HEIGHT = <>"
            };
            drawingdoc.Entities.Add(SideviewHeight);
            #endregion

            Line ll = new Line(Plane.XY, new Point2D(x1, y1), new Point2D(x1, y0));
            drawingdoc.Entities.Add(ll);
       
            return mysheet;
        }


    }
}
#endregion

