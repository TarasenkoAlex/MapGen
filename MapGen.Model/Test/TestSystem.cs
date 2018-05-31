using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.Generalization.Algoritm;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Maps;

namespace MapGen.Model.Test
{
    public class TestSystem
    {
        public Action<TestResult> TestFinished;

        private List<TestCase> _testCases { get; set; }

        private int _maxIdTestCase = -1;

        public void Init()
        {
            string[] dirTests = Directory.GetDirectories(ResourceModel.DIR_TESTS);
            for (int i = 0; i < dirTests.Length; ++i)
            {
                int id = int.Parse(new DirectoryInfo(dirTests[i]).Name.Split('_').Last());
                if (id > _maxIdTestCase)
                {
                    _maxIdTestCase = id;
                }
            }
        }

        public void AddTestCase(DbMap dbMap, SettingGen settingGen, long scale)
        {
            TestCase testCase = new TestCase
            {
                Id = _maxIdTestCase + 1,
                DbMap = dbMap,
                SettingGen = settingGen
            };
            testCase.TestFinished += TestFinishedAction;

            _testCases.Add(testCase);
        }

        private void TestFinishedAction(TestResult testResult)
        {
            TestFinished?.Invoke(testResult);
        }

        public void Run()
        {
            foreach (TestCase testCase in _testCases)
            {
                testCase.Run();
            }
        }
    }

    public class TestCase
    {
        public int Id { get; set; }
        public DbMap DbMap { get; set; }
        public SettingGen SettingGen { get; set; }
        public long Scale { get; set; }

        public Action<TestResult> TestFinished;

        public void Run()
        {
            try
            {
                IMGAlgoritm mgAlgoritm = new CLMGAlgoritm(SettingGen);
                DbMap outDbMap;
                string message;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Reset();
                stopwatch.Start();
                bool isSuccess = mgAlgoritm.Execute(Scale, DbMap, out outDbMap, out message);
                stopwatch.Stop();

                TestResult testResult = new TestResult
                {
                    IdTestCase = Id,
                    Time = stopwatch.ElapsedMilliseconds,
                    IsSuccess = isSuccess
                };

                TestFinished?.Invoke(testResult);
            }
            catch
            {
            }
        }
    }

    public class TestResult
    {
        public int IdTestCase { get; set; }
        public long Time { get; set; }
        public bool IsSuccess { get; set; }
    }

}
