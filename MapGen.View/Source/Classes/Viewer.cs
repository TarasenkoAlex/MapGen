using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MapGen.View.GUI.Windows;
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
                _mainWindow.GraphicMap = value;
            }
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

        #endregion

        #region Region events of MainWindow.

        /// <summary>
        /// Событие выбора елемента View "Файл.База данных карт".
        /// </summary>
        public event Action MenuItemListMapsOnClick;

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
                tableMapsWindow.ChooseMap += TableMapsWindow_ChooseMap;
                tableMapsWindow.Maps = ConvertStringToMapView(tableMaps);
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
        /// Отрисовка карты в главном окне.
        /// </summary>
        public void DrawSeaMap()
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow.DrawSeaMap();
            });
        }

        /// <summary>
        /// Подписка событий MainWindow.
        /// </summary>
        private void SubscribeEventsOfMainWindow()
        {
            _mainWindow.MenuItemListMapsOnClick += MenuItemListMapsOn_Click;
            _mainWindow.ZoomEvent += MainWindow_ZoomEvent;
        }
        
        private void MenuItemListMapsOn_Click()
        {
            MenuItemListMapsOnClick?.Invoke();
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

    }
}
