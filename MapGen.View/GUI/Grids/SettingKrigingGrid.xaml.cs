using System;
using System.Globalization;
using System.Windows.Controls;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Grids
{
    /// <summary>
    /// Interaction logic for SettingKrigingGrid.xaml
    /// </summary>
    public partial class SettingKrigingGrid : Grid, ISettingKriging
    {
        public SettingKrigingGrid()
        {
            InitializeComponent();
        }

        public Grid Grid => this;

        public VSettingInterpolKriging SettingInterpol
        {
            get
            {
                VSettingInterpolKriging setting = new VSettingInterpolKriging();
                Dispatcher.Invoke(() =>
                {
                    setting.MinRadiusOfEnvirons = Convert.ToDouble(TextBoxMinRadiusOfEnvirons.Text);
                    setting.MinCountPointsOfEnvirons = Convert.ToInt32(TextBoxMinCountPointsOfEnvirons.Text);
                    setting.StepEncreaseOfEnvirons = Convert.ToDouble(TextBoxStepEncreaseOfEnvirons.Text);
                    setting.Variogram = GetVVariograms();
                    setting.A = Convert.ToDouble(TextBoxA.Text);
                    setting.C = Convert.ToDouble(TextBoxC.Text);
                    setting.C0 = Convert.ToDouble(TextBoxC0.Text);
                });
                return setting;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    TextBoxMinRadiusOfEnvirons.Text = value.MinRadiusOfEnvirons.ToString(CultureInfo.InvariantCulture);
                    TextBoxMinCountPointsOfEnvirons.Text = value.MinCountPointsOfEnvirons.ToString();
                    TextBoxStepEncreaseOfEnvirons.Text = value.StepEncreaseOfEnvirons.ToString(CultureInfo.InvariantCulture);
                    ComboBoxVariogram.Text = GetStringFromVariogram(value.Variogram);
                    TextBoxA.Text = value.A.ToString(CultureInfo.InvariantCulture);
                    TextBoxC.Text = value.C.ToString(CultureInfo.InvariantCulture);
                    TextBoxC0.Text = value.C0.ToString(CultureInfo.InvariantCulture);
                });
            }
        }

        private VVariograms GetVVariograms()
        {
            string variogramStr = ComboBoxVariogram.SelectionBoxItem.ToString();
            switch (variogramStr)
            {
                case "Сферическая": return VVariograms.Spherial;
                case "Гауссова": return VVariograms.Gauss;
                case "Экспоненциальная": return VVariograms.Exponent;
                case "Линейная": return VVariograms.Linear;
                case "Круговая": return VVariograms.Circle;
                default: return VVariograms.Spherial;
            }
        }

        private string GetStringFromVariogram(VVariograms variogram)
        {
            switch (variogram)
            {
                case VVariograms.Spherial: return "Сферическая";
                case VVariograms.Gauss: return "Гауссова";
                case VVariograms.Exponent: return "Экспоненциальная";
                case VVariograms.Linear: return "Линейная";
                case VVariograms.Circle: return "Круговая";
                default: return "Сферическая";
            }
        }
    }
}
