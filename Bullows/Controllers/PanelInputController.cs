using Bullows.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Bullows.Repositories.Repositories;
using Bullows.Model;
using System.Data;
using Bullows.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bullows.Business;
using devDept.Eyeshot;
using System.IO.Compression;


namespace Bullows.Controllers
{
    public class PanelInputController : BaseController
    {
        static int PanelID = 0; static int SaveFlag = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        private readonly DataTable TableBoundingBox;
       
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
            ViewBag.ActivePage = "PanelInput";
            ViewBag.SuccessText = "";

            return View(new PanelInputModel());

        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]   
       public IActionResult SavePanelInput(PanelInputModel model, int selectedProjectID, DesignDocument model1, PanelInputDetails objtbl)
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

            // Generate the file paths
            string generatedFilePath = UowBusiness.panelInput.SweepMethod(model);
            string developmentPath = UowBusiness.panelInput.Development(model);
                //  string AllViewsPath = UowBusiness.panelInput.detailsdrawing(model1, model);
                PanelID = _uow.PanelInputRepository.Save(model, flag, objtbl, selectedProjectID);
                // Create the zip file in memory
                using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Add the first file to the zip
                    archive.CreateEntryFromFile(generatedFilePath, Path.GetFileName(generatedFilePath));

                    // Add the second file to the zip
                    archive.CreateEntryFromFile(developmentPath, Path.GetFileName(developmentPath));
                }

                // Reset the memory stream position to the beginning
                memoryStream.Position = 0;

                // Return the zip file as a downloadable file
                return File(memoryStream.ToArray(), "application/zip", "PanelDrawings.zip");
            }
        }
        catch (Exception ex)
        {
            // Handle any errors that may occur
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


        public JsonResult GetPanelInputs(int projectId)
        {
            var panelInputs = _uow.projectRepository.FillPanelInputsDropDown(projectId);
            var panelInputIds = panelInputs.Select(pi => pi.PanelInputID).ToList();
            return Json(panelInputIds);
        }      

    }
}

    
    

