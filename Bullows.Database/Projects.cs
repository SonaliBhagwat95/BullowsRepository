using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bullows.Database
{
    public class Projects
    {
        [Key]
        public int ProjectID { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string ProjectCode { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string ProjectName { get; set; }        
        [Column(TypeName = "varchar(100)")]
        public string CustomerName { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
    }
}
 