using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IMainWindow
    {
        Window OwnerWindow { set; }
        Window Window { get; }
        RegMatrixView RegMatrix { set; }

        void ShowMainWindow();
        void CreateTriangleCollectionMap();
        void ShowLoadingMap();

        event Action MenuItemListMapsOnClick;
    }
}