using System;
using System.Collections.Generic;
using System.IO;
using MapGen.Model.Database.EDM;

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

        /// <summary>
        /// Нахождение расстояния между двумя точками на плоскости.
        /// </summary>
        /// <returns>расстояние между двумя точками</returns>
        public static double DistanceBetweenTwoPoints2D(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        /// <summary>
        /// Нахождение расстояния между двумя точками в трехмерии.
        /// </summary>
        /// <returns>расстояние между двумя точками</returns>
        public static double DistanceBetweenTwoPoints3D(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        }

        /// <summary>
        /// Формирование окрестности точек точки.
        /// </summary>
        /// <param name="x">x - координата точки, вокруг которой берется окрестность.</param>
        /// <param name="y">y - координата точки, вокруг которой берется окрестность</param>
        /// <param name="cloudPoints">Набор исходных точек</param>
        /// <param name="minRadiusOfEnvirons"> Минимамальный радиус для окрестности в виде окружности.</param>
        /// <param name="minCountPointsOfEnvirons">Минимальное количество точек в окрестности.</param>
        /// <param name="stepEncreaseOfEnvirons">Шаг увеличения окрестности.</param>
        /// <returns>номера точек окрестности относительно исходного списка точек</returns>
        public static List<int> GetSurroundOfPoint(double x, double y, Point[] cloudPoints, double minRadiusOfEnvirons, double stepEncreaseOfEnvirons, int minCountPointsOfEnvirons)
        {
            List<int> listIndexPointsSurround = new List<int>();

            double prevMinRadius = 0;   // Радиус окрестности на предыдущей итерации.

            while (minCountPointsOfEnvirons > listIndexPointsSurround.Count)
            {
                for (int i = 0; i < cloudPoints.Length; i++)
                {
                    var currentDistance = DistanceBetweenTwoPoints2D(cloudPoints[i].X, cloudPoints[i].Y, x, y);
                    if ((currentDistance <= minRadiusOfEnvirons) && (currentDistance > prevMinRadius))
                        listIndexPointsSurround.Add(i);
                }
                if (minCountPointsOfEnvirons > listIndexPointsSurround.Count)
                {
                    // Сохраняем радиус на предыдущей итерации.
                    prevMinRadius = minRadiusOfEnvirons;
                    // Увеличение радиуса окрестности.
                    minRadiusOfEnvirons += stepEncreaseOfEnvirons;
                }
            }

            return listIndexPointsSurround;
        }

        /// <summary>
        /// Удаление всех лементов директории.
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DeleteAllElementsOnDirectry(string dirPath)
        {
            foreach (var dir in Directory.GetDirectories(dirPath))
            {
                Directory.Delete(dir);
            }

            foreach (var file in Directory.GetFiles(dirPath))
            {
                File.Delete(file);
            }
        }
    }
}
