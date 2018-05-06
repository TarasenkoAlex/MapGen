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
        bool IsRunningProgressBar { set; }
        string NameProcess { set; }
        #endregion

        #region Region events.
        event Action MenuItemListMapsClick;
        event Action MenuItemSettingsInterpolClick;
        event Action MenuItemSettingsGenClick;
        event Action<int> ZoomEvent;
        #endregion

        #region Region methods.
        void ShowMainWindow();
        void DrawSeaMapWithResetCamera();
        void DrawSeaMapWithoutResetCamera();
        #endregion
    }
}