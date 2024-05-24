using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class tblAddContactPerson
    {
        [Key]
        public int ContactId { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "*")]
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }
       
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }


    }
}
