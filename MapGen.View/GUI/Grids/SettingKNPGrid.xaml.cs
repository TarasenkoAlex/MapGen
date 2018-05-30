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
using System.Windows.Shapes;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingClustering;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Grids
{
    /// <summary>
    /// Interaction logic for SettingKNPGrid.xaml
    /// </summary>
    public partial class SettingKNPGrid : Grid, ISettingKNP
    {
        public SettingKNPGrid()
        {
            InitializeComponent();
        }

        public Grid Grid => this;

        public VSettingCLKNP SettingCl
        {
            get
            {
                VSettingCLKNP setting = new VSettingCLKNP();
                Dispatcher.Invoke(() =>
                {
                    setting.MaxDegreeOfParallelism = Convert.ToInt32(TextBoxMaxDegreeOfParallelism.Text);
                });
                return setting;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    TextBoxMaxDegreeOfParallelism.Text = value.MaxDegreeOfParallelism.ToString(CultureInfo.InvariantCulture);
                });
            }
        }
    }
}
