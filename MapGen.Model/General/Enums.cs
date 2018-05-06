namespace MapGen.Model.General
{
    /// <summary> Формулы для определения норм отбора. </summary>
    public enum SelectionRules
    {
        /// <summary> Формула Топфера для определения норм отбора. </summary>
        Topfer = 0,
    }

    /// <summary> Вариограмма. </summary>
    public enum Variograms
    {
        /// <summary> Сферическая вариограмма. </summary>
        Spherial = 0,

        /// <summary> Гауссова вариограмма. </summary>
        Gauss = 1,

        /// <summary> Экспоненциальная вариограмма. </summary>
        Exponent = 2,

        /// <summary> Линейная вариограмма. </summary>
        Linear = 3,

        /// <summary> Круговая вариограмма. </summary>
        Circle = 4
    }

    /// <summary> Базисная функция. </summary>
    public enum BasicFunctions
    {
        /// <summary> Мультиквадрик (MultiQuadric). </summary>
        MultiQuadric = 0,

        /// <summary> Обратный мультиквадрик (InverseMultiQuadric). </summary>
        InverseMultiQuadric = 1,

        /// <summary> Мультилогарифмическая (MultiLog). </summary>
        MultiLog = 2,

        /// <summary> Натуральный кубический сплайн (NaturalCubicSpline). </summary>
        NaturalCubicSpline = 3,

        /// <summary> Сплайн тонкой пластины (ThinPlateSpline). </summary>
        ThinPlateSpline = 4
    }

    /// <summary> Настройка выбора центроидов на первом шаге алгоритма кластеризации.</summary>
    public enum Seedings
    {
        /// <summary> Случайная последовательность. </summary>
        Random = 0,
    };
}
