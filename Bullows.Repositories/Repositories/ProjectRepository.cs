using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;

namespace Bullows.Repositories.Repositories
{
    public class ProjectRepository : GenericRepository<Projects>
    {
        private readonly ISession Session;
        public ProjectRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
       


        public int Save(ProjectModel model, int Flag, Projects objtbl)
        {
            try
            {
                if (Flag == 1)
                {
                    objtbl = _DbContext.Project.Find(model.ProjectID);
                    objtbl.IsDeleted = false;

                    //objtbl.ProjectCode = model.ProjectCode;
                    //objtbl.ProjectName = model.ProjectName;
                    //objtbl.CustomerName = model.CustomerName;
                    objtbl.ModifiedBy = 1;/*(int)Session.GetInt32("UserId");*/
                    objtbl.ModifiedDate = DateTime.Now;
                       
                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    
                }
                else
                {
                    objtbl = new Projects();
                    objtbl.ProjectName = model.ProjectName;
                    objtbl.ProjectCode = model.ProjectCode;
                    objtbl.CustomerName = model.CustomerName;
                    objtbl.IsDeleted = false;
                    objtbl.CreatedBy = 1;/* (int)Session.GetInt32("UserId");*/
                    objtbl.CreatedDate = DateTime.Now;
                    _DbContext.Project.Add(objtbl);
                }
               
                  _DbContext.SaveChanges();
                
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return 1;
        }
        public ProjectModel EditModel(int id)
        {
            try
            {
                return _DbContext.Project
                                 .Where(x => x.ProjectID == id && x.IsDeleted == false)
                                 .Select(x => new ProjectModel()
                                 {
                                     ProjectID = x.ProjectID,
                                     ProjectName = x.ProjectName,
                                     ProjectCode = x.ProjectCode,
                                     CustomerName = x.CustomerName,
                                    CreatedBy = x.CreatedBy != null ? (int)x.CreatedBy : 0,
                                   CreatedDate = x.CreatedDate != null ? (DateTime)x.CreatedDate : DateTime.MinValue

                                 }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ProjectModel> GetAllData()

        {
            
            List<ProjectModel> lstData = _DbContext.Project.Where(x=>x.IsDeleted==false).Select(item => new ProjectModel()
            {
                ProjectID = item.ProjectID,
                ProjectCode = item.ProjectCode,
                ProjectName = item.ProjectName,
                CustomerName= item.CustomerName,
            }).ToList();
            return lstData;
        }
        public int Delete(int id)
        {
            Projects objtbl = _DbContext.Project.Find(id);
            objtbl.IsDeleted = true;
            //objtbl.ModifiedBy = (int)Session.GetInt32("UserId");
           
            _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _DbContext.SaveChanges();
            return 2;
        }       
        public List<ProjectModel> GetprojectList()
        {
            List<ProjectModel> project = _DbContext.Project.OrderBy(x => x.ProjectName).Where(x => x.IsDeleted == false).Select(x => new ProjectModel
            {
                ProjectID = (int)x.ProjectID,
                ProjectName = x.ProjectName
            }).ToList();
            return project;
        }
        public List<ProjectModel> FillProjetcDropDown()
        {
            List<ProjectModel> lstdata = _DbContext.Project.Where(x => x.IsDeleted == false).Select(item => new ProjectModel()
            {
                ProjectID = item.ProjectID,
                ProjectName = item.ProjectName,
            }).ToList();
            return lstdata;
        }

        public List<PanelInputModel> GetRelatedPanelInputs(int ProjectID)
        {
            List<PanelInputModel> panelInputs = _DbContext.PanelInputDetails
                .Where(pi => pi.ProjectID == ProjectID && pi.IsDeleted == false)
                .Select(pi => new PanelInputModel()
                {
                    PanelInputID = pi.PanelInputID,
                })
                .ToList();

            return panelInputs;
        }
        public List<PanelInputModel> FillPanelInputsDropDown(int ProjectID)
        {
            return GetRelatedPanelInputs(ProjectID);
        }
    }
}
