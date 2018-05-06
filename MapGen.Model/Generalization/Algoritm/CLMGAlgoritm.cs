using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Maps;

namespace MapGen.Model.Generalization.Algoritm
{
    public class CLMGAlgoritm : IMGAlgoritm
    {
        /// <summary>
        /// Алгоритм кластеризации.
        /// </summary>
        private ICLAlgoritm _clusteringAlgoritm = new KMeansAlgoritm(new SettingCLKMeans());

        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        private SettingGen _settingGen = new SettingGen();

        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        public SettingGen SettingGen
        {
            get
            {
                return _settingGen;
            }
            set
            {
                _settingGen = value;
                var kmeans = value.SettingCL as SettingCLKMeans;
                if (kmeans != null)
                {
                    _clusteringAlgoritm = new KMeansAlgoritm(kmeans);
                }
            }
        }

        /// <summary>
        /// Создет объект для выполнения алгоритма картографической генерализации методом кластеризации.
        /// </summary>
        /// <param name="settingGen">Настройка генерализации.</param>
        /// <param name="сlusteringAlgoritm">Алгоритм кластеризации.</param>
        public CLMGAlgoritm(SettingGen settingGen)
        {
            SettingGen = settingGen;
        }

        /// <summary>
        /// Выполнить генерализацию.
        /// </summary>
        /// <param name="scale">Масштаб составялемой карты.</param>
        /// <param name="inDbMap">Исходная карта.</param>
        /// <param name="outDbMap">Составляемая карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм генерализации.</returns>
        public bool Execute(long scale, DbMap inDbMap, out DbMap outDbMap, out string message)
        {
            message = string.Empty;

            // Вычисляем количество точек составляемой карты.
            int countPointsOfOutDbMap = 0;
            if (SettingGen.SelectionRule == SelectionRules.Topfer)
            {
                countPointsOfOutDbMap = SelectionFunctions.FunctionTopfer(inDbMap.Scale, scale, inDbMap.CloudPoints.Length);
            }

            // Выполняем кластеризацию.
            Point[] cloudPoints;
            bool isClustering = _clusteringAlgoritm.Execute(inDbMap.CloudPoints, countPointsOfOutDbMap, out cloudPoints, out message);

            // Обрабатываем результат кластеризации.
            if (isClustering)
            {
                outDbMap = new DbMap(inDbMap.Name, inDbMap.Width, inDbMap.Length, scale, inDbMap.Latitude,inDbMap.Longitude, cloudPoints);
                return true;
            }

            outDbMap = null;
            return false;
        }
    }
}
