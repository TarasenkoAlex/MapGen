using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;
using MapGen.Model.Generalization.Algoritm;
using MapGen.Model.Generalization.Setting;
using MapGen.Model.Maps;

namespace MapGen.Model.Test
{
    public class TestSystem
    {
        public event Action<TestResult> TestFinished;

        private readonly List<TestCase> _testCases = new List<TestCase>();

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

        public int GetMaxIdTestCase()
        {
            return _maxIdTestCase;
        }

        public void AddTestCase(DbMap dbMap, SettingGen settingGen, long scale)
        {
            _maxIdTestCase++;
            TestCase testCase = new TestCase
            {
                Id = _maxIdTestCase,
                DbMap = dbMap,
                SettingGen = settingGen,
                Scale = scale
            };
            testCase.TestFinished += TestFinishedAction;

            _testCases.Add(testCase);
        }

        public void RemoveAllTestCase()
        {
            _testCases.Clear();
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

        public event Action<TestResult> TestFinished;

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
                
                string dirResultTests = $"{ResourceModel.DIR_TESTS}\\Test_{Id}";
                if (!Directory.Exists(dirResultTests))
                {
                    Directory.CreateDirectory(dirResultTests);
                }
                // Отрисовываем исходную карту.
                Methods.DeleteAllElementsOnDirectry(dirResultTests);
                DbMap.DrawToBMP($"{dirResultTests}\\{ResourceModel.FILENAME_BEFORE_BMP}");

                // Отрисовываем результирующую карту.
                DbMap.DrawToBMP(mgAlgoritm.Clusters, $"{dirResultTests}\\{ResourceModel.FILENAME_AFTER_BMP}");

                // Сохраняем в файл результаты теста с настройкой.
                string distScaleInfo = $"Масштаб теста: 1:{Scale}";
                string testInfo = $"{DbMap}\n{distScaleInfo}\n{SettingGen}\n{testResult}";
                File.WriteAllText($"{dirResultTests}\\{ResourceModel.FILENAME_TESTINFO}", testInfo);

                TestFinished?.Invoke(testResult);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class TestResult
    {
        public int IdTestCase { get; set; }
        public long Time { get; set; }
        public bool IsSuccess { get; set; }

        public override string ToString()
        {
            return $"Время выполнения: {Time} мс.";
        }
    }

}
