using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class PanelInputDetail
    {
        [Key]
        public int PanelInputID { get; set; }

        [ForeignKey("ProjectID")]
        public int ProjectID { get; set; }
        //public string PanelInputName { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double PanelWidth { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double PanelHeight { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double SheetThickness { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double StandardBend1 { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double StandardBend2 { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double PitchDistance { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double SlotDimentions { get; set; }
       
        public int NoofPanels { get; set; }
        public bool IsDeleted { get; set; }

    }
    //public class Panel_TubeLightDetail
    //{
    //    [Key]
    //    public int TubeLightID { get; set; }

    //    [ForeignKey("PanelInputID")]
    //    public int PanelInputID { get; set; }
    //    public int PartName { get; set; }
    //    public int Type { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double TubelightWidth { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double TubelightHeight { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double XDistance { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double YDistance { get; set; }
    //    public bool IsDeleted { get; set; }
    //}
    //public class Panel_ServiceDoorDetail
    //{
    //    [Key]
    //    public int ServicedoorID { get; set; }
    //    [ForeignKey("PanelInputID")]
    //    public int PanelInputID { get; set; }
    //    public int PartName { get; set; }
    //    public int Type { get; set; }

    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double ServiceDoorHeight { get; set; }

    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double ServiceDoorWidth { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double GlassLength { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]

    //    public double GlassWidth { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]

    //    public double DoorXDistance { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]

    //    public double DoorYDistance { get; set; }
    //    public bool IsDeleted { get; set; }

    //}
    //public class Panel_WatchGlassDetail
    //{
    //    [Key]
    //    public int WatchGlassID { get; set; }
    //    [ForeignKey("PanelInputID")]
    //    public int PanelInputID { get; set; }
    //    public int PartName { get; set; }
    //    public int Type { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public int GlassLength { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public int GlassWidth { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public int DoorXDistance { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public int DoorYDistance { get; set; }
    //    public bool IsDeleted { get; set; }


    //}
    //public class Panel_DuctCutoutDetail
    //{
    //    [Key]
    //    public int DuctCutoutID { get; set; }
    //    [ForeignKey("PanelInputID")]
    //    public int PanelInputID { get; set; }
    //    public int PartName { get; set; }
    //    public int Type { get; set; }
    //    public double CutoutLength { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]

    //    public double CutoutWidth { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]

    //    public double CutoutXDistance { get; set; }
    //    [Required]
    //    [Column(TypeName = "decimal(18,2)")]
    //    public double CutoutYDistance { get; set; }
    //    public bool IsDeleted { get; set; }
    //}
    

    

}
