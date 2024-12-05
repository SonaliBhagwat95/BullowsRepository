
#region
using devDept.Eyeshot.Entities;
using devDept.Eyeshot;
using devDept.Geometry;
using System.Drawing;
using Bullows.Model;
using devregion = devDept.Eyeshot.Entities.Region;
using devDept.Eyeshot.Translators;
using Microsoft.Extensions.Configuration;
using devDept.Eyeshot.Control;
using Bullows.Database;

namespace Bullows.Business
{
    public class AddSheet
    {
        private readonly Dictionary<string, string> _formatBlockNames = new Dictionary<string, string>();
        //  AddSheet objaddsheet = new AddSheet();


        public DrawingDocument AddSheets(string name, string DriveType)
        {
            DrawingDocument dr = new DrawingDocument();
            bool sheetExists = dr.Sheets.Any(s => s.Name == name);
            if (sheetExists)
            {
                // Generate a unique name for the new sheet
                name = GenerateUniqueSheetName(name);
            }

            MySheet sheet = new MySheet(new Sheet(linearUnitsType.Millimeters, 1000, 1000, name), name, DriveType);
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
        public MySheet AddTopViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        {
            // Adjust placement coordinates to fit within the frame
            var top = new VectorView(p.X, p.Y - ((model.H / 2) + model.W), viewType.Top, 1, "Top");


            top.HiddenSegments = true;
            top.Color = Color.Green;

            sheet.Entities.Add(top);
            return sheet;
        }

        public MySheet AddFrontViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        {
            // Adjust placement coordinates to fit within the frame
            var front = new VectorView(p.X, p.Y, viewType.Front, 1, "Front");
            front.HiddenSegments = hiddenseg;
            sheet.Entities.Add(front);
            return sheet;
        }

        public MySheet AddSideViews(MySheet sheet, Point2D p, bool hiddenseg, PaintBoothModel model)
        {

            //var side = new VectorView(p.X - ((model.D / 2) + model.H), p.Y, viewType.Left, 1, "Left");
            var side = new VectorView(p.X - ((model.D / 2) + model.W), p.Y, viewType.Left, 1, "Left");
            side.HiddenSegments = hiddenseg;
            sheet.Entities.Add(side);
            return sheet;
        }

        public MySheet AddTopViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            // Adjust placement coordinates to fit within the frame
            var top = new VectorView(p.X, p.Y, viewType.Top, 1, "Top");
            top.HiddenSegments = hiddenseg;
            sheet.Entities.Add(top);
            return sheet;
        }

        public MySheet AddFrontViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            // Adjust placement coordinates to fit within the frame
            var front = new VectorView(p.X, p.Y, viewType.Front, 1, "Front");
            front.HiddenSegments = hiddenseg;
            sheet.Entities.Add(front);
            return sheet;
        }

        public MySheet AddSideViews(MySheet sheet, Point2D p, bool hiddenseg)
        {
            //var side = new VectorView(p.X - ((model.D / 2) + model.H), p.Y, viewType.Left, 1, "Left");
            var side = new VectorView(p.X, p.Y, viewType.Left, 1, "Left");
            side.HiddenSegments = hiddenseg;
            sheet.Entities.Add(side);
            return sheet;
        }

        public MySheet StandardFrame123(int scaleFator, MySheet sheet, Point2D p, PaintBoothModel model,List<PanelDetails> panels)
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
            var BOMBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(titleBoxX,titleBoxY),
                new Point2D(titleBoxX,titleBoxY+15),
                new Point2D(innerX1,titleBoxY+15),
                new Point2D(innerX1,titleBoxY),

            });
            sheet.Entities.Add(BOMBox);
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

            sheet.Entities.AddRange(new Entity[]
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
                sheet.Entities.Add(l);
                //Passed value to BOM
                sheet.Entities.AddRange(new Entity[]
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
            sheet.Entities.AddRange(new Entity[]
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


            sheet = Views(sheet, innerX0, titleBoxX, titleBoxY, innerY1, model);
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
            var BOMBox = devregion.CreatePolygon(Plane.XY, new Point2D[]
            {
                new Point2D(titleBoxX,titleBoxY),
                new Point2D(titleBoxX,titleBoxY+15),
                new Point2D(innerX1,titleBoxY+15),
                new Point2D(innerX1,titleBoxY),

            });
            sheet.Entities.Add(BOMBox);
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

            sheet.Entities.AddRange(new Entity[]
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
                sheet.Entities.Add(l);
                //Passed value to BOM
                sheet.Entities.AddRange(new Entity[]
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
            sheet.Entities.AddRange(new Entity[]
            {    new Line(titleBoxX,titleBoxY,titleBoxX,y ),
                 new Line(XCoordinate[0] - 10,y,XCoordinate[0]-10,titleBoxY),  //sr.no           
                 new Line(XCoordinate[1]-10,y,XCoordinate[1]-10,titleBoxY),//part no
                 new Line(XCoordinate[2] - 10,y,XCoordinate[2] - 10,titleBoxY),//part name
                 new Line(XCoordinate[3] - 10,y,XCoordinate[3] - 10,titleBoxY),//material
                 new Line(XCoordinate[4] - 10,y,XCoordinate[4] - 10,titleBoxY),//SPECIFICATION
                 new Line(XCoordinate[5] - 10,y,XCoordinate[5]-10,titleBoxY),//QUANTITY
                 new Line(XCoordinate[6] - 10,y,XCoordinate[6] - 10,titleBoxY),
            });


            #endregion


            sheet = Views(sheet, innerX0, titleBoxX, titleBoxY, innerY1, model);
            return sheet;

        }
        public MySheet Views(MySheet drawingdoc, double x0, double x1, double y0, double y1, PaintBoothModel model)
        {
            DesignDocument drawing = new DesignDocument();
            const string Dim = "Dimension";
            Plane verticalPlane = Plane.XY;
            verticalPlane.Rotate(Math.PI / 2, Vector3D.AxisZ);
            drawing.Layers.Add(new Layer(Dim, Color.CornflowerBlue));
            MySheet mysheet = AddTopViews(drawingdoc, new Point2D(x1 - ((model.D / 2 + model.D3)), (y0 + model.W / 2) + (double)(2 * model.standardbend2)), true);
            mysheet = AddFrontViews(drawingdoc, new Point2D(x1 - ((model.D / 2 + model.D3)), y1 - ((model.H / 2) + (double)(5 * model.standardbend2))), true);
            mysheet = AddSideViews(drawingdoc, new Point2D(x0 + (model.W / 2) + (model.W / 3), y1 - ((model.H / 2) + (double)(5 * model.standardbend2))), true);
            #region Top view Dimensions
            LinearDim topviewXDim = new LinearDim(Plane.XY,
                new Point2D((x1 - ((model.D / 2 + model.D3)) - ((model.D / 2) + (model.D3 / 2))), (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D((x1 - ((model.D / 2 + model.D3)) + (model.D / 2) + model.D3 / 2), (y0 + model.W / 2) + (double)(2 * model.standardbend2)),
                new Point2D(x1 - ((model.D / 2 + model.D3)), (((y0 + model.W / 2) + (double)(2 * model.standardbend2))) - (model.D / 2) - 200), 40);
            topviewXDim.LayerName = Dim;

            LinearDim topviewYDim = new LinearDim(verticalPlane,
                new Point2D((y0 + (double)(2 * model.standardbend2)), -x1 + (model.D3 / 2)),
               new Point2D((y0 + model.W + (double)(2 * model.standardbend2)), -x1 + (model.D3 / 2)),
               new Point2D(y0 + model.W / 2, -x1 + (model.D3 / 2) - 175), 40)
            { LayerName = Dim };
            // Add the dimension entity to the drawing document
            drawingdoc.Entities.Add(topviewXDim);
            drawingdoc.Entities.Add(topviewYDim);
            #endregion
            #region Front view Dimensions
            LinearDim FrontviewXDim = new LinearDim(Plane.XY,
                 new Point2D((x1 - ((model.D / 2 + model.D3)) - ((model.D / 2) + (model.D3 / 2))), (y1 - model.H)),
                 new Point2D((x1 - ((model.D / 2 + model.D3)) + (model.D / 2) + model.D3 / 2), (y1 - model.H)),

                new Point2D(x1 - ((model.D / 2 + model.D3)), (((y1 - model.H / 2) + (double)(5 * model.standardbend2))) - ((model.D / 2) + (model.D3 / 2) + 200)), 40);
            FrontviewXDim.LayerName = Dim;
            LinearDim FrontviewYDim = new LinearDim(verticalPlane,
                new Point2D((y1 - (double)(5 * model.standardbend2)), -x1 + (model.D3 / 2)),
              new Point2D((y1 - (model.H + (double)(5 * model.standardbend2))), -x1 + (model.D3 / 2)),
               new Point2D((y1 - model.H / 2), -x1 + (model.D3 / 2) - 175), 40)
            { LayerName = Dim };
            // Add the dimension entity to the drawing document
            drawingdoc.Entities.Add(FrontviewXDim);
            drawingdoc.Entities.Add(FrontviewYDim);
            #endregion

            #region Side view Dimensions
            LinearDim SideviewXDim = new LinearDim(Plane.XY,
                 new Point2D((x0 + model.W / 3), (y1 - (model.H + (double)(5 * model.standardbend2)))),
                 new Point2D((x0 + model.W + model.W / 3), (y1 - model.H)),

                new Point2D(x0 + ((model.W / 2) + model.W / 3), (((y1 - model.H / 2) + (double)(5 * model.standardbend2))) - ((model.D / 2) + (model.D3 / 2) + 200)), 40);
            SideviewXDim.LayerName = Dim;
            // Add the dimension entity to the drawing document
            drawingdoc.Entities.Add(SideviewXDim);

            #endregion
            return mysheet;
        }



    }
}
#endregion

