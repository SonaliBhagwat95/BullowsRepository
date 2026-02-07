using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;


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

        public EnquiryMaster GetEnquiryByNo(string enquiryNo)
        {
            return _DbContext.EnquiryMasters.FirstOrDefault(e => e.SalesNO == enquiryNo);
        }
        
        public int SaveEnquiry(EnquiryModel model, EnquiryMaster tblobj, int flag)
        {
            try
            {
                int ComponentId = GetComponentID();
                int CustomerId = GetCustomerId();
                if (flag == 1)
                {
                    tblobj = new EnquiryMaster();
                    tblobj.ProposalDate = model.ProposalDate;
                    tblobj.SalesNO = model.SalesNO;
                    tblobj.IsDeleted = false;
                    tblobj.DraftType = model.DraftType != null ? model.DraftType : ""; 
                    tblobj.SubTypeOfDraftType = model.SubTypeOfDraft != null ? model.SubTypeOfDraft : "";
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    //tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = 1;
                    tblobj.ModifiedDate = DateTime.Now;
                    tblobj.EnquiryID = model.EnquiryId;
                    tblobj.CustomerID = model.CustomerID;
                    tblobj.ComponentID = ComponentId;
                    tblobj.PlenumHeight = model.HeightOfPlenum!=null? model.HeightOfPlenum:"";
                    _DbContext.Entry(tblobj).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    tblobj = new EnquiryMaster();
                    tblobj.ProposalDate = model.ProposalDate;
                    tblobj.SalesNO = model.SalesNO;
                    tblobj.IsDeleted = false;

                    tblobj.DraftType = model.DraftType != null ? model.DraftType : "";
                    tblobj.SubTypeOfDraftType = model.SubTypeOfDraft != null ? model.SubTypeOfDraft:"";
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = 0;
                    // tblobj.ModifiedDate = DateTime.Now;
                    tblobj.CustomerID = model.CustomerID;
                    tblobj.ComponentID = ComponentId;
                    tblobj.PlenumHeight = model.HeightOfPlenum != null ? model.HeightOfPlenum : "";
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

        private int GetEnquiryID()
        {
            var enquiryId = _DbContext.EnquiryMasters.OrderByDescending(x => x.EnquiryID).Select(x => x.EnquiryID).FirstOrDefault();
            return enquiryId;
        }
        public int SaveMotorTypes(EnquiryModel model, int flag)
        {
            int enquiryid = GetEnquiryID();
            MotorDetails obj = new MotorDetails();
            if(flag == 1)
            {
                obj = new MotorDetails();
                obj.MotorID = model.MotorID;
                obj.EnquiryID = enquiryid;
                obj.MotorTypes = model.MotorType;
                _DbContext.Entry(obj).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                obj = new MotorDetails();
                obj.IsDeleted = false;
                obj.EnquiryID = enquiryid;
                obj.MotorTypes = model.MotorType;
                obj.MotorCatalogID = 1;
                _DbContext.MotorDetails.Add(obj);
            }
            _DbContext.SaveChanges(); 
            return 1;
        }
        public int SaveComponent(EnquiryModel model, ComponentTable tblobj, int flag)
        {
            try
            {
                if (flag == 1)
                {
                    tblobj = new ComponentTable();
                    tblobj.ComponentID = model.ComponentID;
                    tblobj.Category = model.Category;
                    tblobj.Component = model.Component;
                    tblobj.IsDeleted = false;
                    tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
                    tblobj.CreatedDate = DateTime.Now;
                    tblobj.ModifiedBy = (int)(Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0); 
                    tblobj.ModifiedDate = DateTime.Now;                 
                    tblobj.ComponentHandling = model.ComponentHandling;
                    if (model.Shape == "Rectangular" && model.Orientation == "Horizontal")
                    {
                        tblobj.Length = model.LengthSize;
                        tblobj.WidthSize = model.WidthSize;
                        tblobj.HeightSize = model.HeightSize;

                    }
                    else if (model.Shape == "Rectangular" && model.Orientation == "Vertical")
                    {
                        tblobj.Length = model.HeightSize;
                        tblobj.WidthSize = model.WidthSize;
                        tblobj.HeightSize = model.LengthSize;

                    }
                    else if (model.Shape == "Circular" && model.Orientation == "Horizontal")
                    {
                        tblobj.Length = (double)model.Diameter;
                        tblobj.WidthSize = (double)model.Diameter;
                        tblobj.HeightSize = model.CircularHeightSize;

                    }
                    else if (model.Shape == "Circular" && model.Orientation == "Vertical")
                    {
                        tblobj.Length = (double)model.Diameter;
                        tblobj.WidthSize = model.CircularHeightSize;
                        tblobj.HeightSize = (double)model.Diameter;

                    }
                    tblobj.ComponentHandling = model.ComponentHandling;
                    if (model.ComponentHandling == "1")
                    {
                        if (new[] { "1", "2", "6", "7", "8" }.Contains(model.Conveyor))
                        {
                            tblobj.CarringCapacity = model.CarringCapacity;
                            tblobj.OverheadConveyorSubTypes = "NA";
                            tblobj.ConveyorNumber = "NA";
                            tblobj.Pitch = 0;
                            tblobj.Speed = 0;
                        }
                        else if (model.Conveyor == "3")
                        {
                            tblobj.CarringCapacity = 0;
                            tblobj.OverheadConveyorSubTypes = model.OverheadConveyorSubTypes;
                            tblobj.ConveyorNumber = model.ConveyorNumber;
                            tblobj.Pitch = model.Pitch;
                            tblobj.Speed = model.Speed;
                        }
                    }
                    else
                    {
                        tblobj.CarringCapacity = 0;
                        tblobj.OverheadConveyorSubTypes = "NA";
                        tblobj.ConveyorNumber = "NA";
                        tblobj.Pitch = 0;
                        tblobj.Speed = 0;
                    }
                    tblobj.Paint = model.Paint;
                    tblobj.Powder = model.Powder;
                    tblobj.DFT = model.DFT;
                    
                    tblobj.NoOfColors = model.NoOfColors;
                    tblobj.NoOfCoats = model.NoOfCoats;
                    tblobj.Shape = model.Shape;
                    tblobj.Orientation = model.Orientation;
                    //tblobj.Pitch = model.Pitch;
                    //tblobj.Speed = model.Speed;
                    tblobj.LoadingUnloading = model.LoadingUnloading;
                    tblobj.ConsumptionPerDay = model.Consumption;
                    tblobj.Viscosity = model.Viscosity;
                    tblobj.Weight = model.Weight;
                    tblobj.Conveyor = model.Conveyor != null ? model.Conveyor : "NA";
                    tblobj.MaterialofConstruction = model.MaterialofConstruction;
                    tblobj.SurfaceArea = model.SurfaceArea;
                    tblobj.WallThickness = model.WallThickness;
                    tblobj.ProductionRequirement = model.ProductionRequirement;
                    tblobj.Workingdays = model.Workingdays;
                    tblobj.SpecificHeat = model.SpecificHeat;
                    tblobj.EffectiveWorking = model.EffectiveWorking;
                    tblobj.NumberofShifts = model.NumberofShifts;                  
                    tblobj.Image_Path = model.Image_Path;
                    tblobj.ExtractionChamberHeight = model.ExtractionChamberHeight;
                    tblobj.ComponentEntry = model.ComponentEntry == null ? "" : model.ComponentEntry;
                    tblobj.DoorType = model.DoorType==null ? "":model.DoorType;
                    if (model.DoorType == "Manual")
                    {
                        tblobj.DoorSubType = model.TypeOfOperationForManual == null ? "" : model.TypeOfOperationForManual;
                    }
                    else
                    {
                        tblobj.DoorSubType = model.TypeOfOperationForMotorised == null ? "" : model.TypeOfOperationForMotorised;
                    }
                    tblobj.TypeOfHingedDoor= model.TypeOfHingedDoor == null ? "" : model.TypeOfHingedDoor;
                    tblobj.SideDoorLOcation = model.SideDoorLOcation == null ? "" : model.SideDoorLOcation;
                    tblobj.TypeOfPaint = model.TypeOfPaint == null ? "" : model.TypeOfPaint;
                    tblobj.TypeOfPowder = model.TypeOfPowder == null ? "" : model.TypeOfPowder;
                    tblobj.TypeOfHinges = model.TypeOfHinges == null ? "" : model.TypeOfHinges;
                    _DbContext.Entry(tblobj).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
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
                    if (model.Shape == "Rectangular" && model.Orientation == "Horizontal")
                    {
                        tblobj.Length = model.LengthSize;
                        tblobj.WidthSize = model.WidthSize;
                        tblobj.HeightSize = model.HeightSize;
                        
                    }else if (model.Shape == "Rectangular" && model.Orientation == "Vertical")
                    {
                        tblobj.Length = model.HeightSize;
                        tblobj.WidthSize = model.WidthSize;
                        tblobj.HeightSize = model.LengthSize;
                       
                    }
                    else if(model.Shape=="Circular" && model.Orientation == "Horizontal")
                    {
                        tblobj.Length = (double)model.Diameter;
                        tblobj.WidthSize = (double)model.Diameter;
                        tblobj.HeightSize = model.CircularHeightSize;

                    }
                    else if (model.Shape == "Circular" && model.Orientation == "Vertical")
                    {
                        tblobj.Length = (double)model.Diameter;
                        tblobj.WidthSize = model.CircularHeightSize;
                        tblobj.HeightSize = (double)model.Diameter;

                    }
                    tblobj.ComponentHandling = model.ComponentHandling;
                    if(model.ComponentHandling=="1")
                    {
                        if (new[] { "1", "2", "6", "7", "8" }.Contains(model.Conveyor))
                        {
                            tblobj.CarringCapacity = model.CarringCapacity;
                            tblobj.OverheadConveyorSubTypes = "NA";
                            tblobj.ConveyorNumber = "NA";
                            tblobj.Pitch = 0;
                            tblobj.Speed = 0;
                        }
                        else if (model.Conveyor == "3")
                        {
                            tblobj.CarringCapacity = 0;
                            tblobj.OverheadConveyorSubTypes = model.OverheadConveyorSubTypes;
                            tblobj.ConveyorNumber = model.ConveyorNumber;
                            tblobj.Pitch = model.Pitch;
                            tblobj.Speed = model.Speed;
                        }
                    }
                    else
                    {
                        tblobj.CarringCapacity = 0;
                        tblobj.OverheadConveyorSubTypes = "NA";
                        tblobj.ConveyorNumber = "NA";
                        tblobj.Pitch = 0;
                        tblobj.Speed = 0;
                    }
                    
                    tblobj.Paint = model.Paint;
                    tblobj.Powder = model.Powder;
                    tblobj.TypeOfPaint = model.TypeOfPaint == null ? " " : model.TypeOfPaint;
                    tblobj.TypeOfPowder = model.TypeOfPowder == null ? " " : model.TypeOfPowder;
                    tblobj.DFT = model.DFT;
                    tblobj.Conveyor = model.Conveyor != null ? model.Conveyor :"NA" ;
                    tblobj.NoOfColors = model.NoOfColors;
                    tblobj.NoOfCoats = model.NoOfCoats;
               
                    tblobj.LoadingUnloading = model.LoadingUnloading;
                    tblobj.ConsumptionPerDay = model.Consumption;
                    tblobj.Viscosity = model.Viscosity;
                   
                    tblobj.Weight = model.Weight;
                    tblobj.Shape = model.Shape;
                    tblobj.Orientation= model.Orientation;
                  //  tblobj.QtyperAssembly = 0;
                    tblobj.MaterialofConstruction = model.MaterialofConstruction;
                    tblobj.SurfaceArea = model.SurfaceArea;
                    tblobj.WallThickness = model.WallThickness;
                    tblobj.ProductionRequirement = model.ProductionRequirement;
                    tblobj.Workingdays = model.Workingdays;
                    tblobj.SpecificHeat = model.SpecificHeat;
                    tblobj.EffectiveWorking = model.EffectiveWorking;
                    tblobj.NumberofShifts = model.NumberofShifts;

                    tblobj.Image_Path = string.IsNullOrEmpty(model.Image_Path) ? "N/A" : Path.Combine(model.Image_Path);
                    tblobj.ExtractionChamberHeight = model.ExtractionChamberHeight;

                    tblobj.ComponentEntry = model.ComponentEntry==null?"": model.ComponentEntry;
                    tblobj.DoorType = model.DoorType == null ? "" : model.DoorType;
                    if (model.DoorType == "Manual")
                    {
                        tblobj.DoorSubType = model.TypeOfOperationForManual == null ? "" : model.TypeOfOperationForManual;
                    }
                    else
                    {
                        tblobj.DoorSubType = model.TypeOfOperationForMotorised == null ? "" : model.TypeOfOperationForMotorised;
                    }
                    tblobj.TypeOfHingedDoor = model.TypeOfHingedDoor == null ? "" : model.TypeOfHingedDoor;
                    tblobj.SideDoorLOcation = model.SideDoorLOcation == null ? "" : model.SideDoorLOcation;
                    tblobj.DoorWidth = model.SplitDoorWidth == null ? 0 : model.SplitDoorWidth;
                    tblobj.DoorHeight = model.SplitDoorHeight == null ? 0 : model.SplitDoorHeight;
                    tblobj.TypeOfHinges = model.TypeOfHinges == null ? "" : model.TypeOfHinges;
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
        public int GetAllEnquiryCount()
        {
            return _DbContext.EnquiryMasters.Count(c => !c.IsDeleted);
        }

        public EnquiryModel Editmodel(int id)
        {
            var data = (from e in _DbContext.EnquiryMasters
                        join Cust in _DbContext.CustomerMasters on e.CustomerID equals Cust.CustomerID
                        join contact in _DbContext.tblAddContactPersons on e.CustomerID equals contact.CustomerID
                        join comp in _DbContext.ComponentTables on e.ComponentID equals comp.ComponentID
                        join m in _DbContext.MotorDetails on e.EnquiryID equals m.EnquiryID
                        where e.IsDeleted == false && e.EnquiryID == id
                        select new EnquiryModel()
                        {
                            EnquiryId = e.EnquiryID,
                            SalesNO = e.SalesNO,
                            CompanyName = Cust.CompanyName ?? string.Empty,
                            Component = comp.Component ?? string.Empty,
                            ComponentID = comp.ComponentID,
                            CustomerID = Cust.CustomerID,
                            Category = comp.Category,
                            ProposalDate = e.ProposalDate,
                            CustomerAddress = Cust.CustomerAddress,
                            Contactperson = contact.ContactPerson,
                            Designation = contact.Designation,
                            MobileNo = contact.MobileNo,
                            Consumption=comp.ConsumptionPerDay,
                            ComponentHandling=comp.ComponentHandling,
                            Conveyor=comp.Conveyor,
                            LoadingUnloading=comp.LoadingUnloading,
                            EmailId = contact.EmailId,
                            Image_Path = comp.Image_Path != null ? Path.Combine(comp.Image_Path) : string.Empty,
                            Viscosity = comp.Viscosity,
                            DFT = comp.DFT,
                            DraftType=e.DraftType ?? string.Empty,
                            SubTypeOfDraft = e.SubTypeOfDraftType ?? string.Empty,
                            Paint =comp.Paint,
                            Powder=comp.Powder,
                            NoOfCoats=comp.NoOfCoats,
                            NoOfColors=comp.NoOfColors,
                            SpecificHeat=comp.SpecificHeat,
                            Pitch=comp.Pitch,
                            Speed=comp.Speed ,   
                            Shape=comp.Shape ?? string.Empty,
                            OverheadConveyorSubTypes=comp.OverheadConveyorSubTypes ?? string.Empty,
                            ConveyorNumber=comp.ConveyorNumber ?? string.Empty,
                            Orientation =comp.Orientation ?? string.Empty,
                            LengthSize = comp.Length,
                            WidthSize = comp.WidthSize,
                            HeightSize = comp.HeightSize,
                            Weight = comp.Weight,                            
                            MaterialofConstruction = comp.MaterialofConstruction ?? string.Empty,
                            SurfaceArea = comp.SurfaceArea,
                            WallThickness = comp.WallThickness,
                            ProductionRequirement = comp.ProductionRequirement,
                            Workingdays = comp.Workingdays,
                            NumberofShifts = comp.NumberofShifts,
                            EffectiveWorking = comp.EffectiveWorking ,
                            MotorType=m.MotorTypes ?? string.Empty,
                            ComponentEntry=comp.ComponentEntry ?? string.Empty,
                            DoorType=comp.DoorType ?? string.Empty,
                            TypeOfOperationForManual=comp.DoorSubType,
                            TypeOfOperationForMotorised=comp.DoorSubType,
                            TypeOfPaint=comp.TypeOfPaint,
                            TypeOfPowder=comp.TypeOfPowder,
                            ExtractionChamberHeight =comp.ExtractionChamberHeight ?? string.Empty,
                            HeightOfPlenum=e.PlenumHeight ?? string.Empty,
                            SideDoorLOcation=comp.SideDoorLOcation ?? string.Empty,
                            TypeOfHingedDoor=comp.TypeOfHingedDoor ?? string.Empty,
                            TypeOfHinges=comp.TypeOfHinges??string.Empty,
                        
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
            tblobj.CityId = model.CityId;
            tblobj.Pin = model.Pin;
            tblobj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            tblobj.CreatedDate = DateTime.Now;
            tblobj.IsDeleted = false;
            tblobj.ModifiedBy = 0;
            _DbContext.CustomerMasters.Add(tblobj);
            _DbContext.SaveChanges();
            return 1;
        }

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
