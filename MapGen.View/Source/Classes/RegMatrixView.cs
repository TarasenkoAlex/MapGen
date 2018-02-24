namespace MapGen.View.Source.Classes
{
    public class RegMatrixView
    {
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
        public double MaxDepth { get; set; }

    }
}
