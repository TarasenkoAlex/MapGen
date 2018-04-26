using System;
using System.Windows;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingInterpol;

namespace MapGen.View.Source.Interfaces
{
    public interface ISettingInterpol
    {
        #region Region properties.
        IVSettingInterpol SettingInterpol { set; }
        #endregion

        #region Region events.
        event Action<IVSettingInterpol> Save;
        #endregion

        #region Region methods.
        void ShowSettingsInterlopWindow();
        #endregion
    }
}
