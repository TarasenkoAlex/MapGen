using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface ITableMaps
    {
        MapView[] Maps { set; }

        event Action<int> ChooseMap;

        void ShowTableMaps();
    }
}
