using bullows.business;
using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bullows.Controllers
{
    public class PaintBoothController : BaseController
    {
        static int PaintID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        public PaintBoothController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public IActionResult PaintBooth(int id)
        {
            if (id > 0)
            {

            }
            else
            {
                SetPanelHeading("Paint Booth Inputs");
                SaveFlag = 0;
                if (PaintID == 1)
                    SetSuccessMessage("PaintBooth has been saved successfully");
                else if (PaintID == 2)
                    SetErrorMessage("PaintBooth has been deleted successfully");
                else if (PaintID < 0)
                    SetErrorMessage("Something went wrong while saving PaintBooth");
                PaintID = 0;
            }
            return View(new PaintBoothModel());
        }

        public IActionResult Search(string enquiryCode)
        {
            var enquiryDetails = _uow.PaintBoothRepository.GetEnquiryDetailsByCode(enquiryCode);
            if (enquiryDetails != null)
            {
                return Json(new { Length = enquiryDetails.LengthSize, Width = enquiryDetails.WidthSize, Height = enquiryDetails.HeightSize });
            }
            else
            {
                return Json(null);
            }
        }

        public IActionResult SavePaintBoothDetails(PaintBoothModel model, PaintBooth objtbl, int flag, PanelInputModel pmodel)
        {
            try
            {

                //PanelInput panel = new PanelInput();
                PaintBoothDesign panel = new PaintBoothDesign();
            panel.PanelWidth = pmodel.PanelWidth;
            panel.PanelHeight = pmodel.PanelHeight;
            panel.SheetThickness = pmodel.SheetThickness;
            panel.StandardBend1 = pmodel.StandardBend1;
            panel.StandardBend2 = pmodel.StandardBend2;
            panel.PitchDistance = pmodel.PitchDistance;
           
            double totalPanels = model.D / pmodel.PanelWidth;
            double noOfPanels = Math.Floor(totalPanels);
            double smallPanelWidth = model.D - ((noOfPanels) * pmodel.PanelWidth);
            List< DesignDocument> drawing = new List< DesignDocument>();
            DesignDocument assemblyDocument = new DesignDocument();//new
                int j = 0;
            
            for (int i = 0; i < noOfPanels; i++)
            {
               
                    j++;
                panel.PanelWidth = pmodel.PanelWidth;
                panel.PanelHeight = pmodel.PanelHeight;
                panel.SheetThickness = pmodel.SheetThickness;
                panel.StandardBend1 = pmodel.StandardBend1;
                panel.StandardBend2 = pmodel.StandardBend2;
                panel.PitchDistance = pmodel.PitchDistance;
                drawing.Add(panel.PanelsInPaintBooth(j)); 
                //DesignDocument panelDrawing = panel.SweepMethod(j);//new
                //    foreach (Entity entity in panelDrawing.Entities)
                //    {
                //        assemblyDocument.Entities.Add((Entity)entity.Clone());
                //    }
                }
                if (smallPanelWidth > 0)
                {
                    j++;
                    panel.PanelWidth = smallPanelWidth;
                    panel.PanelHeight = pmodel.PanelHeight;
                    panel.SheetThickness = pmodel.SheetThickness;
                    panel.StandardBend1 = pmodel.StandardBend1;
                    panel.StandardBend2 = pmodel.StandardBend2;
                    panel.PitchDistance = pmodel.PitchDistance;
                    drawing.Add(panel.PanelsInPaintBooth(j));

                    //   DesignDocument smallPanelDrawing = panel.SweepMethod(j);

                    //       // Import the small panel DWG into the assembly document
                    //       foreach (Entity entity in smallPanelDrawing.Entities)
                    //       {
                    //           assemblyDocument.Entities.Add((Entity)entity.Clone());
                    //       }
                    //   }

                    PaintID =_uow.PaintBoothRepository.SavePaintBooth(model,objtbl,flag);
                    //SaveAsAssembly(assemblyDocument, "C:/Bullows/bullows_drawing/Assembly " + DateTime.Now.ToString("hh-mm") + ".dwg");
                    SaveFlag = 1;
                }  
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("hello");
                Debug.WriteLine(ex.Message);
            }
            return RedirectToAction("PaintBooth");
        }

        //private void SaveAsAssembly(DesignDocument assemblyDocument, string filePath)
        //{
        //    // Write assembly document to file
        //    assemblyDocument.SaveFile(filePath);
        //}
                 



    }
}
