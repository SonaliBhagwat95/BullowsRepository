using bullows.business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Business
{
    public class UowBusiness
    {
        public static PanelInput panelInput
        {
            get { return new PanelInput(); }
        }
    }
}
