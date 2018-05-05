using System.Collections.Generic;
using System.Threading;
using MapGen.Model;
using MapGen.Model.Interpolation.Setting;
using MapGen.View.Source.Interfaces;
using MapGen.View.Source.Classes;
using MapGen.Model.RegMatrix;
using MapGen.View.Source.Classes.SettingInterpol;

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
            _view.ZoomEvent += View_ZoomEvent;
            _view.SaveSettingsInterpol += View_SaveSettingsInterpol;
        }
        
        /// <summary>
        /// Подписка событий Model.
        /// </summary>
        private void SubscribeEventsOfModel()
        {
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

                    string message;
                    // Загружаем карту из базы данных.
                    if (!_model.LoadDbMap(idm, out message))
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось загрузить карту из базы данных! {message}");
                        return;
                    }

                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(true, out regMatrix, out message))
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

                    // Отображаем карту.
                    _view.DrawSeaMap();

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

                    string message;
                    if (!_model.ExecuteMapGen(scale, out message))
                    {
                        _view.ShowMessageError("Картографическая генерализация", $"Не удалось выполнить картографическую генерализацю! {message}");
                        return;
                    }

                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(false, out regMatrix, out message))
                    {
                        _view.ShowMessageError("Создание регулярной матрицы", $"Не удалось создать регулярную матрицу глубин! {message}");
                        return;
                    }

                    // Конвертируем регулярную матрицу в карту для отрисовки. Передаем во View.
                    _view.GraphicMap = Converter.ToGraphicMap(
                        regMatrix,
                        _model.MapGenSeaMap.Name,
                        _model.MapGenSeaMap.Latitude,
                        _model.MapGenSeaMap.Longitude,
                        _model.MapGenSeaMap.Scale);

                    // Отображаем карту.
                    _view.DrawSeaMap();
                    
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
        /// Обработка события сохранениянастроек интерполяции.
        /// </summary>
        /// <param name="setting"></param>
        private void View_SaveSettingsInterpol(IVSettingInterpol setting)
        {
            new Thread(() =>
                {
                    // Запускаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = true;

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

                    string message;
                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(true, out regMatrix, out message))
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
                    _view.DrawSeaMap();
                    
                    // Останавливаем прогресс-бар главного окна.
                    _view.IsRunningProgressBarMainWindow = false;
                })
                { IsBackground = true }.Start();
        }

        #endregion
    }
}
