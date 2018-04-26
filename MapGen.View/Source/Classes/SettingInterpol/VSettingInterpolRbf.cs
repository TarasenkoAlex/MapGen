using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes.SettingInterpol
{
    public class VSettingInterpolRbf : IVSettingInterpol
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
        /// Базисная функция.
        /// </summary>
        public VBasicFunctions BasicFunction { get; set; } = VBasicFunctions.MultiLog;

        /// <summary>
        /// Фактор сглаживания.
        /// </summary>
        public double R { get; set; } = 1;
    }
}
