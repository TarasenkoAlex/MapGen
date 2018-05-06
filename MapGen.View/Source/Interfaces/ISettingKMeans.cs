using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MapGen.View.Source.Classes.SettingClustering;

namespace MapGen.View.Source.Interfaces
{
    public interface ISettingKMeans
    {
        Grid Grid { get; }
        VSettingCLKMeans SettingCl { get; set; }
    }
}
