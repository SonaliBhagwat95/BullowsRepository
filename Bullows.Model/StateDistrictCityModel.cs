using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class StateDistrictCityModel
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }

    }
}
