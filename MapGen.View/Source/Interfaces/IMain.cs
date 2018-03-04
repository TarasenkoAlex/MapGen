using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IMain
    {
        #region Region properties.
        RegMatrixView RegMatrix { set; }
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