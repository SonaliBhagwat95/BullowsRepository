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
    public class MaterialRepository: GenericRepository<tblMOC>
    {
        private readonly ISession Session;
        public MaterialRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public List<MaterialModel> GetAllMaterial()
        {
            List<MaterialModel> getMaterial=_DbContext.tblMOC.Where(m=>m.IsDeleted==false)
                .Select(item => new MaterialModel()
                {
                    MOCID=item.MOCId,
                    MOC=item.MOC,
                    Density=item.Density,
                    Rate=item.Rate,
                }).ToList();
            return getMaterial;   
        }
        public int Save(MaterialModel model, int Flag, tblMOC tblobj)
        {
            try
            {
                if(Flag==1)
                {
                    tblobj = _DbContext.tblMOC.Find(model.MOCID);
                    tblobj.MOC=model.MOC; 
                    tblobj.Density=model.Density;
                    tblobj.Rate=model.Rate;
                    tblobj.ModifiedBy = 1;
                    tblobj.ModifiedDate = DateTime.Now;
                    tblobj.IsDeleted = false;
                   _DbContext.Entry(tblobj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    tblobj = new tblMOC();
                    tblobj.MOC = model.MOC;
                    tblobj.Density = model.Density;
                    tblobj.Rate = model.Rate;
                    tblobj.CreatedBy= (int)(Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0);
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = 0;
                    _DbContext.tblMOC.Add(tblobj);
                }
                _DbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return 1;
        }
        public MaterialModel EditModel(int id)
        {
            return _DbContext.tblMOC.Where(x => x.MOCId == id && x.IsDeleted == false).Select(x => new MaterialModel
            {
                MOCID=x.MOCId,
                IsDeleted=x.IsDeleted,
                MOC=x.MOC,
                Density=x.Density,  
                Rate=x.Rate,
                CreatedBy = x.CreatedBy != null ? (int)x.CreatedBy : 0,
                CreatedDate = x.CreatedDate != null ? (DateTime)x.CreatedDate : DateTime.MinValue,
                ModifiedBy=1,
                ModifiedDate=DateTime.Now


            }).FirstOrDefault();
        }
        public int Delete(int id)
        {
            tblMOC obj= _DbContext.tblMOC.Find(id);
            obj.IsDeleted= true;
            _DbContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _DbContext.SaveChanges();
            return 2;
        }

        public List<MaterialModel> GetAllMaterialForBO()
        {
            List<MaterialModel> getMaterial = _DbContext.tblMOC.Where(m => m.MOCId > 6)
                .Select(item => new MaterialModel()
                {
                    MOCID = item.MOCId,
                    MOC = item.MOC,                  
                    Rate = item.Rate,
                }).ToList();
            return getMaterial;
        }
    }
}
