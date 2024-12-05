using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;


namespace Bullows.Repositories.Repositories
{
    public class PanelInputRepository : GenericRepository<PanelInputDetails>
    {
        private readonly ISession Session;
        public PanelInputRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        public int Save(PanelInputModel model, int flag, PanelInputDetails objtbl, int selectedProjectID)
        {
            try
            {

                if (flag == 1)
                {
                    objtbl.IsDeleted = false;
                    objtbl.ProjectID = model.ProjectID;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.SheetThickness = model.SheetThickness;
                   // objtbl.SlotDimentions = model.SlotDimentions;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.NoofPanels = 1;
                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                }
                else
                {
                    objtbl = _DbContext.PanelInputDetail.Find(model.ProjectID);
                    objtbl = new PanelInputDetails();
                    objtbl.IsDeleted = false;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.ProjectID = model.ProjectID;
                    objtbl.PanelHeight = model.PanelHeight;
                    objtbl.SheetThickness = model.SheetThickness;
                  //  objtbl.SlotDimentions = model.SlotDimentions;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.NoofPanels = model.NoofPanels;
                    _DbContext.PanelInputDetail.Add(objtbl);
                }
                _DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;

        }
        public int saveCutoutDetails(PanelInputModel model, PanelCutout tblobj)
        {
            int panelInpuId = GetPanelInputId();
            tblobj = new PanelCutout();
            tblobj.IsDeleted = false;
            tblobj.ProjectID = model.ProjectID;
            tblobj.PanelInputID = panelInpuId;
            tblobj.PartName = model.PartName;
            tblobj.CutoutLength = model.CutoutLength;
            tblobj.CutoutWidth = model.CutoutWidth;
            tblobj.CutoutXDistance = model.CutoutXDistance;
            tblobj.CutoutYDistance = model.CutoutYDistance;
            _DbContext.PanelCutouts.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }
        //        public PanelInputModel EditPanelModel(int id)
        //        {
        //            try
        //            {
        //                return _DbContext.PanelInputDetails
        //                                 .Where(x => x.PanelInputID == id && x.IsDeleted == false)
        //                                 .Select(x => new PanelInputModel()
        //                                 {
        //                                     PanelInputID = x.PanelInputID,
        //                                     PanelWidth = x.PanelWidth,
        //                                     PanelHeight = x.PanelHeight,
        //                                     SheetThickness = x.SheetThickness,
        //                                     StandardBend1 = x.StandardBend1,
        //                                     StandardBend2 = x.StandardBend2,
        //                                     PitchDistance = x.PitchDistance,
        //                                     NoofPanels = x.NoofPanels,
        //                                     SlotDimentions = x.SlotDimentions
        //                                 }).FirstOrDefault();

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        //       
        //        //}
        private int GetPanelInputId()
        {
            var panelInputId = _DbContext.PanelInputDetail.OrderByDescending(x => x.PanelInputID).Select(x => x.PanelInputID).FirstOrDefault();
            return panelInputId;
        }
       
    }
}
