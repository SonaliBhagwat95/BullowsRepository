using Bullows.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Bullows.Repositories.Repositories;
using Bullows.Model;
using System.Data;
using Bullows.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bullows.Business;
using devDept.Eyeshot;



namespace Bullows.Controllers
{
    public class PanelInputController : BaseController
    {
        static int PanelID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        private readonly DataTable TableBoundingBox;
        private int TubeLightID;
        public PanelInputController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
            TableBoundingBox = new DataTable();
        }
        public IActionResult PanelInput(int id = 0,int ProjectID=0)
        {
            ViewBag.Project = new SelectList(_uow.projectRepository.FillProjetcDropDown(), "ProjectID", "ProjectName");
           ViewBag.Panel = new SelectList(_uow.projectRepository.FillPanelInputsDropDown(ProjectID), "PanelInputID");
            if (id > 0)
            {
                //SaveFlag = 1;
                //SetPanelHeading("Edit panelInput details");
                //var data = _uow.PanelInputRepository.EditPanelModel(id);
                //if (data == null)
                //    return HttpNotFound();
                //else
                //    return View(data);
            }
            else
            {
                ViewBag.Projects = _uow.projectRepository.GetprojectList();
               
                SetPanelHeading("PanelInput Details");
                SaveFlag = 0;
                if (PanelID == 1)
                    SetSuccessMessage("PanelInput has been saved successfully");
                else if (PanelID == 2)
                    SetErrorMessage("PanelInput has been deleted successfully");
                else if (PanelID < 0)
                    SetErrorMessage("Something went wrong while saving PanelInput");
                PanelID = 0;
            }
            return View(new PanelInputModel());

        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult SavePanelInput(PanelInputModel model, int Flag, PanelInputDetail objtbl,int selectedProjectID /*PanelInput objimpdraw*/, DesignDocument model1,PanelCutout tblobj)
        {
           
            try
            {
               
                int flag = 0;
                flag = SaveFlag;
                model.ProjectID = selectedProjectID;
                var Panelinput = UowBusiness.panelInput;
                Panelinput.PanelWidth = model.PanelWidth;
                Panelinput.PanelHeight = model.PanelHeight;
                Panelinput.SheetThickness = model.SheetThickness;
                Panelinput.StandardBend1 = model.StandardBend1;
                Panelinput.StandardBend2 = model.StandardBend2;
                Panelinput.CutoutLength = model.CutoutLength;
                Panelinput.CutoutWidth = model.CutoutWidth;
                Panelinput.CutoutXDistance = model.CutoutXDistance;
                Panelinput.CutoutYDistance = model.CutoutYDistance;
                Panelinput.PitchDistance = model.PitchDistance;

                UowBusiness.panelInput.SweepMethod(model);
              
                PanelID = _uow.PanelInputRepository.Save(model, flag, objtbl, selectedProjectID);
                if (PanelID > 0)
                {
                    _uow.PanelInputRepository.saveCutoutDetails(model, tblobj);
                }
                SaveFlag = 1;
            }
            catch(Exception ex)
            {
               
                // Enhanced logging
                ViewBag.Message = $"An error occurred: {ex.Message}";
                // Log detailed error information to a file or logging system
                System.IO.File.WriteAllText("C:/path_to_logs/error.log", ex.ToString());
            }
                      
            return RedirectToAction("PanelInput");
        }
        
        public JsonResult GetPanelInputs(int projectId)
        {
            var panelInputs = _uow.projectRepository.FillPanelInputsDropDown(projectId);
            var panelInputIds = panelInputs.Select(pi => pi.PanelInputID).ToList();
            return Json(panelInputIds);
        }
       

    }
    
}
