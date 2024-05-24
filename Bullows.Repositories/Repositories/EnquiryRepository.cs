using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Repositories.Repositories
{
    public class EnquiryRepository : GenericRepository<EnquiryMaster>
    {
        private readonly ISession Session;
        public EnquiryRepository(BullowsDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._DbContext = context;
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        private int GetComponentID()
        {
            var componentId = _DbContext.ComponentTables.OrderByDescending(x => x.ComponentID).Select(x => x.ComponentID).FirstOrDefault();
            return componentId;
        }
        private int GetCustomerId()
        {
            var customerid = _DbContext.CustomerMasters.OrderByDescending(x => x.CustomerID).Select(x => x.CustomerID).FirstOrDefault();
            return customerid;
        }
        public int SaveEnquiry(EnquiryModel model, EnquiryMaster tblobj, int flag)
        {
            try
            {
                int ComponentId = GetComponentID();
                int CustomerId = GetCustomerId();
                if (flag == 1)
                {

                }
                else
                {
                    tblobj = new EnquiryMaster();
                    tblobj.ProposalDate = model.ProposalDate;
                    tblobj.SalesNO = model.SalesNO;
                    tblobj.IsDeleted = false;
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = 1;
                    tblobj.ModifiedDate = DateTime.Now;
                    tblobj.CustomerID = CustomerId;
                    tblobj.ComponentID = ComponentId;
                    _DbContext.EnquiryMasters.Add(tblobj);

                }
                _DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }

        public int SaveComponent(EnquiryModel model, ComponentTable tblobj, int flag)
        {
            try
            {
                if (flag == 1)
                {

                }
                else
                {
                    tblobj = new ComponentTable();
                    tblobj.Category = model.Category;
                    tblobj.Component = model.Component;
                    tblobj.IsDeleted = false;
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = 0;
                   
                    tblobj.Length = model.LengthSize;
                    tblobj.WidthSize = model.WidthSize;


                    tblobj.ComponentHandling = model.ComponentHandling;
                    tblobj.Paint = model.Paint;
                    tblobj.Powder = model.Powder;
                    tblobj.DFT = model.DFT;
                    tblobj.Conveyor = model.Conveyor;
                    tblobj.NoOfColors = model.NoOfColors;
                    tblobj.NoOfCoats = model.NoOfCoats;
                    tblobj.Pitch = model.Pitch;
                    tblobj.Speed = model.Speed;
                    tblobj.LoadingUnloading = model.LoadingUnloading;
                    tblobj.ConsumptionPerDay = model.Consumption;
                    tblobj.Viscosity = model.Viscosity;
                    tblobj.HeightSize = model.HeightSize;
                    tblobj.Weight = model.Weight;
                    tblobj.QtyperAssembly = model.QtyperAssembly;
                    tblobj.MaterialofConstruction = model.MaterialofConstruction;
                    tblobj.SurfaceArea = model.SurfaceArea;
                    tblobj.WallThickness = model.WallThickness;
                    tblobj.ProductionRequirement = model.ProductionRequirement;
                    tblobj.Workingdays = model.Workingdays;
                    tblobj.EffectiveWorking = model.EffectiveWorking;
                    tblobj.NumberofShifts = model.NumberofShifts;
                    tblobj.Image_Path = model.Image_Path;
                    _DbContext.ComponentTables.Add(tblobj);

                }
                _DbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }
        

        public List<EnquiryModel> PopulateGrid()
        {
            List<EnquiryModel> lstEnq = new List<EnquiryModel>();
            lstEnq = (from e in _DbContext.EnquiryMasters
                      join Cust in _DbContext.CustomerMasters on e.CustomerID equals Cust.CustomerID
                      join comp in _DbContext.ComponentTables on e.ComponentID equals comp.ComponentID
                      where e.IsDeleted == false
                      select new EnquiryModel()
                      {
                          EnquiryId = e.EnquiryID,
                          SalesNO = e.SalesNO,
                          CompanyName = Cust.CompanyName,
                          Component = comp.Component,
                          ComponentID = comp.ComponentID,
                          CustomerID = Cust.CustomerID

                      }).ToList();
            return lstEnq;
        }

        //IFormFile GetImagepath(string ImagePath)
        //{
        //    FileStream fileStream = new FileStream(ImagePath,FileMode.Open,FileAccess.Read);
        //        IFormFile formFile = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(ImagePath));
        //    fileStream.Close();
        //    return formFile;
        //}
        //public static IFormFile GetImagepath(string ImagePath)
        //{
        //    FileStream fileStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
        //    FormFile formFile = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(ImagePath));
        //    fileStream.Close();
        //    return formFile;
        //}

        public EnquiryModel Editmodel(int id/*string _webHostEnvironment*/)
        {
            var data = (from e in _DbContext.EnquiryMasters
                        join Cust in _DbContext.CustomerMasters on e.CustomerID equals Cust.CustomerID
                        join comp in _DbContext.ComponentTables on e.ComponentID equals comp.ComponentID
                        where e.IsDeleted == false && e.EnquiryID == id
                        select new EnquiryModel()
                        {
                            EnquiryId = e.EnquiryID,
                            SalesNO = e.SalesNO,
                            CompanyName = Cust.CompanyName,
                            Component = comp.Component,
                            ComponentID = comp.ComponentID,
                            CustomerID = Cust.CustomerID,
                            Category = comp.Category,
                            ProposalDate = e.ProposalDate,
                            CustomerAddress = Cust.CustomerAddress,
                            //ContactPerson = Cust.ContactPerson,
                            //Designation = Cust.Designation,
                            //MobileNo = Cust.MobileNo,
                            //EmailId = Cust.EmailId,
                            Image_Path = comp.Image_Path,
                            imageFile = CreateFormFileFromPath(comp.Image_Path),
                            LengthSize = comp.Length,
                            WidthSize = comp.WidthSize,
                            HeightSize = comp.HeightSize,
                            Weight = comp.Weight,
                            QtyperAssembly = comp.QtyperAssembly,
                            MaterialofConstruction = comp.MaterialofConstruction,
                            SurfaceArea = comp.SurfaceArea,
                            WallThickness = comp.WallThickness,
                            ProductionRequirement = comp.ProductionRequirement,
                            Workingdays = comp.Workingdays,
                            NumberofShifts = comp.NumberofShifts,
                            EffectiveWorking = comp.EffectiveWorking

                        }).FirstOrDefault();


            return data;



        }
        public int saveCustomerDetails(EnquiryModel model, CustomerMaster tblobj, int flag)
        {
            tblobj = new CustomerMaster();
            tblobj.CompanyName = model.CompanyName;
            tblobj.CustomerAddress = model.CustomerAddress;
            tblobj.Designation = model.Designation;
            tblobj.StateId = model.StateId;
            tblobj.DistrictId = model.DistrictId;
            tblobj.CityId = model.CityId;
            tblobj.Pin = model.Pin;
            tblobj.PAN = model.PAN;
            tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.IsDeleted = false;
            tblobj.ModifiedBy = 0;
            _DbContext.CustomerMasters.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }



        //public List<EnquiryModel> FillComanyDropDown()
        //{
        //    List<EnquiryModel> lstdata = _DbContext.CustomerMasters.Where(x => x.IsDeleted == false).Select(item => new EnquiryModel()
        //    {
        //        CustomerID = item.CustomerID,
        //        CompanyName = item.CompanyName,
        //    }).ToList();
        //    return lstdata;
        //}
        public static IFormFile CreateFormFileFromPath(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(filePath));
            return formFile;
        }

        public int Delete(int id = 0)
    {
        EnquiryMaster tblenq = _DbContext.EnquiryMasters.Find(id);
        tblenq.IsDeleted = true;
        _DbContext.Entry(tblenq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _DbContext.SaveChanges();
        return 2;
    }
}

   
}
