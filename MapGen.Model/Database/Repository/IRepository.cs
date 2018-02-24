using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.Database.Repository
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        void Create(T item);
        void Delete(long id);
    }
}
