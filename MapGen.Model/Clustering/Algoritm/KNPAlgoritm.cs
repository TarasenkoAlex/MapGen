using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm.Kernel;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Clustering.Algoritm
{
    public class KNPAlgoritm : ICLAlgoritm
    {

        /// <summary>
        /// Кластера.
        /// </summary>
        public Cluster[] Clusters => _algoritmKNP.Clusters;

        /// <summary>
        /// Настройка кластеризации.
        /// </summary>
        private readonly SettingCLKNP _setting;

        /// <summary>
        /// Алгоритм кратчайший незамкнутый путь.
        /// </summary>
        private KNP _algoritmKNP = new KNP();

        /// <summary>
        /// Создает объект для выполнения кластеризации алгоритмом кратчайший незамкнутый путь.
        /// </summary>
        /// <param name="setting">Настройка кластеризации.</param>
        public KNPAlgoritm(SettingCLKNP setting)
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
                _algoritmKNP = new KNP(countPointsOfOutDbMap)
                {
                    ParallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _setting.MaxDegreeOfParallelism
                    }
                };
                _algoritmKNP.Learn(sourceCloudPoints, out distCloudPoints);

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
