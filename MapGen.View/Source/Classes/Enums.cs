using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes
{
    public enum VVariograms
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

    public enum VBasicFunctions
    {
        /// <summary>
        /// Мультиквадрик (MultiQuadric).
        /// </summary>
        MultiQuadric = 0,
        /// <summary>
        /// Обратный мультиквадрик (InverseMultiQuadric).
        /// </summary>
        InverseMultiQuadric = 1,
        /// <summary>
        /// Мультилогарифмическая (MultiLog).
        /// </summary>
        MultiLog = 2,
        /// <summary>
        /// Натуральный кубический сплайн (NaturalCubicSpline).
        /// </summary>
        NaturalCubicSpline = 3,
        /// <summary>
        /// Сплайн тонкой пластины (ThinPlateSpline).
        /// </summary>
        ThinPlateSpline = 4
    }
}
