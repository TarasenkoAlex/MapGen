using System.Collections.Generic;
using System.Threading;
using MapGen.Model;
using MapGen.Model.Interpolation.Setting;
using MapGen.View.Source.Interfaces;
using MapGen.View.Source.Classes;
using MapGen.Model.RegMatrix;
using MapGen.Model.Test;
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.View.Source.Classes.SettingGen;

namespace MapGen.Presenter
{
    public class Presenter
    {
        #region Region private fields.

        /// <summary>
        /// Объект View.
        /// </summary>
        private readonly IView _view;

        /// <summary>
        /// Объект Model.
        /// </summary>
        private readonly IModel _model;

        #endregion
        
        #region Region properties.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        public object MainWindow => _view.MainWindow;

        #endregion
        
        #region Region constructor.

        /// <summary>
        /// Создание объекта Presenter.
        /// </summary>
        /// <param name="model">Объект Model.</param>
        /// <param name="view">Объект View.</param>
        public Presenter(IModel model, IView view)
        {
            // Инициализация Model и View.
            _model = model;
            _view = view;

            // Подписка на события View.
            SubscribeEventsOfView();

            // Подписка на события Model.
            SubscribeEventsOfModel();

            // Открываем главное окно программы.
            _view.ShowMainWindow();
        }

        #endregion
        
        #region Region private methods.
        
        /// <summary>
        /// Подписка событий View.
        /// </summary>
        private void SubscribeEventsOfView()
        {
            _view.LoadDbMap += View_LoadDbMap;
            _view.MenuItemListMapsClick += View_MenuItemListMapsClick;
            _view.MenuItemSettingsInterpolClick += View_MenuItemSettingsInterpolClick;
            _view.MenuItemSettingsGenClick += View_MenuItemSettingsGenClick;
            _view.ZoomEvent += View_ZoomEvent;
            _view.SaveSettingsInterpol += View_SaveSettingsInterpol;
            _view.SaveSettingsGen += View_SaveSettingsGen;
            _view.MenuItemTestSystemClick += View_MenuItemTestSystemClick;
            _view.RunAllTests += View_RunAllTests;
        }

        /// <summary>
        /// Подписка событий Model.
        /// </summary>
        private void SubscribeEventsOfModel()
        {
            _model.TestFinished += Model_TestFinished;
        }

        /// <summary>
        /// Обработка события завершения теста.
        /// </summary>
        private void Model_TestFinished(TestResult testResult)
        {
            new Thread(() =>
                {
                    VTestResult vTestResult = Converter.ToVTestResult(testResult);
                    _view.TestFinished(vTestResult);
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события загрузки списка карт из базы данных.
        /// </summary>
        private void View_MenuItemListMapsClick()
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Загрузка базы данных";

                    string message;
                    // Подключаемся к базе данных.
                    if (!_model.ConnectToDatabase(out message))
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось подключиться к базе данных! {message}");
                        return;
                    }

                    List<string[]> maps;
                    if (_model.GetDbMaps(out maps, out message))
                    {
                        _view.ShowTableDbMaps(maps);
                    }
                    else
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось загрузить список карт! {message}");
                    }

                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                {IsBackground = true}.Start();
        }

        /// <summary>
        /// Событие выбора элемента из списка карт базы данных. Созданиие регулярной матрицы.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        private void View_LoadDbMap(int idm)
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Загрузка базы данных";

                    string message;
                    // Загружаем карту из базы данных.
                    if (!_model.LoadDbMap(idm, out message))
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось загрузить карту из базы данных! {message}");
                        return;
                    }

                    _view.NameProcess = "Интерполяция";

                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(out regMatrix, out message))
                    {
                        _view.ShowMessageError("Создание регулярной матрицы", $"Не удалось создать регулярную матрицу глубин! {message}");
                        return;
                    }

                    // Конвертируем регулярную матрицу в карту для отрисовки. Передаем во View.
                    _view.GraphicMap = Converter.ToGraphicMap(
                        regMatrix, 
                        _model.SourceSeaMap.Name, 
                        _model.SourceSeaMap.Latitude, 
                        _model.SourceSeaMap.Longitude, 
                        _model.SourceSeaMap.Scale);

                    // Выставляем исходный мастаб.
                    _view.SourcScale = _model.SourceSeaMap.Scale;

                    // Отображаем карту.
                    _view.DrawSeaMapWithResetCamera();

                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                {IsBackground = true}.Start();
        }

        /// <summary>
        /// Обработка события изменения масштаба.
        /// </summary>
        /// <param name="scale">Масштаб.</param>
        private void View_ZoomEvent(int scale)
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Генерализация";
                    
                    string message;
                    if (!_model.ExecuteMapGen(scale, out message))
                    {
                        _view.ShowMessageError("Картографическая генерализация", $"Не удалось выполнить картографическую генерализацю! {message}");
                        return;
                    }

                    _view.NameProcess = "Интерполяция";

                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(out regMatrix, out message))
                    {
                        _view.ShowMessageError("Создание регулярной матрицы", $"Не удалось создать регулярную матрицу глубин! {message}");
                        return;
                    }

                    // Конвертируем регулярную матрицу в карту для отрисовки. Передаем во View.
                    _view.GraphicMap = Converter.ToGraphicMap(
                        regMatrix,
                        _model.CurrentSeaMap.Name,
                        _model.CurrentSeaMap.Latitude,
                        _model.CurrentSeaMap.Longitude,
                        _model.CurrentSeaMap.Scale);

                    // Отображаем карту.
                    _view.DrawSeaMapWithoutResetCamera();
                    
                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события открытия окна с настройками интерполяции.
        /// </summary>
        private void View_MenuItemSettingsInterpolClick()
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Загрузка настроек интерполяции";

                    // Загружаем настройки интерполяции из model в view.
                    var kriging = _model.SettingInterpol as ISettingInterpolKriging;
                    if (kriging != null)
                    {
                        _view.SettingInterpol = Converter.ToIVSettingInterpol(kriging);
                    }
                    else
                    {
                        var rbf = _model.SettingInterpol as ISettingInterpolRbf;
                        if (rbf != null)
                        {
                            _view.SettingInterpol = Converter.ToIVSettingInterpol(rbf);
                        }
                        else
                        {
                            _view.ShowMessageError("Загрузка настроек интерполяции", $"Не удалось загрузить настройки интерполяции!");
                        }
                    }

                    // Отображаем окно с настройками интерполяции.
                    _view.ShowSettingsInterlopWindow();

                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события открытия окна с настройками генерализации.
        /// </summary>
        private void View_MenuItemSettingsGenClick()
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Загрузка настроек генерализации";

                    // Загружаем настройки генерализации из model в view.
                    _view.SettingGen = Converter.ToVSettingGen(_model.SettingGen);

                    // Отображаем окно с настройками генерализации.
                    _view.ShowSettingsGenWindow();

                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события сохранения настроек интерполяции.
        /// </summary>
        /// <param name="setting"></param>
        private void View_SaveSettingsInterpol(IVSettingInterpol setting)
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Сохранение настроек интерполяции";

                    // Сохранение настройки интерполяции в model.
                    var kriging = setting as VSettingInterpolKriging;
                    if (kriging != null)
                    {
                        _model.SettingInterpol = Converter.ToISettingInterpol(kriging);
                    }
                    else
                    {
                        var rbf = setting as VSettingInterpolRbf;
                        if (rbf != null)
                        {
                            _model.SettingInterpol = Converter.ToISettingInterpol(rbf);
                        }
                        else
                        {
                            _view.ShowMessageError("Сохранение настроек интерполяции",
                                $"Не удалось сохранить настройки интерполяции!");
                        }
                    }

                    _view.NameProcess = "Интерполяция";

                    string message;
                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(out regMatrix, out message))
                    {
                        _view.ShowMessageError("Создание регулярной матрицы",
                            $"Не удалось создать регулярную матрицу глубин! {message}");
                        return;
                    }

                    // Конвертируем регулярную матрицу в карту для отрисовки. Передаем во View.
                    _view.GraphicMap = Converter.ToGraphicMap(
                        regMatrix, 
                        _model.SourceSeaMap.Name,
                        _model.SourceSeaMap.Latitude,
                        _model.SourceSeaMap.Longitude,
                        _model.SourceSeaMap.Scale);

                    // Отображаем карту.
                    _view.DrawSeaMapWithoutResetCamera();
                    
                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события сохранения настроек генерализации.
        /// </summary>
        /// <param name="setting"></param>
        private void View_SaveSettingsGen(VSettingGen setting)
        {
            new Thread(() =>
            {
                // Запускаем прогресс-бар главного окна.
                _view.IsRunningProgressBarMainWindow = true;
                _view.NameProcess = "Сохранение настроек генерализации";

                // Сохранение настройки интерполяции в model.
                _model.SettingGen = Converter.ToSettingGen(setting);

                // Отображаем карту.
                _view.DrawSeaMapWithoutResetCamera();

                // Останавливаем прогресс-бар главного окна.
                _view.IsRunningProgressBarMainWindow = false;
            })
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка события открытия окна с тестовой системой.
        /// </summary>
        private void View_MenuItemTestSystemClick()
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;
                    _view.NameProcess = "Загрузка тестовой системы";

                    // Удаляем все предыдущее TestCase.
                    _model.RemoveAllTestCase();

                    // Получаем максимальный id теста.
                    int maxIdTestCase = _model.GetMaxIdTestCase();

                    // Отображаем окно с настройками генерализации.
                    _view.ShowTestSystemWindow(maxIdTestCase);

                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        /// <summary>
        /// Обработка запуска всех тестов.
        /// </summary>
        private void View_RunAllTests(List<VTestCase> vTestCases)
        {
            new Thread(() =>
                {
                    List<TestCase> testCases = new List<TestCase>();
                    vTestCases.ForEach(el => testCases.Add(Converter.ToVTestResult(el)));
                    _model.RunTestSystem(testCases);
                })
                {IsBackground = true}.Start();
        }

        #endregion
    }
}
