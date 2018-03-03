using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.General
{
    static class Methods
    {
        /// <summary>
        /// Извлечение сообщения ошибки из Exception.
        /// </summary>
        /// <param name="ex">Exception.</param>
        /// <returns>Сообщение.</returns>
        public static string CalcMessageException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex.Message;
        }
    }
}
