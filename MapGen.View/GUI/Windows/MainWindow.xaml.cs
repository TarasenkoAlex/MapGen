using System;
using System.Collections.Concurrent;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;
using SharpGL;
using SharpGL.SceneGraph;

namespace MapGen.View.GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {

        #region Region properties.

        public Window OwnerWindow
        {
            set { Owner = value; }
        }

        public Window Window => this;

        public RegMatrixView RegMatrix
        {
            set
            {
                _regMatrix = value;
                _surfaceMaker = new DrawingObjects.SurfaceMaker(value.MaxDepth);
            }
        }

        #endregion


        #region Region public events.

        public event Action MenuItemListMapsOnClick;

        #endregion


        #region Region public methods.

        public void ShowMainWindow()
        {
            Show();
        }

        public void CreateTriangleCollectionMap()
        {
            if (_regMatrix != null)
                _triangleCollecton = _triangleCollectionMaker.CreateTriangleCollectionMap(_regMatrix);
        }

        public void ShowLoadingMap()
        {
            _isDrawMap = true;
        }

        #endregion


        #region Region private fields.

        private bool _isDrawMap;

        private RegMatrixView _regMatrix;

        private TriangleCollectionMaker _triangleCollectionMaker;

        private ConcurrentBag<DrawingObjects.Triangle> _triangleCollecton;

        private DrawingObjects.SurfaceMaker _surfaceMaker;

        private Camera _camera;

        private Timer _cameraTimer;

        #endregion


        #region Region constructer.

        public MainWindow()
        {
            InitializeComponent();
            InitializeFields();
            InitOpenGL();
            BindingEventsHeadButtonWindow();
            BindingEventMenuButtonWindow();
        }

        #endregion


        #region Region private methods.

        private void InitializeFields()
        {
            _isDrawMap = false;
            _triangleCollectionMaker = new TriangleCollectionMaker();
            _triangleCollecton = new ConcurrentBag<DrawingObjects.Triangle>();

            //ElapsedEventHandler threadCameraTimer = EventCameraTimer;
            //_cameraTimer.Elapsed += threadCameraTimer;
            //_cameraTimer.Interval = 500;
            //_cameraTimer.Start();
        }

        private void EventCameraTimer(object source, ElapsedEventArgs e)
        {
            // Обновляем взгляд камеры.
            OpenGL gl = OpenGlControl.OpenGL;
            _camera.Look(ref gl);
        }

        private void BindingEventMenuButtonWindow()
        {
            MenuItemListMaps.Click += MenuItemListMaps_OnClick;
        }

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
            ButtonClose.Click += (s, e) => Close();
        }

        #endregion


        #region Region events OpenGLControl.

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  получаем ссылку на окно OpenGL 
            OpenGL gl = args.OpenGL;


            //  Задаем цвет очистки экрана
            gl.ClearColor(0, 0, 0, 0);
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            if (_isDrawMap)
            {
                // Получаемт ссылку на элемент управления OpenGL
                OpenGL gl = args.OpenGL;

                //  Очищает буфер кадра 
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                //  Восстанавливает начальную точку сисетмы координат.
                gl.LoadIdentity();

                //// Обновляем взгляд камеры.
                _camera.Look(ref gl);

                gl.PushMatrix();

                // Отображаем карту.
                _surfaceMaker.DrawSurface(gl, ref _triangleCollecton);

                gl.PopMatrix();

                // Запрещаем отрисовку.
                _isDrawMap = false;
            }
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {

        }

        #endregion


        #region Region events menu.

        private void MenuItemListMaps_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItemListMapsOnClick?.Invoke();
        }

        #endregion


        #region Region private methods.

        /// <summary>
        /// Инициализация OpenGL.
        /// </summary>
        private void InitOpenGL()
        {
            // Получаемт ссылку на элемент управления OpenGL
            OpenGL gl = OpenGlControl.OpenGL;

            //// инициализация Glut 
            ////gl.glutInit();
            ////gl.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            //// очитка окна 
            //gl.ClearColor(255, 255, 255, 1);

            //// установка порта вывода в соотвествии с размерами элемента Screen 
            //gl.Viewport(0, 0, (int)OpenGlControl.Width, (int)OpenGlControl.Height);


            //// настройка проекции 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(120, (float)OpenGlControl.Width / (float)OpenGlControl.Height, 0.1, 200);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            //// настройка параметров OpenGL для визуализации 
            //gl.Enable(OpenGL.GL_DEPTH_TEST);

            _camera = new Camera(0.0f, 0.0f, 0.75f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveLeftRight(-float.Parse(ResourcesView.MoveSpeed));
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.D:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.S:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.W:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.Q:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed) / 10);
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.E:
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed) / 10);
                        _camera.Look(ref gl);
                        //_isDrawMap = true;
                        break;
                    }
            }
        }

        #endregion.


    }
}
