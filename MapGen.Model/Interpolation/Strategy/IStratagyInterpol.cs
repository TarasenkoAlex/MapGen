using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Maps;

namespace MapGen.Model.Interpolation.Strategy
{
    public interface IStratagyInterpol
    {
        /// <summary>
        /// Заполнение регулярной матрицы глубин точками.
        /// </summary>
        /// <param name="map">Карта.</param>
        /// <param name="regMatrix">Регулярная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли заполнена регулярная матрица глубин.</returns>
        bool FillingRegMatrix(DbMap map, ref RegMatrix.RegMatrix regMatrix, out string message);
    }
}
