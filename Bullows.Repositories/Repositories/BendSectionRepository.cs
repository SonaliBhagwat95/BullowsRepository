using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Bullows.Repositories.Repositories
{
    public class BendSectionRepository:GenericRepository<BendSectionTable>
    {
        private readonly ISession Session;
        public BendSectionRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public List<BendSectionModel> GetAllData()
        {
            var AllData = _DbContext.BendSectionTable.Where(x => x.IsDeleted == false).Select(s => new
            {
               s.BendId,
               s.W,
               s.H,
               s.T,
               s.L,
               s.Length,
               s.SectionName,
               //s.SlotLocation


            });
            List<BendSectionModel> BendModels = AllData.Select(item => new BendSectionModel()
            {
                  BendId= item.BendId,
                  W= item.W,
                  H= item.H,
                  T= item.T,
                  L= item.L,
                  Length= item.Length,
                  SectionName= item.SectionName,
                  //SlotLocation= item.SlotLocation

            }).ToList();
            return BendModels;

        }

        //public int SaveBendSectionDetails(BendSectionModel model,BendSectionTable tblobj)
        //{
        //    tblobj= new BendSectionTable();
        //    tblobj.IsDeleted = false;

        //    tblobj.W = model.W;
        //    tblobj.H = model.H;
        //    tblobj.T = model.T;
        //    tblobj.L = model.L;
        //    tblobj.Length = model.Length;
        //    tblobj.L1 = 0;
        //    tblobj.SectionName = model.SectionName;                    
        //    if(model.SectionName=="CSection")
        //    {
        //        tblobj.PitchDistance = (decimal)model.PitchDistance;
        //        tblobj.slotDimentions = model.SlotDimentions;
        //        tblobj.SlotLocation = model.SlotLocation;
        //    }
        //    else
        //    {
        //        tblobj.PitchDistance = 0;
        //        tblobj.slotDimentions ="N/A";
        //        tblobj.SlotLocation = "N/A";

        //    }
        //    if(model.SectionName=="Corner")
        //        tblobj.L1 = model.L1;
        //    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
        //    tblobj.CreatedDate = DateTime.Now;          
        //    tblobj.ModifiedBy = 0;
        //    tblobj.ModifiedDate = null;
        //    _DbContext.BendSectionTable.Add(tblobj);
        //    _DbContext.SaveChanges();
        //    return 1;
        //}
        public int SaveBendSectionDetails(BendSectionModel model, BendSectionTable tblobj, List<SlotDetail> slotDetailsList)
        {
            tblobj = new BendSectionTable();
            tblobj.IsDeleted = false;
            tblobj.W = model.W;
            tblobj.H = model.H;
            tblobj.T = model.T;
            tblobj.L = model.L;
            tblobj.Length = model.Length;
            tblobj.L1 = 0;
            tblobj.SectionName = model.SectionName;           
            if (model.SectionName == "Corner")
                tblobj.L1 = model.L1;

            tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.ModifiedBy = 0;
            tblobj.ModifiedDate = null;
            if(model.AddSlotchk)
            {
                var slotDetailsJson = JsonConvert.SerializeObject(slotDetailsList);
                tblobj.SlotDetails = slotDetailsJson;
            }
            else
                tblobj.SlotDetails = "NA";

            _DbContext.BendSectionTable.Add(tblobj);
           _DbContext.SaveChanges();
            return 1;
        }


        public int Delete(int id=0)
        {
            BendSectionTable tblbend= _DbContext.BendSectionTable.Find(id);
            tblbend.IsDeleted = true;
            _DbContext.Entry(tblbend).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            _DbContext.SaveChanges();
            return 2;
        }
        
    }
}
