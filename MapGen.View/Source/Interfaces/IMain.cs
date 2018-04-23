using System;
using System.Windows;
using System.Windows.Threading;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IMain
    {
        #region Region properties.
        GraphicMap GraphicMap { set; }
        Dispatcher MyDispatcher { get; }
        #endregion
         
        #region Region events.
        event Action MenuItemListMapsOnClick;
        event Action<int> ZoomEvent;
        #endregion

        #region Region methods.
        void ShowMainWindow();
        void DrawSeaMap();
        #endregion
    }
}