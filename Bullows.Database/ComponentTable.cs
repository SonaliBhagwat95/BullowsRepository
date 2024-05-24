using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class ComponentTable
    {
        [Key]
        public int ComponentID { get; set; }
        [Required(ErrorMessage ="*")]
        public int Category { get; set; }
        public string Component { get; set; }

        public double Length{ get; set; }
        public double WidthSize { get; set; }
        public double HeightSize { get; set; }
        public double Weight { get; set; }
        public int QtyperAssembly { get; set; }

        public string MaterialofConstruction { get; set; }

        public double SurfaceArea { get; set; }

        public double WallThickness { get; set; }

        public int ProductionRequirement { get; set; }

        public int Workingdays { get; set; }

        public int NumberofShifts { get; set; }
        [DisplayName("Choose File")]
        public string Image_Path { get; set; }
        public double EffectiveWorking { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Conveyor { get; set; }
        public double Pitch { get; set; }
        public double Speed { get; set; }
        public string LoadingUnloading { get; set; }
        public string ComponentHandling { get; set; }
        public int NoOfColors { get; set; }
        public int NoOfCoats { get; set; }
        public double Viscosity { get; set; }
        public double Paint { get; set; }
        public double Powder { get; set; }
        public double DFT { get; set; }
        public string ConsumptionPerDay { get; set; }
       
    }
}
