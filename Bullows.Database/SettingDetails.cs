using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class SettingDetails
    {
        [Key]
        public int SId { get; set; }
        [ForeignKey("EnquiryID")]
        public int EnquiryID { get; set; }
        public bool settingStatus { get; set; }
        public double PanelWidth { get; set; } 
        
        public double PanelHeight { get; set; }         
        public double SheetThickness { get; set; }        
        public double StandardBend1 { get; set; }        
        public double StandardBend2 { get; set; } 
        public string Materials { get; set; }     
        public double PitchDistance { get; set; }
        public string SlotDimentions { get; set; }
        public string SalesNO { get; set; }
        public string Section { get; set; }
        public double H { get; set; }
        public double W { get; set; }
        public double T { get; set; }
        public string LightTypes { get; set; }
        public decimal LuxLevel { get; set; }
        public decimal Lumens { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string BendSection { get; set; }
    }
}
