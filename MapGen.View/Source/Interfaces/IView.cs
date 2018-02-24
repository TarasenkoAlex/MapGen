using System;
using System.Collections.Generic;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface IView
    {

        #region Region properties.

        object MainWindow { get; }
        List<string[]> Maps { set; }
        RegMatrixView RegMatrix { set; }
        void ShowLoadingMap();

        #endregion


        #region Region general methods.

        void ShowMainWindow();
        void ShowTableMaps();

        #endregion


        #region Region events.

        event Action ShowMapsOnClick;
        event Action LoadMaps;
        event Action<string[]> LoadMap;

        #endregion

    }
}
