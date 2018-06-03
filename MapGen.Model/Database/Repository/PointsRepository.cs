using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Database.Repository
{
    public class PointsRepository : IRepository<Point>
    {
        #region Region private fields.

        /// <summary>
        /// Контекст базы данных.
        /// </summary>
        private readonly MapGenEntities _db;

        #endregion
        
        #region Region constructor.

        /// <summary>
        /// Инициализация репозитория точек по контексту.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public PointsRepository(MapGenEntities context)
        {
            this._db = context;
        }

        #endregion
        
        #region Region public methods.

        /// <summary>
        /// Получение всех точек репозитория.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> GetAll()
        {
            return _db.Points;
        }

        /// <summary>
        /// Получение точек из репозитории по id карты.
        /// </summary>
        /// <param name="idMap">ID карты.</param>
        /// <returns>Точки из репозитория.</returns>
        public Point[] Get(int idMap)
        {
            return _db.Points.Where(point => point.Idm == idMap).ToArray();
        }

        /// <summary>
        /// Добавление точки в репозиторий.
        /// </summary>
        /// <param name="point">Добавляемая точка.</param>
        public void Create(Point point)
        {
            _db.Points.Add(point);
        }

        /// <summary>
        /// Добавление точки в репозиторий.
        /// </summary>
        /// <param name="point">Добавляемая точка.</param>
        public void Create(List<Point> points)
        {
            _db.Points.AddRange(points);
        }

        /// <summary>
        /// Удаление точки из репозитория по его id.
        /// </summary>
        /// <param name="id">ID удаляемой точки.</param>
        public void Delete(long id)
        {
            Point point = _db.Points.Where(el => el.Idp == id).ToList().First();
            if (point != null)
                _db.Points.Remove(point);
        }

        #endregion

    }
}
