using System;
using System.Collections.Generic;
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
        /// Регулярна матрица глубин.
        /// </summary>
        public RegMatrixView RegMatrix
        {
            set
            {
                _mainWindow.RegMatrix = value;
                _mainWindow.CreateTriangleCollectionMap();
            }
        }

        #endregion
        
        #region Region general events.

        /// <summary>
        /// Событие загрузки карты.
        /// </summary>
        public event Action<int> LoadDbMap;

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
            BindingEventsOfMainWindow();
        }

        #endregion
        
        #region Region methods of TableMapsWindow.
        
        /// <summary>
        /// Открыть окно со списком карт.
        /// </summary>
        public void ShowTableDbMaps(List<string[]> tableMaps)
        {
            ITableMaps tableMapsWindow = new TableMapsWindow();
            tableMapsWindow.ChooseMap += TableMapsWindow_ChooseMap;
            tableMapsWindow.Maps = ConvertStringToMapView(tableMaps);
            tableMapsWindow.ShowTableMaps();
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
                    int.Parse(tMaps[i][2]),
                    int.Parse(tMaps[i][3]),
                    int.Parse(tMaps[i][4]));
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
            _mainWindow.ShowMainWindow();
        }

        /// <summary>
        /// Отрисовка карты в главном окне.
        /// </summary>
        public void DrawMap()
        {
            _mainWindow.DrawMap();
        }

        /// <summary>
        /// Подписка событий MainWindow.
        /// </summary>
        private void BindingEventsOfMainWindow()
        {
            _mainWindow.MenuItemListMapsOnClick += MenuItemListMapsOn_Click;
        }

        /// <summary>
        /// Обработка события выбор елемента View "Файл.База данных карт".
        /// </summary>
        private void MenuItemListMapsOn_Click()
        {
            MenuItemListMapsOnClick?.Invoke();
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
            IMessage message = new MessageWindow();
            message.ShowMessage(title, text, MessageButton.Ok, MessageType.Error);
        }

        #endregion

    }
}
