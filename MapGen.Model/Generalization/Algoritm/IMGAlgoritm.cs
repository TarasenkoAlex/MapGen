using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Maps;

namespace MapGen.Model.Generalization.Algoritm
{
    public interface IMGAlgoritm
    {
        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        SettingGen SettingGen { get; set; }

        /// <summary>
        /// Выполнить генерализацию.
        /// </summary>
        /// <param name="scale">Масштаб составялемой карты.</param>
        /// <param name="inDbMap">Исходная карта.</param>
        /// <param name="outDbMap">Составляемая карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм генерализации.</returns>
        bool Execute(long scale, DbMap inDbMap, out DbMap outDbMap, out string message);
    }
}
