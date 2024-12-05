//using bullows.database;
//using Bullows.Database;
//using Bullows.Model;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bullows.Repositories.Repositories
//{
//    public class SettingDetailsRepository: GenericRepository<SettingDetails>
//    {
//        private readonly ISession Session;
//        public SettingDetailsRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
//        {
//            this._DbContext = context;
//            this.Session = httpContextAccessor.HttpContext.Session;
//        }

//        public int SaveSettingsDetails(SettingModel model, int flag, SettingDetails objtbl)
//        {

//            try
//            {
//                if (flag == 1)
//                {
//                    objtbl = _DbContext.SettingDetails.Find(model.SId);
//                    objtbl.IsDeleted = false;
//                    objtbl.SalesNO = model.SalesNo;
//                    objtbl.PanelWidth = model.PanelWidth;
//                    objtbl.PanelHeight = model.PanelHeight;
//                    objtbl.SheetThickness = model.SheetThickness;
//                    objtbl.SlotDimentions = model.SlotDimention;
//                    objtbl.StandardBend1 = model.StandardBend1;
//                    objtbl.StandardBend2 = model.StandardBend2;
//                    objtbl.Materials = model.Materials;
//                    objtbl.PitchDistance = model.PitchDistance;
//                    objtbl.Section = model.Section;
//                    objtbl.LightTypes = model.LightTypes;
//                    objtbl.LuxLevel = model.LuxLevel;
//                    objtbl.Lumens = model.Lumens;
//                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
//                    objtbl.CreatedDate = DateTime.Now;
//                    objtbl.ModifiedBy = 1;
//                    objtbl.ModifiedDate = DateTime.Now;

//                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
//                }
//                else
//                {

//                    objtbl = new SettingDetails();
//                    objtbl.IsDeleted = false;
//                    objtbl.SalesNO = model.SalesNo;
//                    objtbl.PanelWidth = model.PanelWidth;
//                    objtbl.PanelHeight = model.PanelHeight;
//                    objtbl.SheetThickness = model.SheetThickness;
//                    objtbl.SlotDimentions = model.SlotDimention;
//                    objtbl.StandardBend1 = model.StandardBend1;
//                    objtbl.StandardBend2 = model.StandardBend2;
//                    objtbl.Materials = model.Materials;
//                    objtbl.BendSection = model.BendSection;
//                    objtbl.EnquiryID = model.EnquiryID;
//                    objtbl.LightTypes = model.LightTypes;
//                    objtbl.LuxLevel = model.LuxLevel;
//                    objtbl.Lumens = model.Lumens;

//                    objtbl.settingStatus = true;
//                    string selectedSectionvalue = model.BendSection;
//                    string[] parts = selectedSectionvalue.Split('*');

//                    // Ensure the string splits into exactly 3 parts: H, W, T
//                    if (parts.Length == 3)
//                    {
//                        model. H = int.Parse(parts[0]);
//                        model. W = int.Parse(parts[1]); 
//                        model. T = int.Parse(parts[2]); 
//                    }
//                    objtbl.H = model.H; 
//                    objtbl.W= model.W;
//                    objtbl.T = model.T;
//                    objtbl.PitchDistance = model.PitchDistance;
//                    objtbl.Section = model.Section;
//                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
//                    objtbl.CreatedDate = DateTime.Now;
//                    objtbl.ModifiedBy = 0;
//                    objtbl.ModifiedDate = new DateTime(1753, 1, 1); 

//                    _DbContext.SettingDetails.Add(objtbl);
//                }
//                _DbContext.SaveChanges();

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return 1;
//        }
//        public EnquiryModel GetEnquiryID(string enquiryCode)
//        {
//            var enquiryid = (from enquiry in _DbContext.EnquiryMasters
//                             where enquiry.SalesNO == enquiryCode
//                             select new EnquiryModel()
//                             {
//                                 EnquiryId = enquiry.EnquiryID
//                             }).FirstOrDefault();
//            return enquiryid;
//        }

//        //this code for fetching enquiry code from Setting tabl e where settingstatus = 1;
//        public List<string> SearchEnquiryCodes(string searchTerm)
//        {
//            List<string> detailsList = (from e in _DbContext.SettingDetails

//                                        where e.SalesNO.Contains(searchTerm)&& e.settingStatus==true
//                                        select e.SalesNO).Distinct().ToList();
//            return detailsList;
//        }


//        public List<SettingModel> GetAllData()
//        {
//            List<SettingModel> lstData = _DbContext.SettingDetails.Where(x => x.IsDeleted == false).Select(item => new SettingModel()
//            {
//                SId=item.SId,
//                SalesNo = item.SalesNO,
//                PanelWidth = item.PanelWidth,
//                PanelHeight = item.PanelHeight,
//                SlotDimention = item.SlotDimentions,
//                Materials = item.Materials,
//                PitchDistance = item.PitchDistance,
//                Section = item.Section,
//                SheetThickness = item.SheetThickness,
//                StandardBend1 = item.StandardBend1,
//                StandardBend2 = item.StandardBend2

//            }).ToList();
//            return lstData;
//        }

//        public SettingModel EditModel(int id)
//        {
//            try
//            {
//                return _DbContext.SettingDetails.Where(x => x.SId == id && x.IsDeleted == false).Select(item => new SettingModel()
//                {
//                    SId=item.SId,
//                    SalesNo = item.SalesNO,
//                    PanelWidth = item.PanelWidth,
//                    PanelHeight = item.PanelHeight,
//                    SlotDimention=item.SlotDimentions,
//                    Materials = item.Materials,
//                    PitchDistance = item.PitchDistance,
//                    Section = item.Section,
//                    SheetThickness = item.SheetThickness,
//                    StandardBend1=item.StandardBend1,
//                    StandardBend2=item.StandardBend2,
//                    BendSection = item.BendSection


//                }).FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        public int Delete(int id = 0)
//        {
//            SettingDetails tblenq = _DbContext.SettingDetails.Find(id);
//            tblenq.IsDeleted = true;
//            _DbContext.Entry(tblenq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
//            _DbContext.SaveChanges();
//            return 2;
//        }









//    }
//}
using bullows.database;
using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Repositories.Repositories
{
    public class SettingDetailsRepository : GenericRepository<SettingDetails>
    {
        private readonly ISession Session;
        public SettingDetailsRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public int SaveSettingsDetails(SettingModel model, int flag, SettingDetails objtbl)
        {

            try
            {
                if (flag == 1)
                {
                    objtbl = _DbContext.SettingDetails.Find(model.SId);
                    objtbl.IsDeleted = false;
                    objtbl.SalesNO = model.SalesNo;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.PanelHeight = model.PanelHeight;
                    objtbl.SheetThickness = model.SheetThickness;
                    objtbl.SlotDimentions = model.SlotDimention;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.Materials = model.Materials;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.Section = model.Section;
                    objtbl.LightTypes = model.LightTypes;
                    objtbl.LuxLevel = model.LuxLevel;
                    objtbl.Lumens = model.Lumens;
                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    objtbl.CreatedDate = DateTime.Now;
                    objtbl.ModifiedBy = 1;
                    objtbl.ModifiedDate = DateTime.Now;

                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {

                    objtbl = new SettingDetails();
                    objtbl.IsDeleted = false;
                    objtbl.SalesNO = model.SalesNo;
                    objtbl.PanelWidth = model.PanelWidth;
                    objtbl.PanelHeight = model.PanelHeight;
                    objtbl.SheetThickness = model.SheetThickness;
                    objtbl.SlotDimentions = model.SlotDimention;
                    objtbl.StandardBend1 = model.StandardBend1;
                    objtbl.StandardBend2 = model.StandardBend2;
                    objtbl.Materials = model.Materials;
                    objtbl.BendSection = model.BendSection;
                    objtbl.EnquiryID = model.EnquiryID;
                    objtbl.LightTypes = model.LightTypes;
                    objtbl.LuxLevel = model.LuxLevel;
                    objtbl.Lumens = model.Lumens;

                    objtbl.settingStatus = true;
                    string selectedSectionvalue = model.BendSection;
                    string[] parts = selectedSectionvalue.Split('*');

                    // Ensure the string splits into exactly 3 parts: H, W, T
                    if (parts.Length == 3)
                    {
                        model.H = int.Parse(parts[0]);
                        model.W = int.Parse(parts[1]);
                        model.T = int.Parse(parts[2]);
                    }
                    objtbl.H = model.H;
                    objtbl.W = model.W;
                    objtbl.T = model.T;
                    objtbl.PitchDistance = model.PitchDistance;
                    objtbl.Section = model.Section;
                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    objtbl.CreatedDate = DateTime.Now;
                    objtbl.ModifiedBy = 0;
                    objtbl.ModifiedDate = new DateTime(1753, 1, 1);

                    _DbContext.SettingDetails.Add(objtbl);
                }
                _DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }

        public int SaveTubeLightDetails(SettingModel model, int flag)
        {
            TubeLightDetails objtbl = new TubeLightDetails();
            try
            {
                if (flag == 1)
                {
                    objtbl = _DbContext.TubeLightDetails.Find(model.SId);
                    objtbl.IsDeleted = false;
                    objtbl.IsDeleted = false;
                    objtbl.LightType = model.LightTypes;
                    objtbl.LightSubType = model.LightSubTypes;
                    objtbl.LuxLevel = model.LuxLevel;
                    objtbl.Lumens = model.Lumens;
                    objtbl.EnquiryID = model.EnquiryID;
                    objtbl.SalesNo = model.SalesNo;

                    _DbContext.Entry(objtbl).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {

                    objtbl = new TubeLightDetails();
                    objtbl.IsDeleted = false;
                    objtbl.LightType = model.LightTypes;
                    objtbl.LightSubType = model.LightSubTypes;
                    objtbl.LuxLevel = model.LuxLevel;
                    objtbl.Lumens = model.Lumens;
                    objtbl.EnquiryID = model.EnquiryID;
                    objtbl.SalesNo = model.SalesNo;


                    _DbContext.TubeLightDetails.Add(objtbl);
                }
                _DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }
        public EnquiryModel GetEnquiryID(string enquiryCode)
        {
            var enquiryid = (from enquiry in _DbContext.EnquiryMasters
                             where enquiry.SalesNO == enquiryCode
                             select new EnquiryModel()
                             {
                                 EnquiryId = enquiry.EnquiryID
                             }).FirstOrDefault();
            return enquiryid;
        }

        //this code for fetching enquiry code from Setting tabl e where settingstatus = 1;
        public List<string> SearchEnquiryCodes(string searchTerm)
        {
            List<string> detailsList = (from e in _DbContext.SettingDetails

                                        where e.SalesNO.Contains(searchTerm) && e.settingStatus == true
                                        select e.SalesNO).Distinct().ToList();
            return detailsList;
        }


        public List<SettingModel> GetAllData()
        {
            List<SettingModel> lstData = _DbContext.SettingDetails.Where(x => x.IsDeleted == false).Select(item => new SettingModel()
            {
                SId = item.SId,
                SalesNo = item.SalesNO,
                PanelWidth = item.PanelWidth,
                PanelHeight = item.PanelHeight,
                SlotDimention = item.SlotDimentions,
                Materials = item.Materials,
                PitchDistance = item.PitchDistance,
                Section = item.Section,
                SheetThickness = item.SheetThickness,
                StandardBend1 = item.StandardBend1,
                StandardBend2 = item.StandardBend2

            }).ToList();
            return lstData;
        }

        public SettingModel EditModel(int id)
        {
            try
            {
                return _DbContext.SettingDetails.Where(x => x.SId == id && x.IsDeleted == false).Select(item => new SettingModel()
                {
                    SId = item.SId,
                    SalesNo = item.SalesNO,
                    PanelWidth = item.PanelWidth,
                    PanelHeight = item.PanelHeight,
                    SlotDimention = item.SlotDimentions,
                    Materials = item.Materials,
                    PitchDistance = item.PitchDistance,
                    Section = item.Section,
                    SheetThickness = item.SheetThickness,
                    StandardBend1 = item.StandardBend1,
                    StandardBend2 = item.StandardBend2,
                    BendSection = item.BendSection


                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Delete(int id = 0)
        {
            SettingDetails tblenq = _DbContext.SettingDetails.Find(id);
            tblenq.IsDeleted = true;
            _DbContext.Entry(tblenq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _DbContext.SaveChanges();
            return 2;
        }

    }
}
