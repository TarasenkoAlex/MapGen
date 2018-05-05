using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Interpolation.Strategy;
using MapGen.Model.Maps;
using MapGen.Model.RegMatrix;

namespace MapGen.Model.Interpolation.Strategy
{
    public class StrategyInterpolKriging : IStrategyInterpol
    {
        #region Region properties.

        /// <summary>
        /// Конфигурация для построения регулярной матрицы глубин.
        /// </summary>
        public ISettingInterpolKriging Setting { get; set; }

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает объект с алгоритмом заполения матрицы глубин точками методом Кригинг.
        /// </summary>
        /// <param name="settingKriging">Конфигурация для построения регулярной матрицы глубин.</param>
        public StrategyInterpolKriging(ISettingInterpolKriging settingKriging)
        {
            Setting = settingKriging;
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Заполнение регулярной матрицы глубин точками с помощью метода Кригинг.
        /// </summary>
        /// <param name="map">Карта.</param>
        /// <param name="regMatrix">Регулярная карта.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли заполнена регулярная матрица глубин.</returns>
        public bool FillingRegMatrix(DbMap map, ref RegMatrix.RegMatrix regMatrix, out string message)
        {
            message = string.Empty;
            if (Setting == null)
            {
                message = "Отсутствует конфигурация для построения регулярной матрицы глубин.";
                return false;
            }

            try
            {
                RegMatrix.RegMatrix matrix = regMatrix;

                // Заполенение регулярной матрицы.
                // (x, y) - координаты в секундах.
                //for (long y = 0; y < matrix.Length; ++y)
                Parallel.For(0, matrix.Length, y =>
                {
                    //for (long x = 0; x < matrix.Width; ++x)
                    Parallel.For(0, matrix.Length, x =>
                    {
                        var findIndex = Array.FindIndex(map.CloudPoints, point => point.X == x && point.Y == y);
                        if (findIndex != -1)
                            matrix.Points[y * matrix.Width + x] = new PointRegMatrix
                            {
                                IsSource = true,
                                Depth = map.CloudPoints[findIndex].Depth
                            };
                        else
                            matrix.Points[y * matrix.Width + x] = new PointRegMatrix
                            {
                                IsSource = false,
                                Depth = Kriging(x, y, map.CloudPoints)
                            };
                    });
                });
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время создания регулярной матрицы. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        #endregion

        #region Region private methods.

        /// <summary>
        /// Интерполяция методом Кригинга.
        /// </summary>
        /// <param name="x">x - координата точки, вокруг которой берется окрестность.</param>
        /// <param name="y">y - координата точки, вокруг которой берется окрестность</param>
        /// <param name="cloudPoints">Опорные точки.</param>
        /// <returns>Глубина интерполируемой точки.</returns>
        private double Kriging(double x, double y, Point[] cloudPoints)
        {
            double result = 0.0d;

            // Определение окрестности точек.
            List<int> surroundPoints = Methods.GetSurroundOfPoint(
                x, y, cloudPoints, 
                Setting.MinRadiusOfEnvirons,
                Setting.StepEncreaseOfEnvirons, 
                Setting.MinCountPointsOfEnvirons);

            int size = surroundPoints.Count;

            // Матрица ковариаций.
            double[,] K = new double[size, size];

            // Вектор коэффицентов.
            double[] lamda;

            // Вектор ковариаций между искомой точкой и всеми остальными.
            double[] k = new double[size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    K[i, j] = Methods.DistanceBetweenTwoPoints2D(
                        cloudPoints[surroundPoints[i]].X, cloudPoints[surroundPoints[i]].Y,
                        cloudPoints[surroundPoints[j]].X, cloudPoints[surroundPoints[j]].Y);
                    switch (Setting.Variogram)
                    {
                        case Variograms.Spherial:
                            K[i, j] = SpherialVariogramm(K[i, j], Setting.A, Setting.C, Setting.C0);
                            break;
                        case Variograms.Gauss:
                            K[i, j] = GaussovaVariogramm(K[i, j], Setting.A, Setting.C, Setting.C0);
                            break;
                        case Variograms.Exponent:
                            K[i, j] = ExponentVariogramm(K[i, j], Setting.A, Setting.C, Setting.C0);
                            break;
                        case Variograms.Circle:
                            CircleVariogramm(K[i, j], Setting.A, Setting.C, Setting.C0);
                            break;
                        case Variograms.Linear:
                            K[i, j] = LinearVariogramm(K[i, j], Setting.A, Setting.C, Setting.C0);
                            break;
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                k[i] = Methods.DistanceBetweenTwoPoints2D(x, y, cloudPoints[surroundPoints[i]].X, cloudPoints[surroundPoints[i]].Y);
                switch (Setting.Variogram)
                {
                    case Variograms.Spherial:
                        k[i] = SpherialVariogramm(k[i], Setting.A, Setting.C, Setting.C0);
                        break;
                    case Variograms.Gauss:
                        k[i] = GaussovaVariogramm(k[i], Setting.A, Setting.C, Setting.C0);
                        break;
                    case Variograms.Exponent:
                        k[i] = ExponentVariogramm(k[i], Setting.A, Setting.C, Setting.C0);
                        break;
                    case Variograms.Circle:
                        k[i] = CircleVariogramm(k[i], Setting.A, Setting.C, Setting.C0);
                        break;
                    case Variograms.Linear:
                        k[i] = LinearVariogramm(k[i], Setting.A, Setting.C, Setting.C0);
                        break;
                }
            }

            int info;

            alglib.densesolverreport rep;
            alglib.rmatrixsolve(K, size, k, out info, out rep, out lamda);

            for (int i = 0; i < size; i++)
            {
                result += lamda[i] * cloudPoints[surroundPoints[i]].Depth;
            }

            return result;
        }

        /// <summary>
        /// Гауссова вариограмма.
        /// </summary>
        /// <param name="h">Расстояние между точками.</param>
        /// <param name="a">Ранг.</param>
        /// <param name="c">Вклад дисперсии или порог.</param>
        /// <param name="c0">Эффект самородка.</param>
        /// <returns></returns>
        private double GaussovaVariogramm(double h, double a = 20, double c = 100, double c0 = 0)
        {
            h = Math.Abs(h);
            a = Math.Abs(a);
            c = Math.Abs(c);

            double result = 0;

            result = c0 + c * (1 - Math.Exp(-(h * h) / (a * a)));

            return result;
        }

        /// <summary>
        /// Экспоненциальная вариограмма.
        /// </summary>
        /// <param name="h">Расстояние между точками.</param>
        /// <param name="a">Ранг.</param>
        /// <param name="c">Вклад дисперсии или порог.</param>
        /// <param name="c0">Эффект самородка.</param>
        /// <returns></returns>
        private double ExponentVariogramm(double h, double a = 20, double c = 100, double c0 = 0)
        {
            h = Math.Abs(h);
            a = Math.Abs(a);
            c = Math.Abs(c);

            double result = 0;

            result = c0 + c * (1 - Math.Exp(-h / a));

            return result;
        }

        /// <summary>
        /// Сферическая вариограмма.
        /// </summary>
        /// <param name="h">Расстояние между точками.</param>
        /// <param name="a">Ранг.</param>
        /// <param name="c">Вклад дисперсии или порог.</param>
        /// <param name="c0">Эффект самородка.</param>
        /// <returns></returns>
        private double SpherialVariogramm(double h, double a = 100, double c = 0.1, double c0 = 0)
        {
            h = Math.Abs(h);
            a = Math.Abs(a);
            c = Math.Abs(c);

            double result = 0;
            if (h <= a)
            {
                result = c0 + c * (1.5 * h / a - 0.5 * Math.Pow((h / a), 3));
            }
            else
            {
                result = c0 + c;
            }

            return result;
        }

        /// <summary>
        /// Линейная вариограмма.
        /// </summary>
        /// <param name="h">Расстояние между точками.</param>
        /// <param name="a">Ранг.</param>
        /// <param name="c">Вклад дисперсии или порог.</param>
        /// <param name="c0">Эффект самородка.</param>
        /// <returns></returns>
        private double LinearVariogramm(double h, double a = 100, double c = 0.1, double c0 = 0)
        {
            h = Math.Abs(h);
            a = Math.Abs(a);
            c = Math.Abs(c);

            double result = 0;
            if (h <= a)
            {
                result = c0 + c * (h / a);
            }
            else
            {
                result = c0 + c;
            }

            return result;
        }

        /// <summary>
        /// Круговая вариограмма.
        /// </summary>
        /// <param name="h">Расстояние между точками.</param>
        /// <param name="a">Ранг.</param>
        /// <param name="c">Вклад дисперсии или порог.</param>
        /// <param name="c0">Эффект самородка.</param>
        /// <returns></returns>
        private double CircleVariogramm(double h, double a = 20, double c = 100, double c0 = 0)
        {
            h = Math.Abs(h);
            a = Math.Abs(a);
            c = Math.Abs(c);

            double result = 0;
            if (h <= a)
            {
                result = c0 + c * (1 - 2 / (Math.PI * Math.Cos(h / a)) + Math.Sqrt(1 - (h * h) / (a * a)));
            }
            else
            {
                result = c0 + c;
            }

            return result;
        }

        #endregion
    }
}
