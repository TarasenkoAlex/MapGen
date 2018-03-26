using System;
using SharpGL;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    public class RegMatrixView
    {
        /// <summary>
        /// Шаг регулярной матрицы высот.
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// Ширина карты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Длина карты.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Точки регулярной матрицы глубин.
        /// </summary>
        public double[] Points { get; set; }

        /// <summary>
        /// Цвета для точек регулярной матрицы глубин.
        /// </summary>
        public GLColor[] GlColorForPoints { get; set; } = { };

        /// <summary>
        /// Максимальная глубина.
        /// </summary>
        public double MaxDepth { get; set; }

        /// <summary>
        /// Инициализация цветов для точек регулярной матрицы глубин.
        /// </summary>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешно ли прошла инициализация.</returns>
        public bool InitColorForPoints(out string message)
        {
            message = string.Empty;

            try
            {
                GlColorForPoints = new GLColor[Points.Length];
                     
                DrawingObjects.DepthScale depthScale = new DrawingObjects.DepthScale(MaxDepth);

                for (int i = 0; i < Points.Length; ++i)
                {
                    GlColorForPoints[i] = depthScale.GetColorDepth(Points[i]);
                }

                return true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                message = $"Ошибка во время инициализация цветов для точек регулярной матрицы глубин. {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Отрисовка поверхности с помощью OpenGl.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        public void DrawSurface(OpenGL gl)
        {
            gl.PointSize(0.3f);
            gl.LineWidth(0.3f);

            for (int i = 0; i < Length - 1; ++i)
            {
                for (int j = 0; j < Width - 1; ++j)
                {
                    gl.Begin(OpenGL.GL_QUADS);

                    gl.Color(GlColorForPoints[i * Width + j]);
                    gl.Vertex(j, i);

                    gl.Color(GlColorForPoints[i * Width + j + 1]);
                    gl.Vertex(j + 1, i);

                    gl.Color(GlColorForPoints[(i + 1) * Width + j + 1]);
                    gl.Vertex(j + 1, i + 1);

                    gl.Color(GlColorForPoints[(i + 1) * Width + j]);
                    gl.Vertex(j, i + 1);

                    gl.End();

                    gl.Color(0.0f, 0.0f, 0.0f);

                    // рисуем ребра
                    gl.Begin(OpenGL.GL_LINES);
                    
                    gl.Vertex(j, i);
                    gl.Vertex(j + 1, i);

                    gl.Vertex(j + 1, i);
                    gl.Vertex(j + 1, i + 1);

                    gl.Vertex(j + 1, i + 1);
                    gl.Vertex(j, i + 1);

                    gl.Vertex(j, i + 1);
                    gl.Vertex(j, i);

                    gl.End();
                }
            }
        }
    }
}
