using System;
using System.Collections.Generic;
using System.Drawing;
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
using MapGen.View.Source.Classes.SettingGen;
using MapGen.View.Source.Interfaces;

namespace MapGen.View.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for TestCaseUserControl.xaml
    /// </summary>
    public partial class TestCaseUserControl : UserControl, ITestCase
    {
        public int Id
        {
            get
            {
                int result = 0;
                Dispatcher.Invoke(() => { result = Convert.ToInt32(LabelId.Content); });
                return result;
            }
            set { Dispatcher.Invoke(() => { LabelId.Content = value.ToString(); }); }
        }

        public VSettingGen SettingGen
        {
            get
            {
                VSettingGen setting = new VSettingGen();

                Dispatcher.Invoke(() =>
                {
                    setting.SelectionRule = VSelectionRules.Topfer;
                    setting.SettingCL = GetIVSettingCL();
                });

                return setting;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    var kmeans = value.SettingCL as VSettingCLKMeans;
                    if (kmeans != null)
                    {
                        ComboBoxAlgoritm.Text = "K - средних";
                        TextBoxMaxDegreeOfParallelism.Text = kmeans.MaxDegreeOfParallelism.ToString();
                        TextBoxMaxItarations.Text = kmeans.MaxItarations.ToString();
                    }
                    else
                    {
                        var knp = value.SettingCL as VSettingCLKNP;
                        if (knp != null)
                        {
                            ComboBoxAlgoritm.Text = "Кр. незамкнутый путь";
                            TextBoxMaxDegreeOfParallelism.Text = knp.MaxDegreeOfParallelism.ToString();
                        }
                    }
                });
            }
        }

        public long SourceScale
        {
            get
            {
                long result = 0;
                Dispatcher.Invoke(() => { result = Convert.ToInt64(LabelSourceScale.Content); });
                return result;
            }
            set { Dispatcher.Invoke(() => { LabelSourceScale.Content = value.ToString(); }); }
        }

        public long DistScale
        {
            get
            {
                long result = 0;
                Dispatcher.Invoke(() => { result = Convert.ToInt64(TextBoxDistScale.Text); });
                return result;
            }
            set { Dispatcher.Invoke(() => { TextBoxDistScale.Text = value.ToString(); }); }
        }

        public bool IsRunningProgressBar
        {
            get
            {
                bool result = false;
                Dispatcher.Invoke(() => { result = ProgressBar.Visibility == Visibility.Visible; });
                return result;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    if (value)
                    {
                        ProgressBar.Visibility = Visibility.Visible;
                        ImageIsSuccess.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ProgressBar.Visibility = Visibility.Collapsed;
                        ImageIsSuccess.Visibility = Visibility.Visible;
                    }
                });
            }
        }

        public bool IsSuccess
        {
            get { return _isSuccess; }
            set
            {
                _isSuccess = value;
                ImageIsSuccess.Source = ConvertToImageSource(value ? ResourcesView.success : ResourcesView.error);
            }
        }

        public long Time
        {
            get
            {
                long result = 0;
                Dispatcher.Invoke(() => { result = Convert.ToInt64(LabelTime.Content);});
                return result;
            }
            set { Dispatcher.Invoke(() => { LabelTime.Content = value.ToString(); }); }
        }

        private bool _isSuccess = false;

        public TestCaseUserControl()
        {
            InitializeComponent();
            ComboBoxAlgoritm.SelectionChanged += ComboBoxAlgoritm_SelectionChanged;
        }

        private void ComboBoxAlgoritm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string algoritm = ComboBoxAlgoritm.SelectionBoxItem.ToString();
            TextBoxMaxItarations.Visibility = algoritm == "K - средних" ? Visibility.Collapsed : Visibility.Visible;
            LabelMaxItarations.Visibility = algoritm == "K - средних" ? Visibility.Collapsed : Visibility.Visible;
        }

        private ImageSource ConvertToImageSource(System.Drawing.Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private IVSettingCL GetIVSettingCL()
        {
            string algoritm = ComboBoxAlgoritm.SelectionBoxItem.ToString();
            switch (algoritm)
            {
                case "K - средних":
                {
                    VSettingCLKMeans setting = new VSettingCLKMeans
                    {
                        Seeding = VSeedings.Random,
                        MaxItarations = Convert.ToInt32(TextBoxMaxItarations.Text),
                        MaxDegreeOfParallelism = Convert.ToInt32(TextBoxMaxDegreeOfParallelism.Text)
                    };
                    return setting;
                }
                case "Кр. незамкнутый путь":
                {
                    VSettingCLKNP setting = new VSettingCLKNP
                    {
                        MaxDegreeOfParallelism = Convert.ToInt32(TextBoxMaxDegreeOfParallelism.Text)
                    };
                    return setting;
                }
                default:
                {
                    VSettingCLKMeans setting = new VSettingCLKMeans
                    {
                        Seeding = VSeedings.Random,
                        MaxItarations = Convert.ToInt32(TextBoxMaxItarations.Text),
                        MaxDegreeOfParallelism = Convert.ToInt32(TextBoxMaxDegreeOfParallelism.Text)
                    };
                    return setting;
                }
            }
        }
    }
}
