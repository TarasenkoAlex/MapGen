using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface ITableMapsWindow
    {
        MapView[] Maps { set; }
        Window OwnerWindow { set; }
        Window Window { get; }

        void ShowTableMaps();

        event Action<string[]> ChooseMap;
    }
}
