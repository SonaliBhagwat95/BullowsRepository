using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Bullows.Database
{
    public class Users
    {
        [Key]
        public int EmpId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Password { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ConfirmPassword { get; set; }
        public string EmailId { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string MobileNo { get; set; }
        [Required]
        [DefaultValue(false)]
        public int IsDeleted { get; set; }
        [Required]
        [ForeignKey("UserRoleID")]
        public int UserRoleID { get; set; }
        public virtual UserRoles UserRole { get; set; }
    }
}
