using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes.SettingClustering
{
    public class VSettingCLKMeans : IVSettingCL
    {
        /// <summary>
        /// Настройка выбора центроидов на первом шаге алгоритма.
        /// </summary>
        public VSeedings Seeding { get; set; } = VSeedings.Random;

        /// <summary>
        /// Максимальное количество итераций.
        /// </summary>
        public int MaxItarations { get; set; } = -1;

        /// <summary>
        /// Максимальная степень параллелизма.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = -1;
    }
}
