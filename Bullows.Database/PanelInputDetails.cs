using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class PanelInputDetails
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
   
    

    

}
