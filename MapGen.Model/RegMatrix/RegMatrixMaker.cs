using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.General;
using MapGen.Model.Maps;
using MapGen.Model.Interpolation.Strategy;

namespace MapGen.Model.RegMatrix
{
    public class RegMatrixMaker : IRegMatrixMaker
    {
        /// <summary>
        /// Словарь кэффициентов, где key - масштаб, value - коэффициент масштабирования исходных координат карты.
        /// </summary>
        private Dictionary<long, double> _scaleCoeffDict;

        /// <summary>
        /// Стратегия интерполяции.
        /// </summary>
        public IStratagyInterpol StratagyInterpol { get; set; }

        /// <summary>
        /// Создет объект для создания регуряной матрицы глубин.
        /// </summary>
        /// <param name="strategyInterpol"></param>
        public RegMatrixMaker(IStratagyInterpol strategyInterpol)
        {
            StratagyInterpol = strategyInterpol;
            InitScaleCoeffDict();
        }

        /// <summary>
        /// Создание регулярной матрицы глубин.
        /// </summary>
        /// <param name="map">Карта.</param>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <param name="regMatrix">Регулярная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли создана регулярная матрица глубин.</returns>
        public bool CreateRegMatrix(DbMap map, long scale, out RegMatrix regMatrix, out string message)
        {
            if (!InitRegMatrixWithoutFilling(map, scale, out regMatrix, out message))
            {
                return false;
            }
            return StratagyInterpol.FillingRegMatrix(map, ref regMatrix, out message);
        }

        /// <summary>
        /// Инициализация регулярной матрицы без заполнения точками.
        /// </summary>
        /// <param name="map">Карта.</param>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <param name="regMatrix">Регулярная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошла инициализация.</returns>
        private bool InitRegMatrixWithoutFilling(DbMap map, long scale, out RegMatrix regMatrix, out string message)
        {
            regMatrix = new RegMatrix();
            message = string.Empty;

            // Инициализация регулярной матрицы.
            try
            {
                regMatrix.Step = _scaleCoeffDict[scale];

                regMatrix.Width = (int)map.Width + 1;

                regMatrix.Length = (int)map.Length + 1;

                regMatrix.Points = new double[regMatrix.Width * regMatrix.Length];
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время инициализации регулярной матрицы. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Инициализация словаря кэффициентов масштабирвоания.
        /// </summary>
        private void InitScaleCoeffDict()
        {
            double startCoeff = 1.0d;

            _scaleCoeffDict = new Dictionary<long, double>
            {
                {10000, startCoeff},
                {25000, startCoeff / 2},
                {50000, startCoeff / (2 * 2)},
                {100000, startCoeff / (2 * 2 * 2)},
                {200000, startCoeff / (2 * 2 * 2 * 2)},
                {300000, startCoeff / (2 * 2 * 2 * 2 * 2)},
                {500000, startCoeff / (2 * 2 * 2 * 2 * 2 * 2)},
                {1000000, startCoeff / (2 * 2 * 2 * 2 * 2 * 2 * 2)},
            };
        }
    }
}
