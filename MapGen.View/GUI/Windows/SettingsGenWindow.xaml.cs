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
using MapGen.View.GUI.Grids;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingClustering;
using MapGen.View.Source.Classes.SettingGen;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Interaction logic for SettingsGenWindow.xaml
    /// </summary>
    public partial class SettingsGenWindow : Window, ISettingGen
    {
        #region Region properties.

        public VSettingGen SettingGen
        {
            private get { return _settingGen;}
            set
            {
                _settingGen = value;

                if (value.SelectionRule == VSelectionRules.Topfer)
                {
                    RadioButtonTopfer.IsChecked = true;
                }

                if (value.SettingCL is VSettingCLKMeans)
                {
                    _isKMeans = true;
                    _settingKMeansGrid = new SettingKMeansGrid
                    {
                        SettingCl = value.SettingCL as VSettingCLKMeans
                    };

                    RadioButtonKMeans.IsChecked = true;
                    RadioButtonKNP.IsChecked = false;
                    _content = _settingKMeansGrid.Grid;
                }

                if (value.SettingCL is VSettingCLKNP)
                {
                    _isKMeans = false;
                    _settingKNPGrid = new SettingKNPGrid
                    {
                        SettingCl = value.SettingCL as VSettingCLKNP
                    };

                    RadioButtonKMeans.IsChecked = false;
                    RadioButtonKNP.IsChecked = true;
                    _content = _settingKNPGrid.Grid;
                }
            }
        }

        #endregion

        #region Region events.

        public event Action<VSettingGen> Save;

        #endregion

        #region Region private fields.

        /// <summary>
        /// Настройка генерализации.
        /// </summary>
        private VSettingGen _settingGen = new VSettingGen();

        /// <summary>
        /// Формула генерализации.
        /// </summary>
        private VSelectionRules _selectionRule = VSelectionRules.Topfer;

        /// <summary>
        /// Выбран К - средних?.
        /// </summary>
        private bool _isKMeans = true;

        /// <summary>
        /// Grid с настройками K - средних.
        /// </summary>
        private ISettingKMeans _settingKMeansGrid = new SettingKMeansGrid();

        /// <summary>
        /// Grid с настройками кратчайший незамкнутый путь.
        /// </summary>
        private ISettingKNP _settingKNPGrid = new SettingKNPGrid();

        #endregion

        public SettingsGenWindow()
        {
            InitializeComponent();
            SubscribeEventsWindow();
        }

        /// <summary>
        /// Подписка событий окна.
        /// </summary>
        private void SubscribeEventsWindow()
        {
            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) => Close();

            // Обработка кнопки сохранения.
            ButtonSave.Click += ButtonSave_Click;

            // Обработка изменения радиобатона формулы генерализации.
            RadioButtonTopfer.Checked += (sender, args) => { _selectionRule = VSelectionRules.Topfer; };

            // Обработка изменения радиобатона кластеризации.
            RadioButtonKMeans.Checked += (sender, args) =>
            {
                _isKMeans = true;
                _content = _settingKMeansGrid.Grid;
            };
            RadioButtonKNP.Checked += (sender, args) =>
            {
                _isKMeans = false;
                _content = _settingKNPGrid.Grid;
            };
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Close();

            SettingGen.SelectionRule = _selectionRule;

            if (_isKMeans)
            {
                SettingGen.SettingCL = _settingKMeansGrid.SettingCl;
            }
            else
            {
                SettingGen.SettingCL = _settingKNPGrid.SettingCl;
            }

            Save?.Invoke(SettingGen);
        }

        private Grid _content
        {
            get { return GridSettings; }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    if (value != null) // Если value == null то в меню больше пунктов, чем в классе Grid
                    {
                        GridSettings.Children.Clear();
                        GridSettings.Children.Add(value);
                    }
                });
            }
        }

        #region Region public methods.

        /// <summary>
        /// Отобразить окно.
        /// </summary>
        public void ShowSettingsGenWindow()
        {
            ShowDialog();
        }

        #endregion
    }
}
