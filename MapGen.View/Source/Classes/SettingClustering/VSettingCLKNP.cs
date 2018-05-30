using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes.SettingClustering
{
    public class VSettingCLKNP : IVSettingCL
    {
        /// <summary>
        /// Максимальная степень параллелизма.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = -1;
    }
}
