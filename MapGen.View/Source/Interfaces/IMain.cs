using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IMain
    {
        #region Region properties.
        Window OwnerWindow { set; }
        Window Window { get; }
        RegMatrixView RegMatrix { set; }
        #endregion

        #region Region events.
        event Action MenuItemListMapsOnClick;
        #endregion

        #region Region methods.
        void ShowMainWindow();
        void CreateTriangleCollectionMap();
        void DrawMap();
        #endregion
    }
}