using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Clustering.Algoritm.Kernel;
using Point = MapGen.Model.Database.EDM.Point;

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
        /// Долгота начала карты.
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Широта начала карты.
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Ширина карты (сек.).
        /// </summary>
        public long Width { get; set; }

        /// <summary>
        /// Длина карты (сек.).
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Масштаб карты (1 : Scale).
        /// </summary>
        public long Scale { get; set; }

        /// <summary>
        /// Облако точек карты.
        /// </summary>
        public Point[] CloudPoints { get; set; }

        #endregion

        #region Public methods.


        public void DrawToBMP(KMeansCluster[] clusters, string pathBMP)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)(Width + 1), (int)(Length + 1));
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            foreach (KMeansCluster cluster in clusters)
            {
                graphics.FillRectangle(new SolidBrush(Color.Blue), (int) cluster.MapGenCentroid.X, (int) cluster.MapGenCentroid.Y, 1, 1);
            }

            bitmap.Save(pathBMP);
        }

        #endregion

        #region Region cosntructor.

        /// <summary>
        /// Карта.
        /// </summary>
        /// <param name="name">Имя карты.</param>
        /// <param name="width">Ширина карты.</param>
        /// <param name="length">Длина карты.</param>
        /// <param name="scale">Масштаб.</param>
        /// <param name="latitude">Долгота начала карты.</param>
        /// <param name="longitude">Широта начала карты.</param>
        /// <param name="cloudPoints">Облако точек карты.</param>
        public DbMap(string name, long width, long length, long scale, string latitude, string longitude, Point[] cloudPoints)
        {
            Name = name;
            Width = width;
            Length = length;
            Scale = scale;
            Latitude = latitude;
            Longitude = longitude;
            CloudPoints = cloudPoints;
        }

        #endregion
    }
}
