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

                // Формируем данные для метода кластеризации.
                double[][] observations = new double[sourceCloudPoints.Length][];
                for (int i = 0; i < sourceCloudPoints.Length; ++i)
                {
                    observations[i] = new double[3]
                    {
                        sourceCloudPoints[i].X,
                        sourceCloudPoints[i].Y,
                        sourceCloudPoints[i].Depth
                    };
                }

                // Выполняем кластеризаци.
                KMeans algoritmKMeans = new KMeans(countPointsOfOutDbMap);
                Stopwatch st = new Stopwatch();
                st.Reset();
                st.Start();
                KMeansClusterCollection clusters = algoritmKMeans.Learn(observations);
                st.Stop();

                int[] labels = clusters.Decide(observations);

                // Формируем выходные данные.
                distCloudPoints = new Point[countPointsOfOutDbMap];
                for (int i = 0; i < sourceCloudPoints.Length; ++i)
                {
                    if (distCloudPoints[labels[i]] == null)
                    {
                        distCloudPoints[labels[i]] = new Point
                        {
                            X = (long) observations[i][0],
                            Y = (long) observations[i][1],
                            Depth = (long) observations[i][2]
                        };
                    }
                    else
                    {
                        if (distCloudPoints[labels[i]].Depth <= (long) observations[i][2])
                        {
                            distCloudPoints[labels[i]].X = (long) observations[i][0];
                            distCloudPoints[labels[i]].Y = (long) observations[i][1];
                            distCloudPoints[labels[i]].Depth = (long) observations[i][2];
                        }
                    }
                }

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
