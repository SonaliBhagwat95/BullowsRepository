using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class tblState
    {
        [Key]
        public int StateId { get; set; }
        public string State { get; set; }
        public bool Isdeleted { get; set;}
    }
    public class tblDistrict
    {
        [Key]
        public int DistrictId { get; set; }
        [ForeignKey("StateId")]
        public int StateId { get; set; }
        public string District { get; set; }
        public bool Isdeleted { get; set; }
    }
    public class tblCity
    {
        [Key]
        public int CityId { get; set; }
        [ForeignKey("DistrictId")]
        public int DistrictId { get; set; }
        public string City { get; set; }
        public bool Isdeleted { get; set; }
    }
}
