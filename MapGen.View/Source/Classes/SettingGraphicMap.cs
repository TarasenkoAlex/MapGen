using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes
{
    public class SettingGraphicMap
    {
        public bool IsDrawDataMap { get; set; } = true;
        public bool IsDrawStripsEdgeOfMap { get; set; } = true;
        public bool IsDrawGridLatitudeAndLongitude { get; set; } = true;
        public bool IsDrawSourcePointsOfMap { get; set; } = true;
    }
}
