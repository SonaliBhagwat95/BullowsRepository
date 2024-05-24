using devDept.Eyeshot.Entities;
using devDept.Eyeshot;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Business
{


    public class MySheet : Sheet
    {
        public MySheet(Sheet another, string partName, string Drive) : base(another)
        {
            CreateTitleBlock(2, Color.White, (float)0.15);
            GetLogo();
            parts = partName;
            DriveType = Drive;
        }
        public double attributeHeight { get; set; }
        static string DriveType = string.Empty;

        static string parts = string.Empty;
        protected override Entity[] CreateTitleBlock(double borderWidth, Color color, float lineWeight = 0.15F)
        {
            List<Entity> ents = new List<Entity>();
            Table table = TableData(ents, borderWidth);
            Table table1 = TableData1(ents, borderWidth);
            Table table2 = TableData2(ents, borderWidth);
            Table table3 = TableData3(ents, borderWidth);


            return ents.ToArray();
        }
        public Entity sc;
        public double tableheight(double[] rowsHeights, double prevtableht)
        {
            double tableheight = prevtableht;
            double tableheight1 = 0;
            for (int rowht = 0; rowht < rowsHeights.Length; rowht++)
            {
                if (rowht == 0)
                    tableheight = tableheight + rowsHeights[rowht];
                else
                    tableheight = rowsHeights[rowht];
                tableheight1 = tableheight1 + tableheight;

            }
            return tableheight1;
        }
        public double tablewidth(double[] columnsWidths)
        {
            double tableheight = 0;
            double tableheight1 = 0;
            for (int colwidth = 0; colwidth < columnsWidths.Length; colwidth++)
            {
                tableheight = columnsWidths[colwidth];
                tableheight1 = tableheight1 + tableheight;

            }
            return tableheight1;
        }
        private Table TableData(List<Entity> ents, double borderWidth)
        {
            #region SR.NO.ForFirstTable
            float lineWeight = 0.015F;
            Color myColor = Color.White;
            double myWidth = borderWidth;

            double[] rowsHeights = { 20 };

            double[] columnsWidths = { 21, 48, 14, 14 };

            double textHeight = 1.3;
            attributeHeight = 3;

            Table table = new Table(Plane.XY, rowsHeights.Length, columnsWidths.Length, rowsHeights, columnsWidths, textHeight);
            table.HorCellMargin = 0.9;
            table.VerCellMargin = 0.9;

            table.Translate(myWidth - tablewidth(columnsWidths), tableheight(rowsHeights, 0));

            CompositeCurve cc = DrawSymbol();

            cc.Translate((myWidth - tablewidth(columnsWidths) + 5) / 0.15, tableheight(rowsHeights, 0) + 9 / 0.15);
            cc.Scale(0.15);


            table.ColorMethod = colorMethodType.byEntity;
            table.Color = myColor;
            table.LineWeightMethod = colorMethodType.byEntity;
            table.LineWeight = lineWeight;
            //ents.Add(cc);
            ents.Add(table);
            #endregion
            table.SetTextString(0, 1, "This drawing is the exclusive property of M/S." +
                "- - - - - - - - - - - - - - - - - - " +
                "and subject their recall .It is confidential and must not be made public" +
                "or copied or reproduced without previous written permission.Reproducing or handling over third party is strictly prohibitted.");
            table.SetWrap(0, 1, true);
            table.SetAlignment(0, 1, Text.alignmentType.TopLeft);


            table.SetTextString(0, 2, "DRG. NO.");
            table.SetAlignment(0, 2, Text.alignmentType.MiddleLeft);


            table.SetTextString(0, 3, "REV.");
            table.SetAlignment(0, 3, Text.alignmentType.MiddleLeft);


            return table;

        }
        private Table TableData1(List<Entity> ents, double borderWidth)
        {
            #region SR.NO.ForFirstTable
            double myWidth = borderWidth;

            double[] rowsHeights = { 5, 5, 5, 5 };

            //double[] columnsWidths = { 30, 11, 12, 12, 47 };
            double[] columnsWidths = { 27, 11, 12, 12, 35 };

            double textHeight = 1.3;
            attributeHeight = 1.3;

            Table table = new Table(Plane.XY, rowsHeights.Length, columnsWidths.Length, rowsHeights, columnsWidths, textHeight);
            table.HorCellMargin = 0.9;
            table.VerCellMargin = 0.9;

            table.Translate(myWidth - tablewidth(columnsWidths), tableheight(rowsHeights, 20));

            string[] colname = { "SR.NO.", "DESCRIPTION", "MATERIAL", "SIZE", "QTY.", "WEIGHT", "REMARK" };


            table.MergeCells(0, 0, 3, 0);
            table.MergeCells(0, 4, 3, 4);

            #endregion
            float lineWeight = 0.15F;
            Color myColor = Color.White;

            table.SetTextString(0, 2, "Name");
            table.SetAlignment(0, 2, Text.alignmentType.TopLeft);

            table.SetTextString(0, 3, "Date");
            table.SetAlignment(0, 3, Text.alignmentType.TopLeft);

            table.SetTextString(1, 1, "Approved");
            table.SetAlignment(1, 1, Text.alignmentType.TopLeft);

            table.SetTextString(2, 1, "Checked");
            table.SetAlignment(2, 1, Text.alignmentType.TopLeft);

            table.SetTextString(0, 4, "Title:-");//50 H.P. FAN, COUPLE DRIVE.      MFG DRG OF IMPELLER"

            table.SetWrap(0, 4, true);
            table.SetAlignment(0, 4, Text.alignmentType.TopLeft);
            ents.Add(new devDept.Eyeshot.Entities.Attribute(new Point3D(280 - 35, 38.5), "POWER", String.Empty, attributeHeight)
            {
                Alignment = Text.alignmentType.MiddleCenter
            });
            //ents.Add(new devDept.Eyeshot.Entities.Attribute(new Point3D(267 - 24, 38.5), "FAN_TYPE", String.Empty, attributeHeight)
            //{
            //    Alignment = Text.alignmentType.MiddleCenter
            //});
            ents.Add(new devDept.Eyeshot.Entities.Attribute(new Point3D(280 - 22, 38.5), "DRIVE_TYPE", String.Empty, attributeHeight)
            {
                Alignment = Text.alignmentType.MiddleCenter
            });


            table.SetTextString(3, 1, "Drawn");
            table.SetAlignment(3, 1, Text.alignmentType.TopLeft);

            table.ColorMethod = colorMethodType.byEntity;
            table.Color = myColor;
            table.LineWeightMethod = colorMethodType.byEntity;
            table.LineWeight = lineWeight;
            ents.Add(table);
            return table;

        }
        private Table TableData2(List<Entity> ents, double borderWidth)
        {
            #region SR.NO.ForFirstTable
            float lineWeight = 0.15F;
            Color myColor = Color.White;
            double myWidth = borderWidth;

            double[] rowsHeights = { 5, 25 };

            double[] columnsWidths = { 12, 85 };

            double textHeight = 1.3;
            attributeHeight = 1.3;

            Table table = new Table(Plane.XY, rowsHeights.Length, columnsWidths.Length, rowsHeights, columnsWidths, textHeight);
            table.HorCellMargin = 0.9;
            table.VerCellMargin = 0.9;

            table.Translate(myWidth - tablewidth(columnsWidths), tableheight(rowsHeights, 20 + 20));

            table.MergeCells(0, 0, 0, 1);
            table.MergeCells(1, 0, 1, 1);

            table.SetTextString(0, 0, "Project:-");
            table.SetAlignment(0, 0, Text.alignmentType.TopLeft);
            ents.Add(new devDept.Eyeshot.Entities.Attribute(new Point3D(285 - 95, 68), "Project", String.Empty, attributeHeight)
            {
                Alignment = Text.alignmentType.MiddleCenter
            });
            table.ColorMethod = colorMethodType.byEntity;
            table.Color = myColor;
            table.LineWeightMethod = colorMethodType.byEntity;
            table.LineWeight = lineWeight;
            ents.Add(table);

            #endregion

            return table;

        }
        private Table TableData3(List<Entity> ents, double borderWidth)
        {
            #region SR.NO.ForFirstTable
            float lineWeight = 0.15F;
            Color myColor = Color.White;
            double myWidth = borderWidth;

            double[] rowsHeights = { 5, 5, 5, 5 };

            double[] columnsWidths = { 13, 45, 10, 12, 17 };

            double textHeight = 1.3;
            attributeHeight = 3;

            Table table = new Table(Plane.XY, rowsHeights.Length, columnsWidths.Length, rowsHeights, columnsWidths, textHeight);
            table.HorCellMargin = 0.9;
            table.VerCellMargin = 0.9;

            table.Translate(myWidth - tablewidth(columnsWidths), tableheight(rowsHeights, 20 + 20 + 40));

            string[] colname = { "SR.NO.", "DESCRIPTION", "MATERIAL", "SIZE", "QTY.", "WEIGHT", "REMARK" };

            table.SetTextString(3, 0, "ISSUE");
            //  table.SetStyleName(3, 0, "Romans");
            string s = table.GetStyleName(3, 0);

            table.SetAlignment(3, 0, Text.alignmentType.MiddleCenter);

            table.SetTextString(3, 1, "REVISIONS");
            table.SetAlignment(3, 1, Text.alignmentType.MiddleCenter);
            s = table.GetStyleName(3, 1);

            table.SetTextString(3, 2, "DRN");
            table.SetAlignment(3, 2, Text.alignmentType.MiddleCenter);

            table.SetTextString(3, 3, "CHD");
            table.SetAlignment(3, 3, Text.alignmentType.MiddleCenter);

            table.SetTextString(3, 4, "DATE");
            table.SetAlignment(3, 4, Text.alignmentType.MiddleCenter);


            table.SetTextString(2, 0, "A");

            table.SetAlignment(2, 0, Text.alignmentType.MiddleCenter);
            table.SetTextString(2, 1, "FOR COMMENTS");

            table.SetAlignment(2, 1, Text.alignmentType.MiddleCenter);

            table.ColorMethod = colorMethodType.byEntity;
            table.Color = myColor;
            table.LineWeightMethod = colorMethodType.byEntity;
            table.LineWeight = lineWeight;
            ents.Add(table);

            #endregion


            return table;

        }
        public CompositeCurve DrawSymbol()
        {
            double rad1 = 14.77;
            double rad2 = 20.267;
            double startX = 23 + 20;
            double startY = 11.5;
            double startX1 = 46.325;
            double startY1 = 20.20;
            double line = 22;
            double lineX = 40;
            double lineX0 = 43 + 55;
            Block blkdrw = new Block("blkdrw");

            //polyline(startX, -startY, startX, startY, startX1 + startX, startY1, startX1 + startX, -startY1, startX, -startY, blkdrw);
            LinearPath polygon = new LinearPath(Plane.XY,
                new Point2D(startX, -startY),
                new Point2D(startX, startY),
                new Point2D(startX1 + startX, startY1),
                new Point2D(startX1 + startX, -startY1),
            new Point2D(startX, -startY)
            );
            CompositeCurve sdc = new CompositeCurve(new Circle(Plane.XY, rad1), new Circle(Plane.XY, rad2), polygon,
            new Line(new Point3D(-line, 0, 0), new Point3D(line, 0, 0)),
            new Line(new Point3D(0, -line, 0), new Point3D(0, line, 0)),
                   new Line(new Point3D(lineX, 0, 0), new Point3D(lineX0, 0, 0)));

            double[] columnsWidths = { 10, 30, 15, 20, 10, 10, 10 };
            double borderwidth = 2;
            double x = borderwidth - tablewidth(columnsWidths);//2=borderwidth
            double scale = 0.25;
            
            return sdc;
        }
        protected override Entity[] GetLogo()
        {
            return null;
        }
    }
}

