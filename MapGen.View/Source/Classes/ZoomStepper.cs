using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes
{
    public class ZoomStepper
    {
        /// <summary>
        /// Все рассматриваемые масштабы.
        /// </summary>
        private readonly List<int> _enableScale = new List<int> { 10000, 25000, 50000, 100000, 200000, 300000, 500000, 1000000 };

        /// <summary>
        /// Поиск индекса масштаба.
        /// </summary>
        /// <param name="scale">Масштаб.</param>
        /// <returns></returns>
        public int FindIndex(long scale)
        {
            return _enableScale.FindIndex(s => s == scale);
        }

        /// <summary>
        /// Получить следующий масштаб.
        /// </summary>
        /// <param name="scale">Текущий масштаб.</param>
        /// <returns></returns>
        public int NextScale(long scale)
        {
            int currIndex = _enableScale.FindIndex(el => el == scale);
            return _enableScale[currIndex + 1];
        }

        /// <summary>
        /// Получить предыдущий масштаб.
        /// </summary>
        /// <param name="scale">Текущий масштаб.</param>
        /// <returns></returns>
        public int PrevScale(long scale)
        {
            int currIndex = _enableScale.FindIndex(el => el == scale);
            return _enableScale[currIndex - 1];
        }
    }
}
