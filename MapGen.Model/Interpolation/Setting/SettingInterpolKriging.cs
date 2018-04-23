namespace MapGen.Model.Interpolation.Setting
{
    /// <summary>
    /// Класс настройки интерполяции методом Крайгинга.
    /// </summary>
    public class SettingInterpolKriging : ISettingInterpolKriging
    {
        /// <summary>
        /// Шаг регулярной матрицы.
        /// </summary>
        public double Step { get; set; } = 1.0;

        /// <summary>
        /// Минимальный радиус окрестности.
        /// </summary>
        public double MinRadiusOfEnvirons { get; set; } = 3;

        /// <summary>
        /// Минимальное количество точек в окрестности.
        /// </summary>
        public double MinCountPointsOfEnvirons { get; set; } = 10;

        /// <summary>
        /// Шаг увеличения окрестности.
        /// </summary>
        public double StepEncreaseOfEnvirons { get; set; } = 1;

        /// <summary>
        /// Вариограмма.
        /// </summary>
        public Variograms Variogram { get; set; } = Variograms.Spherial;

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
