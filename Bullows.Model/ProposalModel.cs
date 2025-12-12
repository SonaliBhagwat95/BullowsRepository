using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public  class ProposalModel
    {
        public string SalesNO { get; set; }
        public int  EnquiryID { get; set; }
       
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public double WidthSize { get; set; }
        public double HeightSize { get; set; }
        public double Length { get; set; }
        public double Workingdays { get; set; }
        public double NumberofShifts { get; set; }
        public double EffectiveWorking { get; set; }
        public double Weight { get; set; }
        public int ProductionRequirement { get; set; }
        public string ComponentHandling { get; set; }
        public double Viscosity { get; set; }
        public double DFT { get; set; }
        public int NoOfColors { get; set; }
        public double SpecificHeat { get; set; }
        public string LoadingUnloading { get; set; }
        public int Category { get; set; }
        public string CategoryText { get; set; }

        public string Materials { get; set; }
        public double EXhaustCapacity { get; set; }
        public decimal Motorcapacity { get; set; }
        public decimal Lumens { get; set; }
        public int TubelightQuantity { get; set; }
        public decimal MotorCapacity { get; set; }
    }
}
