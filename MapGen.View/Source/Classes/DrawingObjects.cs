using System;
using SharpGL;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    public class DrawingObjects
    {
        /// <summary>
        /// Класс шкала глубин.
        /// </summary>
        public class DepthScale
        {
            /// <summary>
            /// Глубин на шкале глубин, стоящая перед "Глубже".
            /// </summary>
            public double Range { get; }

            /// <summary>
            /// Шаг шкалы гюлубин.
            /// </summary>
            public double StepScale { get; }

            /// <summary>
            /// Создает объект шкала глубин для вычислений.
            /// </summary>
            /// <param name="maxDepth">Максимальная глубина на карте.</param>
            public DepthScale(double maxDepth)
            {
                StepScale = Math.Truncate(maxDepth / (int.Parse(ResourcesView.CountDepthScale) - 1));
                Range = StepScale * (int.Parse(ResourcesView.CountDepthScale) - 1);
            }

            /// <summary>
            /// Получить цвет точки в зависимости от шкалы глубин.
            /// </summary>
            /// <param name="depth">Глубина точки.</param>
            /// <returns>Цвет.</returns>
            public GLColor GetColorDepth(double depth)
            {
                string[] rgb = {"0,0", "0,0", "0,0"};
                
                int numColor = (int)Math.Truncate(depth / StepScale) + 1;

                switch (numColor)
                {
                    case 1:
                    {
                        rgb = ResourcesView.ColorDepth1.Split('|');
                        break;
                    }
                    case 2:
                    {
                        rgb = ResourcesView.ColorDepth2.Split('|');
                        break;
                    }
                    case 3:
                    {
                        rgb = ResourcesView.ColorDepth3.Split('|');
                        break;
                    }
                    case 4:
                    {
                        rgb = ResourcesView.ColorDepth4.Split('|');
                        break;
                    }
                    case 5:
                    {
                        rgb = ResourcesView.ColorDepth5.Split('|');
                        break;
                    }
                    case 6:
                    {
                        rgb = ResourcesView.ColorDepth6.Split('|');
                        break;
                    }
                    case 7:
                    {
                        rgb = ResourcesView.ColorDepth7.Split('|');
                        break;
                    }
                    case 8:
                    {
                        rgb = ResourcesView.ColorDepth8.Split('|');
                        break;
                    }
                }
                
                return new GLColor(float.Parse(rgb[0])/255f, float.Parse(rgb[1])/255f, float.Parse(rgb[2])/255f, 1.0f);
            }
        }
        
    }
}
