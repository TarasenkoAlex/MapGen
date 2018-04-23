﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.Maps;
using MapGen.Model.RegMatrix;

namespace MapGen.Model.Interpolation.Strategy
{
    public class StrategyInterpolRbf : IStrategyInterpol
    {
        #region Region properties.

        /// <summary>
        /// Конфигурация для построения регулярной матрицы глубин.
        /// </summary>
        public ISettingInterpolRbf Setting { get; set; }

        #endregion

        #region Region constructor.

        /// <summary>
        /// Создает объект с алгоритмом заполения матрицы глубин точками методом RBF.
        /// </summary>
        /// <param name="settingKriging">Конфигурация для построения регулярной матрицы глубин.</param>
        public StrategyInterpolRbf(ISettingInterpolRbf settingRbf)
        {
            Setting = settingRbf;
        }

        #endregion

        #region Region public methods.

        /// <summary>
        /// Заполнение регулярной матрицы глубин точками с помощью метода RBF.
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
                // Заполенение регулярной матрицы.
                // (x, y) - координаты в секундах.
                for (long y = 0; y < regMatrix.Length; ++y)
                {
                    for (long x = 0; x < regMatrix.Width; ++x)
                    {
                        var findIndex = Array.FindIndex(map.CloudPoints, point => point.X == x && point.Y == y);
                        if (findIndex != -1)
                            regMatrix.Points[y * regMatrix.Width + x] = new PointRegMatrix
                            {
                                IsSource = true,
                                Depth = map.CloudPoints[findIndex].Depth
                            };
                        else
                            regMatrix.Points[y * regMatrix.Width + x] = new PointRegMatrix
                            {
                                IsSource = false,
                                Depth = RBF(x, y, map.CloudPoints)
                            };
                    }
                }
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
        /// Интерполяция методом RBF.
        /// </summary>
        /// <param name="x">x - координата точки, вокруг которой берется окрестность.</param>
        /// <param name="y">y - координата точки, вокруг которой берется окрестность</param>
        /// <param name="cloudPoints">Опорные точки.</param>
        /// <returns>Глубина интерполируемой точки.</returns>
        private double RBF(double x, double y, Point[] cloudPoints)
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
            double[] lamda = new double[size];

            // Вектор ковариаций между искомой точкой и всеми остальными.
            double[] k = new double[size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    K[i, j] = Methods.DistanceBetweenTwoPoints(
                        cloudPoints[surroundPoints[i]].X, cloudPoints[surroundPoints[i]].Y,
                        cloudPoints[surroundPoints[j]].X, cloudPoints[surroundPoints[j]].Y);
                    switch (Setting.BasicFunction)
                    {
                        case BasicFunctions.MultiQuadric:
                            K[i, j] = MultiQuadric(K[i, j]);
                            break;
                        case BasicFunctions.InverseMultiQuadric:
                            K[i, j] = InverseMultiQuadric(K[i, j]);
                            break;
                        case BasicFunctions.MultiLog:
                            K[i, j] = MultiLog(K[i, j]);
                            break;
                        case BasicFunctions.NaturalCubicSpline:
                            K[i, j] = NaturalCubicSpline(K[i, j]);
                            break;
                        case BasicFunctions.ThinPlateSpline:
                            K[i, j] = ThinPlateSpline(K[i, j]);
                            break;
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                K[size, i] = 1;
                K[i, size] = 1;
            }
            K[size, size] = 0;

            for (int i = 0; i < size; i++)
            {
                k[i] = Methods.DistanceBetweenTwoPoints(x, y, cloudPoints[surroundPoints[i]].X, cloudPoints[surroundPoints[i]].Y);
                switch (Setting.BasicFunction)
                {
                    case BasicFunctions.MultiQuadric:
                        k[i] = MultiQuadric(k[i]);
                        break;
                    case BasicFunctions.InverseMultiQuadric:
                        k[i] = InverseMultiQuadric(k[i]);
                        break;
                    case BasicFunctions.MultiLog:
                        k[i] = MultiLog(k[i]);
                        break;
                    case BasicFunctions.NaturalCubicSpline:
                        k[i] = NaturalCubicSpline(k[i]);
                        break;
                    case BasicFunctions.ThinPlateSpline:
                        k[i] = ThinPlateSpline(k[i]);
                        break;
                }
            }

            k[size] = 1;
            int info;

            alglib.densesolverreport rep = new alglib.densesolverreport();
            alglib.rmatrixsolve(K, size + 1, k, out info, out rep, out lamda);

            for (int i = 0; i < size; i++)
            {
                result += lamda[i] * cloudPoints[surroundPoints[i]].Depth;
            }

            result += lamda[size];

            return result;
        }

        private static double MultiQuadric(double h, double w = 1)
        {
            return Math.Pow((h * h + w * w), 0.5);
        }
        private static double InverseMultiQuadric(double h, double w = 1)
        {
            return Math.Pow((h * h + w * w), -0.5);
        }
        private static double MultiLog(double h, double w = 1)
        {
            return Math.Log(h * h + w * w);
        }
        private static double NaturalCubicSpline(double h, double w = 1)
        {
            return Math.Pow((h * h + w * w), 3.0f / 2.0f);
        }
        private static double ThinPlateSpline(double h, double w = 1)
        {
            return (h * h + w * w) * Math.Log(h * h + w * w);
        }

        #endregion
    }
}
