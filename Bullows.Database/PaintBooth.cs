using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Database
{
    public class PaintBooth
    {
        public int PaintBoothID { get; set; }
       
        public int EnquiryID { get; set; }
        
        public double D1 { get; set; }

        public double JobSize { get; set; }
       
        public double D2 { get; set; }
       
        public double D3 { get; set; }
       
        public double W1 { get; set; }
       
        public double W2 { get; set; }
       
        public double W3 { get; set; }
       
        public double D { get; set; }
        

        public double H1 { get; set; }
       
        public double H2 { get; set; }
        
        public double W { get; set; }
        public double H { get; set; }
        public double PanelWidth { get; set; }
        public double PanelHeight { get; set; }
        public double SheetThickness { get; set; }
        public double StandardBend1 { get; set; }
        public double StandardBend2 { get; set; }
        public double PitchDistance { get; set; }
        public int NoofPanels { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

       

    }
}
