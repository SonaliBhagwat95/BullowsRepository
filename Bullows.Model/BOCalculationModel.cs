using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class BOCalculationModel
    {
        public string EnquiryCode { get; set; }
        public decimal TotalPriceOfFrame { get; set; }
        public decimal TotalPriceOfPanels { get; set; }
        public decimal RowMaterials { get; set; }
        public decimal BoughtOut { get; set; }
        public bool IsPriceBidApproved { get; set; }
        public List<MaterialModel> Materials { get; set; }
        public List <MaterialModel> FrameDetails { get; set; }
        public List <PaintBoothModel> PaintBooth { get; set; }
        public List <SettingModel> TubeLightDetails { get; set; }
        public List<ExhaustDuctingModel> ExhaustDuctDetails { get; set; }
    }
}
