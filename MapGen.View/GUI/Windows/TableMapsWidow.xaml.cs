using System;
using System.Windows;
using System.Windows.Input;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableMapsWindow.xaml
    /// </summary>
    public partial class TableMapsWindow : ITableMaps
    {
        #region Region properties.

        /// <summary>
        /// Список карт.
        /// </summary>
        public MapView[] Maps
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    _maps = value;
                    for (int i = 0; i < value.Length; ++i)
                        GridTableMaps.Items.Add(value[i]);
                });
            }
        }

        #endregion
        
        #region Region events.

        /// <summary>
        /// Событие выбора элемента из списка карт.
        /// </summary>
        public event Action<int> ChooseMap;

        #endregion

        #region Region private fields.

        /// <summary>
        /// Список карт.
        /// </summary>
        private MapView[] _maps;

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает окно со списком карт.
        /// </summary>
        public TableMapsWindow()
        {
            InitializeComponent();
            InitFields();
            BindingEventsButtonWindow();
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Отобразить окно со списком карт.
        /// </summary>
        public void ShowTableMaps()
        {
            ShowDialog();
        }

        #endregion
        
        #region Region private methods.

        /// <summary>
        /// Инициализация private полей.
        /// </summary>
        private void InitFields()
        {
            _maps = new MapView[] { };
        }

        /// <summary>
        /// Подписка событий окна.
        /// </summary>
        private void BindingEventsButtonWindow()
        {
            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) => Close();
            
            // Обработка выбора строки в таблице.
            GridTableMaps.MouseDoubleClick += (s, e) => SelectMap();
        }

        #endregion
        
        #region Region handler events.
        
        /// <summary>
        /// Обработка события выбора элемента из списка карт.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SelectMap();
        }

        /// <summary>
        /// Обработка события загрузки карты.
        /// </summary>
        private void SelectMap()
        {
            MapView selectedItem = (MapView)GridTableMaps.SelectedItem;
            if (selectedItem != null)
            {
                Close();
                ChooseMap?.Invoke(selectedItem.Id);
            }
        }

        #endregion
    }
}
