using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class PanelInputModel
    {
        public int selectedProjectID;
       // public int selectedPanelInputID;

        [Required(ErrorMessage = "*")]
        public int PanelInputID { get; set; }
        [Required(ErrorMessage = "*")]
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int Type { get; set; }
        public string PartDescription { get; set; }
        public double PanelWidth { get; set; } = 1140;

        [Required(ErrorMessage = "*")]
        public double PanelHeight { get; set; } = 2390;
        [Required(ErrorMessage = "*")]
        public double SheetThickness { get; set; } = 1.2;
        [Required(ErrorMessage = "*")]
        public double StandardBend1 { get; set; } = 15;
        [Required(ErrorMessage = "*")]
        public double StandardBend2 { get; set; } = 38;

        [Required(ErrorMessage = "*")]
        public double PitchDistance { get; set; } = 150;
        [Required(ErrorMessage = "*")]
        public double SlotDimentions { get; set; }
        [Required(ErrorMessage = "*")]
        public int NoofPanels { get; set; }=1;
       
       public int TubeLightID { get; set; }
        public double TubelightWidth { get; set; }
        [Required(ErrorMessage = "*")]
        public double TubelightHeight { get; set; }
        [Required(ErrorMessage = "*")]
        public double XDistance { get; set; }
        [Required(ErrorMessage = "*")]
        public double YDistance { get; set; }

        //Service Door 
        public int ServicedoorID { get; set; }
        public double ServiceDoorHeight { get; set; }
        public double ServiceDoorWidth { get; set; }
        public double GlassLength { get; set; }

        public double GlassWidth { get; set; }
        [Required(ErrorMessage = "*")]
        public double DoorXDistance { get; set; }
        [Required(ErrorMessage = "*")]
        public double DoorYDistance { get; set; }

        //Duct cutout
        public int DuctCutoutID { get; set; }
        [Required(ErrorMessage = "*")]
        public double CutoutLength { get; set; }
        [Required(ErrorMessage = "*")]
        public double CutoutWidth { get; set; }
        [Required(ErrorMessage = "*")]
        public double CutoutXDistance { get; set; }
        [Required(ErrorMessage = "*")]
        public double CutoutYDistance { get; set; }
        public bool IsDeleted { get; set; }

        public int PartName { get; set; }
    }
}
