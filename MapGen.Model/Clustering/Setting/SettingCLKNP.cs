using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm.Kernel;
using MapGen.Model.General;

namespace MapGen.Model.Clustering.Setting
{
    public class SettingCLKNP : ISettingCL
    {
        /// <summary>
        /// Максимальная степень параллелизма.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = -1;

        public override string ToString()
        {
            string maxDegreeOfParallelism = $"Максимальная степень параллелизма: {MaxDegreeOfParallelism}";

            string result = $"{maxDegreeOfParallelism}";

            return result;
        }
    }
}
