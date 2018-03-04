using System;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface ITableMaps
    {
        #region Region properties.
        MapView[] Maps { set; }
        #endregion

        #region Region events.
        event Action<int> ChooseMap;
        #endregion

        #region Region methods.
        void ShowTableMaps();
        #endregion
    }
}
