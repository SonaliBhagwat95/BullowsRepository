//using Bullows.Database;
//using Bullows.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata.Ecma335;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace Bullows.Repositories.Repositories
//{
//    public class CostingRepository : GenericRepository<PanelDetails>
//    {
//        private readonly ISession Session;
//        public CostingRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
//        {
//            this._DbContext = context;
//            this.Session = httpContextAccessor.HttpContext.Session;
//        }

//        public List<PaintBoothModel> GetAllByEnquiryID(string  EnquiryCode)
//        {
//            List<PaintBoothModel> detailsList = new List<PaintBoothModel>();
//            detailsList = (from e in _DbContext.EnquiryMasters
//                           join p in _DbContext.PanelDetails
//                           on e.EnquiryID equals p.EnquiryId
//                           join s in _DbContext.SettingDetails
//                           on e.EnquiryID equals s.EnquiryID
//                           where e.SalesNO == EnquiryCode
//                           select new PaintBoothModel()
//                           {
//                               EnquiryId= p.EnquiryId,
//                               PanelPosition= p.PanelPosition,
//                               EqualPanelWidthForD=p.StandardPanelDepth,
//                               EqualPanelWidthByH =p.StandardPanelHeight,
//                               FrameWidth=p.FrameWidth,
//                               FrameHeight=p.FrameHeight,
//                               SheetThickness= p.SheetThickness,
//                               NoofPanels= p.NoOfPanels,
//                               PanelWeight=p.PanelWeight,
//                               Section=s.Section
//                           }).ToList();
//            return detailsList;
//        }


//        public List<string> SearchEnquiryCodes(string searchTerm)
//        {
//            List<string> detailsList = (from e in _DbContext.EnquiryMasters
//                                        join p in _DbContext.PaintBoothDetails on e.EnquiryID equals p.EnquiryId
//                                        where e.SalesNO.Contains(searchTerm)
//                                        select e.SalesNO).Distinct().ToList();
//            return detailsList;
//        }

//        public int GetCostingCountByEnquiryID()
//        {

//            return _DbContext.PanelDetails.Select(c => c.EnquiryId).Distinct().Count();

//        }
//    }
//}
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bullows.Repositories.Repositories
{
    public class CostingRepository : GenericRepository<PanelDetails>
    {
        private readonly ISession Session;
        public CostingRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public List<PaintBoothModel> GetAllByEnquiryID(string EnquiryCode)
        {
            List<PaintBoothModel> detailsList = new List<PaintBoothModel>();
            detailsList = (from e in _DbContext.EnquiryMasters
                           join p in _DbContext.PanelDetails
                           on e.EnquiryID equals p.EnquiryId
                           join s in _DbContext.SettingDetails
                           on e.EnquiryID equals s.EnquiryID
                           where e.SalesNO == EnquiryCode
                           select new PaintBoothModel()
                           {
                               EnquiryId = p.EnquiryId,
                               PanelPosition = p.PanelPosition,
                               EqualPanelWidthForD = p.StandardPanelDepth,
                               EqualPanelWidthByH = p.StandardPanelHeight,
                               FrameWidth = p.FrameWidth,
                               FrameHeight = p.FrameHeight,
                               SheetThickness = p.SheetThickness,
                               NoofPanels = p.NoOfPanels,
                               PanelWeight = p.PanelWeight,
                               Section = s.Section
                           }).ToList();
            return detailsList;
        }


        public List<string> SearchEnquiryCodes(string searchTerm)
        {
            List<string> detailsList = (from e in _DbContext.EnquiryMasters
                                        join p in _DbContext.PaintBoothDetails on e.EnquiryID equals p.EnquiryId
                                        where e.SalesNO.Contains(searchTerm)
                                        select e.SalesNO).Distinct().ToList();
            return detailsList;
        }
        public List<MaterialModel> GetAllFrameDetails(string EnquiryCode)
        {
            List<MaterialModel> lstData = _DbContext.FilterFrameDetails
                .Where(x => x.IsDeleted == false && x.SalesNO == EnquiryCode)
                .Select(item => new MaterialModel()
                {
                    FID = item.FID,
                    Width = (int)item.FrameWidth,
                    Height = (int)item.FrameHeight,
                    Quantity = item.Quantity,

                }).ToList();
            return lstData;
        }
        public List<SettingModel> GetAlllightDetails(string EnquiryCode)
        {
            var lstData = _DbContext.TubeLightDetails
                .Where(x => x.IsDeleted == false && x.SalesNo == EnquiryCode)
                .Select(item => new SettingModel()
                {
                    LightTypes = item.LightType,
                    LightSubTypes = item.LightSubType,
                    LuxLevel = (decimal)item.LuxLevel,
                    Lumens = (decimal)item.Lumens,
                    Quantity = item.Quantity

                }).ToList();
            return lstData;
        }
        public List<PaintBoothModel> GetAllBlowerDetails(string EnquiryCode)
        {

            var lstData = (from paintBooth in _DbContext.PaintBoothDetails
                           join enquiry in _DbContext.EnquiryMasters
                           on paintBooth.EnquiryId equals enquiry.EnquiryID
                           join motor in _DbContext.MotorDetails
                           on enquiry.EnquiryID equals motor.EnquiryID
                           join motorFlange in _DbContext.tblMotorFlange
                           on motor.MotorCatalogID equals motorFlange.MotorCatalogID
                           where enquiry.SalesNO == EnquiryCode && paintBooth.IsDeleted == false && motor.IsDeleted == false
                           select new PaintBoothModel
                           {
                               CapacityofBlowerinH = paintBooth.CapacityofBlowerinHr,
                               CapacityofBlowerRoundOf = (double)paintBooth.RoundingCapacity,
                               RatedOutputHP = (decimal)motorFlange.RatedOutputHP // Adding motor data
                           }).ToList();

            return lstData;
        }
        public int GetCostingCountByEnquiryID()
        {

            return _DbContext.PanelDetails.Select(c => c.EnquiryId).Distinct().Count();

        }
    }
}

