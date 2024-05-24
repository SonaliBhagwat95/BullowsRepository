
using Bullows.Business;
using Bullows.Model;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using devregion = devDept.Eyeshot.Entities.Region;

namespace bullows.business
{
    public class PanelInput

    {
       
       
        private IHttpContextAccessor httpcontextaccessor;


        public DesignDocument drawing;
        public DesignDocument docdrawing;
        private Sheet sheet;
        private string name;
        public int ProjectID { get; set; }
       
        private string bendlayer { get; set; }
        public double PanelWidth { get; set; }
        public double PanelHeight { get; set; }
        public double SheetThickness { get; set; }
        public double StandardBend1 { get; set; }
        public double StandardBend2 { get; set; }
        public double PitchDistance { get; set; }
        public int NoofPanels { get; set; }
        devregion devregion { get; set; }

        public int[] shroudarr;
        private int[] Panelinputs;
        public int[] stiff;
        public double CutoutLength { get; set; }
        public double CutoutWidth { get; set; }
        public double CutoutXDistance { get; set; }
        public double CutoutYDistance { get; set; }
        public Point3D[] Points { get; private set; }


        public PanelInput()
        {
            drawing = new DesignDocument();
            docdrawing = new DesignDocument();
            Points = new Point3D[4];
        }

        public PanelInput(Sheet sheet, string name)
        {
            this.sheet = sheet;
            this.name = name;

        }
       
       
       
        public DesignDocument SweepMethod(PanelInputModel model)
        {
            try
            {
                DesignDocument drawing = new DesignDocument();
              
            var rectangle1 = devregion.CreatePolygon(new Point3D[]
            {
                  new Point3D(0,0,0),
                  new Point3D(0,0, model.PanelHeight),
                  new Point3D(model.PanelWidth,0,model.PanelHeight),
                  new Point3D(model.PanelWidth,0,0)
            });
            #region cutout

            Point3D[] cutout = new Point3D[]
            {
                 new Point3D(model.CutoutXDistance, 0, model.CutoutYDistance),
                 new Point3D((model.CutoutXDistance + model.CutoutWidth), 0, model.CutoutYDistance),
                 new Point3D((model.CutoutXDistance + model.CutoutWidth), 0, (model.CutoutYDistance + model.CutoutLength)),
                 new Point3D(model.CutoutXDistance, 0, (model.CutoutYDistance + model.CutoutLength)),
             };

            var cutout1 = devregion.CreatePolygon(cutout);
            Brep brep = rectangle1.ExtrudeAsBrep( model.SheetThickness);
            brep.ExtrudeRemove(cutout1,(- model.PanelWidth));
            drawing.Entities.Add(brep);
                #endregion cutout
                #region sweep
                

                LinearPath rail = new LinearPath(new Point3D[]
            {
                  new Point3D(0, 0,0),
                  new Point3D(0,0,model.PanelHeight),
                  new Point3D(model.PanelWidth,0,model.PanelHeight),
                  new Point3D(model.PanelWidth, 0,0),
                 new Point3D(0, 0, 0)
            });
            var section = devregion.CreatePolygon(new Point3D[]
            {
                      new Point3D(0, 0,0),
                      new Point3D(model.SheetThickness, 0,0),
                      new Point3D(model.SheetThickness, model.StandardBend2 - model.SheetThickness,0),
                      new Point3D(model.StandardBend1, model.StandardBend2 - model.SheetThickness,0),
                      new Point3D(model.StandardBend1, model.StandardBend2,0),
                      new Point3D(0,model.StandardBend2, 0)
            });

            Solid frame = section.SweepAsSolid(rail, 0);
            frame.Translate(0, 0, model.SheetThickness);
            //for generating holes on yz Plane
            
            double divisionresult = model.PanelHeight / model.PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);

            double multipliedresult = wholenumberpart * model.PitchDistance;
            double samespacedivide = model.PanelHeight - multipliedresult;

            for (int i = 0; i < (wholenumberpart + 1); i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else
                    centerz = model.PanelHeight - (i + 1) * model.PitchDistance + (samespacedivide / 2);
                devregion ssr2 = devregion.CreateSlot(Plane.YZ, (model.StandardBend2 / 2), centerz, 5, 2, 1.5708);
                ssr2.Translate(0, 0, samespacedivide / 2);
                frame.ExtrudeRemove(ssr2, model.PanelWidth, 0);
                drawing.Entities.Add(frame);
            }


            //for generating holes on xy Plane
            double divisionresult1 = model.PanelWidth / model.PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);

            double multipliedresult1 = wholenumberpart1 * model.PitchDistance;
            double samespacedivide1 = model.PanelWidth - multipliedresult1;


            for (int i = 0; i <= (wholenumberpart1 + 1); i++)
            {
                double centerx;
                devregion circle1;
                if (i == 0)
                    centerx = samespacedivide1 / 2;
                else
                    centerx = (model.PanelWidth) - (((i + 1) * model.PitchDistance) + (samespacedivide1 / 2));
                circle1 = devregion.CreateSlot(Plane.XY, (centerx), (model.StandardBend2 / 2), 5, 2);

                frame.ExtrudeRemove(circle1, model.PanelHeight, 0);
                drawing.Entities.Add(frame);
            }
                #endregion sweep
                //SaveDrawingFiles(drawing, model.ProjectID);


                #region writefile
                var projectpath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
                projectpath += "/" + model.ProjectID;

                if (!Directory.Exists(projectpath + "/Bullows Panel Drawing"))
                    Directory.CreateDirectory(projectpath + "/Bullows Panel Drawing");

                WriteAutodeskParams auto = new WriteAutodeskParams(drawing);

                WriteAutodesk dwgg1 = new WriteAutodesk(auto, projectpath + "/Bullows Panel Drawing/" + "PanelDrawing" + DateTime.Now.ToString("hh-mm") + ".dwg");
                dwgg1.DoWork();

                Write3DPdfParams pdf = new Write3DPdfParams(drawing);
                Write3DPDF pdf1 = new Write3DPDF(pdf, projectpath + "/bullows_Paneldrawing.pdf");
                pdf1.DoWork();

            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }


            #endregion writefile


            detailsdrawing(drawing, model);
            Development(model);
            return drawing;
        }

        //private void SaveDrawingFiles(DesignDocument drawing, int projectId)
        //{
        //    var projectPath = GetProjectPath(projectId);
        //    if (!Directory.Exists(projectPath))
        //    {
        //        Directory.CreateDirectory(projectPath);
        //        Console.WriteLine($"Created directory: {projectPath}");
        //    }

        //    WriteAutodeskParams autoParams = new(drawing);
        //    string dwgFilePath = $"{projectPath}/PanelDrawing_{DateTime.Now:HH-mm}.dwg";
        //    new WriteAutodesk(autoParams, dwgFilePath).DoWork();
        //    Console.WriteLine($"Saved DWG file: {dwgFilePath}");

        //    Write3DPdfParams pdfParams = new(drawing);
        //    string pdfFilePath = $"{projectPath}/panel_drawing.pdf";
        //    new Write3DPDF(pdfParams, pdfFilePath).DoWork();
        //    Console.WriteLine($"Saved PDF file: {pdfFilePath}");
        //}
        //private string GetProjectPath(int projectId)
        //{
        //    var projectPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
        //    return $"{projectPath}/{projectId}/Bullows Panel Drawing";
        //}

        private void LogError(Exception ex)
        {
            System.IO.File.WriteAllText("C:/path_to_logs/error.log", ex.ToString());
        }




        public DesignDocument Development(PanelInputModel model)
        {
            #region Development
            //
            var rectangle1 = devregion.CreatePolygon(new Point3D[]
            {
                 new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),0),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),0),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight),
                 new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight)
            });


            Layer mylayer = new Layer("bendlayer");
            mylayer.Color = Color.FromArgb(165, 82, 165);
            docdrawing.Layers.Add(mylayer);
            rectangle1.LayerName = "bendlayer";
            docdrawing.Entities.Add(rectangle1);


            //for right side
            LinearPath linear1 = new LinearPath(new Point3D[]
            {
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),0),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)+(model.StandardBend1 + model.StandardBend2)),0),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)+(model.StandardBend1 + model.StandardBend2)),model.PanelHeight),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight)
            });
            docdrawing.Entities.Add(linear1);
            #region bend
            //for bendline right side
            LinearPath bendlineright = new LinearPath(new Point3D[]
             {
                new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)+(model.StandardBend2)),0),
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)+(model.StandardBend2)),model.PanelHeight),
            });
            bendlineright.LayerName = "bendlayer";
            docdrawing.Entities.Add(bendlineright);

            //for bendline left side
            LinearPath bendlineleft = new LinearPath(new Point3D[]
             {
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)-(model.StandardBend2)),0),
                 new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)-(model.StandardBend2)),model.PanelHeight),
            });
            bendlineleft.LayerName = "bendlayer";
            docdrawing.Entities.Add(bendlineleft);

            //for bendline bottom side
            LinearPath bendlinelebottom = new LinearPath(new Point3D[]
             {
               new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),-(model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),-(model.StandardBend2)),
            });
            bendlinelebottom.LayerName = "bendlayer";
            docdrawing.Entities.Add(bendlinelebottom);


            //for bendline top side
            LinearPath bendlineletop = new LinearPath(new Point3D[]
             {
              new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight+(model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight+(model.StandardBend2)),
            });
            bendlineletop.LayerName = "bendlayer";
            docdrawing.Entities.Add(bendlineletop);

            //for left side
            LinearPath linear2 = new LinearPath(new Point3D[]
            {
                 new Point3D(((model.PanelWidth)+(model.PanelWidth/2+model.PanelWidth/2)*(0.2)),0),
                 new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)-(model.StandardBend1 + model.StandardBend2)),0),
                 new Point3D((model.PanelWidth)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)-(model.StandardBend1 + model.StandardBend2)),model.PanelHeight),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),model.PanelHeight)
            });
            docdrawing.Entities.Add(linear2);

            //for bottom
            LinearPath linear3 = new LinearPath(new Point3D[]
            {
                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),0),
                new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),-(model.StandardBend1 + model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),-(model.StandardBend1 + model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),0),
            });
            docdrawing.Entities.Add(linear3);
            //linear path for top
            LinearPath linear4 = new LinearPath(new Point3D[]
            {

                 new Point3D((model.PanelWidth*2)+((model.PanelWidth/2 + model.PanelWidth/2)*(0.2)),model.PanelHeight),
                new Point3D((model.PanelWidth*2)+((model.PanelWidth/2+model.PanelWidth/2)*(0.2)),model.PanelHeight+(model.StandardBend1 + model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),model.PanelHeight+(model.StandardBend1 + model.StandardBend2)),
                new Point3D((model.PanelWidth)+((model.PanelWidth/2+ model.PanelWidth/2)*(0.2)),model.PanelHeight),
            });
            docdrawing.Entities.Add(linear4);
            #endregion bend
            //creating holes on xy direction 
            double divisionresult = model.PanelHeight / model.PitchDistance;
            int wholenumberpart = (int)Math.Floor(divisionresult);
            // wholenumberpart = wholenumberpart + 1;
            // calculate the remaining space after creating whole slots
            double multipliedresult = (wholenumberpart) * model.PitchDistance;
            double samespacedivide = model.PanelHeight - multipliedresult;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart; i++)
            {
                double centerz;
                if (i == 0)
                    centerz = samespacedivide / 2;
                else
                    // centerz = PanelHeight - (i + 0) * PitchDistance +(samespacedivide / 2);
                    centerz = samespacedivide / 2 + (i + 0) * model.PitchDistance;
                // create a slot
                devregion slot = devregion.CreateSlot(Plane.XY, (model.PanelWidth) + ((model.PanelWidth / 2 + model.PanelWidth / 2) * (0.2) - (model.StandardBend2 / 2)), centerz, 5, 2, 1.5708);

                devregion slot1 = devregion.CreateSlot(Plane.XY, (model.PanelWidth) + ((model.PanelWidth / 2 + model.PanelWidth / 2) * (0.2) - (model.StandardBend2 / 2) + (model.PanelWidth + model.StandardBend2)), centerz, 5, 2, 1.5708);
                slot.Translate(0, 0, samespacedivide / 2);
                slot.Color = Color.Yellow;
                // add the slot to the drawing
                docdrawing.Entities.Add(slot);
                docdrawing.Entities.Add(slot1);
            }

            //create slots on width
            double divisionresult1 = model.PanelWidth / model.PitchDistance;
            int wholenumberpart1 = (int)Math.Floor(divisionresult1);
            // wholenumberpart1 = wholenumberpart1 + 1;
            // calculate the remaining space after creating whole slots
            double multipliedresult1 = wholenumberpart1 * model.PitchDistance;
            double samespacedivide1 = model.PanelWidth - multipliedresult1;

            // create slots and add them to the drawing
            for (int i = 0; i <= wholenumberpart1; i++)
            {
                double centery;
                if (i == 0)
                    centery = samespacedivide1 / 2;
                else
                    //centery = /*(PanelWidth) - (((i + 0) * PitchDistance) - (samespacedivide1 / 2));*/
                    centery = samespacedivide1 / 2 + (i + 0) * model.PitchDistance;

                // create a slot
                devregion slot2 = devregion.CreateSlot(Plane.XY, ((model.PanelWidth) + ((model.PanelWidth / 2 + model.PanelWidth / 2) * (0.2)) + centery), (-model.StandardBend2 / 2), 5, 2, 0);

                devregion slot3 = devregion.CreateSlot(Plane.XY, ((model.PanelWidth) + ((model.PanelWidth / 2 + model.PanelWidth / 2) * (0.2)) + centery), (model.PanelHeight + model.StandardBend2 / 2), 5, 2, 0);
                slot2.Translate(0, 0, samespacedivide1 / 2);
                slot2.Color = Color.Yellow;
                // add the slot to the drawing
                docdrawing.Entities.Add(slot2);
                docdrawing.Entities.Add(slot3);
            }
            var projectpath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            projectpath +="/"+ model.ProjectID;
            if (!Directory.Exists(projectpath + "/Development"))
                Directory.CreateDirectory(projectpath + "/Development");

            WriteAutodeskParams auto = new WriteAutodeskParams(docdrawing);
            WriteAutodesk dwgg2 = new WriteAutodesk(auto, projectpath + "/Development/Development " + DateTime.Now.ToString("hh-mm") + ".dwg");
            dwgg2.DoWork();
            return drawing;
            #endregion development
        }
        public DesignDocument detailsdrawing(DesignDocument model,PanelInputModel pmodel)
        {
            var projectpath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"].ToString();
            projectpath += "/" + pmodel.ProjectID;

            string partname = "panelinput";
            model.Units = linearUnitsType.Millimeters;
            AddSheet asfp = new AddSheet();

            DrawingDocument drawingdoc = new DrawingDocument();
            string drivetype = "";// = objdrivetype;
            drawingdoc = asfp.AddSheets(partname, drivetype);

            Point3D bx = model.Entities.BoxSize;
            Point3D boxsize = new Point3D(bx.X, bx.Z, bx.Y);
            double sv = 0;
            if (boxsize.X <= 100)
                sv = 20;
            if (boxsize.Y <= 100)
                sv = 20;
            if (sv != 20)
            {
                if (boxsize.X >= boxsize.Y)
                    sv = boxsize.X / 28;
                if (boxsize.X < boxsize.Y)
                    sv = boxsize.Y / 28;
            }
            #region addviews
            // adds some views
            shroudarr = new int[model.Entities.Count];
            Panelinputs = new int[model.Entities.Count];
            stiff = new int[model.Entities.Count];
            for (int a = 0; a < model.Entities.Count; a++)
            {
                shroudarr[a] = a;
                Panelinputs[a] = a;
                stiff[a] = a;
            }
            int numtoremove = 1;
            int numtoremove1 = 2;
            shroudarr = shroudarr.Where(val => val != numtoremove).ToArray();
            Panelinputs = Panelinputs.Where(val => val != numtoremove1).ToArray();
            int stf2 = 3;
            stiff = stiff.Where(val => val != stf2).ToArray();

            MySheet mysheet = asfp.AddTopViews((MySheet)drawingdoc.ActiveSheet, model, drawingdoc, new Point2D(0, 0), true);
            mysheet = asfp.AddFrontViews((MySheet)drawingdoc.ActiveSheet, model, drawingdoc, new Point2D(0, 0), true, pmodel);
            mysheet = asfp.AddSideViews((MySheet)drawingdoc.ActiveSheet, model, drawingdoc, new Point2D(0, 0), true, pmodel);
            ViewBuilder vb = new ViewBuilder(model, drawingdoc);

            vb.DoWork();
            vb.AddTo(drawingdoc);

            drawingdoc.ActiveSheet = mysheet;
            #endregion addviews
            #region writefiles

            if (!Directory.Exists(projectpath + "/MFG Drawing"))
                Directory.CreateDirectory(projectpath + "/MFG Drawing");

            WriteAutodeskParams auto = new WriteAutodeskParams(drawingdoc);
            WriteAutodesk dwgg1 = new WriteAutodesk(auto, projectpath + "/MFG Drawing/MFG_Drawing.dwg");
            dwgg1.DoWork();
            #endregion writefiles

            return model;
        }

    }
}