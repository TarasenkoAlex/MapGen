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
    
    public enum Variograms
    {
        /// <summary>
        /// Сферическая вариограмма.
        /// </summary>
        Spherial = 0,
        /// <summary>
        /// Гауссова вариограмма.
        /// </summary>
        Gauss = 1,
        /// <summary>
        /// Экспоненциальная вариограмма.
        /// </summary>
        Exponent = 2,
        /// <summary>
        /// Линейная вариограмма.
        /// </summary>
        Linear = 3,
        /// <summary>
        /// Круговая вариограмма.
        /// </summary>
        Circle = 4
    }
}
