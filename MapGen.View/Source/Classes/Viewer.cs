using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MapGen.View.GUI.Windows;
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.Source.Classes
{
    public class View : IView
    {
        #region Region private fields.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        private readonly IMain _mainWindow;

        /// <summary>
        /// Загружена ли карта.
        /// </summary>
        private bool _isLoadMap = false;

        #endregion

        #region Region properties.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        public object MainWindow => _mainWindow;

        /// <summary>
        /// Диспатчер.
        /// </summary>
        public Dispatcher Dispatcher => _mainWindow.MyDispatcher;

        /// <summary>
        /// Карта для отрисовки.
        /// </summary>
        public GraphicMap GraphicMap
        {
            set
            {
                _isLoadMap = true;
                _mainWindow.GraphicMap = value;
            }
        }

        /// <summary>
        /// Настройка интерполяции.
        /// </summary>
        public IVSettingInterpol SettingInterpol { get; set; }

        /// <summary>
        /// Запустить или остановить прогресс-бар главного окна.
        /// </summary>
        public bool IsRunningProgressBarMainWindow
        {
            set { _mainWindow.IsRunningProgressBar = value; }
        }

        #endregion

        #region Region general events.

        /// <summary>
        /// Событие загрузки карты.
        /// </summary>
        public event Action<int> LoadDbMap;

        /// <summary>
        /// Событие изменения масштаба.
        /// </summary>
        public event Action<int> ZoomEvent;

        /// <summary>
        /// Событие сохранения настроек интерполяции.
        /// </summary>
        public event Action<IVSettingInterpol> SaveSettingsInterpol;

        #endregion

        #region Region events of MainWindow.

        /// <summary>
        /// Событие выбора елемента View "Файл.База данных карт".
        /// </summary>
        public event Action MenuItemListMapsClick;

        /// <summary>
        /// Событие выбора елемента View "Сервис.Настройки.Интерполяция".
        /// </summary>
        public event Action MenuItemSettingsInterpolClick;

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает объект View.
        /// </summary>
        public View()
        {
            // Создаем главное окно программы MapGeneralization.
            _mainWindow = new MainWindow();
            // Подписка на события MainWindow.
            SubscribeEventsOfMainWindow();
        }

        #endregion
        
        #region Region methods of TableMapsWindow.

        /// <summary>
        /// Открыть окно со списком карт.
        /// </summary>
        public void ShowTableDbMaps(List<string[]> tableMaps)
        {
            Dispatcher.Invoke(() =>
            {
                ITableMaps tableMapsWindow = new TableMapsWindow();
                tableMapsWindow.Maps = ConvertStringToMapView(tableMaps);
                tableMapsWindow.ChooseMap += TableMapsWindow_ChooseMap;
                tableMapsWindow.ShowTableMaps();
            });
        }

        /// <summary>
        /// Конвертация списка карт Model в список карт View.
        /// </summary>
        /// <param name="tMaps">Список карт Model.</param>
        /// <returns>Список карт View.</returns>
        private MapView[] ConvertStringToMapView(List<string[]> tMaps)
        {
            MapView[]  tableMaps = new MapView[tMaps.Count];

            for (int i = 0; i < tMaps.Count; ++i)
            {
                tableMaps[i] = new MapView(
                    int.Parse(tMaps[i][0]),
                    tMaps[i][1],
                    tMaps[i][2],
                    tMaps[i][3],
                    int.Parse(tMaps[i][4]),
                    int.Parse(tMaps[i][5]),
                    int.Parse(tMaps[i][6]));
            }

            return tableMaps;
        }

        /// <summary>
        /// Событие выбора элемента из списка карт базы данных.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        private void TableMapsWindow_ChooseMap(int idm)
        {
            LoadDbMap?.Invoke(idm);
        }
        
        #endregion
        
        #region Region methods of MainWindow.

        /// <summary>
        /// Окрыть главное окно программы MapGen.
        /// </summary>
        public void ShowMainWindow()
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow.ShowMainWindow();
            });
        }

        /// <summary>
        /// Отрисовка карты в главном окне с выставлением камеры на начальное положение.
        /// </summary>
        public void DrawSeaMapWithResetCamera()
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow.DrawSeaMapWithResetCamera();
            });
        }

        /// <summary>
        /// Отрисовка карты в главном окне без выставлением камеры на начальное положение.
        /// </summary>
        public void DrawSeaMapWithoutResetCamera()
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow.DrawSeaMapWithoutResetCamera();
            });
        }

        /// <summary>
        /// Подписка событий MainWindow.
        /// </summary>
        private void SubscribeEventsOfMainWindow()
        {
            _mainWindow.MenuItemListMapsClick += MenuItemListMaps_Click;
            _mainWindow.MenuItemSettingsInterpolClick += MenuItemSettingsInterpol_Click;
            _mainWindow.ZoomEvent += MainWindow_ZoomEvent;
        }

        private void MenuItemSettingsInterpol_Click()
        {
            MenuItemSettingsInterpolClick?.Invoke();
        }

        private void MenuItemListMaps_Click()
        {
            MenuItemListMapsClick?.Invoke();
        }

        private void MainWindow_ZoomEvent(int scale)
        {
            ZoomEvent?.Invoke(scale);
        }

        #endregion

        #region Region methods of MessageWindow.

        /// <summary>
        /// Отображение окна сообщения с ошибкой.
        /// </summary>
        /// <param name="title">Заголовок окна.</param>
        /// <param name="text">Текст сообщения в окне.</param>
        public void ShowMessageError(string title, string text)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        #endregion


        #region Region methods of windows settings.

        /// <summary>
        /// Отображение окна с настройками интерполции.
        /// </summary>
        public void ShowSettingsInterlopWindow()
        {
            Dispatcher.Invoke(() =>
            {
                ISettingInterpol settingInterpolWindow = new SettingsInterlopWindow();
                settingInterpolWindow.SettingInterpol = SettingInterpol;
                settingInterpolWindow.Save += SettingInterpolWindow_Save;
                settingInterpolWindow.ShowSettingsInterlopWindow();
            });
        }

        private void SettingInterpolWindow_Save(IVSettingInterpol settings)
        {
            if (_isLoadMap)
            {
                SaveSettingsInterpol?.Invoke(settings);
            }
        }

        #endregion
    }
}
