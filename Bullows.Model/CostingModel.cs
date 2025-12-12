using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class CostingModel
    {
        public string  EnquiryCode { get; set; }
        public List<PaintBoothModel> DetailsList { get; set; } = new List<PaintBoothModel>();
        public List<PaintBoothModel> FilteredDetailsList { get; set; }
        public List<PaintBoothModel> metalBaffles { get; set; }
        public double RMWeightOfPanels { get; set; }
        public double TotalWeightOfFrame { get; set; }
        public double TotalWeight { get; set; }
        public double TotalPriceofPanels { get; set; }
        public double TotalPriceofFrame { get; set; }

        public List<MaterialModel> Materials { get; set; }

        public bool IsInFilteredList(PaintBoothModel panel)
        {
            return FilteredDetailsList.Any(f => f.EnquiryId == panel.EnquiryId &&
                                                f.PanelPosition == panel.PanelPosition &&
                                                //f.EqualPanelWidthForD == panel.EqualPanelWidthForD &&
                                                f.EqualPanelWidthByH == panel.EqualPanelWidthByH &&
                                                f.SheetThickness == panel.SheetThickness &&
                                                f.NoofPanels == panel.NoofPanels &&
                                                f.PanelWeight == panel.PanelWeight &&
                                                f.Section==panel.Section);
        }
    }
}
