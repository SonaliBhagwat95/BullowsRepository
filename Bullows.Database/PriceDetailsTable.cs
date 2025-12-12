using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class PriceDetailsTable
    {
        [Key]
        public int PID { get; set; }
        [ForeignKey("EnquiryId")]
        public int EnquiryId { get; set; }
        public string SalesNo { get; set; }
        public decimal TotalPriceOfRawMaterials { get; set; }
        public decimal TotalPriceOfBoughtOut { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
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
        public decimal TVCCost { get; set; }
        public decimal PandFCost { get; set; }
        public bool ConfirmPriceBID { get; set; }

    }
}
