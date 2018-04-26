namespace MapGen.Model.Interpolation.Setting
{
    public interface ISettingInterpolGeneral
    {
        /// <summary>
        /// Минимальный радиус окрестности.
        /// </summary>
        double MinRadiusOfEnvirons { get; set; }
        /// <summary>
        /// Минимальное количество точек в окрестности.
        /// </summary>
        int MinCountPointsOfEnvirons { get; set; }
        /// <summary>
        /// Шаг увеличения окрестности.
        /// </summary>
        double StepEncreaseOfEnvirons { get; set; }
    }
}
