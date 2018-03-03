using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.RegMatrix
{
    public class RegMatrix
    {
        #region Region properties.

        /// <summary>
        /// Шаг регулярной матрицы высот.
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// Ширина карты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Длина карты.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Точки регулярной матрицы высот.
        /// </summary>
        public double[] Points { get; set; }

        /// <summary>
        /// Максимальная глубина.
        /// </summary>
        public double MaxDepth => Points.Max();

        #endregion
    }
}
