using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.General;

namespace MapGen.Model.Generalization.Setting
{
    public class SettingGen
    {
        /// <summary> 
        /// Формулы для определения норм отбора.
        /// </summary>
        public SelectionRules SelectionRule { get; set; } = SelectionRules.Topfer;

        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        public ISettingCL SettingCL { get; set; } = new SettingCLKMeans();

        public override string ToString()
        {
            string selectionRule = $"Формула для определения норм отбора: {SelectionRule.ToString()}";
            string settingCL = SettingCL.ToString();

            string result = $"{selectionRule}\n{settingCL}";

            return result;
        }
    }
}
