using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class PanelCutout
    {
        [Key]
        public int CutoutID { get; set; }

        [ForeignKey("Cutoutprojectid")]
        public int ProjectID { get; set; }
        [ForeignKey("PanelInputID")]
        public int PanelInputID { get; set; }
        public int PartName { get; set; }
        public double CutoutLength { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]

        public double CutoutWidth { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]

        public double CutoutXDistance { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public double CutoutYDistance { get; set; }
        public bool IsDeleted { get; set; }
    }
}
