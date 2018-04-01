using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database;
using MapGen.Model.Database.DbWorker;
using MapGen.Model.Database.EDM;
using MapGen.Model.Database.UnitOfWork;
using MapGen.Model.General;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Interpolation.Strategy;
using MapGen.Model.Maps;
using MapGen.Model.RegMatrix;

namespace MapGen.Model
{
    public class Model : IModel
    {
        #region Region private fields.
        
        /// <summary>
        /// Клиент баазы данных.
        /// </summary>
        private readonly DatabaseWorker _databaseWorker;

        /// <summary>
        /// Создает регулряную матрицу по облаку точек.
        /// </summary>
        private readonly IRegMatrixMaker _regMatrixMaker;

        #endregion

        #region Region public fields.
        
        /// <summary>
        /// Исходная загруженная карта.
        /// </summary>
        public DbMap SeaMap { get; private set; }

        /// <summary>
        /// Стратегия интерполяции при помощи метода Кригинг.
        /// </summary>
        public StrategyInterpolKriging StrategyInterpolKriging { get; set; }

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает объект Model.
        /// </summary>
        public Model()
        {
            // public.
            StrategyInterpolKriging = new StrategyInterpolKriging(new SettingInterpolationKriging());
           
            // private.
            _databaseWorker = new DatabaseWorker();
            _regMatrixMaker = new RegMatrixMaker(StrategyInterpolKriging);
        }

        #endregion
        
        #region Region public methods. Database.
        
        /// <summary>
        /// Подключение к базе данных.
        /// </summary>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли рошло подключение.</returns>
        public bool ConnectToDatabase(out string message)
        {
            if (!_databaseWorker.IsExistDatabase)
            {
                if (!_databaseWorker.CreateDatabase(out message))
                {
                    return false;
                }
            }
            return _databaseWorker.Connect(out message);
        }

        /// <summary>
        /// Чтение списка карт с описанием из базы данных.
        /// </summary>
        /// <param name="maps">Список карт.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли прошло чтение.</returns>
        public bool GetDbMaps(out List<string[]> maps, out string message)
        {
            message = string.Empty;
            maps = new List<string[]>();

            try
            {
                List<Map> getallmaps;
                if (!_databaseWorker.GetAllMaps(out getallmaps, out message))
                {
                    return false;
                }

                foreach (Map map in getallmaps)
                {
                    maps.Add(new[]
                    {
                        map.Idm.ToString(),
                        map.Name,
                        map.Latitude,
                        map.Longitude,
                        map.Width.ToString(),
                        map.Length.ToString(),
                        map.Scale.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                message = Methods.CalcMessageException(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Чтение списка карт с описанием из базы данных.
        /// </summary>
        /// <param name="maps">Список карт.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли прошло чтение.</returns>
        public bool GetDbMaps(out List<Map> maps, out string message)
        {
            return _databaseWorker.GetAllMaps(out maps, out message);
        }

        /// <summary>
        /// Загрузка карты по его id.
        /// </summary>
        /// <param name="idMap">Id загружаемой карты.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли прошло загрука.</returns>
        public bool LoadDbMap(int idMap, out string message)
        {
            message = string.Empty;
            Map map;
            Point[] cloudPoints;

            // Загрузка карты.
            if (!_databaseWorker.GetMap(idMap, out map, out message))
            {
                return false;
            }
            // Загрузка облака точек.
            if (!_databaseWorker.GetPoints(idMap, out cloudPoints, out message))
            {
                return false;
            }
            
            try
            {
                // Сохраняем карту.
                SeaMap = new DbMap(map.Name, map.Width, map.Length, map.Scale, cloudPoints);
            }
            catch (Exception ex)
            {
                message = Methods.CalcMessageException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Создание регулярной матрицы глубин.
        /// </summary>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <param name="regMatrix">Регулярная матрица глубин.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли прошло создание.</returns>
        public bool CreateRegMatrix(long scale, out RegMatrix.RegMatrix regMatrix, out string message)
        {
            // В зависимости от настройки интерполяции создает регулярную матрицу.
            return _regMatrixMaker.CreateRegMatrix(SeaMap, scale, out regMatrix, out message);
        }

        #endregion

        #region Region private methods.

        

        #endregion
    }
}
