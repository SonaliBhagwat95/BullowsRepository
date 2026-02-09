using devDept.Eyeshot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class PanelDrawResult
    {
        public DesignDocument drawing { get; set; }
        public double Weight { get; set; }
        //public bool IsSmallPanel { get; set; }
        public string lstpath { get; set; }
    }


}
