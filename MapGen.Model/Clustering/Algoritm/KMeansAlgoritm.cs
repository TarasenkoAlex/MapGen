using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.IO;
using Accord.MachineLearning;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Clustering.Algoritm
{
    public class KMeansAlgoritm : ICLAlgoritm
    {
        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        private KMeansClSetting _setting;

        /// <summary>
        /// Создает объект для выполнения кластеризации методом К-средних.
        /// </summary>
        /// <param name="setting">Настройка кластеризации.</param>
        public KMeansAlgoritm(KMeansClSetting setting)
        {
            _setting = setting;
        }

        /// <summary>
        /// Выполнить кластеризацию.
        /// </summary>
        /// <param name="sourceCloudPoints">Исходное множество точек.</param>
        /// <param name="countPointsOfOutDbMap">Количество кластеров.</param>
        /// <param name="distCloudPoints">Множество точек, которые являются представителями кластеров.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм кластеризации.</returns>
        public bool Execute(Point[] sourceCloudPoints, int countPointsOfOutDbMap, out Point[] distCloudPoints, out string message)
        {
            try
            {
                message = string.Empty;
                
                // Выполняем кластеризацию.
                Kernel.KMeans algoritmKMeans = new Kernel.KMeans(countPointsOfOutDbMap);
                Stopwatch st = new Stopwatch();
                st.Reset();
                st.Start();
                algoritmKMeans.Learn(sourceCloudPoints, out distCloudPoints, out message);
                st.Stop();

                return true;
            }
            catch (Exception ex)
            {
                distCloudPoints = null;
                message = General.Methods.CalcMessageException(ex);
                return false;
            }
        }
    }
}
