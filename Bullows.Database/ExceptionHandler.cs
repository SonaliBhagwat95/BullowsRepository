using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class ExceptionHandler
    {
        [Key]
        public int Id { get; set; }

        public string Classname { get; set; }

        public string Methodname { get; set; }

        public string Error { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UserId { get; set; }
    }
}
