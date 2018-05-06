using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.View.Source.Classes.SettingClustering;

namespace MapGen.View.Source.Classes.SettingGen
{
    public class VSettingGen
    {
        /// <summary> 
        /// Формулы для определения норм отбора. 
        /// </summary>
        public VSelectionRules SelectionRule { get; set; } = VSelectionRules.Topfer;

        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        public IVSettingCL SettingCL { get; set; }
    }
}
