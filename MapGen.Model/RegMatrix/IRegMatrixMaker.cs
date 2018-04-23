using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Interpolation.Strategy;
using MapGen.Model.Maps;

namespace MapGen.Model.RegMatrix
{
    public interface IRegMatrixMaker
    {
        /// <summary>
        /// Стратегия интерполяции.
        /// </summary>
        IStrategyInterpol StratagyInterpol { get; set; }

        /// <summary>
        /// Создание регулярной матрицы глубин.
        /// </summary>
        /// <param name="map">Карта.</param>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <param name="regMatrix">Регулярная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли создана регулярная матрица глубин.</returns>
        bool CreateRegMatrix(DbMap map, long scale, out RegMatrix regMatrix, out string message);
    }
}
