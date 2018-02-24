using System;
using System.Collections.Generic;
using MapGen.View.GUI.Windows;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.Source.Classes
{
    public class Viewer : IView
    {

        #region Region public events.

        /// <summary>
        /// Событие открытия окна со списком карт.
        /// </summary>
        public event Action ShowMapsOnClick;

        /// <summary>
        /// Событие загрузки карт.
        /// </summary>
        public event Action LoadMaps;

        /// <summary>
        /// Событи загрузки одной карты.
        /// </summary>
        public event Action<string[]> LoadMap;

        #endregion


        #region Region private fields.

        private readonly IMainWindow _mainWindow;

        private MapView[] _tableMaps;

        #endregion


        #region Region properties.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        public object MainWindow => _mainWindow;

        /// <summary>
        /// Карты.
        /// </summary>
        public List<string[]> Maps
        {
            set
            {
                _tableMaps = new MapView[value.Count];

                for (int i = 0; i < value.Count; ++i)
                {
                    _tableMaps[i] = new MapView(
                        int.Parse(value[i][0]),
                        value[i][1],
                        int.Parse(value[i][2]),
                        int.Parse(value[i][3]),
                        int.Parse(value[i][4]));
                }
            }
        }

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


        #region Region constructor.

        public Viewer()
        {
            _tableMaps = new MapView[] { };
            // Создаем главное окно программы MapGeneralization.
            _mainWindow = new MainWindow();
            // Подписка на события MainWindow.
            BindingEventsMainWindow();
        }

        #endregion


        #region Region methods TableMapsWindow.
        
        /// <summary>
        /// Открыть окно о списком карт.
        /// </summary>
        public void ShowTableMaps()
        {
            ITableMapsWindow tableMapsWindow = new TableMapsWindow();
            tableMapsWindow.ChooseMap += TableMapsWindow_ChooseMap;
            LoadMaps?.Invoke();
            tableMapsWindow.Maps = _tableMaps;
            tableMapsWindow.ShowTableMaps();
        }

        private void TableMapsWindow_ChooseMap(string[] map)
        {
            LoadMap?.Invoke(map);
        }

        #endregion


        #region Region methods MainWindow.

        /// <summary>
        /// Окрыть главное окно программы MapGeneralization.
        /// </summary>
        public void ShowMainWindow()
        {
            _mainWindow.ShowMainWindow();
        }

        public void ShowLoadingMap()
        {
            _mainWindow.ShowLoadingMap();
        }

        private void BindingEventsMainWindow()
        {
            _mainWindow.MenuItemListMapsOnClick += MenuItemListMapsOn_Click;
        }
        private void MenuItemListMapsOn_Click()
        {
            ShowMapsOnClick?.Invoke();
        }

        #endregion

    }
}
