using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class EnquiryMaster
    {
        [Key]
        public int EnquiryID { get; set; }
        [ForeignKey("CustomerID")]
        public int CustomerID { get; set; }
        [ForeignKey("EmpId")]
        public int? CreatedBy { get; set; }
        [ForeignKey("ComponentID")]
        public int ComponentID { get; set; }
        public DateTime ProposalDate { get; set; }
        public string SalesNO { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
       
    }
}
