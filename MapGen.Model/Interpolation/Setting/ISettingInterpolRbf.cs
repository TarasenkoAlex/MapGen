using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.Interpolation.Setting
{
    public interface ISettingInterpolRbf : IGeneralSettingInterpol
    {
        /// <summary>
        /// Базисная функция.
        /// </summary>
        BasicFunctions BasicFunction { get; set; }
    }

    public enum BasicFunctions
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
