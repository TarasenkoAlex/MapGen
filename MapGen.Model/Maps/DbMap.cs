using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
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

        private const int CoeffDraw = 6;
        private const int Distance = 60;

        /// <summary>
        /// Отрисовка точек карты в bmp изображение и отображением кластеров.
        /// </summary>
        public void DrawToBMP(Cluster[] clusters, string pathBMP)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                (int)(Width + 1) * (CoeffDraw + Distance) + Distance,
                (int)(Length + 1) * (CoeffDraw + Distance) + Distance);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            // Выставляем фон изображения.
            graphics.Clear(Color.White);

            foreach (Cluster cluster in clusters)
            {
                foreach (int pointIndex in cluster)
                {
                    // Отрисовка точки.
                    graphics.FillRectangle(new SolidBrush(Color.Black),
                        (int) CloudPoints[pointIndex].X * (CoeffDraw + Distance) + Distance,
                        (int) CloudPoints[pointIndex].Y * (CoeffDraw + Distance) + Distance,
                        CoeffDraw, CoeffDraw);

                    // Отрисовка надписи с глубиной.
                    graphics.DrawString(Math.Round(CloudPoints[pointIndex].Depth, 1).ToString(CultureInfo.InvariantCulture),
                        new Font("Arial", 10),
                        new SolidBrush(Color.Black),
                        (int) CloudPoints[pointIndex].X * (CoeffDraw + Distance) + Distance + CoeffDraw / 2,
                        (int) CloudPoints[pointIndex].Y * (CoeffDraw + Distance) + Distance + CoeffDraw / 2);

                    // Отрисока линии, соединяющей текущую точку с центром кластера.
                    graphics.DrawLine(
                        new Pen(Color.Blue),
                        (int)CloudPoints[cluster.MapGenCentroid].X * (CoeffDraw + Distance) + Distance + CoeffDraw / 2,
                        (int)CloudPoints[cluster.MapGenCentroid].Y * (CoeffDraw + Distance) + Distance + CoeffDraw / 2,
                        (int)CloudPoints[pointIndex].X * (CoeffDraw + Distance) + Distance + CoeffDraw / 2,
                        (int)CloudPoints[pointIndex].Y * (CoeffDraw + Distance) + Distance + CoeffDraw / 2);
                }

                // Отрисовка центра кластера.
                graphics.FillRectangle(new SolidBrush(Color.Black),
                    (int)CloudPoints[cluster.MapGenCentroid].X * (CoeffDraw + Distance) + Distance,
                    (int)CloudPoints[cluster.MapGenCentroid].Y * (CoeffDraw + Distance) + Distance,
                    CoeffDraw, CoeffDraw);

                // Отрисока круга вокруг центра кластера.
                graphics.DrawEllipse(
                    new Pen(Color.Blue, 3),
                    (int)CloudPoints[cluster.MapGenCentroid].X * (CoeffDraw + Distance) + Distance - 3 * CoeffDraw + CoeffDraw / 2,
                    (int)CloudPoints[cluster.MapGenCentroid].Y * (CoeffDraw + Distance) + Distance - 3 * CoeffDraw + CoeffDraw / 2,
                    6 * CoeffDraw, 6 * CoeffDraw);
            }

            // Сохранение изображения.
            bitmap.Save(pathBMP);
        }

        /// <summary>
        /// Отрисовка точек карты в bmp изображение.
        /// </summary>
        public void DrawToBMP(string pathBMP)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                (int)(Width + 1) * (CoeffDraw + Distance) + Distance,
                (int)(Length + 1) * (CoeffDraw + Distance) + Distance);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            // Выставляем фон изображения.
            graphics.Clear(Color.White);

            foreach (Point point in CloudPoints)
            {
                // Отрисовка точки.
                graphics.FillRectangle(new SolidBrush(Color.Black),
                    (int) point.X * (CoeffDraw + Distance) + Distance,
                    (int) point.Y * (CoeffDraw + Distance) + Distance,
                    CoeffDraw, CoeffDraw);

                // Отрисовка надписи с глубиной.
                graphics.DrawString(Math.Round(point.Depth, 1).ToString(CultureInfo.InvariantCulture),
                    new Font("Arial", 10),
                    new SolidBrush(Color.Black),
                    (int) point.X * (CoeffDraw + Distance) + Distance + CoeffDraw / 2,
                    (int) point.Y * (CoeffDraw + Distance) + Distance + CoeffDraw / 2);
            }

            // Сохранение изображения.
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
