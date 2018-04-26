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
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.Grids
{
    /// <summary>
    /// Interaction logic for SettingRbfPage.xaml
    /// </summary>
    public partial class SettingRbfGrid : Grid, ISettingRbf
    {
        public SettingRbfGrid()
        {
            InitializeComponent();
        }

        public Grid Grid => this;


        public VSettingInterpolRbf SettingInterpol
        {
            get
            {
                VSettingInterpolRbf setting = new VSettingInterpolRbf();
                Dispatcher.Invoke(() =>
                {
                    setting.MinRadiusOfEnvirons = Convert.ToDouble(TextBoxMinRadiusOfEnvirons.Text);
                    setting.MinCountPointsOfEnvirons = Convert.ToInt32(TextBoxMinCountPointsOfEnvirons.Text);
                    setting.StepEncreaseOfEnvirons = Convert.ToDouble(TextBoxStepEncreaseOfEnvirons.Text);
                    setting.BasicFunction = GetVBasicFunction();
                    setting.R = Convert.ToDouble(TextBoxR.Text);
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
                    ComboBoxBasicFunction.Text = GetStringFromBasicFunction(value.BasicFunction);
                    TextBoxR.Text = value.R.ToString(CultureInfo.InvariantCulture);
                });
            }
        }

        private VBasicFunctions GetVBasicFunction()
        {
            string funcStr = ComboBoxBasicFunction.SelectionBoxItem.ToString();
            switch (funcStr)
            {
                case "MultiLog": return VBasicFunctions.MultiLog;
                case "InverseMultiQuadric": return VBasicFunctions.InverseMultiQuadric;
                case "MultiQuadric": return VBasicFunctions.MultiQuadric;
                case "NaturalCubicSpline": return VBasicFunctions.NaturalCubicSpline;
                case "ThinPlateSpline": return VBasicFunctions.ThinPlateSpline;
                default: return VBasicFunctions.MultiLog;
            }
        }

        private string GetStringFromBasicFunction(VBasicFunctions func)
        {
            switch (func)
            {
                case VBasicFunctions.MultiLog: return "MultiLog";
                case VBasicFunctions.InverseMultiQuadric: return "InverseMultiQuadric";
                case VBasicFunctions.MultiQuadric: return "MultiQuadric";
                case VBasicFunctions.NaturalCubicSpline: return "NaturalCubicSpline";
                case VBasicFunctions.ThinPlateSpline: return "ThinPlateSpline";
                default: return "MultiLog";
            }
        }
    }
}
