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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableMapsWindow.xaml
    /// </summary>
    public partial class TableMapsWindow : Window, ITableMapsWindow
    {

        #region Region private fields.

        private MapView[] _maps;

        #endregion


        #region Region events.

        public event Action<string[]> ChooseMap;

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

        public Window OwnerWindow
        {
            set { Owner = value; }
        }

        public Window Window => this;

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
        }

        #endregion


        #region Region handler events.

        private void GridTableMaps_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectMap();
        }
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
                ChooseMap?.Invoke(new[]
                {
                    selectedItem.Id.ToString(),
                    selectedItem.Name,
                    selectedItem.Width.ToString(),
                    selectedItem.Length.ToString(),
                    selectedItem.Scale.ToString()
                });
            }
        }

        #endregion


    }
}
