using System;
using System.Collections.Generic;
using System.Globalization;
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
using MapGen.View.Source.Classes.SettingClustering;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Grids
{
    /// <summary>
    /// Interaction logic for SettingKMeansGrid.xaml
    /// </summary>
    public partial class SettingKMeansGrid : Grid, ISettingKMeans
    {
        public SettingKMeansGrid()
        {
            InitializeComponent();
        }

        public Grid Grid => this;

        public VSettingCLKMeans SettingCl
        {
            get
            {
                VSettingCLKMeans setting = new VSettingCLKMeans();
                Dispatcher.Invoke(() =>
                {
                    setting.Seeding = GetVSeedings();
                    setting.MaxDegreeOfParallelism = Convert.ToInt32(TextBoxMaxDegreeOfParallelism.Text);
                    setting.MaxItarations = Convert.ToInt32(TextBoxMaxItarations.Text);
                });
                return setting;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    ComboBoxSeeding.Text = GetStringFromVSeedings(value.Seeding);
                    TextBoxMaxDegreeOfParallelism.Text = value.MaxDegreeOfParallelism.ToString(CultureInfo.InvariantCulture);
                    TextBoxMaxItarations.Text = value.MaxItarations.ToString(CultureInfo.InvariantCulture);
                });
            }
        }

        private VSeedings GetVSeedings()
        {
            string variogramStr = ComboBoxSeeding.SelectionBoxItem.ToString();
            switch (variogramStr)
            {
                case "Случайный": return VSeedings.Random;
                default: return VSeedings.Random;
            }
        }

        private string GetStringFromVSeedings(VSeedings seedings)
        {
            switch (seedings)
            {
                case VSeedings.Random: return "Случайный";
                default: return "Случайный";
            }
        }
    }
}
