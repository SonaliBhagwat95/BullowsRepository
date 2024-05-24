using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devregion = devDept.Eyeshot.Entities.Region;

namespace Bullows.Business
{
    public class PaintBoothDesign
    {
        public DesignDocument drawing { get; set; }
        public int PaintBoothID { get; set; }

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
        public double PanelWidth { get; set; }
        public double PanelHeight { get; set; }
        public double SheetThickness { get; set; }
        public double StandardBend1 { get; set; }
        public double StandardBend2 { get; set; }
        public double PitchDistance { get; set; }
        public int NoofPanels { get; set; }

        public DesignDocument PanelsInPaintBooth(int j)
        {
            drawing = new();
            #region sweep


            LinearPath rail = new LinearPath(new Point3D[]
            {
                  new Point3D(0, 0,0),
                  new Point3D(0,0, PanelHeight),
                  new Point3D(PanelWidth,0,PanelHeight),
                  new Point3D(PanelWidth, 0,0),
                 new Point3D(0, 0, 0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                      new Point3D(0, 0,0),
                      new Point3D(SheetThickness, 0,0),
                      new Point3D(SheetThickness, StandardBend2 - SheetThickness,0),
                      new Point3D(StandardBend1, StandardBend2 - SheetThickness,0),
                      new Point3D(StandardBend1, StandardBend2,0),
                      new Point3D(0,StandardBend2, 0)
            });

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, SheetThickness);
            //for generating holes on yz Plane

            double divisionresult = PanelHeight / PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            double multipliedresult = wholenumberpart * PitchDistance;
            double samespacedivide = PanelHeight - multipliedresult;

            for (int i = 0; i < (wholenumberpart + 1); i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else
                    centerz = PanelHeight - (i + 1) * PitchDistance + (samespacedivide / 2);
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


            for (int i = 0; i <= (wholenumberpart1 + 1); i++)
            {
                double centerx;
                devregion circle1;
                if (i == 0)
                    centerx = samespacedivide1 / 2;
                else
                    centerx = (PanelWidth) - (((i + 1) * PitchDistance) + (samespacedivide1 / 2));
                circle1 = devregion.CreateSlot(Plane.XY, (centerx), (StandardBend2 / 2), 5, 2);

                frame.ExtrudeRemove(circle1, PanelHeight, 0);
                drawing.Entities.Add(frame);
            }
            #endregion sweep


            #region writefile
            if (!Directory.Exists("C:/Bullows/bullows_drawing"))
                Directory.CreateDirectory("C:/Bullows/bullows_drawing");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawing);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, "C:/Bullows/bullows_drawing/Panel " + j + " " + DateTime.Now.ToString("hh-mm") + ".dwg");
            dwgg1.DoWork();

            Write3DPdfParams pdf = new Write3DPdfParams(drawing);
            Write3DPDF pdf1 = new Write3DPDF(pdf, "C:/Bullows/bullows_drawing/bullows_drawing.pdf");
            pdf1.DoWork();

            #endregion writefile
            return drawing;
        }
    }
}
