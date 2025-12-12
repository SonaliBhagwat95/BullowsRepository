using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class MaterialModel
    {
        public int MOCID { get; set; }
        public string MOC {  get; set; }    
        public double Density {  get; set; }    
        public double Rate {  get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        // Add this property for BO Calculations
        public decimal Quantity { get; set; } 
        public decimal Cost { get; set; }
        public int FID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double filterWeight { get; set; }

    }
}
