using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.View.Source.Classes.SettingClustering;
using MapGen.View.Source.Classes.SettingGen;
using MapGen.View.Source.Classes.SettingInterpol;

namespace MapGen.View.Source.Interfaces
{
    public interface ISettingGen
    {
        #region Region properties.
        VSettingGen SettingGen { set; }
        #endregion

        #region Region events.
        event Action<VSettingGen> Save;
        #endregion

        #region Region methods.
        void ShowSettingsGenWindow();
        #endregion
    }
}
