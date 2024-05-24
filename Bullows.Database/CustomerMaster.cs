using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class CustomerMaster
    {
        [Key]
        public int CustomerID { get; set; }
        [Required(ErrorMessage ="*")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "*")]
        public string CustomerAddress { get; set; }
        
        [Required(ErrorMessage = "*")]
        public string Designation { get; set; }

       
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey ("StateId")]
        public int StateId { get; set; }
        [ForeignKey("DistrictId")]
        public int DistrictId { get; set; }
        [ForeignKey("CityId")]
        public int CityId { get; set; }
        public int Pin { get; set; }
        public string PAN { get; set; }


    }
}
