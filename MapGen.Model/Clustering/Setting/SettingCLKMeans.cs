using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm.Kernel;
using MapGen.Model.General;

namespace MapGen.Model.Clustering.Setting
{
    public class SettingCLKMeans : ISettingCL
    {
        /// <summary>
        /// Максимальное количество итераций.
        /// </summary>
        public int MaxItarations { get; set; } = -1;

        /// <summary>
        /// Максимальная степень параллелизма.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = -1;

        /// <summary>
        /// Настройка выбора центроидов на первом шаге алгоритма.
        /// </summary>
        public Seedings Seeding { get; set; } = Seedings.Random;
    }
}
