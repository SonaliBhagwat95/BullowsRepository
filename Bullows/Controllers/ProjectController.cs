using Bullows.Models;
using Bullows.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Bullows.Repositories.Repositories;
using System.Diagnostics;
using Bullows.Model;
using Bullows.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bullows.Controllers
{
    public class ProjectController : BaseController
    {
       static int PID = 0; static int SaveFlag = 0;static int pageIndex = 0;
        //private readonly ILogger<ProjectController> _logger;
        //public ProjectController(ILogger<ProjectController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        public ProjectController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Project(int id = 0)
        {
            ViewBag.GridData = _uow.projectRepository.GetAllData();
            if(id>0)
            {
                SaveFlag = 1;
                SetPanelHeading("Edit Project Details");
                var data = _uow.projectRepository.EditModel(id);
                if (data == null)
                    return HttpNotFound();
                else
                    return View(data);
            }
            else
            {
                SetPanelHeading("Project Details");
                SaveFlag = 0;
                if (PID == 1)
                    SetSuccessMessage("Project has been saved successfully");
                else if (PID == 2)
                    SetErrorMessage("Project has been deleted successfully");
                else if (PID < 0)
                    SetErrorMessage("Something went wrong while saving Project");
                PID = 0;

            }
            return View( new ProjectModel());
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SaveProject(ProjectModel model, int Flag, Projects objtbl)
        {
            
            int flag = 0;
            flag = SaveFlag;
            PID = _uow.projectRepository.Save(model, flag, objtbl);
            SaveFlag = 1;
           

            return RedirectToAction("Project");
        }
        public IActionResult Delete(int id =0)
        {
            PID = _uow.projectRepository.Delete(id);
            return RedirectToAction("Project");
            PID = 2;
        }

    }
}