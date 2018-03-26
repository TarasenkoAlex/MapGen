using System;
using System.Windows;
using System.Windows.Threading;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IMain
    {
        #region Region properties.
        RegMatrixView RegMatrix { set; }
        Dispatcher MyDispatcher { get; }
        #endregion

        #region Region events.
        event Action MenuItemListMapsOnClick;
        #endregion

        #region Region methods.
        void ShowMainWindow();
        void DrawSeaMap();
        #endregion
    }
}