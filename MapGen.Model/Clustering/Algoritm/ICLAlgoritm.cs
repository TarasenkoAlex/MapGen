using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Clustering.Algoritm
{
    public interface ICLAlgoritm
    {
        /// <summary>
        /// Выполнить кластеризацию.
        /// </summary>
        /// <param name="sourceCloudPoints">Исходное множество точек.</param>
        /// <param name="countPointsOfOutDbMap">Количество кластеров.</param>
        /// <param name="distCloudPoints">Множество точек, которые являются представителями кластеров.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм кластеризации.</returns>
        bool Execute(Point[] sourceCloudPoints, int countPointsOfOutDbMap, out Point[] distCloudPoints, out string message);
    }
}
