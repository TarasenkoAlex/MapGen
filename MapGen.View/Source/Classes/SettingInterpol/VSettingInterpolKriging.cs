using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes.SettingInterpol
{
    public class VSettingInterpolKriging : IVSettingInterpol
    {
        /// <summary>
        /// Минимальный радиус окрестности.
        /// </summary>
        public double MinRadiusOfEnvirons { get; set; } = 3;

        /// <summary>
        /// Минимальное количество точек в окрестности.
        /// </summary>
        public int MinCountPointsOfEnvirons { get; set; } = 10;

        /// <summary>
        /// Шаг увеличения окрестности.
        /// </summary>
        public double StepEncreaseOfEnvirons { get; set; } = 1;

        /// <summary>
        /// Вариограмма.
        /// </summary>
        public VVariograms Variogram { get; set; } = VVariograms.Spherial;

        /// <summary>
        /// Ранг.
        /// </summary>
        public double A { get; set; } = 20;

        /// <summary>
        /// Вклад дисперсии или порог.
        /// </summary>
        public double C { get; set; } = 100;

        /// <summary>
        /// Эффект самородка.
        /// </summary>
        public double C0 { get; set; } = 0;
    }
}
