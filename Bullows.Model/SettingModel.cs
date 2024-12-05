using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class SettingModel
    {
        public int SId { get; set; }
        public int EnquiryID { get; set; }
        public bool settingStatus { get; set; }
        public double PanelWidth { get; set; } = 1140;

        [Required(ErrorMessage = "*")]
        //[Range(0, 2390, ErrorMessage = "PanelHeight must be less than or equal to 2390.")]
        public double PanelHeight { get; set; } = 2390;
        [Required(ErrorMessage = "*")]
        public double SheetThickness { get; set; } = 1.2;
        [Required(ErrorMessage = "*")]
        public double StandardBend1 { get; set; } = 15;
        [Required(ErrorMessage = "*")]
        public double StandardBend2 { get; set; } = 38;
        public double H { get; set; }
        public double W { get; set; }
        public double T { get; set; }
        public string Materials { get; set; }
        public string BendSection { get; set; }
        public string StructuralMember { get; set; }
        public string BendStructural { get; set; }


        [Required(ErrorMessage = "*")]
        public double PitchDistance { get; set; } = 150;
        [Required(ErrorMessage = "*")]
        public string SlotDimention { get; set; }
        public string SalesNo { get; set; }
        public string Section { get; set; }
        public string LightTypes { get; set; }
        public string LightSubTypes { get; set; }
        public decimal LuxLevel { get; set; }
        public decimal Lumens { get; set; }
        public int Quantity { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }


    }
}
