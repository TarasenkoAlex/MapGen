using System;
using System.Collections.Generic;
using System.Windows.Threading;
using MapGen.View.GUI.Windows;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingGen;
using MapGen.View.Source.Classes.SettingInterpol;

namespace MapGen.View.Source.Interfaces
{
    public interface IView
    {
        #region Region properties.
        object MainWindow { get; }
        GraphicMap GraphicMap { set; }
        Dispatcher Dispatcher { get; }
        IVSettingInterpol SettingInterpol { get; set; }
        VSettingGen SettingGen { get; set; }
        bool IsRunningProgressBarMainWindow { set; }
        string NameProcess { set; }
        long SourcScale { get; set; }
        #endregion

        #region Region general events.
        event Action<int> LoadDbMap;
        event Action<int> ZoomEvent;
        event Action<IVSettingInterpol> SaveSettingsInterpol;
        event Action<VSettingGen> SaveSettingsGen;
        event Action<List<VTestCase>> RunAllTests;
        #endregion

        #region Region events of MainWindow.
        event Action MenuItemListMapsClick;
        event Action MenuItemSettingsInterpolClick;
        event Action MenuItemSettingsGenClick;
        event Action MenuItemTestSystemClick;
        #endregion

        #region Region methods of TableMapsWindow.
        void ShowTableDbMaps(List<string[]> tableMaps);
        #endregion

        #region Region methods of MainWindow.
        void ShowMainWindow();
        void DrawSeaMapWithResetCamera();
        void DrawSeaMapWithoutResetCamera();
        #endregion

        #region Region methods of MessageWindow.
        void ShowMessageError(string title, string text);
        #endregion

        #region Region methods of windows settings.
        void ShowSettingsInterlopWindow();
        void ShowSettingsGenWindow();
        #endregion

        #region Region methods of windows test system.
        void ShowTestSystemWindow(int maxIdTestCase);
        void TestFinished(VTestResult vTestResult);
        #endregion

    }
}
