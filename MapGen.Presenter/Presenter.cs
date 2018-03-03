using System.Collections.Generic;
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
        }

        /// <summary>
        /// Событие выбора элемента из списка карт базы данных. Созданиие регулярной матрицы.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        private void View_LoadDbMap(int idm)
        {
            RegMatrix regMatrix;
            string message;
            if (!_model.LoadDbMap(idm, out message))    // Загружаме карты из базы данных.
            {
                // Ошибка загрузки.
                return;
            }
            
            if (!_model.CreateRegMatrix(out regMatrix, out message))   // Строим регулярную матрицу глубин.
            {
                // Ошибка создания регулярной матрицы.
                return;
            }
            
            // Передаем в View регулярную матрицу для отображения.
            _view.RegMatrix = ConvertRegMatrixToRegMatrixView(regMatrix);
            
            // Отображаем карту.
            _view.DrawMap();
        }

        /// <summary>
        /// Конвертация регулярной матрицы Model в регулярную матрицу View.
        /// </summary>
        /// <param name="regMatrix">Регулярная матрица Model.</param>
        /// <returns>Регулярная матрица View.</returns>
        private RegMatrixView ConvertRegMatrixToRegMatrixView(RegMatrix regMatrix)
        {
            RegMatrixView result = new RegMatrixView()
            {
                Step = regMatrix.Step,
                Width = regMatrix.Width,
                Length = regMatrix.Length,
                MaxDepth = regMatrix.MaxDepth,
                Points = regMatrix.Points
            };

            return result;
        }

        #endregion

    }
}
