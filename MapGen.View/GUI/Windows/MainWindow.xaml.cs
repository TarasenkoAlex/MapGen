using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;
using SharpGL;
using SharpGL.SceneGraph;
using MGCamera = MapGen.View.Source.Classes.MGCamera;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpGL.Enumerations;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMain
    {
        #region Region properties.

        /// <summary>
        /// Карта для отрисовки.
        /// </summary>
        public GraphicMap GraphicMap
        { 
            set
            {
                Dispatcher.Invoke(() =>
                {
                    _graphicMap = value;

                    LabelNameMap.Content = value.Name;
                    LabelLatitude.Content = value.Latitude;
                    LabelLongitude.Content = value.Longitude;
                    LabelWidth.Content = value.Width - 1;
                    LabelLength.Content = value.Length - 1;
                    LabelScale.Content = $"1:{value.Scale}";
                    LabelCountPoints.Content = value.CountSourcePoints;

                    RefreshEnableButtonOfZoom();
                    RefreshDepthScalePanel();
                });
            }
        }

        /// <summary>
        /// Диспатчер главного окна.
        /// </summary>
        public Dispatcher MyDispatcher => Dispatcher;

        /// <summary>
        /// Запустить или остановить прогресс-бар.
        /// </summary>
        public bool IsRunningProgressBar
        {
            set
            {
                Dispatcher.Invoke(() => { ProgressBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed; });
            }
        }

        #endregion

        #region Region events.

        /// <summary>
        /// Событие выбора элемента View "Файл.База данных карт".
        /// </summary>
        public event Action MenuItemListMapsClick;

        /// <summary>
        /// Событие выбора элемента View "Сервис.Настройки.Интерполяция".
        /// </summary>
        public event Action MenuItemSettingsInterpolClick;

        /// <summary>
        /// Событие выбора элемента View "Сервис.Настройки.Генерализация".
        /// </summary>
        public event Action MenuItemSettingsGenClick;

        /// <summary>
        /// Событие изменения масштаба.
        /// </summary>
        public event Action<int> ZoomEvent;

        #endregion

        #region Region private fields.

        /// <summary>
        /// Все рассматриваемые масштабы.
        /// </summary>
        private readonly List<int> _enableScale = new List<int> {10000, 25000, 50000, 100000, 200000, 300000, 500000, 1000000};

        /// <summary>
        /// Карта для отрисовки.
        /// </summary>
        private GraphicMap _graphicMap;

        /// <summary>
        /// Флаг, отвечающий перерисовать ли карту.
        /// </summary>
        private bool _isDrawMap;
       
        /// <summary>
        /// Текущая камера.
        /// </summary>
        private MGCamera _currentCamera;

        /// <summary>
        /// Исходная камера.
        /// </summary>
        private readonly MGCamera _initialCamera = new MGCamera
        {
            Target = new Vertex(0.0f, 0.0f, 0.0f),
            Position = new Vertex(0.0f, 0.0f, 1.0f),
            UpVector = new Vertex(0.0f, 1.0f, 0.0f)
        };

        /// <summary>
        /// Коэффициент сжатия по X.
        /// </summary>
        private double _xCoeff = 1.0d;

        /// <summary>
        /// Коэффициент сжатия по Y.
        /// </summary>
        private double _yCoeff = 1.0d;

        /// <summary>
        /// Чекбокс для тулбара.
        /// </summary>
        private bool _isCheckToolBar = true;

        /// <summary>
        /// Чекбокс для строки состояния.
        /// </summary>
        private bool _isCheckStatusBar = true;

        /// <summary>
        /// Класс для определения следующего и предыдущего масштаба.
        /// </summary>
        private readonly ZoomStepper _zoomStepper = new ZoomStepper();

        /// <summary>
        /// Настройка отображения карты.
        /// </summary>
        private readonly SettingGraphicMap _settingGraphicMap = new SettingGraphicMap();

        #endregion

        #region Region constructer.

        /// <summary>
        /// Создает объект главного окна программы.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeFields();
            InitOpenGl();
            SubscribeEventsButtonHeadWindow();
            SubscribeEventButtonMenu();
            SubscribeEventButtonWindow();
        }

        /// <summary>
        /// Инициализация private полей.
        /// </summary>
        private void InitializeFields()
        {
            
        }

        /// <summary>
        /// Подписка на собития кнопок заголовка главного окна.
        /// </summary>
        private void SubscribeEventsButtonHeadWindow()
        {
            // Обработка кнопки свернуть.
            ButtonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;

            // Обработка кнопки развернуть (восстановить).
            ButtonMaximize.Click += (s, e) =>
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    ButtonMaximize.ToolTip = "Развернуть";
                    _isDrawMap = true;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                    ButtonMaximize.ToolTip = "Восстановить";
                    _isDrawMap = true;
                }
            };

            // Обработка кнопки закрытия.
            ButtonClose.Click += (s, e) =>
            {
                Close();
            };
        }

        /// <summary>
        /// Подписка на события кнопок меню главного окна.
        /// </summary>
        private void SubscribeEventButtonMenu()
        {
            // Файл.
            MenuItemListMaps.Click += MenuItemListMaps_Click;
            MenuItemExit.Click += MenuItemExit_Click;

            // Сервис.
            MenuItemSettingsInterpol.Click += MenuItemSettingsInterpol_Click;
            MenuItemSettingsGen.Click += MenuItemSettingsGen_Click;

            // Вид.
            MenuItemToolBar.Click += MenuItemToolBar_Click;
            MenuItemStatusBar.Click += MenuItemStatusBar_Click;
        }

        /// <summary>
        /// Подписка на события оставшихся кнопок окна.
        /// </summary>
        private void SubscribeEventButtonWindow()
        {
            // Приблизить.
            ButtonZoomPlus.Click += ButtonZoomPlus_Click;

            // Отдалить. 
            ButtonZoomMinus.Click += ButtonZoomMinus_Click;
            
            // Отрисовка содержимого карты (вкл / выкл).
            ButtonLockUnlockDrawData.Click += ButtonLockUnlockDrawData_Click;

            // Отрисовка краев карты (вкл / выкл).
            ButtonLockUnlockDrawStripsEdge.Click += ButtonLockUnlockDrawStripsEdge_Click;

            // Отрисовка линий широт и долготы (вкл / выкл).
            ButtonLockUnlockDrawGrid.Click += ButtonLockUnlockDrawGrid_Click;

            // Отрисовка исходных точек карты (вкл / выкл).
            ButtonLockUnlockDrawSourcePoints.Click += ButtonLockUnlockDrawSourcePoints_Click;

            // Вернуть камеру в исходное положение.
            ButtonInitialCamera.Click += ButtonInitialCamera_Click;
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Открыть главное окно.
        /// </summary>
        public void ShowMainWindow()
        {
            Show();
        }

        /// <summary>
        /// Отрисовка карты в главном окне с выставлением камеры на начальное положение.
        /// </summary>
        public void DrawSeaMapWithResetCamera()
        {
            if (_graphicMap != null)
            {
                _currentCamera = (MGCamera)_initialCamera.Clone();
                _isDrawMap = true;
            }
            else
            {
                MessageBox.Show("Не загружена карта!", "Процесс отрисовки карты", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Отрисовка карты в главном окне без выставлением камеры на начальное положение.
        /// </summary>
        public void DrawSeaMapWithoutResetCamera()
        {
            if (_graphicMap != null)
            {
                _isDrawMap = true;
            }
            else
            {
                MessageBox.Show("Не загружена карта!", "Процесс отрисовки карты", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Region OpenGL.

        /// <summary>
        /// Инициализация OpenGL.
        /// </summary>
        private void InitOpenGl()
        {
            // Получаем ссылку на элемент управления OpenGl.
            OpenGL gl = OpenGlControl.OpenGL;

            // Установка порта вывода в соотвествии с размерами элемента Screen.
            gl.Viewport(0, 0, (int)OpenGlControl.ActualWidth, (int)OpenGlControl.ActualHeight);

            // Настройка проекции. 
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            gl.Ortho2D(1, OpenGlControl.ActualWidth, 1, OpenGlControl.ActualHeight);
            //gl.Perspective(120, (float)OpenGlControl.Width / (float)OpenGlControl.Height, 0.1, 200);
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();

            // Настройка параметров OpenGl для визуализации.
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            // Инициализация камеры.
            _currentCamera = (MGCamera)_initialCamera.Clone();
        }

        /// <summary>
        /// Инициализация OpenGl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  получаем ссылку на окно OpenGL 
            OpenGL gl = args.OpenGL;

            //  Задаем цвет очистки экрана
            gl.ClearColor(0, 0, 0, 0);
        }

        /// <summary>
        /// Обработка события отрисовки в OpenGLControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //if (_isDrawMap)
            //{
                // Получаем ссылку на элемент управления OpenGL.
                OpenGL gl = args.OpenGL;

                // Очищает буфер кадра.
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                // Восстанавливает начальную точку системы координат.
                gl.LoadIdentity();

                // Поворот системы координат на 180 градусов вокруг оси X.
                gl.Rotate(180, 0, 0);

                // Обновляем взгляд камеры.
                _currentCamera.Look(gl);

                //gl.PushMatrix();

                // Отображаем карту.
                _graphicMap?.Draw(gl, _xCoeff, _yCoeff, _settingGraphicMap);

                //gl.PopMatrix();
               
                // Запрещаем отрисовку.
                _isDrawMap = false;
            //}
        }

        /// <summary>
        /// Обработка события изменения размера OpenGLControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            CalculateXYCoeffs();
            _isDrawMap = true;
        }

        /// <summary>
        /// Вычисление коеффициентов сжатия.
        /// </summary>
        private void CalculateXYCoeffs()
        {
            if (OpenGlControl.ActualWidth > OpenGlControl.ActualHeight)
            {
                _xCoeff = 1.0d;
                _yCoeff = OpenGlControl.ActualWidth / OpenGlControl.ActualHeight;
            }
            else
            {
                _xCoeff = OpenGlControl.ActualHeight / OpenGlControl.ActualWidth;
                _yCoeff = 1.0d;
            }
        }

        #endregion

        #region Region processing events of menu.

        /// <summary>
        /// Событие выбора элемента View "Файл.Выход".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие выбора элемента View "Файл.База данных карт".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemListMaps_Click(object sender, RoutedEventArgs e)
        {
            MenuItemListMapsClick?.Invoke();
        }

        /// <summary>
        /// Событие выбора элемента View "Вид.Панель иструментов".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemToolBar_Click(object sender, RoutedEventArgs e)
        {
            _isCheckToolBar = !_isCheckToolBar;
            MenuItemToolBar.IsChecked = _isCheckToolBar;
            ToolBarRowDefinition.Height = _isCheckToolBar ? new GridLength(30) : new GridLength(0);
        }

        /// <summary>
        /// Событие выбора элемента View "Вид.Строка состояния".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemStatusBar_Click(object sender, RoutedEventArgs e)
        {
            _isCheckStatusBar = !_isCheckStatusBar;
            MenuItemStatusBar.IsChecked = _isCheckStatusBar;
            StatusBarRowDefinition.Height = _isCheckStatusBar ? new GridLength(30) : new GridLength(0);
        }

        /// <summary>
        /// Событие выбора элемента View "Сервис.Настройки.Интерполяция".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSettingsInterpol_Click(object sender, RoutedEventArgs e)
        {
            MenuItemSettingsInterpolClick?.Invoke();
        }

        /// <summary>
        /// Событие выбора элемента View "Сервис.Настройки.Генерализация".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSettingsGen_Click(object sender, RoutedEventArgs e)
        {
            MenuItemSettingsGenClick?.Invoke();
        }

        #endregion

        #region Region processing events.

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A: // Движение камеры влево.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveLeftRight(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.D: // Движение камеры вправо.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.S: // Движение камеры вниз.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.W: // Движение камеры вверх.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Q: // Движение камеры назад.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.E: // Движение камеры вперед.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Left: // Движение камеры влево.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveLeftRight(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Right: // Движение камеры вправо.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Down: // Движение камеры вниз.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Up: // Движение камеры вверх.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.OemMinus: // Движение камеры назад.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.OemPlus: // Движение камеры вперед.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _currentCamera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
            }
        }

        private void ButtonZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            if (_graphicMap != null)
            {
                ZoomEvent?.Invoke(_zoomStepper.PrevScale(_graphicMap.Scale));
            }
        }

        private void ButtonZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            if (_graphicMap != null)
            {
                ZoomEvent?.Invoke(_zoomStepper.NextScale(_graphicMap.Scale));
            }
        }

        private void ButtonLockUnlockDrawSourcePoints_Click(object sender, RoutedEventArgs e)
        {
            if (_settingGraphicMap.IsDrawSourcePointsOfMap)
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawSourcePointsOfMap = false;
                InitImageOnImage(ImageLockUnlockDrawSourcePoints, ResourcesView.window_opengl_points_lock);
            }
            else
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawSourcePointsOfMap = true;
                InitImageOnImage(ImageLockUnlockDrawSourcePoints, ResourcesView.window_opengl_points_unlock);
            }
        }

        private void ButtonLockUnlockDrawGrid_Click(object sender, RoutedEventArgs e)
        {
            if (_settingGraphicMap.IsDrawGridLatitudeAndLongitude)
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawGridLatitudeAndLongitude = false;
                InitImageOnImage(ImageLockUnlockDrawGrid, ResourcesView.window_opengl_grid_lock);
            }
            else
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawGridLatitudeAndLongitude = true;
                InitImageOnImage(ImageLockUnlockDrawGrid, ResourcesView.window_opengl_grid_unlock);
            }
        }

        private void ButtonLockUnlockDrawStripsEdge_Click(object sender, RoutedEventArgs e)
        {
            if (_settingGraphicMap.IsDrawStripsEdgeOfMap)
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawStripsEdgeOfMap = false;
                InitImageOnImage(ImageLockUnlockDrawStripsEdge, ResourcesView.window_opengl_stripsedge_lock);
            }
            else
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawStripsEdgeOfMap = true;
                InitImageOnImage(ImageLockUnlockDrawStripsEdge, ResourcesView.window_opengl_stripsedge_unlock);
            }
        }

        private void ButtonLockUnlockDrawData_Click(object sender, RoutedEventArgs e)
        {
            if (_settingGraphicMap.IsDrawDataMap)
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawDataMap = false;
                InitImageOnImage(ImageLockUnlockDrawData, ResourcesView.window_opengl_data_lock);
            }
            else
            {
                _isDrawMap = true;
                _settingGraphicMap.IsDrawDataMap = true;
                InitImageOnImage(ImageLockUnlockDrawData, ResourcesView.window_opengl_data_unlock);
            }
        }

        private void ButtonInitialCamera_Click(object sender, RoutedEventArgs e)
        {
            _currentCamera = (MGCamera)_initialCamera.Clone();
            _isDrawMap = true;
        }

        #endregion

        #region Region private methods.

        /// <summary>
        /// Обновление доступности кнопок масштабирования.
        /// </summary>
        private void RefreshEnableButtonOfZoom()
        {
            int findIndex = _zoomStepper.FindIndex(_graphicMap.Scale);
            if (findIndex == 0)
            {
                ButtonZoomMinus.IsEnabled = true;
                ButtonZoomPlus.IsEnabled = false;
            } 
            else if (findIndex == _enableScale.Count - 1)
            {
                ButtonZoomMinus.IsEnabled = false;
                ButtonZoomPlus.IsEnabled = true;
            }
            else
            {
                ButtonZoomMinus.IsEnabled = true;
                ButtonZoomPlus.IsEnabled = true;
            }
        }

        /// <summary>
        /// Обновление шкалы глубин.
        /// </summary>
        private void RefreshDepthScalePanel()
        {
            DrawingObjects.DepthScale depthScale = new DrawingObjects.DepthScale(_graphicMap.MaxDepth);
            double[] depths = depthScale.GetDepthScale();
            if (depths.Length != 0)
            {
                LabelDScale0.Content = "0";
                LabelDScale1.Content = depths[0];
                LabelDScale2.Content = depths[1];
                LabelDScale3.Content = depths[2];
                LabelDScale4.Content = depths[3];
                LabelDScale5.Content = depths[4];
                LabelDScale6.Content = depths[5];
                LabelDScale7.Content = depths[6];
                LabelDScale8.Content = "глубже";
            }
        }

        /// <summary>
        /// Инициализация Image другой картинкой.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="source"></param>
        private void InitImageOnImage(System.Windows.Controls.Image image, Bitmap source)
        {
            Dispatcher.Invoke(() =>
            {
                IntPtr hsource = source.GetHbitmap();
                ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hsource,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                image.Source = wpfBitmap;
            });
        }

        private Bitmap GetSnapShot(int width, int height)
        {
            OpenGL gl = OpenGlControl.OpenGL;
            var snapShotBmp = new Bitmap(width * 10, height * 10);
            BitmapData bmpData = snapShotBmp.LockBits(
                new Rectangle(0, 0, width * 10, height * 10),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            gl.ReadPixels(0, 0, width * 10, height * 10, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);
            snapShotBmp.UnlockBits(bmpData);
            return snapShotBmp;
        }
        
        #endregion

    }
}

