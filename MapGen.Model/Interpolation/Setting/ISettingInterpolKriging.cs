using MapGen.Model.General;

namespace MapGen.Model.Interpolation.Setting
{
    public interface ISettingInterpolKriging : ISettingInterpolGeneral
    {
        /// <summary>
        /// Вариограмма.
        /// </summary>
        Variograms Variogram { get; set; }
        /// <summary>
        /// Ранг.
        /// </summary>
        double A { get; set; }
        /// <summary>
        /// Вклад дисперсии или порог.
        /// </summary>
        double C { get; set; }
        /// <summary>
        /// Эффект самородка.
        /// </summary>
        double C0 { get; set; }
    }
}
