using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Database.Repository
{
    public class MapsRepository : IRepository<Map>
    {

        #region Region private fields.

        private MapGenEntities _db;

        #endregion


        #region Region constructor.

        /// <summary>
        /// Инициализация репозитория карт по контексту.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public MapsRepository(MapGenEntities context)
        {
            this._db = context;
        }

        #endregion


        #region Region public methods.

        /// <summary>
        /// Получение всех карт из репозитория.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Map> GetAll()
        {
            return _db.Maps;
        }

        /// <summary>
        /// Добавление карты в репозиторий.
        /// </summary>
        /// <param name="map">Добавляемая карта.</param>
        public void Create(Map map)
        {
            _db.Maps.Add(map);
        }

        /// <summary>
        /// Удаление карты из репозитория по его id.
        /// </summary>
        /// <param name="id">ID удаляемой карты.</param>
        public void Delete(long id)
        {
            Map map = _db.Maps.Where(el => el.Idm == id).ToList().First();
            if (map != null)
                _db.Maps.Remove(map);
        }

        #endregion

    }
}
