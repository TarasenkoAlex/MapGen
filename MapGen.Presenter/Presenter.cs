using System.Collections.Generic;
using System.Threading;
using MapGen.Model;
using MapGen.View.Source.Interfaces;
using MapGen.View.Source.Classes;
using MapGen.Model.RegMatrix;

namespace MapGen.Presenter
{
    public class Presenter
    {
        #region Region private fields.

        /// <summary>
        /// Объект View.
        /// </summary>
        private readonly IView _view;

        /// <summary>
        /// Объект Model.
        /// </summary>
        private readonly IModel _model;

        #endregion
        
        #region Region properties.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        public object MainWindow => _view.MainWindow;

        #endregion
        
        #region Region constructor.

        /// <summary>
        /// Создание объекта Presenter.
        /// </summary>
        /// <param name="model">Объект Model.</param>
        /// <param name="view">Объект View.</param>
        public Presenter(IModel model, IView view)
        {
            // Инициализация Model и View.
            _model = model;
            _view = view;

            // Инициализация полей Presenter.
            InitializeFields();

            // Подписка на события View.
            BindingEventsOfView();

            // Подписка на события Model.
            BindingEventsOfModel();

            // Открываем главное окно программы.
            _view.ShowMainWindow();
        }

        #endregion
        
        #region Region private methods.

        /// <summary>
        /// Инициализация полей Presenter.
        /// </summary>
        private void InitializeFields()
        {
            
        }

        /// <summary>
        /// Подписка событий View.
        /// </summary>
        private void BindingEventsOfView()
        {
            _view.LoadDbMap += View_LoadDbMap;
            _view.MenuItemListMapsOnClick += View_MenuItemListMapsOnClick;
        }

        /// <summary>
        /// Подписка событий Model.
        /// </summary>
        private void BindingEventsOfModel()
        {
        }

        /// <summary>
        /// Обработка события загрузки списка карт из базы данных.
        /// </summary>
        private void View_MenuItemListMapsOnClick()
        {
            new Thread(() =>
                {
                    string message;
                    // Подключаемся к базе данных.
                    if (!_model.ConnectToDatabase(out message))
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось подключиться к базе данных! {message}");
                        return;
                    }

                    List<string[]> maps;
                    if (_model.GetDbMaps(out maps, out message))
                    {
                        _view.ShowTableDbMaps(maps);
                    }
                    else
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось загрузить список карт! {message}");
                    }
                })
                {IsBackground = true}.Start();
        }

        /// <summary>
        /// Событие выбора элемента из списка карт базы данных. Созданиие регулярной матрицы.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        private void View_LoadDbMap(int idm)
        {
            new Thread(() =>
                { 
                    string message;
                    // Загружаем карту из базы данных.
                    if (!_model.LoadDbMap(idm, out message))
                    {
                        _view.ShowMessageError("Загрузка базы данных", $"Не удалось загрузить карту из базы данных! {message}");
                        return;
                    }

                    RegMatrix regMatrix;
                    // Строим регулярную матрицу глубин.
                    if (!_model.CreateRegMatrix(_model.SeaMap.Scale, out regMatrix, out message))
                    {
                        _view.ShowMessageError("Создание регулярной матрицы", $"Не удалось создать регулярную матрицу глубин! {message}");
                        return;
                    }

                    // Конвертируем регулярную матрицу в карту для отрисовки. Передаем во View.
                    _view.GraphicMap = ConvertRegMatrixToGraphicMap(regMatrix, _model.SeaMap.Scale);

                    // Отображаем карту.
                    _view.DrawSeaMap();
                })
                {IsBackground = true}.Start();
        }

        /// <summary>
        /// Конвертация регулярной матрицы в карту для отрисовки.
        /// </summary>
        /// <param name="regMatrix">Регулярная матрица Model.</param>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <returns>Карта для отрисовки.</returns>
        private GraphicMap ConvertRegMatrixToGraphicMap(RegMatrix regMatrix, long scale)
        {
            GraphicMap graphicMap = new GraphicMap
            {
                Scale = scale,
                Width = regMatrix.Width,
                Length = regMatrix.Length,
                MaxDepth = regMatrix.MaxDepth,
                Points = new Point3dColor[regMatrix.Points.Length]
            };

            DrawingObjects.DepthScale depthScale = new DrawingObjects.DepthScale(graphicMap.MaxDepth);

            for (int i = 0; i < regMatrix.Length; ++i)
            {
                for (int j = 0; j < regMatrix.Width; ++j)
                {
                    graphicMap.Points[i * regMatrix.Width + j] = new Point3dColor
                    {
                        X = regMatrix.Step * j,
                        Y = regMatrix.Step * i,
                        Depth = regMatrix.Points[i * regMatrix.Width + j],
                        Color = depthScale.GetColorDepth(regMatrix.Points[i * regMatrix.Width + j])
                };
                }
            }

            return graphicMap;
        }

        #endregion
    }
}
