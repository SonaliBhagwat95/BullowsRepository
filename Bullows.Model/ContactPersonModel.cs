using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class ContactPersonModel
    {
        public int ContactId { get; set; }
        public int CustomerID { get; set; }

        public string CompanyName { get; set; }

        public string CustomerAddress { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string Designation { get; set; }
        public string EmailId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
