using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class EnquiryModel
    {
        public DateTime ProposalDate { get; set; }

        public int EnquiryId { get; set; }

        public string SalesNO { get; set; }

       
        public int CustomerID { get; set; }
        public int ComponentID { get; set; }

       public string ComponentHandling { get; set; }
        public int NoOfColors { get; set; }
        public int NoOfCoats { get; set; }

        public string SpecificHeat { get; set; }
        public string CompanyName { get; set; }

        public string CustomerAddress { get; set; }

       

        public string Designation { get; set; }

        
        public int Category { get; set; }

        public string Component { get; set; }

        public double LengthSize { get; set; }
        public double WidthSize { get; set; }
        public double HeightSize { get; set; }
        public double Weight { get; set; }
        public int QtyperAssembly { get; set; }

        public string MaterialofConstruction { get; set; }

        public double SurfaceArea { get; set; }

        public double WallThickness { get; set; }
        public string Conveyor { get; set; }
        public double Pitch { get; set; }
        public double Speed { get; set; }
        public string LoadingUnloading { get; set; }
        public int ProductionRequirement { get; set; }
        public double Viscosity { get; set; }
        public double Paint { get; set; }
        public double Powder { get; set; }
        public double DFT { get; set; }
        public int Workingdays { get; set; }

        public int  NumberofShifts { get; set; }
        public double EffectiveWorking { get; set; }
        public string Image_Path { get; set; }
        public IFormFile imageFile { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int StateId { get; set; }

        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public string State { get; set; }
        public int Pin { get; set; }
        public string PAN { get; set; }
        public string Contactperson { get; set; }
        public string MobileNo { get; set; }

        public string EmailId { get; set; }

        public string Consumption { get; set; }

        public EnquiryModel()
        {
            ContactPersons = new List<ContactPersonModel>();
        }
        public  List<ContactPersonModel> ContactPersons { get; set; }

       

    }
}
