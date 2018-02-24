using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.Database.Repository;

namespace MapGen.Model.Database
{
    public class UnitOfWork
    {

        #region Region private fields.

        private readonly MapGenEntities _db;
        private PointsRepository _pointsRepository;
        private MapsRepository _mapsRepository;
        private bool _disposed = false;

        #endregion


        #region Region properties.

        /// <summary>
        /// Репозиторий точек.
        /// </summary>
        public PointsRepository Points
        {
            get
            {
                if (_pointsRepository == null)
                    _pointsRepository = new PointsRepository(_db);
                return _pointsRepository;
            }
        }

        /// <summary>
        /// Репозиторий карт.
        /// </summary>
        public MapsRepository Maps
        {
            get
            {
                if (_mapsRepository == null)
                    _mapsRepository = new MapsRepository(_db);
                return _mapsRepository;
            }
        }

        #endregion


        #region Region constructor.

        /// <summary>
        /// Создает объект для работы с базой данных.
        /// </summary>
        public UnitOfWork()
        {
            _db = new MapGenEntities();
        }

        #endregion


        #region Region public methods.

        /// <summary>
        /// Сохранить изменения в базе данных.
        /// </summary>
        public void Save()
        {
            _db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this._disposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
