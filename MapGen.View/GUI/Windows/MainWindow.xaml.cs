using System;
using System.Collections.Concurrent;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Interfaces;
using SharpGL;
using SharpGL.SceneGraph;
using MapGenCamera = MapGen.View.Source.Classes.MapGenCamera;

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
        public GraphicMap GraphicMap { get; set; }

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
        /// Флаг, отвечающий перерисовать ли карту.
        /// </summary>
        private bool _isDrawMap;
       
        /// <summary>
        /// Камера.
        /// </summary>
        private MapGenCamera _camera;

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
        /// Инициализация OpenGL.
        /// </summary>
        private void InitOpenGl()
        {
            // Получаем ссылку на элемент управления OpenGl
            OpenGL gl = OpenGlControl.OpenGL;

            // Установка порта вывода в соотвествии с размерами элемента Screen.
            gl.Viewport(0, 0, (int)OpenGlControl.Width, (int)OpenGlControl.Height);

            // Настройка проекции. 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(1, OpenGlControl.Width, 1, OpenGlControl.Height);
            //gl.Perspective(120, (float)OpenGlControl.Width / (float)OpenGlControl.Height, 0.1, 200);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
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
            MenuItemListMaps.Click += MenuItemListMaps_OnClick;
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
            if (GraphicMap != null)
            {
                _isDrawMap = true;
            }
            else
            {
                IMessage message = new MessageWindow();
                message.ShowMessage("Процесс отрисовки карты", "Не загружена карта!", MessageButton.Ok, MessageType.Error);
            }
        }

        #endregion

        #region Region processing events of OpenGLControl.

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
            if (_isDrawMap)
            {
                // Получаемт ссылку на элемент управления OpenGL
                OpenGL gl = args.OpenGL;

                //  Очищает буфер кадра 
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                //  Восстанавливает начальную точку системы координат.
                gl.LoadIdentity();

                // Поворот системы координат на 180 градусов вокруг оси X.
                gl.Rotate(180, 0, 0);

                // Обновляем взгляд камеры.
                _camera.Look(gl);
                
                gl.PushMatrix();

                // Отображаем карту.
                GraphicMap?.DrawSurface(gl);

                gl.PopMatrix();

                gl.Flush();

                OpenGlControl.InvalidateVisual();

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
            _isDrawMap = true;
        }

        #endregion

        #region Region processing events of menu.

        /// <summary>
        /// Событие выбора елемента View "Файл.База данных карт".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemListMaps_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItemListMapsOnClick?.Invoke();
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
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
                case Key.D: // Движение камеры вправо.
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
                case Key.S: // Движение камеры вниз.
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
                case Key.W: // Движение камеры вверх.
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
                case Key.Q: // Движение камеры вперед.
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed));
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
                case Key.E: // Движение камеры назад.
                    {
                        OpenGL gl = OpenGlControl.OpenGL;
                        _camera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed));
                        //_camera.Look(gl);
                        _isDrawMap = true;
                        break;
                    }
            }
        }

        #endregion.

        #region Region private methods.



        #endregion.
    }
}

