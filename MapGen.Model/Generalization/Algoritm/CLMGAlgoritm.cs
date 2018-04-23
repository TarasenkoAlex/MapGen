using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Generalization.Strategy;
using MapGen.Model.Maps;

namespace MapGen.Model.Generalization.Algoritm
{
    public class CLMGAlgoritm : IMGAlgoritm
    {
        /// <summary>
        /// Стратегия кластеризации.
        /// </summary>
        public IStrategyGen StrategyGeneralization { get; set; }

        /// <summary>
        /// Создет объект для выполнения алгоритма картографической генерализации методом кластеризации.
        /// </summary>
        /// <param name="strategyGeneralization">Стратегия алгоритма.</param>
        public CLMGAlgoritm(IStrategyGen strategyGeneralization)
        {
            StrategyGeneralization = strategyGeneralization;
        }

        /// <summary>
        /// Выполнить генерализацию.
        /// </summary>
        /// <param name="scale">Масштаб.</param>
        /// <param name="inDbMap">Исходная карта.</param>
        /// <param name="outDbMap">Выходная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм генерализации.</returns>
        public bool Execute(long scale, DbMap inDbMap, out DbMap outDbMap, out string message)
        {
            return StrategyGeneralization.Execute(scale, inDbMap, out outDbMap, out message);
        }
    }
}
