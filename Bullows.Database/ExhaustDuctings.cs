using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class ExhaustDuctings
    {
        public int DuctId { get; set; }
        public string BendType { get; set;}
        public decimal DuctWidth { get; set; }
        public decimal DuctHeight { get; set; }
        public decimal DuctThickness { get; set; }
        public string SalesNo { get; set; }
        public int DuctLength { get; set; }
        [ForeignKey("EnquiryID")]
        public int EnquiryID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public decimal DuctWeight { get; set; }

    }
}
