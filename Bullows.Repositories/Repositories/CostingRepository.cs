
using Bullows.Database;
using Bullows.Model;
using Microsoft.AspNetCore.Http;


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
                               StandardPanelWidthForD = p.StandardPanelDepth,
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

        public List<PaintBoothModel> GetAllMetalBafflesDetails(string enquiryCode)
        {
            var data = _DbContext.MetalBaffleDetails.Where(x => x.SalesNo == enquiryCode).Select(item => new PaintBoothModel()
            {

                StandardPanelWidthForD = (double)item.BaffleWidth,
                EqualPanelWidthByH = (double)item.BaffleHeight,
                SheetThickness = 0,
                NoofPanels = item.Quantity,
                PanelWeight = (double)item.BaffleWeight
            }).ToList();
            return data;
        }
        public bool UpdateTotalPriceOfFrame(string enquiryCode, decimal totalPriceOfFrame, decimal totalPriceOfPanels)
        {
            // Find the record with the given EnquiryCode
            var enquiry = _DbContext.PanelDetails
                                  .FirstOrDefault(e => e.SalesNo == enquiryCode);

            if (enquiry != null)
            {
                enquiry.CostOfPanels = totalPriceOfPanels;
                enquiry.CostOfFrame += totalPriceOfFrame;

                // Save changes to the database
                _DbContext.SaveChanges();
                return true; // Indicate success
            }

            return false; // Indicate that the record was not found
        }

        private int GetEnquiryID(string enquiryCode)
        {
            var enquiryId = _DbContext.EnquiryMasters
                                     .Where(x => x.SalesNO == enquiryCode)
                                     .Select(x => x.EnquiryID)
                                     .FirstOrDefault();
            return enquiryId;
        }
        public int SavePriceDetailsValues(BOCalculationModel model, string EnquiryCode)
        {
            PriceDetailsTable obj = new PriceDetailsTable();
            int Enquiryid = GetEnquiryID(EnquiryCode);
            obj = new PriceDetailsTable();
            obj.IsDeleted = false;
            obj.SalesNo = EnquiryCode;
            obj.EnquiryId = Enquiryid;
            obj.TotalPriceOfRawMaterials = model.TotalPriceOfPanels + model.TotalPriceOfFrame + model.RowMaterials;
            obj.TotalPriceOfBoughtOut = model.BoughtOut;
            obj.IsPriceBidApproved = model.IsPriceBidApproved;
            obj.CreatedBy = Session.GetInt32("UserId") != null ? Session.GetInt32("UserId") : 0;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedBy = 0;
            obj.ModifiedDate = DateTime.Now;
            _DbContext.PriceDetailsTable.Add(obj);
            _DbContext.SaveChanges();
            return 1;
        }
        public int SaveCostOFFrame(string enquiryCode, CostingModel model)
        {
            // Find the record with the given EnquiryCode
            var enquiry = _DbContext.PanelDetails
                                  .FirstOrDefault(e => e.SalesNo == enquiryCode);

            if (enquiry != null)
            {
                enquiry.CostOfPanels = (decimal)model.TotalPriceofPanels;
                enquiry.CostOfFrame = (decimal)model.TotalPriceofFrame;

                // Save changes to the database
                _DbContext.SaveChanges();
                // Indicate success
            }
            return 1;
        }
        public (decimal TotalPriceOfFrame, decimal TotalPriceOfPanels) GetCostOFFrame(string enquiryCode)
        {
            var enquiry = _DbContext.PanelDetails
                                  .FirstOrDefault(e => e.SalesNo == enquiryCode);
            if (enquiry != null)
            {
                return (enquiry.CostOfFrame, enquiry.CostOfPanels);
            }
            return (0, 0);
        }
        public List<PriceBIDModel> GetEnquiryCodes(string enquiryCode)
        {
            var filteredData = _DbContext.PriceDetailsTable
            .Where(e => e.IsPriceBidApproved == true && e.SalesNo == enquiryCode)
            .Select(e => new PriceBIDModel
            {
             // EnquiryId =  e.EnquiryId,
                SalesNO=   e.SalesNo,
                PriceOfRM =  e.TotalPriceOfRawMaterials,
                PriceOfBO= e.TotalPriceOfBoughtOut,
                
            }).ToList();
            return filteredData;


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
                    filterWeight = (double)item.FrameWeight
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

        public List<ExhaustDuctingModel> GetAllExhaustDuctingDetails(string EnquiryCode)
        {

            var lstData = (from paintBooth in _DbContext.PaintBoothDetails
                           join enquiry in _DbContext.EnquiryMasters
                           on paintBooth.EnquiryId equals enquiry.EnquiryID
                           join duct in _DbContext.ExhaustDuctings
                           on enquiry.EnquiryID equals duct.EnquiryID
                           where enquiry.SalesNO == EnquiryCode &&
                           paintBooth.IsDeleted == false
                           && duct.IsDeleted == false
                           select new ExhaustDuctingModel
                           {
                               BendType = duct.BendType,
                               DuctHeight = duct.DuctHeight,
                               DuctWidth = duct.DuctWidth,
                               DuctThickness = duct.DuctThickness,
                               DuctLength = duct.DuctLength,
                               DuctWeight = duct.DuctWeight
                           }).ToList();

            return lstData;
        }
        public int GetCostingCountByEnquiryID()
        {

            return _DbContext.PanelDetails.Select(c => c.EnquiryId).Distinct().Count();

        }

        public ProposalModel GetAllDetails(string EnquiryCode)
        {
            var data = (from e in _DbContext.EnquiryMasters
                        join c in _DbContext.CustomerMasters
                        on e.CustomerID equals c.CustomerID
                        join cp in _DbContext.tblAddContactPersons
                       on e.CustomerID equals cp.CustomerID
                        join comp in _DbContext.ComponentTables
                        on e.ComponentID equals comp.ComponentID
                        join price in _DbContext.PriceDetailsTable
                        on e.EnquiryID equals price.EnquiryId
                        where e.SalesNO == EnquiryCode
                        select new ProposalModel
                        {
                            EnquiryID = e.EnquiryID,
                            CompanyName = c.CompanyName,
                            ContactPerson = cp.ContactPerson,
                            SalesNO = e.SalesNO,
                            WidthSize = comp.WidthSize,
                            HeightSize = comp.HeightSize,
                            Length = comp.Length,
                            Workingdays = comp.Workingdays,
                            NumberofShifts = comp.NumberofShifts,
                            EffectiveWorking = comp.EffectiveWorking,
                            Weight = comp.Weight,
                            ProductionRequirement = comp.ProductionRequirement,
                            ComponentHandling = comp.ComponentHandling,
                            Viscosity = comp.Viscosity,
                            DFT = comp.DFT,
                            SpecificHeat = comp.SpecificHeat,
                            LoadingUnloading = comp.LoadingUnloading,
                            NoOfColors = comp.NoOfColors,
                            Category = comp.Category,
                            
                        }).FirstOrDefault();
            return data;
        }
        public ProposalModel GetMaterialsValue(string EnquiryCode)
        {
            var data = (from e in _DbContext.EnquiryMasters
                        join p in _DbContext.PaintBoothDetails
                        on e.EnquiryID equals p.EnquiryId
                        join s in _DbContext.SettingDetails
                        on e.EnquiryID equals s.EnquiryID
                        where e.SalesNO == EnquiryCode
                        join t in _DbContext.TubeLightDetails
                        on e.EnquiryID equals t.EnquiryID
                        join m in _DbContext.MotorDetails
                        on e.EnquiryID equals m.EnquiryID
                        join mc in _DbContext.tblMotorFlange
                        on m.MotorCatalogID equals mc.MotorCatalogID
                        select new ProposalModel
                        {
                            EXhaustCapacity = p.CapacityofBlowerinHr,
                            Materials = s.Materials,
                            Lumens=s.Lumens,
                            TubelightQuantity=t.Quantity,
                            MotorCapacity= (decimal)mc.RatedOutputHP

                        }).FirstOrDefault();
            return data;
        }
        public PriceBIDModel FetchPriceBidRecoreds(string EnquiryCode)
        {
           var lstData = _DbContext.PriceDetailsTable
                .Where(x => x.IsDeleted == false && x.SalesNo == EnquiryCode)
                .Select(item => new PriceBIDModel()
                {
                    POVALUE = item.POVALUE,
                    FreightCost = item.FreightCost,
                    Insurance = item.Insurance,
                    EAndCCost=item.EAndCCost,
                    PandFCost=item.PandFCost
                    //filterWeight = (double)item.FrameWeight
                }).FirstOrDefault();
            return lstData;
        }
        public List<PriceBIDModel> GetPriceBIDData(string enquiryCode)
        {
            // Fetch data using LINQ from PriceBIDtable
            var priceBIDData = _DbContext.PriceDetailsTable
                .Where(p => p.SalesNo == enquiryCode && p.IsPriceBidApproved == true)
                .Select(p => new PriceBIDModel
                {
                    PriceOfRM = p.TotalPriceOfRawMaterials,
                    PriceOfBO = p.TotalPriceOfBoughtOut
                })
                .ToList();

            return priceBIDData;
        }
        

        public decimal CalculateTotalWeightByEnquiryCode(string enquiryCode)
        {
            // Fetching PanelWeight from PanelDetails table for the given EnquiryCode
            var totalWeight = _DbContext.PanelDetails
                .Where(p => p.SalesNo == enquiryCode)  // Filter by EnquiryCode
                .Sum(p => p.PanelWeight);  // Sum the PanelWeight from PanelDetails table

            // Fetching and summing PanelWeight from other tables (Table1, Table2, etc.)
            var weightFromFilterFrame = _DbContext.FilterFrameDetails
                .Where(t => t.SalesNO == enquiryCode)
                .Sum(t => t.FrameWeight);

            var weightFromMetalBaffle = _DbContext.MetalBaffleDetails
                .Where(t => t.SalesNo == enquiryCode)
                .Sum(t => t.BaffleWeight);

            var weightFromExhaustDuct = _DbContext.ExhaustDuctings
                .Where(t => t.SalesNo == enquiryCode)
                .Sum(t => t.DuctWeight);

            // Combine the weights from all tables
            var totalWeightAllTables = ((decimal)totalWeight) + weightFromFilterFrame + weightFromMetalBaffle + weightFromExhaustDuct;
            return (decimal)totalWeightAllTables;
        }

        public bool UpdatePriceDetails(PriceBIDModel model)
        {
            var savedRecord = _DbContext.PriceDetailsTable.FirstOrDefault(x => x.SalesNo == model.SalesNO);

            //  Update the Quantity value
            if (savedRecord != null)
            {
                savedRecord.LabourCost = model.LabourCost;
                savedRecord.EAndCCost = model.EAndCCost;
                savedRecord.PandFCost = model.PandFCost;
                savedRecord.FreightCost = model.FreightCost;
                savedRecord.BasicCost = model.BasicCost;
                savedRecord.BestPrice = model.BestPrice;
                savedRecord.Insurance = model.Insurance;
                savedRecord.CommercialFactor = model.CommercialFactor;
                savedRecord.IncentiveFactor = model.IncentiveFactor;
                savedRecord.TPCCost = model.TPCCost;
                savedRecord.DesignChargesCost = model.DesignChargesCost;
                savedRecord.POVALUE = model.POVALUE;
                savedRecord.TVCCost = model.TVCCost;
                savedRecord.ConfirmPriceBID = model.ConfirmPriceBID;
                _DbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

