using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Bullows.Database
{
    public class UserRoles
    {
        [Key]
        public int UserRoleID { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Description { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
