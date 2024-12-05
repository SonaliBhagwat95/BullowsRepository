using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
        public class TubeLightDetails
    {
            public int TubeLightID { get; set; } // Primary key with auto-increment
            [ForeignKey("EnquiryID")]
            public int EnquiryID { get; set; }
            public string SalesNo { get; set; }
            public string? LightType { get; set; }
            public string? LightSubType { get; set; }
            public decimal? LuxLevel { get; set; }
            public decimal? Lumens { get; set; }
            public bool IsDeleted { get; set; }
            public int Quantity { get; set; }
        }
    
}
