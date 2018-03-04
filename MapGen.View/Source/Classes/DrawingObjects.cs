using System;
using System.Collections.Concurrent;
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
                Range = StepScale * int.Parse(ResourcesView.CountDepthScale);
            }

            /// <summary>
            /// Получить цвет точки в зависимости от шкалы глубин.
            /// </summary>
            /// <param name="depth">Глубина точки.</param>
            /// <returns>Цвет.</returns>
            public GLColor GetColorDepth(double depth)
            {
                float g = 0.0f;
                float b = 1.0f;

                int numColor = (int)Math.Truncate(depth / StepScale) + 1;
                
                if (numColor <= 8)
                {
                    g = 1.0f - 0.125f * numColor;
                }
                if (numColor > 8 && numColor <= 10)
                {
                    g = 0.0f;
                    b = 1.0f - 0.125f * numColor;
                }
                if (numColor > 10)
                {
                    g = 0.0f;
                    b = 0.75f;
                }

                return new GLColor(0.0f, g, b, 1.0f);
            }
        }
        
        /// <summary>
        /// Класс, который отвечает за отрисовку поверхности.
        /// </summary>
        public class SurfaceMaker
        {
            /// <summary>
            /// Отрисовка поверхности с помощью OpenGl.
            /// </summary>
            /// <param name="gl">OpenGl.</param>
            /// <param name="collection">Коллекция треугольников.</param>
            /// <param name="maxDepth">Максимальная глубина.</param>
            public void DrawSurface(OpenGL gl, Triangle[] collection, double maxDepth)
            {
                DepthScale depthScale = new DepthScale(maxDepth);

                gl.PointSize(0.3f);
                gl.LineWidth(0.3f);
                
                foreach (Triangle triangle in collection)
                {
                    // рисуем треугольники
                    gl.Begin(OpenGL.GL_TRIANGLES);

                    gl.Color(depthScale.GetColorDepth(triangle.A.Z));
                    gl.Vertex(triangle.A.X, triangle.A.Y);
                    gl.Color(depthScale.GetColorDepth(triangle.B.Z));
                    gl.Vertex(triangle.B.X, triangle.B.Y);
                    gl.Color(depthScale.GetColorDepth(triangle.C.Z));
                    gl.Vertex(triangle.C.X, triangle.C.Y);

                    gl.End();
                    
                    gl.Color(0.0f, 0.0f, 0.0f);

                    // рисуем ребра
                    gl.Begin(OpenGL.GL_LINES);
                    // ребро между точками А и В
                    gl.Vertex(triangle.A.X, triangle.A.Y);
                    gl.Vertex(triangle.B.X, triangle.B.Y);
                    // ребро между точками В и С
                    gl.Vertex(triangle.B.X, triangle.B.Y);
                    gl.Vertex(triangle.C.X, triangle.C.Y);
                    // ребро между точками А и С
                    gl.Vertex(triangle.A.X, triangle.A.Y);
                    gl.Vertex(triangle.C.X, triangle.C.Y);
                    gl.End();
                }                       
            }
        }

        /// <summary>
        /// Класс треугольника для отрисовки поверхности.
        /// </summary>
        public class Triangle
        {
            public Vertex A { get; set; }
            public Vertex B { get; set; }
            public Vertex C { get; set; }

            public Triangle(Vertex a, Vertex b, Vertex c)
            {
                A = a;
                B = b;
                C = c;
            }
        }
    }
}
