using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Maps
{
    /// <summary>
    /// Класс для хранения исходных данных о карте из базы данных.
    /// </summary>
    public class DbMap
    {

        #region Region properties.

        /// <summary>
        /// Имя карты.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ширина карты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Длина карты.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Масштаб карты (1 : Scale).
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Облако точек карты.
        /// </summary>
        public Point[] CloudPoints { get; set; }

        #endregion


        #region Region cosntructor.

        /// <summary>
        /// Карта.
        /// </summary>
        /// <param name="name">Имя карты.</param>
        /// <param name="width">Ширина карты.</param>
        /// <param name="length">Длина карты.</param>
        /// <param name="scale">Масштаб.</param>
        /// <param name="cloudPoints">Облако точек карты.</param>
        public DbMap(string name, int width, int length, int scale, Point[] cloudPoints)
        {
            Name = name;
            Width = width;
            Length = length;
            Scale = scale;
            CloudPoints = cloudPoints;
        }

        #endregion


        #region Region public methods.



        #endregion

    }
}
