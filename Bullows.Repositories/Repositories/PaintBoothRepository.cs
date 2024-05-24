using bullows.business;
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Repositories.Repositories
{
    public class PaintBoothRepository: GenericRepository<PaintBooth>
    {
        private readonly ISession Session;
        public PaintBoothRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }


        
        public EnquiryModel GetEnquiryDetailsByCode(string enquiryCode)
        {
            var enquiryDetails = (from enquiry in _DbContext.EnquiryMasters
                                  join component in _DbContext.ComponentTables
                                  on enquiry.ComponentID equals component.ComponentID
                                  where enquiry.SalesNO == enquiryCode
                                  select new EnquiryModel()
                                  {
                                      LengthSize = component.Length,
                                      WidthSize = component.WidthSize,
                                     HeightSize=component.HeightSize
                                  }).FirstOrDefault();

            return enquiryDetails;
        }

        public int SavePaintBooth(PaintBoothModel model,PaintBooth objtbl,int flag)
        {
            try
            {
                if(flag==1)
                {

                }
                else
                {
                    objtbl = new PaintBooth();
                    objtbl.IsDeleted = false;
                    objtbl.D1 = model.D1;
                    objtbl.D2 = model.D2;
                    objtbl.D3 = model.D3;
                    objtbl.W1 = model.W1;
                    objtbl.W2 = model.W2;
                    objtbl.W3 = model.W3;
                    objtbl.D = model.D1 + model.D2 + model.Depth;
                    objtbl.W = model.W1 + model.W2 + model.Width;
                    objtbl.H = model.H1 + model.H2;
                    objtbl.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    objtbl.CreatedDate = DateTime.Now;
                    objtbl.ModifiedBy = 0;
                    _DbContext.PaintBooths.Add(objtbl);
                }
                _DbContext.SaveChanges();

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return 1;
        }

       
        





    }
}
