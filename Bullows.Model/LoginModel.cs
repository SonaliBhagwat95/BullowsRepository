using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage ="*")]
        public string LoginId { get; set; }
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }
        public string? Message { get; set; }
    }
}
