using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Maps;

namespace MapGen.Model.Generalization.Strategy
{
    public interface IStrategyGen
    {
        /// <summary>
        /// Выполнить генерализацию.
        /// </summary>
        /// <param name="scale">Масштаб.</param>
        /// <param name="inDbMap">Исходная карта.</param>
        /// <param name="outDbMap">Выходная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм генерализации.</returns>
        bool Execute(long scale, DbMap inDbMap, out DbMap outDbMap, out string message);
    }
}
