using MapGen.Model.General;

namespace MapGen.Model.Interpolation.Setting
{
    /// <summary>
    /// Класс настройки интерполяции методом RBF.
    /// </summary>
    public class SettingInterpolRbf : ISettingInterpolRbf, ISettingInterpol
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
        public BasicFunctions BasicFunction { get; set; } = BasicFunctions.MultiLog;

        /// <summary>
        /// Фактор сглаживания.
        /// </summary>
        public double R { get; set; } = 1;
    }
}
