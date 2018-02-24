using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model;
using MapGen.Model.Database;
using MapGen.Model.Database.EDM;
using MapGen.Model.Maps;
using MapGen.View.Source.Interfaces;
using MapGen.View.Source.Classes;
using MapGen.Model.RegMatrix;

namespace MapGen.Presenter
{
    public class Presenter
    {

        #region Region private fields.

        private IView _view;

        /// <summary>
        /// Словарь с регулярными матрицами карт, где ключ - это масштаб.
        /// </summary>
        private Dictionary<int, RegMatrix> _seaMaps;

        /// <summary>
        /// Исходная загруженная карта.
        /// </summary>
        private DbMap _seaMap;

        /// <summary>
        /// Создает регулряную матрицу по облаку точек.
        /// </summary>
        private RegMatrixMaker _regMatrixMaker;

        #endregion


        #region Region properties.

        /// <summary>
        /// Главное окно программы.
        /// </summary>
        public object MainWindow => _view.MainWindow;

        #endregion


        #region Region constructor.

        public Presenter()
        {
            // Инициализация полей Presenter.
            InitializeFields();
            // Подписка на события View.
            BindingEventsView();
            // Открываем главное окно программы.
            _view.ShowMainWindow();
        }

        #endregion


        #region Region private methods.

        private void InitializeFields()
        {
            _seaMaps = new Dictionary<int, RegMatrix>();
            _regMatrixMaker = new RegMatrixMaker();
            _view = new Viewer();
        }

        private void BindingEventsView()
        {
            _view.ShowMapsOnClick += View_ShowMapsOnClick;
            _view.LoadMaps += View_LoadMapsAndShowOnView;
            _view.LoadMap += View_LoadMapAndShowOnView;
        }

        private void View_ShowMapsOnClick()
        {
            _view.ShowTableMaps();
        }

        private void View_LoadMapsAndShowOnView()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            var uowMaps = unitOfWork.Maps.GetAll();
            List<string[]> maps = new List<string[]>();
            foreach (Map map in uowMaps)
            {
                maps.Add(new[]
                {
                    map.Idm.ToString(),
                    map.Name,
                    map.Width.ToString(),
                    map.Length.ToString(),
                    map.Scale.ToString()
                });
            }
            _view.Maps = maps;
        }

        private void View_LoadMapAndShowOnView(string[] map)
        {
            int idMap = int.Parse(map[0]);
            string nameMap = map[1];
            int widthMap = int.Parse(map[2]);
            int lengthMap = int.Parse(map[3]);
            int scaleMap = int.Parse(map[4]);

            // Считываем облако точек для текущей карты.
            UnitOfWork unitOfWork = new UnitOfWork();
            Point[] cloudPoints = unitOfWork.Points.Get(idMap);

            // Сохраняем исходную карту.
            _seaMap = new DbMap(nameMap, widthMap, lengthMap, scaleMap, cloudPoints);

            // Зачищаме буфер хранения карт по масштабам.
            _seaMaps.Clear();

            // Строим регулярную матрицу глубин с шагом по умолчанию 1.0.
            RegMatrix regMatrix = _regMatrixMaker.CreateRegMatrix();
            _seaMaps.Add(scaleMap, regMatrix);

            // Передавем в View регулярную матрицу для отображения.
            _view.RegMatrix = ConvertRegMatrixToRegMatrixView(ref regMatrix);

            // Отображаем карту.
            _view.ShowLoadingMap();
        }

        private RegMatrixView ConvertRegMatrixToRegMatrixView(ref RegMatrix regMatrix)
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
