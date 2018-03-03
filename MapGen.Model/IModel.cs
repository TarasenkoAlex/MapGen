using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Maps;

namespace MapGen.Model
{
    public interface IModel
    {
        #region Region properties.
        ISettingInterpolation SettingInterpolation { get; set; }
        DbMap SeaMap { get; }
        #endregion

        #region Region methods. Database.
        bool ConnectToDatabase(out string message);
        bool GetDbMaps(out List<string[]> maps, out string message);
        bool LoadDbMap(int idMap, out string message);
        bool GetDbMaps(out List<Map> maps, out string message);
        bool CreateRegMatrix(out RegMatrix.RegMatrix regMatrix, out string message);
        #endregion
    }
}
