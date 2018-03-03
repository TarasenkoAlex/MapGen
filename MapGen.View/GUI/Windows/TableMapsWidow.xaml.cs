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
    public partial class TableMapsWindow : Window, ITableMaps
    {
        #region Region private fields.

        private MapView[] _maps;

        #endregion
        
        #region Region events.

        public event Action<int> ChooseMap;

        #endregion
        
        #region Region properties.

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
        
        #region Region constructor.

        public TableMapsWindow()
        {
            InitializeComponent();
            InitFields();
            BindingEventsButtonWindow();
        }

        #endregion

        #region Region public methods.

        public void ShowTableMaps()
        {
            ShowDialog();
        }

        #endregion
        
        #region Region private methods.

        private void InitFields()
        {
            _maps = new MapView[] { };
        }

        private void BindingEventsButtonWindow()
        {
            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) => Close();
            
            // Обработка выбора строки в таблице.
            GridTableMaps.MouseDoubleClick += (s, e) => SelectMap();
        }

        #endregion
        
        #region Region handler events.
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SelectMap();
        }
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
