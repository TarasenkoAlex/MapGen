﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm;
using MapGen.Model.Database;
using MapGen.Model.Database.DbWorker;
using MapGen.Model.Database.EDM;
using MapGen.Model.Database.UnitOfWork;
using MapGen.Model.General;
using MapGen.Model.Generalization.Algoritm;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Interpolation.Strategy;
using MapGen.Model.Maps;
using MapGen.Model.RegMatrix;
using MapGen.Model.Test;

namespace MapGen.Model
{
    public class Model : IModel
    {
        #region Region private fields.

        /// <summary>
        /// Использовать для отрисовки исходную карту.
        /// </summary>
        private bool _isUseSourceMap = true;

        /// <summary>
        /// Клиент баазы данных.
        /// </summary>
        private readonly DatabaseWorker _databaseWorker;

        /// <summary>
        /// Настройка интерполяции.
        /// </summary>
        private ISettingInterpol _settingInterpol;

        /// <summary>
        /// Создает регулярную матрицу по облаку точек.
        /// </summary>
        private readonly IRegMatrixMaker _regMatrixMaker;
        
        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        private SettingGen _settingGen;

        /// <summary>
        /// Создет объект для выполнения алгоритма картографической генерализации методом кластеризации.
        /// </summary>
        private readonly IMGAlgoritm _mgAlgoritm;

        /// <summary>
        /// Тестовая система.
        /// </summary>
        private readonly TestSystem _testSystem;

        #endregion

        #region Region properties.

        /// <summary>
        /// Исходная загруженная карта.
        /// </summary>
        public DbMap SourceSeaMap { get; private set; }

        /// <summary>
        /// Карта после картографической генерализации.
        /// </summary>
        public DbMap MapGenSeaMap { get; private set; }

        /// <summary>
        /// Отображаемая карта.
        /// </summary>
        public DbMap CurrentSeaMap => _isUseSourceMap ? SourceSeaMap : MapGenSeaMap;

        /// <summary>
        /// Настройка интерполяции.
        /// </summary>
        public ISettingInterpol SettingInterpol
        {
            get { return _settingInterpol; }
            set
            {
                _settingInterpol = value;
                var kriging = _settingInterpol as ISettingInterpolKriging;
                if (kriging != null)
                {
                    _regMatrixMaker.StratagyInterpol = new StrategyInterpolKriging(kriging);
                }
                else
                {
                    var rbf = _settingInterpol as ISettingInterpolRbf;
                    if (rbf != null)
                    {
                        _regMatrixMaker.StratagyInterpol = new StrategyInterpolRbf(rbf);
                    }
                }
            }
        }

        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        public SettingGen SettingGen
        {
            get { return _settingGen; }
            set
            {
                _settingGen = value;
                _mgAlgoritm.SettingGen = value;
            }
        }

        /// <summary>
        /// Событие завершения теста.
        /// </summary>
        public event Action<TestResult> TestFinished;

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает объект Model.
        /// </summary>
        public Model()
        {
            if (!Directory.Exists(ResourceModel.DIR_RUNTIME))
            {
                Directory.CreateDirectory(ResourceModel.DIR_RUNTIME);
            }
            if (!Directory.Exists(ResourceModel.DIR_TESTS))
            {
                Directory.CreateDirectory(ResourceModel.DIR_TESTS);
            }

            // private.
            _testSystem = new TestSystem();
            _testSystem.Init();
            _testSystem.TestFinished += TestFinishedHandler;

            _databaseWorker = new DatabaseWorker();

            _settingInterpol = new SettingInterpolKriging();
            _regMatrixMaker = new RegMatrixMaker(new StrategyInterpolKriging(new SettingInterpolKriging()));

            _settingGen = new SettingGen();
            _mgAlgoritm = new CLMGAlgoritm(new SettingGen());
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
                SourceSeaMap = new DbMap(map.Name, map.Width, map.Length, map.Scale, map.Latitude, map.Longitude, cloudPoints);

                // Отрисовываем в файл.
               Methods.DeleteAllElementsOnDirectry(ResourceModel.DIR_RUNTIME);
                SourceSeaMap.DrawToBMP($"{ResourceModel.DIR_RUNTIME}Before.bmp");

                // Выставляем, что необходимо отрисовывать исходную карту.
                _isUseSourceMap = true;
            }
            catch (Exception ex)
            {
                message = Methods.CalcMessageException(ex);
                return false;
            }
            return true;
        }
        
        #endregion

        #region Region public methods. RegMatrix.

        /// <summary>
        /// Создание регулярной матрицы глубин.
        /// </summary>
        /// <param name="regMatrix">Регулярная матрица глубин.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Успешно ли прошло создание.</returns>
        public bool CreateRegMatrix(out RegMatrix.RegMatrix regMatrix, out string message)
        {
            // В зависимости от настройки интерполяции создает регулярную матрицу.
            if (_isUseSourceMap)
            {
                return _regMatrixMaker.CreateRegMatrix(SourceSeaMap, SourceSeaMap.Scale, out regMatrix, out message);
            }
            return _regMatrixMaker.CreateRegMatrix(MapGenSeaMap, MapGenSeaMap.Scale, out regMatrix, out message);
        }

        #endregion

        #region Region public methods. MapGen.

        /// <summary>
        /// Выполнить генерализацию.
        /// </summary>
        /// <param name="scale">Масштаб.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошел алгоритм генерализации.</returns>
        public bool ExecuteMapGen(long scale, out string message)
        {
            message = string.Empty;
            if (scale == SourceSeaMap.Scale)
            {
                // Отрисовываем в файл.
                Methods.DeleteAllElementsOnDirectry(ResourceModel.DIR_RUNTIME);
                SourceSeaMap.DrawToBMP($"{ResourceModel.DIR_RUNTIME}Before.bmp");

                // Выставляем, что необходимо отрисовывать исходную карту.
                _isUseSourceMap = true;

                return true;
            }

            DbMap outDbMap;

            // В зависимости от настройки выполнить генерализацию.
            bool result = _mgAlgoritm.Execute(scale, SourceSeaMap, out outDbMap, out message);

            // Сохраняем получившуюся карту.
            if (result)
            {
                MapGenSeaMap = outDbMap;
            }

            // Выставляем, что необходимо отрисовывать карту, полученную после генерализации.
            _isUseSourceMap = false;

            // Отрисовываем в файл.
            SourceSeaMap.DrawToBMP(_mgAlgoritm.Clusters, $"{ResourceModel.DIR_RUNTIME}After.bmp");

            return result;
        }

        #endregion

        #region Region public methods. TestSystem.

        /// <summary>
        /// Добавить TestCase.
        /// </summary>
        public void AddTestCase(SettingGen settingGen, long scale)
        {
            _testSystem.AddTestCase(SourceSeaMap, settingGen, scale);
        }

        /// <summary>
        /// Удалить все TestCase.
        /// </summary>
        public void RemoveAllTestCase()
        {
            _testSystem.RemoveAllTestCase();
        }
        
        /// <summary>
        /// Запуск тестовой системы.
        /// </summary>
        public void RunTestSystem(List<TestCase> testCases)
        {
            testCases.ForEach(el => _testSystem.AddTestCase(SourceSeaMap, el.SettingGen, el.Scale));
            _testSystem.Run();
        }

        /// <summary>
        /// Получить максимальный доступный id теста.
        /// </summary>
        /// <returns></returns>
        public int GetMaxIdTestCase()
        {
            return _testSystem.GetMaxIdTestCase();
        }

        #endregion

        #region Region private methods.

        private void TestFinishedHandler(TestResult testResult)
        {
            TestFinished?.Invoke(testResult);
        }

        #endregion
    }
}
