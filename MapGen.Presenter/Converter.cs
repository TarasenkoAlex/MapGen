using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.RegMatrix;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingInterpol;

namespace MapGen.Presenter
{
    public static class Converter
    {
        /// <summary>
        /// Конвертировать ISettingInterpolKriging в IVSettingInterpol.
        /// </summary>
        /// <param name="kriging"></param>
        /// <returns></returns>
        public static IVSettingInterpol ToIVSettingInterpol(ISettingInterpolKriging kriging)
        {
            IVSettingInterpol interpol = new VSettingInterpolKriging
            {
                MinRadiusOfEnvirons = kriging.MinRadiusOfEnvirons,
                MinCountPointsOfEnvirons = kriging.MinCountPointsOfEnvirons,
                StepEncreaseOfEnvirons = kriging.StepEncreaseOfEnvirons,
                Variogram = ConvertVariogramsToVVariograms(kriging.Variogram),
                A = kriging.A,
                C = kriging.C,
                C0 = kriging.C0
            };
            return interpol;
        }

        /// <summary>
        /// Конвертировать ISettingInterpolRbf в IVSettingInterpol.
        /// </summary>
        /// <param name="rbf"></param>
        /// <returns></returns>
        public static IVSettingInterpol ToIVSettingInterpol(ISettingInterpolRbf rbf)
        {
            IVSettingInterpol interpol = new VSettingInterpolRbf()
            {
                MinRadiusOfEnvirons = rbf.MinRadiusOfEnvirons,
                MinCountPointsOfEnvirons = rbf.MinCountPointsOfEnvirons,
                StepEncreaseOfEnvirons = rbf.StepEncreaseOfEnvirons,
                BasicFunction = ConvertBasicFunctionsToVBasicFunctions(rbf.BasicFunction),
                R = rbf.R
            };
            return interpol;
        }

        /// <summary>
        /// Конвертировать VSettingInterpolKriging в ISettingInterpol.
        /// </summary>
        /// <param name="kriging"></param>
        /// <returns></returns>
        public static ISettingInterpol ToISettingInterpol(VSettingInterpolKriging kriging)
        {
            ISettingInterpol interpol = new SettingInterpolKriging
            {
                MinRadiusOfEnvirons = kriging.MinRadiusOfEnvirons,
                MinCountPointsOfEnvirons = kriging.MinCountPointsOfEnvirons,
                StepEncreaseOfEnvirons = kriging.StepEncreaseOfEnvirons,
                Variogram = ConvertVVariogramsToVariograms(kriging.Variogram),
                A = kriging.A,
                C = kriging.C,
                C0 = kriging.C0
            };
            return interpol;
        }

        /// <summary>
        /// Конвертировать VSettingInterpolRbf в ISettingInterpol.
        /// </summary>
        /// <param name="rbf"></param>
        /// <returns></returns>
        public static ISettingInterpol ToISettingInterpol(VSettingInterpolRbf rbf)
        {
            ISettingInterpol interpol = new SettingInterpolRbf
            {
                MinRadiusOfEnvirons = rbf.MinRadiusOfEnvirons,
                MinCountPointsOfEnvirons = rbf.MinCountPointsOfEnvirons,
                StepEncreaseOfEnvirons = rbf.StepEncreaseOfEnvirons,
                BasicFunction = ConvertVBasicFunctionsToBasicFunctions(rbf.BasicFunction),
                R = rbf.R
            };
            return interpol;
        }
        
        /// <summary>
        /// Конвертация вариаграммы из model в вариаграмму view.
        /// </summary>
        /// <param name="variograms"></param>
        /// <returns></returns>
        private static VVariograms ConvertVariogramsToVVariograms(Variograms variograms)
        {
            switch (variograms)
            {
                case Variograms.Circle: return VVariograms.Circle;
                case Variograms.Exponent: return VVariograms.Exponent;
                case Variograms.Gauss: return VVariograms.Gauss;
                case Variograms.Linear: return VVariograms.Linear;
                case Variograms.Spherial: return VVariograms.Spherial;
                default: return VVariograms.Spherial;
            }
        }

        /// <summary>
        /// Конвертация вариаграммы из view в вариаграмму model.
        /// </summary>
        /// <param name="variograms"></param>
        /// <returns></returns>
        private static Variograms ConvertVVariogramsToVariograms(VVariograms variograms)
        {
            switch (variograms)
            {
                case VVariograms.Circle: return Variograms.Circle;
                case VVariograms.Exponent: return Variograms.Exponent;
                case VVariograms.Gauss: return Variograms.Gauss;
                case VVariograms.Linear: return Variograms.Linear;
                case VVariograms.Spherial: return Variograms.Spherial;
                default: return Variograms.Spherial;
            }
        }

        /// <summary>
        /// Конвертация радиальной базисной функции из model в радиальную базисную функцию view.
        /// </summary>
        /// <param name="basicFunctions"></param>
        /// <returns></returns>
        private static VBasicFunctions ConvertBasicFunctionsToVBasicFunctions(BasicFunctions basicFunctions)
        {
            switch (basicFunctions)
            {
                case BasicFunctions.MultiLog: return VBasicFunctions.MultiLog;
                case BasicFunctions.MultiQuadric: return VBasicFunctions.MultiQuadric;
                case BasicFunctions.InverseMultiQuadric: return VBasicFunctions.InverseMultiQuadric;
                case BasicFunctions.NaturalCubicSpline: return VBasicFunctions.NaturalCubicSpline;
                case BasicFunctions.ThinPlateSpline: return VBasicFunctions.ThinPlateSpline;
                default: return VBasicFunctions.MultiLog;
            }
        }

        /// <summary>
        /// Конвертация радиальной базисной функции из view в радиальную базисную функцию model.
        /// </summary>
        /// <param name="basicFunctions"></param>
        /// <returns></returns>
        private static BasicFunctions ConvertVBasicFunctionsToBasicFunctions(VBasicFunctions basicFunctions)
        {
            switch (basicFunctions)
            {
                case VBasicFunctions.MultiLog: return BasicFunctions.MultiLog;
                case VBasicFunctions.MultiQuadric: return BasicFunctions.MultiQuadric;
                case VBasicFunctions.InverseMultiQuadric: return BasicFunctions.InverseMultiQuadric;
                case VBasicFunctions.NaturalCubicSpline: return BasicFunctions.NaturalCubicSpline;
                case VBasicFunctions.ThinPlateSpline: return BasicFunctions.ThinPlateSpline;
                default: return BasicFunctions.MultiLog;
            }
        }
        
        /// <summary>
        /// Конвертация регулярной матрицы в карту для отрисовки.
        /// </summary>
        /// <param name="regMatrix">Регулярная матрица Model.</param>
        /// <param name="longitude">Долгота.</param>
        /// <param name="scale">Масштаб карты (1 : scale).</param>
        /// <param name="name">Имя карты.</param>
        /// <param name="latitude">Широта.</param>
        /// <returns>Карта для отрисовки.</returns>
        public static GraphicMap ToGraphicMap(RegMatrix regMatrix, string name, string latitude, string longitude, long scale)
        {
            GraphicMap graphicMap = new GraphicMap
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Scale = scale,
                Width = regMatrix.Width,
                Length = regMatrix.Length,
                MaxDepth = regMatrix.MaxDepth,
                Points = new Point3DColor[regMatrix.Points.Length],
                WidthEdgeOfMap = regMatrix.Step / 4,
                WidthEndOfLine = regMatrix.Step / 8,
                PointSize = 8.0f
            };

            DrawingObjects.DepthScale depthScale = new DrawingObjects.DepthScale(graphicMap.MaxDepth);

            for (int i = 0; i < regMatrix.Length; ++i)
            {
                for (int j = 0; j < regMatrix.Width; ++j)
                {
                    var point = regMatrix.Points[i * regMatrix.Width + j];
                    graphicMap.Points[i * regMatrix.Width + j] = new Point3DColor
                    {
                        IsSource = point.IsSource,
                        X = regMatrix.Step * j,
                        Y = regMatrix.Step * i,
                        Depth = point.Depth,
                        Color = depthScale.GetColorDepth(point.Depth)
                    };
                }
            }

            return graphicMap;
        }
    }
}
