using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public enum DoorSide
    {
        Left,
        Right
    }
    public class DoorDimensionsModel
    {
        public string doorType { get; set; }
        public string doorSubType { get; set; }
        public string sideDoorLocation { get; set; }
        public double doorWidth { get; set; }
        public double doorHeight { get; set; }
        public double xOffeset { get; set; }
        public double xOffesetForFrontDoor { get; set; }
        public double yOffeset { get; set; }
        //public string Side { get; set; }
        public DoorSide Side { get; set; }

    }
}
