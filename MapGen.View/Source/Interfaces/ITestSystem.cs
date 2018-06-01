using System;
using System.Collections.Generic;
using System.Windows;
using MapGen.View.Source.Classes;

namespace MapGen.View.Source.Interfaces
{
    public interface ITestSystem
    {
        #region Region events.
        event Action<List<VTestCase>> RunAllTests;
        #endregion

        #region Region methods.
        void ShowTestSystem();
        void TestFinished(VTestResult vTestResult);
        #endregion
    }
}
