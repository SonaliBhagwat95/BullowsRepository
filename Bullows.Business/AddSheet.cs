using devDept.Eyeshot.Entities;
using devDept.Eyeshot;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using bullows.business;
using Bullows.Model;

namespace Bullows.Business
{
    public class AddSheet
    {
        public DrawingDocument AddSheets(string name, string DriveType)
        {
            DrawingDocument dr = new DrawingDocument();
            MySheet sheet = new MySheet(new Sheet(linearUnitsType.Millimeters, 1000, 1000, "Sheet1"), name, DriveType);
            dr = AddLayers(dr, sheet);
            return dr;
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
        public MySheet AddTopViews(MySheet sheet, DesignDocument model, DrawingDocument drawings1, Point2D p, bool hiddenseg)
        {
            var top = new VectorView(p.X, p.Y, viewType.Top, 1, "Top");
            top.HiddenSegments = hiddenseg;

            sheet.Entities.Add(top);
            //top.InsertionPoint = new Point3D(p.X+top.Width / 2, p.Y+top.Height / 2, 0);
            return sheet;
        }
        public MySheet AddFrontViews(MySheet sheet, DesignDocument model, DrawingDocument drawings1, Point2D p, bool hiddenseg, PanelInputModel panel)
        {
            var front = new VectorView(p.X, panel.PanelHeight / 2 + panel.PanelHeight / 2 * (0.2), viewType.Front, 1, "Front");
            front.HiddenSegments = hiddenseg;
            sheet.Entities.Add(front);
            return sheet;
        }
        public MySheet AddSideViews(MySheet sheet, DesignDocument model, DrawingDocument drawings1, Point2D p, bool hiddenseg, PanelInputModel panel)
        {
            var side = new VectorView(-(panel.PanelWidth / 2 + panel.PanelWidth / 2 * (0.2)), panel.PanelHeight / 2 + panel.PanelHeight / 2 * (0.2), viewType.Left, 1, "Left");
            side.HiddenSegments = hiddenseg;
            sheet.Entities.Add(side);
            return sheet;
        }
    }
}