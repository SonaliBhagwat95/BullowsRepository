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
using Bullows.Database.Migrations;

namespace Bullows.Repositories.Repositories
{
    public class PanelInputRepository : GenericRepository<PanelInputDetail>
    {
        private readonly ISession Session;
        public PanelInputRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        public int Save(PanelInputModel model, int flag, PanelInputDetail objtbl, int selectedProjectID)
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
                    objtbl.SlotDimentions = model.SlotDimentions;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.NoofPanels = 1;
                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                }
                else
                {
                    objtbl = _DbContext.PanelInputDetails.Find(model.ProjectID);
                    objtbl = new PanelInputDetail();
                    objtbl.IsDeleted = false;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.ProjectID = model.ProjectID;
                    objtbl.PanelHeight = model.PanelHeight;
                    objtbl.SheetThickness = model.SheetThickness;
                    objtbl.SlotDimentions = model.SlotDimentions;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.NoofPanels = model.NoofPanels;
                    _DbContext.PanelInputDetails.Add(objtbl);
                }
                _DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;

        }
        public int saveCutoutDetails(PanelInputModel model,PanelCutout tblobj)
        {
            int panelInpuId = GetPanelInputId();
            tblobj = new PanelCutout();
            tblobj.IsDeleted= false;
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
        public PanelInputModel EditPanelModel(int id)
        {
            try
            {
                return _DbContext.PanelInputDetails
                                 .Where(x => x.PanelInputID == id && x.IsDeleted == false)
                                 .Select(x => new PanelInputModel()
                                 {
                                     PanelInputID = x.PanelInputID,
                                     PanelWidth = x.PanelWidth,
                                     PanelHeight = x.PanelHeight,
                                     SheetThickness = x.SheetThickness,
                                     StandardBend1 = x.StandardBend1,
                                     StandardBend2 = x.StandardBend2,
                                     PitchDistance = x.PitchDistance,
                                     NoofPanels = x.NoofPanels,
                                     SlotDimentions = x.SlotDimentions
                                 }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<PanelInputModel> GetAllData()
        //{
        //    List<PanelInputModel> lstdata = _DbContext.Panel_TubeLightDetails.Where(c => c.IsDeleted == false).Select(item => new PanelInputModel()
        //    {
        //        PanelInputID = item.PanelInputID,
        //        PartDescription = item.PartName==1? "TubeLight":
        //        item.PartName == 2 ? "ServiceDoor" :
        //        item.PartName == 3 ? "WatchGlass" : "DuctCutouts",
        //        TubelightHeight = item.TubelightHeight,
        //        TubelightWidth = item.TubelightWidth,
        //        XDistance = item.XDistance,
        //        YDistance = item.YDistance,

        //    }).ToList();
        //    return lstdata;
        //}
        //public List<PanelInputModel> GetAllServiceData()
        //{
        //    List<PanelInputModel> lstdata = _DbContext.Panel_ServiceDoorDetails.Where(c => c.IsDeleted == false).Select(item => new PanelInputModel()
        //    {
        //        PanelInputID = item.PanelInputID,
        //        PartDescription = item.PartName == 1 ? "TubeLight" :
        //        item.PartName == 2 ? "ServiceDoor" :
        //        item.PartName == 3 ? "WatchGlass" : "DuctCutouts",
        //        ServiceDoorWidth = item.ServiceDoorWidth,
        //        ServiceDoorHeight = item.ServiceDoorHeight,
        //        XDistance = item.DoorXDistance,
        //        YDistance = item.DoorYDistance,
            
        //    }).ToList();
        //    return lstdata;
        //}

        //public List<PanelInputModel>GetWatchGlassData()
        //{
        //    List<PanelInputModel> lstdata = _DbContext.Panel_WatchGlassDetails.Where(x => x.IsDeleted == false).Select(item => new PanelInputModel()
        //    {
        //        PanelInputID = item.PanelInputID,
        //        PartDescription = item.PartName == 1 ? "TubeLight" :
        //        item.PartName == 2 ? "ServiceDoor" :
        //        item.PartName == 3 ? "WatchGlass" : "DuctCutouts",
        //        GlassLength = item.GlassLength,
        //        GlassWidth = item.GlassWidth,
        //        XDistance = item.DoorXDistance,
        //        YDistance = item.DoorYDistance,
        //    }).ToList();
        //    return lstdata;
        //}
        //public List<PanelInputModel> GetDuctCutoutData()
        //{
        //    List<PanelInputModel> lstdata = _DbContext.Panel_DuctCutoutDetails.Where(x => x.IsDeleted == false).Select(item => new PanelInputModel()
        //    {
        //        PanelInputID=item.PanelInputID,
        //        PartDescription = item.PartName == 1 ? "TubeLight" :
        //        item.PartName == 2 ? "ServiceDoor" :
        //        item.PartName == 3 ? "WatchGlass" : "DuctCutouts",
        //        CutoutLength = item.CutoutLength,
        //        CutoutWidth = item.CutoutWidth,
        //        XDistance = item.CutoutXDistance,
        //        YDistance = item.CutoutYDistance,

        //    }).ToList();
        //    return lstdata;
        //}
        private int GetPanelInputId()
        {
            var panelInputId = _DbContext.PanelInputDetails.OrderByDescending(x => x.PanelInputID).Select(x => x.PanelInputID).FirstOrDefault();
            return panelInputId;
        }
        //public int saveTubeLightDetails(Panel_TubeLightDetail obj, int flag, string[] selectedPart)
        //{
        //    try
        //    {
        //        int panelInpuId = GetPanelInputId();
        //        if (flag == 1)
        //        {
        //            //obj.IsDeleted = false;
        //            //obj.TubeLightID = TubeLightID;

        //            //PartName = selectedPart.PartName;
        //            //obj.Type = selectedPart.Type;
        //            //obj.TubelightWidth = model.TubelightWidth;
        //            //obj.TubelightHeight = model.TubelightHeight;
        //            //obj.XDistance = model.XDistance;
        //            //obj.YDistance = model.YDistance;
        //            //_DbContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //        }
        //        else
        //        {
        //            obj = new Panel_TubeLightDetail();
        //            obj.PanelInputID = panelInpuId;
        //            obj.PartName = int.Parse(selectedPart[0]);
        //            obj.Type = int.Parse(selectedPart[1]);
        //            obj.TubelightWidth = int.Parse(selectedPart[2]);
        //            obj.TubelightHeight = int.Parse(selectedPart[3]);
        //            obj.XDistance = int.Parse(selectedPart[4]);
        //            obj.YDistance = int.Parse(selectedPart[5]);
        //            _DbContext.Panel_TubeLightDetails.Add(obj);
        //        }
        //        _DbContext.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return 1;
        //}
        //public int saveServiceDoor(Panel_ServiceDoorDetail objtbl, int flag, string[] selectedPart)
        //{
        //    try
        //    {
        //        int panelInpuId = GetPanelInputId();
        //        if (flag == 1)
        //        {
        //            //objtbl.IsDeleted = false;
        //            //objtbl.PanelInputID = panelInpuId;
        //            //objtbl.PartName = model.ProjectName;
        //            //objtbl.Type = model.Type;
        //            //objtbl.ServiceDoorHeight = model.ServiceDoorHeight;
        //            //objtbl.ServiceDoorWidth = model.ServiceDoorWidth;
        //            //objtbl.GlassLength = model.GlassLength;
        //            //objtbl.GlassWidth = model.GlassWidth;
        //            //objtbl.DoorXDistance = model.DoorXDistance;
        //            //objtbl.DoorYDistance = model.DoorYDistance;
        //            //_DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //        }
        //        else
        //        {
        //            objtbl = new Panel_ServiceDoorDetail();
        //            objtbl.PanelInputID = panelInpuId;
        //            objtbl.PartName = int.Parse(selectedPart[0]);
        //            objtbl.Type = int.Parse(selectedPart[1]);
        //            objtbl.ServiceDoorHeight = int.Parse(selectedPart[2]);
        //            objtbl.ServiceDoorWidth = int.Parse(selectedPart[3]);
        //            objtbl.GlassLength = int.Parse(selectedPart[4]);
        //            objtbl.GlassWidth = int.Parse(selectedPart[5]);
        //            objtbl.DoorXDistance = int.Parse(selectedPart[6]);
        //            objtbl.DoorYDistance = int.Parse(selectedPart[7]);
        //            _DbContext.Panel_ServiceDoorDetails.Add(objtbl);
        //        }
        //        _DbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return 1;
        //}
        //public int saveWatchDetails(Panel_WatchGlassDetail objtbl1, int flag, string[] selectedPart)
        //{
        //    try
        //    {
        //        int panelInpuId = GetPanelInputId();
        //        if (flag == 1)
        //        {
        //            //objtbl.IsDeleted = false;
        //            //objtbl.PanelInputID = panelInpuId;
        //            //objtbl.PartName = model.ProjectName;
        //            //objtbl.Type = model.Type;
        //            //objtbl.ServiceDoorHeight = model.ServiceDoorHeight;
        //            //objtbl.ServiceDoorWidth = model.ServiceDoorWidth;
        //            //objtbl.GlassLength = model.GlassLength;
        //            //objtbl.GlassWidth = model.GlassWidth;
        //            //objtbl.DoorXDistance = model.DoorXDistance;
        //            //objtbl.DoorYDistance = model.DoorYDistance;
        //            //_DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //        }
        //        else
        //        {
        //            objtbl1 = new Panel_WatchGlassDetail();
        //            objtbl1.PanelInputID = panelInpuId;
        //            objtbl1.PartName = int.Parse(selectedPart[0]);
        //            objtbl1.Type = int.Parse(selectedPart[1]);
        //            objtbl1.GlassLength = int.Parse(selectedPart[2]);
        //            objtbl1.GlassWidth = int.Parse(selectedPart[3]);
        //            objtbl1.DoorXDistance = int.Parse(selectedPart[4]);
        //            objtbl1.DoorYDistance = int.Parse(selectedPart[5]);
        //            _DbContext.Panel_WatchGlassDetails.Add(objtbl1);
        //        }
        //        _DbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return 1;

        //}

        //public int SaveDuctCutout(Panel_DuctCutoutDetail obj1, int flag, string[] selectedPart)
        //{
        //    try
        //    {
        //        int panelInpuId = GetPanelInputId();
        //        if (flag==1)
        //        {

        //        }
        //        else
        //        {
        //            obj1 = new Panel_DuctCutoutDetail();
        //            obj1.PanelInputID = panelInpuId;
        //            obj1.PartName = int.Parse(selectedPart[0]);
        //            obj1.Type = int.Parse(selectedPart[1]);
        //            obj1.CutoutLength = int.Parse(selectedPart[2]);
        //            obj1.CutoutWidth = int.Parse(selectedPart[3]);
        //            obj1.CutoutXDistance = int.Parse(selectedPart[4]);
        //            obj1.CutoutYDistance = int.Parse(selectedPart[5]);
        //            _DbContext.Panel_DuctCutoutDetails.Add(obj1);
        //        }
        //        _DbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return 1;
        //}

        public PanelInputModel Get( int PanelInputID)
        {
            if (PanelInputID <= 0)
                throw new Exception("PanelInputID not found in Panel");
            var entity =_DbContext.PanelInputDetails.Where(x => x.PanelInputID == PanelInputID).FirstOrDefault();
           
            if (entity.PanelInputID == null)
                throw new Exception("Invalid PanelInputID");
            else
                return JsonConvert.DeserializeObject<PanelInputModel>(entity.PanelInputID.ToString());
        }

        
      

        //public PanelInputModel GetPanalIdAndDuctId(int PanelInputID,int DuctCutoutID)
        //{
        //    var result = (from p in _DbContext.PanelInputDetails
        //                  join d in _DbContext.Panel_DuctCutoutDetails
        //                  on p.PanelInputID equals d.PanelInputID
        //                  where p.PanelInputID == PanelInputID && d.DuctCutoutID == DuctCutoutID
        //                  select new
        //                  {
        //                      p.PanelInputID,
        //                      p.PanelWidth,
        //                      p.PanelHeight,
        //                      p.SheetThickness,
        //                      p.StandardBend1,
        //                      p.StandardBend2,
        //                      d.DuctCutoutID,
        //                      d.PartName,
        //                      d.CutoutLength,
        //                      d.CutoutWidth,
        //                      d.CutoutXDistance,
        //                      d.CutoutYDistance,
        //                      d.Type
        //                  }).OrderByDescending(x => x.PanelInputID & DuctCutoutID).Select(item => new PanelInputModel()
        //                  {
        //                      PanelInputID = item.PanelInputID,
        //                      PanelWidth=item.PanelWidth,
        //                      PanelHeight=item.PanelHeight,
        //                      SheetThickness=item.SheetThickness,
        //                      StandardBend1=item.StandardBend1,
        //                      StandardBend2=item.StandardBend2,
        //                      DuctCutoutID=item.DuctCutoutID,
        //                      PartName=item.PartName,
        //                      CutoutLength=item.CutoutLength,
        //                      CutoutWidth=item.CutoutWidth,


        //                  }).ToList();
        //}

    }
}
