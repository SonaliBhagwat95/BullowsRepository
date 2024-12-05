using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class tblMOC
    {
        [Key]
        public int MOCId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string MOC { get; set; }
        public double Density { get; set; }
        public double Rate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
    }
}
