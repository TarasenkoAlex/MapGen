using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Interpolation.Strategy;
using MapGen.Model.Maps;
using MapGen.Model.Test;

namespace MapGen.Model
{
    public interface IModel
    {
        #region Region properties.
        DbMap SourceSeaMap { get; }
        DbMap MapGenSeaMap { get; }
        DbMap CurrentSeaMap { get; }
        ISettingInterpol SettingInterpol { get; set; }
        SettingGen SettingGen { get; set; }
        event Action<TestResult> TestFinished;
        #endregion

        #region Region methods. Database.
        bool ConnectToDatabase(out string message);
        bool GetDbMaps(out List<string[]> maps, out string message);
        bool LoadDbMap(int idMap, out string message);
        bool GetDbMaps(out List<Map> maps, out string message);
        #endregion

        #region Region public methods. RegMatrix.
        bool CreateRegMatrix(out RegMatrix.RegMatrix regMatrix, out string message);
        #endregion

        #region Region public methods. MapGen.
        bool ExecuteMapGen(long scale, out string message);
        #endregion

        #region Region public methods. TestSystem.
        void AddTestCase(SettingGen settingGen, long scale);
        void RemoveAllTestCase();
        void RunTestSystem(List<TestCase> testCases);
        int GetMaxIdTestCase();

        #endregion
    }
}
