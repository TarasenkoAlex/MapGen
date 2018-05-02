using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Setting;

namespace MapGen.Model.Generalization.Setting
{
    public class SettingGen
    {
        /// <summary> 
        /// Формулы для определения норм отбора. 
        /// </summary>
        public SelectionRule SelectionRule { get; set; } = SelectionRule.Topfer;

        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        public ICLSetting ClSetting { get; set; }
    }
}
