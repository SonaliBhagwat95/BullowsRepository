//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using System.Drawing;
//using devDept.Eyeshot;
//using devDept.Eyeshot.Entities;
//using devDept.Eyeshot.Translators;
//using devDept.Geometry;
//using devDept.Graphics;
//using System.IO;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using System.Diagnostics;
//using System.ComponentModel;
//using devDept;
//using System.Drawing.Printing;
//using Newtonsoft.Json.Converters;
//using devDept.Eyeshot.Milling;
//using System.Xml;
//using System.Security.Cryptography;

//namespace Bullows.Business
//{
//    public class DetailingForPanelInput
//    {
//        Point2D Pmax = new Point2D();
//        static string path = string.Empty;
//        public DesignDocument DetailsDrawing(DesignDocument model)
//        {
//          //var getPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FolderPathConfig")["AbsolutePath"];
//           //path = getPath.ToString();
//        //    string partName = "Impeller";
//            model.Units = linearUnitsType.Millimeters;
//            AddSheet asfp = new AddSheet();
//            // Creates a drawing document and adds sheets and views
//            DrawingDocument drawingDoc = new DrawingDocument();
//            //string DriveType = objimpdraw.DriveType;

//            //drawingDoc = asfp.AddSheets(partName, DriveType);

//            Point3D bx = model.Entities.BoxSize;
//            Point3D boxSize = new Point3D(bx.X, bx.Z, bx.Y);
//            double sv = 0;
//            if (boxSize.X <= 100)
//                sv = 20;
//            if (boxSize.Y <= 100)
//                sv = 20;
//            if (sv != 20)
//            {
//                if (boxSize.X >= boxSize.Y)
//                    sv = boxSize.X / 28;
//                if (boxSize.X < boxSize.Y)
//                    sv = boxSize.Y / 28;
//            }

//            // Adds some views
//            //ShroudArr = new int[model.Entities.Count];
//            //BackPltArr = new int[model.Entities.Count];
//            //Stiff = new int[model.Entities.Count];
//            //for (int a = 0; a < model.Entities.Count; a++)
//            //{
//            //    ShroudArr[a] = a;
//            //    BackPltArr[a] = a;
//            //    Stiff[a] = a;
//            //}
//            //int numToRemove = 1;
//            //int numToRemove1 = 2;
//            //ShroudArr = ShroudArr.Where(val => val != numToRemove).ToArray();
//            //BackPltArr = BackPltArr.Where(val => val != numToRemove1).ToArray();
//            //int stf2 = 3;
//            //Stiff = Stiff.Where(val => val != stf2).ToArray();

           
//            MySheet mysheet = asfp.AddTopViews((MySheet)drawingDoc.ActiveSheet, model, drawingDoc, new Point2D(0, 0), false);
//            mysheet = asfp.AddFrontViews((MySheet)drawingDoc.ActiveSheet, model, drawingDoc, new Point2D(0, 0), true);
//            mysheet = asfp.AddIsometricViews((MySheet)drawingDoc.ActiveSheet, model, drawingDoc, new Point2D(0, 0));


           
//            mysheet = ChnageInsertionOfviews(mysheet);

           
//            VectorView sideView = (VectorView)mysheet.Entities[3];
//            BlockReference blkbasic = (BlockReference)mysheet.Entities[6];
//            Point2D Pmax = new Point2D();
//            Pmax.X =sideView.InsertionPoint.X + sideView.BoxSize.X/2 / 2 + 100;
//            Pmax.Y = sideView.InsertionPoint.Y + sideView.InsertionPoint.Y / 2 +100;
//            Block block;
//            double scalefac = asfp.forscalefactor(Pmax);
//            BlockReference br = CreateFormatBlock(mysheet, out block, drawingDoc);
//            //br.Scale(3);
//            br.InsertionPoint = new Point3D(0, 0, 0);
//            drawingDoc.ActiveSheet.Entities.Add(br);
//            drawingDoc.Blocks.Add(block);
//            mysheet.Entities.Add(br);

//            // Creates the view builder
//            ViewBuilder vb = new ViewBuilder(model, drawingDoc);

//            BackgroundWorker bgw = new BackgroundWorker()
//            {
//                WorkerSupportsCancellation = true,
//                WorkerReportsProgress = true
//            };

//            bgw.DoWork += (sender, eventArgs) =>
//            {
//                ((WorkUnit)eventArgs.Argument).DoWork(bgw, eventArgs);
//            };

//            bgw.ProgressChanged += (sender, eventArgs) =>
//            {
//                Console.Write(".");
//            };

//            bgw.RunWorkerCompleted += (sender, eventArgs) =>
//            {

//                vb.AddTo(drawingDoc);

//                drawingDoc.ActiveSheet = mysheet;

//            };

//            bgw.RunWorkerAsync(vb);

//            return model;
//        }


//        public MySheet ChnageInsertionOfviews(MySheet mysheet)
//        {            
//            VectorView topView = (VectorView)mysheet.Entities[0];
//            VectorView frontView = (VectorView)mysheet.Entities[1];
//            VectorView sideView = (VectorView)mysheet.Entities[2];
//            topView.InsertionPoint = new Point3D(0,0, 0);
//            frontView.InsertionPoint = new Point3D(topView.InsertionPoint.X, topView.InsertionPoint.Y + topView.BoxSize.Y / 2);
//            sideView.InsertionPoint = new Point3D(frontView.InsertionPoint.X+frontView.BoxSize.X/2, frontView.InsertionPoint.Y);
//            return mysheet;
//        }
//        private readonly Dictionary<string, string> _formatBlockNames = new Dictionary<string, string>();
//        public BlockReference CreateFormatBlock(Sheet sheet, out Block block, DrawingDocument drawings)
//        {
//            if (_formatBlockNames.ContainsKey(sheet.Name))
//            {
//                drawings.Blocks.Remove(_formatBlockNames[sheet.Name]); // removes both block and related block reference.          
//                _formatBlockNames.Remove(sheet.Name);
//            }

//            BlockReference br = sheet.BuildA4LANDSCAPEISO(out block);
//            _formatBlockNames.Add(sheet.Name, br.BlockName); // adds format block name to the dictionary

//            // Initializes attributes
            
//            AddSheet asfp = new AddSheet();
//            br.Scale(asfp.forscalefactor(Pmax));
//            br.InsertionPoint = new Point3D(0, 0, 0);
//            return br;
//        }

//    }
//}
