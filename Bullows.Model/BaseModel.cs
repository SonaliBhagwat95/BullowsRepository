using System;
using System.Collections.Generic;
using System.Text;

namespace Bullows.Model
{
    public class BaseModel
    {
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }      
        public bool IsDeleted { get; set; }
    }
}
