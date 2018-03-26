namespace MapGen.Model.Interpolation.Setting
{
    public interface IGeneralSettingInterpol
    {
        /// <summary>
        /// Шаг регулярной матрицы.
        /// </summary>
        double Step { get; set; }
        /// <summary>
        /// Минимальный радиус окрестности.
        /// </summary>
        double MinRadiusOfEnvirons { get; set; }
        /// <summary>
        /// Минимальное количество точек в окрестности.
        /// </summary>
        double MinCountPointsOfEnvirons { get; set; }
        /// <summary>
        /// Шаг увеличения окрестности.
        /// </summary>
        double StepEncreaseOfEnvirons { get; set; }
    }
}
