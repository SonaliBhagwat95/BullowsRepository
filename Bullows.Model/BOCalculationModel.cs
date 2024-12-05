using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class BOCalculationModel
    {
        public List<MaterialModel> Materials { get; set; }
        public List <MaterialModel> FrameDetails { get; set; }
        public List <PaintBoothModel> PaintBooth { get; set; }
        public List <SettingModel> TubeLightDetails { get; set; }
    }
}
