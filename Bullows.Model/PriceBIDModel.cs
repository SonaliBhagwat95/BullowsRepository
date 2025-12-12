using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class PriceBIDModel
    {

        public string SalesNO { get; set; }
        public decimal PriceOfRM { get; set; }
        public decimal PriceOfBO { get; set; }
        public bool IsPriceBidApproved { get; set; }
       
        public decimal LabourCost { get; set; }
        public decimal EAndCCost { get; set; }
        public decimal FreightCost { get; set; }
        public decimal BasicCost { get; set; }
        public decimal BestPrice { get; set; }
        public decimal Insurance { get; set; }
        public decimal CommercialFactor { get; set; }
        public decimal IncentiveFactor { get; set; }
        public decimal TPCCost { get; set; }
        public decimal DesignChargesCost { get; set; }
        public decimal POVALUE { get; set; }
        public bool ConfirmPriceBID { get; set; }
        public decimal PandFCost { get; set; }
        public decimal TVCCost { get; set; }
        public decimal Throughputvalue { get; set; }
        public decimal Overheads { get; set; }



    }
}
