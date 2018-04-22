using System;
using System.Collections.Concurrent;
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
using MapGenCamera = MapGen.View.Source.Classes.MapGenCamera;
using System.IO;
using SharpGL.Enumerations;

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
                });
            }
        }

        /// <summary>
        /// Диспатчер главного окна.
        /// </summary>
        public Dispatcher MyDispatcher => Dispatcher;

        #endregion

        #region Region events.

        /// <summary>
        /// Событие выбора елемента View "Файл.База данных карт".
        /// </summary>
        public event Action MenuItemListMapsOnClick;

        /// <summary>
        /// Событие изменения масштаба.
        /// </summary>
        public event Action<int> ChangeScale;

        #endregion

        #region Region private fields.

        /// <summary>
        /// Карта для отрисовки.
        /// </summary>
        private GraphicMap _graphicMap;

        /// <summary>
        /// Флаг, отвечающий перерисовать ли карту.
        /// </summary>
        private bool _isDrawMap;
       
        /// <summary>
        /// Камера.
        /// </summary>
        private MapGenCamera _camera;

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
        private bool IsCheckToolBarr = true;

        /// <summary>
        /// Чекбокс для строки состояния.
        /// </summary>
        private bool IsCheckStatusBar = true;

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
            BindingEventsHeadButtonWindow();
            BindingEventMenuButtonWindow();
        }

        /// <summary>
        /// Инициализация private полей.
        /// </summary>
        private void InitializeFields()
        {
            _isDrawMap = false;
        }

        /// <summary>
        /// Подписка на собития кнопок заголовка главного окна.
        /// </summary>
        private void BindingEventsHeadButtonWindow()
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
        /// Подписка на события кнопок меню гавного окна.
        /// </summary>
        private void BindingEventMenuButtonWindow()
        {
            // Файл.
            MenuItemListMaps.Click += MenuItemListMaps_Click;

            // Вид.
            MenuItemToolBar.Click += MenuItemToolBar_Click;
            MenuItemStatusBar.Click += MenuItemStatusBar_Click;
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
        /// Отрисовка карты в главном окне.
        /// </summary>
        public void DrawSeaMap()
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
            _camera = new MapGenCamera
            {
                Target = new Vertex(0.0f, 0.0f, 0.0f),
                Position = new Vertex(0.0f, 0.0f, 1.0f),
                UpVector = new Vertex(0.0f, 1.0f, 0.0f)
            };
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
            _isDrawMap = true;
            if (_isDrawMap)
            {
                // Получаем ссылку на элемент управления OpenGL.
                OpenGL gl = args.OpenGL;

                // Очищает буфер кадра.
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                // Восстанавливает начальную точку системы координат.
                gl.LoadIdentity();

                // Поворот системы координат на 180 градусов вокруг оси X.
                gl.Rotate(180, 0, 0);

                // Обновляем взгляд камеры.
                _camera.Look(gl);

                //gl.PushMatrix();

                // Отображаем карту.
                _graphicMap?.Draw(gl, _xCoeff, _yCoeff);

                //gl.PopMatrix();
               
                // Запрещаем отрисовку.
                _isDrawMap = false;
            }
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
        /// Событие выбора елемента View "Файл.База данных карт".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemListMaps_Click(object sender, RoutedEventArgs e)
        {
            MenuItemListMapsOnClick?.Invoke();
        }

        /// <summary>
        /// Событие выбора елемента View "Вид.Панель иструментов".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemToolBar_Click(object sender, RoutedEventArgs e)
        {
            IsCheckToolBarr = !IsCheckToolBarr;
            MenuItemToolBar.IsChecked = IsCheckToolBarr;
            ToolBarRowDefinition.Height = IsCheckToolBarr ? new GridLength(30) : new GridLength(0);
        }

        /// <summary>
        /// Событие выбора елемента View "Вид.Строка состояния".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemStatusBar_Click(object sender, RoutedEventArgs e)
        {
            IsCheckStatusBar = !IsCheckStatusBar;
            MenuItemStatusBar.IsChecked = IsCheckStatusBar;
            StatusBarRowDefinition.Height = IsCheckStatusBar ? new GridLength(30) : new GridLength(0);
        }
        
        #endregion

        #region Region processing events of window.

        /// <summary>
        /// Обработка события нажатия клавиш.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A: // Движение камеры влево.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveLeftRight(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.D: // Движение камеры вправо.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.S: // Движение камеры вниз.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.W: // Движение камеры вверх.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.Q: // Движение камеры вперед.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
                case Key.E: // Движение камеры назад.
                {
                    OpenGL gl = OpenGlControl.OpenGL;
                    _camera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed));
                    _isDrawMap = true;
                    break;
                }
            }
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

        #endregion.

    }
}

