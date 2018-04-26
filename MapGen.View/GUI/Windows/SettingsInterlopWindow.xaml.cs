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
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Interaction logic for SettingsInterlopWindow.xaml
    /// </summary>
    public partial class SettingsInterlopWindow : Window, ISettingInterpol
    {
        
        #region Region properties.

        public IVSettingInterpol SettingInterpol
        {
            set
            {
                if (value is VSettingInterpolKriging)
                {
                    _isKriging = true;
                    _settingKrigingGrid = new SettingKrigingGrid
                    {
                        SettingInterpol = value as VSettingInterpolKriging
                    };

                    RadioButtonKriging.IsChecked = true;
                    _content = _settingKrigingGrid.Grid;
                }   
                else if (value is VSettingInterpolRbf)
                {
                    _isKriging = false;
                    _settingRbfGrid = new SettingRbfGrid
                    {
                        SettingInterpol = value as VSettingInterpolRbf
                    };

                    RadioButtonRbf.IsChecked = true;
                    _content = _settingRbfGrid.Grid;
                }
            }
        }

        #endregion

        #region Region events.

        public event Action<IVSettingInterpol> Save;

        #endregion

        #region Region private fields.

        /// <summary>
        /// Выбран Кригинг?.
        /// </summary>
        private bool _isKriging;

        /// <summary>
        /// Page с настройками Кригинга.
        /// </summary>
        private ISettingKriging _settingKrigingGrid;

        /// <summary>
        /// Page с настройками RBF.
        /// </summary>
        private ISettingRbf _settingRbfGrid;

        #endregion

        public SettingsInterlopWindow()
        {
            InitializeComponent();
            InitFields();
            SubscribeEventsWindow();
        }

        #region Region private methods.

        /// <summary>
        /// Инициализация private полей.
        /// </summary>
        private void InitFields()
        {
            _isKriging = true;
            SettingInterpol = new VSettingInterpolKriging();
            _settingKrigingGrid = new SettingKrigingGrid();
            _settingRbfGrid = new SettingRbfGrid();
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

            // Обработка изменения радиобатона.
            RadioButtonKriging.Checked += (sender, args) =>
            {
                _isKriging = true;
                _content = _settingKrigingGrid.Grid;
            };
            RadioButtonRbf.Checked += (sender, args) =>
            {
                _isKriging = false;
                _content = _settingRbfGrid.Grid;
            };
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
            if (_isKriging)
            {
                Save?.Invoke(_settingKrigingGrid.SettingInterpol);
            }
            else
            {
                Save?.Invoke(_settingRbfGrid.SettingInterpol);
            }
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

        #endregion

        #region Region public methods.

        /// <summary>
        /// Отобразить окно.
        /// </summary>
        public void ShowSettingsInterlopWindow()
        {
            ShowDialog();
        }

        #endregion

    }
}
