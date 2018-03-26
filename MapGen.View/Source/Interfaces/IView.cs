using System;
using System.Collections.Generic;
using System.Windows.Threading;
using MapGen.View.GUI.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IView
    {
        #region Region properties.
        object MainWindow { get; }
        RegMatrixView RegMatrix { set; }
        Dispatcher Dispatcher { get; }
        #endregion

        #region Region general events.
        event Action<int> LoadDbMap;
        #endregion

        #region Region events of MainWindow.
        event Action MenuItemListMapsOnClick;
        #endregion

        #region Region methods of TableMapsWindow.
        void ShowTableDbMaps(List<string[]> tableMaps);
        #endregion

        #region Region methods of MainWindow.
        void ShowMainWindow();
        void DrawSeaMap();
        #endregion

        #region Region methods of MessageWindow.
        void ShowMessageError(string title, string text);
        #endregion
    }
}
