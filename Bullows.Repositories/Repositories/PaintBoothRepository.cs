//using bullows.business;
//using bullows.database;
//using Bullows.Database;
//using Bullows.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Xbim.IO.Xml.BsConf;

//namespace Bullows.Repositories.Repositories
//{
//    public class PaintBoothRepository : GenericRepository<PaintBooth>
//    {
//        private readonly ISession Session;
//        public PaintBoothRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
//        {
//            this._DbContext = context;
//            this.Session = httpContextAccessor.HttpContext.Session;
//        }
//        static double totalPanelsforD;
//        static double smallPanelWidthforD;
//        static int noOfPanelsforD;
//        public EnquiryModel GetEnquiryDetailsByCode(string enquiryCode)
//        {
//            var enquiryDetails = (from enquiry in _DbContext.EnquiryMasters
//                                  join component in _DbContext.ComponentTables
//                                  on enquiry.ComponentID equals component.ComponentID
//                                  where enquiry.SalesNO == enquiryCode
//                                  select new EnquiryModel()
//                                  {
//                                      EnquiryId=enquiry.EnquiryID,
//                                     // SalesNO=enquiry.SalesNO,
//                                      LengthSize = component.Length,
//                                      WidthSize = component.WidthSize,
//                                      HeightSize = component.HeightSize
//                                  }).FirstOrDefault();

//            return enquiryDetails;
//        }
//        public bool Exists(int enquiryId)
//        {
//            return _DbContext.PaintBoothDetails
//                .Any(pb => pb.EnquiryId == enquiryId && pb.designStatus == true);
//        }
//        public int SavePaintBooth(PaintBoothModel model,PanelInputModel pmodel, int flag,EnquiryModel enquiry)
//        {
//            PaintBoothDetails objtbl = new PaintBoothDetails();
//            try
//            {
//                if (flag == 1)
//                {

//                }
//                else
//                {
//                    objtbl = new PaintBoothDetails();
//                    objtbl.IsDeleted = false;
//                    objtbl.EnquiryId = enquiry.EnquiryId;
//                    objtbl.D1 = model.D1;
//                    objtbl.D2 = model.D2;
//                    objtbl.D3 = model.D3;
//                    objtbl.W1 = model.W1;
//                    objtbl.W2 = model.W2;
//                    objtbl.W3 = model.W3;
//                    objtbl.D = model.D1 + model.D2 + model.Depth;
//                    objtbl.W = model.W1 + model.W2 + model.Width;
//                    objtbl.H = model.H1 + model.H2+model.Height;
//                    objtbl.H1 = model.H1;
//                    objtbl.H2 = model.H2;
//                    objtbl.CrossSectionalAreaOfBlower = model.CrossSectionalArea;
//                    objtbl.VelocityofBlower = model.VelocityofBlower;
//                    objtbl.CapacityofBlowerincubicm = model.CapacityofBlower;
//                    objtbl.CapacityofBlowerinHr = model.CapacityofBlowerinH;                  
//                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
//                    objtbl.CreatedDate = DateTime.Now;
//                    objtbl.ModifiedBy = 0;
//                    objtbl.designStatus = true;
//                    _DbContext.PaintBoothDetails.Add(objtbl);
//                }
//                _DbContext.SaveChanges();

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return 1;
//        }
//        public void SaveMotorDeatils(int enquiryID,int MotorCatalogID)
//        {
//            MotorDetails objtbl=new MotorDetails();
//            objtbl.EnquiryID = enquiryID;
//            objtbl.MotorCatalogID = MotorCatalogID;
//            objtbl.IsDeleted= false;
//            _DbContext.MotorDetails.Add(objtbl);
//            _DbContext.SaveChanges();
//        }
//        public PaintBoothModel GetSettingDetailsByCode(string enquiryCode)
//        {
//            var paintBoothModel = new PaintBoothModel
//            {
//                Settings = _DbContext.SettingDetails
//                                   .Where(s => s.SalesNO == enquiryCode)
//                                   .Select(s => new SettingModel
//                                   {
//                                       SId = s.SId,
//                                       SalesNo = s.SalesNO,
//                                       PanelWidth = s.PanelWidth,
//                                       PanelHeight = s.PanelHeight,
//                                       SheetThickness = s.SheetThickness,
//                                       SlotDimention = s.SlotDimentions,
//                                       StandardBend1 = s.StandardBend1,
//                                       StandardBend2 = s.StandardBend2,
//                                       Materials = s.Materials,
//                                       H = s.H,
//                                       W = s.W,
//                                       T = s.T,
//                                       PitchDistance = s.PitchDistance,
//                                       Section = s.Section,
//                                       LightTypes=s.LightTypes,
//                                       LuxLevel = s.LuxLevel,
//                                       Lumens= s.Lumens
//                                   }).ToList()
//            };
//            return paintBoothModel;
//        }
//        public int GetAllPaintBoothDesignCount()
//        {
//            return _DbContext.PaintBoothDetails.Count(c => !c.IsDeleted);
//        }

//        public List<string> GetEnquiryCodes(string term)
//        {
//            return _DbContext.EnquiryMasters
//                           .Where(e => e.SalesNO.StartsWith(term))
//                           .Select(e => e.SalesNO)
//                           .ToList();
//        }

//        public List<MotorFlangeModel> GetMotorDeatils(decimal BlowerHpCalculation)
//        {          
//            // Fetching and mapping records to MotorFlangeModel
//            var records = _DbContext.tblMotorFlange
//                .Where(record => record.RatedOutputHP > BlowerHpCalculation)
//                .Select(record => new MotorFlangeModel // Create instances of MotorFlangeModel
//                {
//                    MotorCatalogID=record.MotorCatalogID,
//                    RatedOutputHP = record.RatedOutputHP,
//                    Framesize = record.Framesize,
//                    TypeReference = record.TypeReference,
//                    RatedSpeed = record.RatedSpeed,
//                    RatedCurrent = record.RatedCurrent,
//                    EfficiencyFullLoad = record.EfficiencyFullLoad 
//                })
//                .OrderBy(record => record.RatedOutputHP) 
//                .Take(3)
//                .ToList();

//            return records; 
//        }
//        public string GetEnquiryCode(int enquiryid)
//        {
//           var enquiryNo=_DbContext.EnquiryMasters.Where(c=>c.EnquiryID== enquiryid).Select(c => c.SalesNO).FirstOrDefault();
//            return enquiryNo;
//        }

//        //public List<PaintBoothModel> GetPanelDetailsByCode(string enquiryCode)
//        //{
//        //    var panelDetails = _DbContext.PanelDetails.Where(s => s.SalesNo == enquiryCode)
//        //                          .Select(s => new PaintBoothModel
//        //                          {
//        //                              StandardPanelWidthForD = s.StandardPanelDepth,
//        //                              PanelHeightforW = s.StandardPanelWidth,
//        //                              PanelHeightforH = s.StandardPanelHeight,
//        //                              standardbend1 = s.StandardBend1,
//        //                              standardbend2 = s.StandardBend2,
//        //                              PitchDistance = s.PitchDistance,
//        //                              SheetThickness = s.SheetThickness,
//        //                              PanelPosition = s.PanelPosition,
//        //                              SlotDimention = s.SlotDimention
//        //                          }).ToList();

//        //    return panelDetails;
//        //}
//        public List<PaintBoothModel> GetPanelDetailsByCode(string enquiryCode)
//        {
//            var panelDetails = _DbContext.PanelDetails
//                .Where(s => s.SalesNo == enquiryCode)
//                .GroupBy(s => new
//                {
//                    s.StandardPanelDepth,
//                    s.StandardPanelWidth,
//                    s.StandardPanelHeight,
//                    s.StandardBend1,
//                    s.StandardBend2,
//                    s.PitchDistance,
//                    s.SheetThickness,
//                    s.PanelPosition,
//                    s.SlotDimention
//                })
//                .Select(g => new PaintBoothModel
//                {
//                    StandardPanelWidthForD = g.Key.StandardPanelDepth,
//                    PanelHeightforW = g.Key.StandardPanelWidth,
//                    PanelHeightforH = g.Key.StandardPanelHeight,
//                    standardbend1 = g.Key.StandardBend1,
//                    standardbend2 = g.Key.StandardBend2,
//                    PitchDistance = g.Key.PitchDistance,
//                    SheetThickness = g.Key.SheetThickness,
//                    PanelPosition = g.Key.PanelPosition,
//                    SlotDimention = g.Key.SlotDimention
//                }).ToList();

//            return panelDetails;
//        }





//    }
//}

using bullows.database;
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;



namespace Bullows.Repositories.Repositories
{
    public class PaintBoothRepository : GenericRepository<PaintBooth>
    {
        private readonly ISession Session;
        public PaintBoothRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        static double totalPanelsforD;
        static double smallPanelWidthforD;
        static int noOfPanelsforD;
        public EnquiryModel GetEnquiryDetailsByCode(string enquiryCode)
        {
            var enquiryDetails = (from enquiry in _DbContext.EnquiryMasters
                                  join component in _DbContext.ComponentTables
                                  on enquiry.ComponentID equals component.ComponentID
                                  where enquiry.SalesNO == enquiryCode
                                  select new EnquiryModel()
                                  {
                                      EnquiryId = enquiry.EnquiryID,
                                      // SalesNO=enquiry.SalesNO,
                                      LengthSize = component.Length,
                                      WidthSize = component.WidthSize,
                                      HeightSize = component.HeightSize
                                  }).FirstOrDefault();

            return enquiryDetails;
        }
        public bool Exists(int enquiryId)
        {
            return _DbContext.PaintBoothDetails
                .Any(pb => pb.EnquiryId == enquiryId && pb.designStatus == true);
        }
        public int SavePaintBooth(PaintBoothModel model, PanelInputModel pmodel, int flag, EnquiryModel enquiry)
        {
            PaintBoothDetails objtbl = new PaintBoothDetails();
            try
            {
                if (flag == 1)
                {

                }
                else
                {
                    objtbl = new PaintBoothDetails();
                    objtbl.IsDeleted = false;
                    objtbl.EnquiryId = enquiry.EnquiryId;
                    objtbl.D1 = model.D1;
                    objtbl.D2 = model.D2;
                    objtbl.D3 = model.D3;
                    objtbl.W1 = model.W1;
                    objtbl.W2 = model.W2;
                    objtbl.W3 = model.W3;
                    objtbl.D = model.D1 + model.D2 + model.Depth;
                    objtbl.W = model.W1 + model.W2 + model.Width;
                    objtbl.H = model.H1 + model.H2 + model.Height;
                    objtbl.H1 = model.H1;
                    objtbl.H2 = model.H2;
                    objtbl.CrossSectionalAreaOfBlower = model.CrossSectionalArea;
                    objtbl.VelocityofBlower = model.VelocityofBlower;
                    objtbl.CapacityofBlowerincubicm = model.CapacityofBlower;
                    objtbl.CapacityofBlowerinHr = model.CapacityofBlowerinH;
                    objtbl.RoundingCapacity= (decimal)model.CapacityofBlowerRoundOf;
                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    objtbl.CreatedDate = DateTime.Now;
                    objtbl.ModifiedBy = 0;
                    objtbl.designStatus = true;
                    _DbContext.PaintBoothDetails.Add(objtbl);
                }
                _DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }
        public void SaveMotorDeatils(int enquiryID, int MotorCatalogID)
        {
            var savedRecord = _DbContext.MotorDetails
                                        .FirstOrDefault(x => x.EnquiryID == enquiryID);

            //  Update the Quantity value
            if (savedRecord != null)
            {
                savedRecord.MotorCatalogID = MotorCatalogID;               
                _DbContext.SaveChanges();  
            }
            
        }
        public PaintBoothModel GetSettingDetailsByCode(string enquiryCode)
        {
            var paintBoothModel = new PaintBoothModel
            {
                Settings = _DbContext.SettingDetails
                                   .Where(s => s.SalesNO == enquiryCode)
                                   .Select(s => new SettingModel
                                   {
                                       SId = s.SId,
                                       SalesNo = s.SalesNO,
                                       PanelWidth = s.PanelWidth,
                                       PanelHeight = s.PanelHeight,
                                       SheetThickness = s.SheetThickness,
                                       SlotDimention = s.SlotDimentions,
                                       StandardBend1 = s.StandardBend1,
                                       StandardBend2 = s.StandardBend2,
                                       Materials = s.Materials,
                                       H = s.H,
                                       W = s.W,
                                       T = s.T,
                                       PitchDistance = s.PitchDistance,
                                       Section = s.Section,
                                       LightTypes = s.LightTypes,
                                       LuxLevel = s.LuxLevel,
                                       Lumens = s.Lumens
                                   }).ToList()
            };
            return paintBoothModel;
        }
        public int GetAllPaintBoothDesignCount()
        {
            return _DbContext.PaintBoothDetails.Count(c => !c.IsDeleted);
        }

        public List<string> GetEnquiryCodes(string term)
        {
            return _DbContext.EnquiryMasters
                           .Where(e => e.SalesNO.StartsWith(term))
                           .Select(e => e.SalesNO)
                           .ToList();
        }

        public List<MotorFlangeModel> GetMotorDeatils(decimal BlowerHpCalculation)
        {
            // Fetching and mapping records to MotorFlangeModel
            var records = _DbContext.tblMotorFlange
                .Where(record => record.RatedOutputHP > BlowerHpCalculation)
                .Select(record => new MotorFlangeModel // Create instances of MotorFlangeModel
                {
                    MotorCatalogID = record.MotorCatalogID,
                    RatedOutputHP = record.RatedOutputHP,
                    Framesize = record.Framesize,
                    TypeReference = record.TypeReference,
                    RatedSpeed = record.RatedSpeed,
                    RatedCurrent = record.RatedCurrent,
                    EfficiencyFullLoad = record.EfficiencyFullLoad
                })
                .OrderBy(record => record.RatedOutputHP)
                .Take(3)
                .ToList();

            return records;
        }
        public string GetEnquiryCode(int enquiryid)
        {
            var enquiryNo = _DbContext.EnquiryMasters.Where(c => c.EnquiryID == enquiryid).Select(c => c.SalesNO).FirstOrDefault();
            return enquiryNo;
        }

        //public List<PaintBoothModel> GetPanelDetailsByCode(string enquiryCode)
        //{
        //    var panelDetails = _DbContext.PanelDetails.Where(s => s.SalesNo == enquiryCode)
        //                          .Select(s => new PaintBoothModel
        //                          {
        //                              StandardPanelWidthForD = s.StandardPanelDepth,
        //                              PanelHeightforW = s.StandardPanelWidth,
        //                              PanelHeightforH = s.StandardPanelHeight,
        //                              standardbend1 = s.StandardBend1,
        //                              standardbend2 = s.StandardBend2,
        //                              PitchDistance = s.PitchDistance,
        //                              SheetThickness = s.SheetThickness,
        //                              PanelPosition = s.PanelPosition,
        //                              SlotDimention = s.SlotDimention
        //                          }).ToList();

        //    return panelDetails;
        //}
        public List<PaintBoothModel> GetPanelDetailsByCode(string enquiryCode)
        {
            var panelDetails = _DbContext.PanelDetails
                .Where(s => s.SalesNo == enquiryCode)
                .GroupBy(s => new
                {
                    s.StandardPanelDepth,
                    s.StandardPanelWidth,
                    s.StandardPanelHeight,
                    s.StandardBend1,
                    s.StandardBend2,
                    s.PitchDistance,
                    s.SheetThickness,
                    s.PanelPosition,
                    s.SlotDimention
                })
                .Select(g => new PaintBoothModel
                {
                    StandardPanelWidthForD = g.Key.StandardPanelDepth,
                    PanelHeightforW = g.Key.StandardPanelWidth,
                    PanelHeightforH = g.Key.StandardPanelHeight,
                    standardbend1 = g.Key.StandardBend1,
                    standardbend2 = g.Key.StandardBend2,
                    PitchDistance = g.Key.PitchDistance,
                    SheetThickness = g.Key.SheetThickness,
                    PanelPosition = g.Key.PanelPosition,
                    SlotDimention = g.Key.SlotDimention
                }).ToList();

            return panelDetails;
        }

        public List<Bullows.Database.PressureDrop> GetAallWaterColumnDetails()
        {
            return _DbContext.PressureDrop.Where(x => x.IsDeleted == false).ToList();
        }



    }
}

