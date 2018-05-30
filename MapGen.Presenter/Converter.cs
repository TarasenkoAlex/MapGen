using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model;
using MapGen.Model.Clustering.Setting;
using MapGen.Model.General;
using MapGen.Model.Interpolation.Setting;
using MapGen.Model.RegMatrix;
using MapGen.View.Source.Classes;
using MapGen.View.Source.Classes.SettingInterpol;
using MapGen.Model.Generalization.Setting;
using MapGen.View.Source.Classes.SettingClustering;
using MapGen.View.Source.Classes.SettingGen;

namespace MapGen.Presenter
{
    public static class Converter
    {
        #region Public methods.

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
            
            int countSourcePoints = 0;
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
                    countSourcePoints = point.IsSource ? countSourcePoints + 1 : countSourcePoints;
                }
            }

            graphicMap.CountSourcePoints = countSourcePoints;

            return graphicMap;
        }

        /// <summary>
        /// Конвертировать SettingGen в VSettingGen.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static VSettingGen ToVSettingGen(SettingGen setting)
        {
            VSettingGen vsetting = null;

            var kmeans = setting.SettingCL as SettingCLKMeans;
            if (kmeans != null)
            {
                vsetting = new VSettingGen()
                {
                    SelectionRule = ConvertSelectionRulesToVSelectionRules(setting.SelectionRule),
                    SettingCL = ConvertSettingClKMeansToIVSettingCL(kmeans)
                };
            }

            var knp = setting.SettingCL as SettingCLKNP;
            if (knp != null)
            {
                vsetting = new VSettingGen()
                {
                    SelectionRule = ConvertSelectionRulesToVSelectionRules(setting.SelectionRule),
                    SettingCL = ConvertSettingClKNPToIVSettingCL(knp)
                };
            }

            return vsetting;
        }

        /// <summary>
        /// Конвертировать VSettingGen в SettingGen.
        /// </summary>
        /// <param name="vsetting"></param>
        /// <returns></returns>
        public static SettingGen ToSettingGen(VSettingGen vsetting)
        {
            SettingGen setting = null;

            var kmeans = vsetting.SettingCL as VSettingCLKMeans;
            if (kmeans != null)
            {
                setting = new SettingGen()
                {
                    SelectionRule = ConvertVSelectionRulesToSelectionRules(vsetting.SelectionRule),
                    SettingCL = ConvertVSettingCLKMeansToSettingCLKMeans(kmeans)
                };
            }
            else
            {
                var knp = vsetting.SettingCL as VSettingCLKNP;
                if (knp != null)
                {
                    setting = new SettingGen()
                    {
                        SelectionRule = ConvertVSelectionRulesToSelectionRules(vsetting.SelectionRule),
                        SettingCL = ConvertVSettingCLKNPToSettingCLKMeans(knp)
                    };
                }
            }

            return setting;
        }

        #endregion

        #region Private methods.

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
        /// Конвертация VSettingCLKNP в SettingCLKNP.
        /// </summary>
        /// <param name="vsetting"></param>
        /// <returns></returns>
        private static SettingCLKNP ConvertVSettingCLKNPToSettingCLKMeans(VSettingCLKNP vsetting)
        {
            SettingCLKNP setting = new SettingCLKNP
            {
                MaxDegreeOfParallelism = vsetting.MaxDegreeOfParallelism,
            };
            return setting;
        }

        /// <summary>
        /// Конвертация VSettingCLKMeans в SettingCLKMeans.
        /// </summary>
        /// <param name="vsetting"></param>
        /// <returns></returns>
        private static SettingCLKMeans ConvertVSettingCLKMeansToSettingCLKMeans(VSettingCLKMeans vsetting)
        {
            SettingCLKMeans setting = new SettingCLKMeans
            {
                Seeding = ConvertVSeedingsToSeedings(vsetting.Seeding),
                MaxDegreeOfParallelism = vsetting.MaxDegreeOfParallelism,
                MaxItarations = vsetting.MaxItarations
            };
            return setting;
        }

        /// <summary>
        /// Конвертация SettingCLKNP в IVSettingCL.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static IVSettingCL ConvertSettingClKNPToIVSettingCL(SettingCLKNP setting)
        {
            IVSettingCL vsetting = new VSettingCLKNP()
            {
                MaxDegreeOfParallelism = setting.MaxDegreeOfParallelism,
            };
            return vsetting;
        }

        /// <summary>
        /// Конвертация SettingCLKMeans в IVSettingCL.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static IVSettingCL ConvertSettingClKMeansToIVSettingCL(SettingCLKMeans setting)
        {
            IVSettingCL vsetting = new VSettingCLKMeans
            {
                Seeding = ConvertSeedingsToVSeedings(setting.Seeding),
                MaxDegreeOfParallelism = setting.MaxDegreeOfParallelism,
                MaxItarations = setting.MaxItarations
            };
            return vsetting;
        }

        /// <summary>
        /// Конвертация Seedings в VSeedings.
        /// </summary>
        /// <param name="seeding"></param>
        /// <returns></returns>
        private static VSeedings ConvertSeedingsToVSeedings(Seedings seeding)
        {
            switch (seeding)
            {
                case Seedings.Random: return VSeedings.Random;
                default: return VSeedings.Random;
            }
        }

        /// <summary>
        /// Конвертация VSeedings в Seedings.
        /// </summary>
        /// <param name="vseeding"></param>
        /// <returns></returns>
        private static Seedings ConvertVSeedingsToSeedings(VSeedings vseeding)
        {
            switch (vseeding)
            {
                case VSeedings.Random: return Seedings.Random;
                default: return Seedings.Random;
            }
        }

        /// <summary>
        /// Конвертация SelectionRules в VSelectionRules.
        /// </summary>
        /// <param name="selectionRule"></param>
        /// <returns></returns>
        private static VSelectionRules ConvertSelectionRulesToVSelectionRules(SelectionRules selectionRule)
        {
            switch (selectionRule)
            {
                case SelectionRules.Topfer: return VSelectionRules.Topfer;
                default: return VSelectionRules.Topfer;
            }
        }

        /// <summary>
        /// Конвертация VSelectionRules в SelectionRules.
        /// </summary>
        /// <param name="vselectionRule"></param>
        /// <returns></returns>
        private static SelectionRules ConvertVSelectionRulesToSelectionRules(VSelectionRules vselectionRule)
        {
            switch (vselectionRule)
            {
                case VSelectionRules.Topfer: return SelectionRules.Topfer;
                default: return SelectionRules.Topfer;
            }
        }

        #endregion
    }
}
