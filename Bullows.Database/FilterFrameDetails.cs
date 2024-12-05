using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class FilterFrameDetails
    {
        public int FID { get; set; }
        public decimal FrameWidth { get; set; }
        public decimal FrameHeight { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("EnquiryID")]
        public int EnquiryID { get; set; }
        public string SalesNO { get; set; }
        public bool IsDeleted { get; set; }
    }
}
