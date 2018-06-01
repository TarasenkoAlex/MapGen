using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MapGen.View.GUI.UserControls;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Interaction logic for TestSystemWindow.xaml
    /// </summary>
    public partial class TestSystemWindow : Window, ITestSystem
    {
        #region Region events.

        public event Action<List<VTestCase>> RunAllTests;

        #endregion

        #region Region private fields.

        private readonly List<ITestCase> _testCases = new List<ITestCase>();
        private int _maxIdTestCase;
        private long _sourceScale;

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает окно со списком карт.
        /// </summary>
        public TestSystemWindow(int maxIdTestCase, long sourceScale)
        {
            InitializeComponent();
            InitFields(maxIdTestCase, sourceScale);
            SubscribeEventsButtonWindow();
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Отобразить окно с тестовой системой.
        /// </summary>
        public void ShowTestSystem()
        {
            ShowDialog();
        }

        /// <summary>
        /// Отобразить завершение теста.
        /// </summary>
        public void TestFinished(VTestResult vTestResult)
        {
            ITestCase testCase = _testCases.Find(el => el.Id == vTestResult.IdTestCase);
            testCase.IsRunningProgressBar = false;
            testCase.IsSuccess = vTestResult.IsSuccess;
            testCase.Time = vTestResult.Time;
            ListBoxTestCases.Items.Refresh();
        }

        #endregion

        #region Region private methods.

        /// <summary>
        /// Инициализация private полей.
        /// </summary>
        private void InitFields(int maxIdTestCase, long sourceScale)
        {
            ListBoxTestCases.ItemsSource = _testCases;
            _maxIdTestCase = maxIdTestCase;
            _sourceScale = sourceScale;
        }
        
        /// <summary>
        /// Подписка событий окна.
        /// </summary>
        private void SubscribeEventsButtonWindow()
        {
            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) => Close();

            // Обработка кнопки добавления теста.
            ButtonAddTestCase.Click += ButtonAddTestCase_Click;

            // Обработка кнпоки запуска тестов.
            ButtonRunTests.Click += ButtonRunTests_Click;
        }

        private void ButtonAddTestCase_Click(object sender, RoutedEventArgs e)
        {
            _maxIdTestCase++;
            _testCases.Add(new TestCaseUserControl
            {
                Id = _maxIdTestCase,
                SourceScale = _sourceScale
            });
            ListBoxTestCases.Items.Refresh();
        }

        private void ButtonRunTests_Click(object sender, RoutedEventArgs e)
        {
            List<VTestCase> testCases = new List<VTestCase>();

            foreach (ITestCase tCase in _testCases)
            {
                VTestCase vTestCase = new VTestCase
                {
                    Scale = tCase.DistScale,
                    SettingGen = tCase.SettingGen
                };
                testCases.Add(vTestCase);
                tCase.IsRunningProgressBar = true;
            }

            RunAllTests?.Invoke(testCases);
        }

        #endregion
    }
}
