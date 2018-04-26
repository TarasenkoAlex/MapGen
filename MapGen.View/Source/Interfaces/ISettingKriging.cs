using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MapGen.View.Source.Classes.SettingInterpol;

namespace MapGen.View.Source.Interfaces
{
    public interface ISettingKriging
    {
        Grid Grid { get; }
        VSettingInterpolKriging SettingInterpol { get; set; }
    }
}
