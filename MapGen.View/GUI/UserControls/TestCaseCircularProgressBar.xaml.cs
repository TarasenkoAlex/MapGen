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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapGen.View.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for TestCaseCircularProgressBar.xaml
    /// </summary>
    public partial class TestCaseCircularProgressBar : UserControl
    {
        static TestCaseCircularProgressBar()
        {
            //Use a default Animation Framerate of 30, which uses less CPU time
            //than the standard 50 which you get out of the box
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(
            //    typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 20 });
        }

        public TestCaseCircularProgressBar()
        {
            InitializeComponent();
        }
    }
}
