using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    public class GraphicMap
    {
        /// <summary>
        /// Масштаб карты (1 : Scale).
        /// </summary>
        public long Scale { get; set; }

        /// <summary>
        /// Ширина карты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Длина карты.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Точки карты. Хранятся в виде матрицы размером Width х Length.
        /// </summary>
        public  Point3dColor[] Points { get; set; } = { };

        /// <summary>
        /// Максимальная глубина.
        /// </summary>
        public double MaxDepth { get; set; }
        
        /// <summary>
        /// Отрисовка карты с помощью OpenGl.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        public void DrawSurface(OpenGL gl)
        {
            gl.PointSize(0.3f);
            gl.LineWidth(0.3f);

            int indexPoint = 0;
            for (int iy = 0; iy < Length - 1; ++iy)
            {
                for (int jx = 0; jx < Width - 1; ++jx)
                {
                    gl.Begin(OpenGL.GL_QUADS);

                    indexPoint = iy * Width + jx;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = iy * Width + jx + 1;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = (iy + 1) * Width + jx + 1;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = (iy + 1) * Width + jx;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    gl.End();

                    gl.Color(0.0f, 0.0f, 0.0f);

                    // рисуем ребра
                    gl.Begin(OpenGL.GL_LINES);

                    indexPoint = iy * Width + jx;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);
                    indexPoint = iy * Width + jx + 1;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = iy * Width + jx + 1;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);
                    indexPoint = (iy + 1) * Width + jx + 1;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = (iy + 1) * Width + jx + 1;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);
                    indexPoint = (iy + 1) * Width + jx;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    indexPoint = (iy + 1) * Width + jx;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);
                    indexPoint = iy * Width + jx;
                    gl.Vertex(Points[indexPoint].X, Points[indexPoint].Y);

                    gl.End();
                }
            }
        }
    }

    public class Point3dColor
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Depth { get; set; }
        public GLColor Color { get; set; }
    }
}
