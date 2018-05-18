using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm.Kernel;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.Database.EDM;
using KMeans = MapGen.Model.Clustering.Algoritm.Kernel.KMeans;

namespace MapGen.Model.Clustering.Algoritm
{
    public class KMeansAlgoritm : ICLAlgoritm
    {

        /// <summary>
        /// Кластера.
        /// </summary>
        public KMeansCluster[] Clusters => _algoritmKMeans.Clusters;


        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        private readonly SettingCLKMeans _setting;

        /// <summary>
        /// Алгоритм k - средних.
        /// </summary>
        private KMeans _algoritmKMeans = new KMeans();
        

        /// <summary>
        /// Создает объект для выполнения кластеризации методом К-средних.
        /// </summary>
        /// <param name="setting">Настройка кластеризации.</param>
        public KMeansAlgoritm(SettingCLKMeans setting)
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
                _algoritmKMeans = new KMeans(countPointsOfOutDbMap)
                {
                    Seeding = _setting.Seeding,
                    MaxItarations = _setting.MaxItarations,
                    ParallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _setting.MaxDegreeOfParallelism
                    }
                };
                _algoritmKMeans.Learn(sourceCloudPoints, out distCloudPoints);

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
