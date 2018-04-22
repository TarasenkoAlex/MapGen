using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    public class GraphicMap
    {
        /// <summary>
        /// Длина концов линий широты и долготы.
        /// </summary>
        private const double WidthEndOfLine = 0.025;

        /// <summary>
        /// Ширина краев карты.
        /// </summary>
        private const double WidthEdgeOfMap = 0.1d;

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
        public  Point3DColor[] Points { get; set; } = { };

        /// <summary>
        /// Максимальная глубина.
        /// </summary>
        public double MaxDepth { get; set; }

        /// <summary>
        /// Отрисовка карты с помощью OpenGl.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        public void Draw(OpenGL gl, double xCoeff, double yCoeff)
        {
            // Отрисовка содержимого карты.
            DrawDataMap(gl, xCoeff, yCoeff);

            // Отрисовка краев карты.
            DrawStripsEdgeOfMap(gl, xCoeff, yCoeff);
            
            // Отрисовка линий широт и долготы.
            DrawLinesLatitudeAndLongitude(gl, xCoeff, yCoeff);

            // Отрисовка исходных точек карты.
            DrawSourcePointsOfMap(gl, xCoeff, yCoeff);
        }

        /// <summary>
        /// Отрисовка содержимого карты.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        private void DrawDataMap(OpenGL gl, double xCoeff, double yCoeff)
        {
            gl.PointSize(0.3f);
            for (int iy = 0; iy < Length - 1; ++iy)
            {
                for (int jx = 0; jx < Width - 1; ++jx)
                {
                    // Отрисовываем полигоны, закрашенные градиентно.
                    gl.Begin(BeginMode.Quads);

                    var indexPoint = iy * Width + jx;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X * xCoeff, Points[indexPoint].Y * yCoeff);

                    indexPoint = iy * Width + jx + 1;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X * xCoeff, Points[indexPoint].Y * yCoeff);

                    indexPoint = (iy + 1) * Width + jx + 1;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X * xCoeff, Points[indexPoint].Y * yCoeff);

                    indexPoint = (iy + 1) * Width + jx;
                    gl.Color(Points[indexPoint].Color);
                    gl.Vertex(Points[indexPoint].X * xCoeff, Points[indexPoint].Y * yCoeff);

                    gl.End();
                }
            }
        }
        
        /// <summary>
        /// Отрисовка краев карты.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        private void DrawStripsEdgeOfMap(OpenGL gl, double xCoeff, double yCoeff)
        {
            gl.Color(1f, 1f, 1f);

            // Верх.
            gl.Begin(BeginMode.Quads);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, -WidthEdgeOfMap * yCoeff);
            gl.Vertex(Points[Width - 1].X * xCoeff + WidthEdgeOfMap, -WidthEdgeOfMap * yCoeff);
            gl.Vertex((Points[Width - 1].X + WidthEdgeOfMap) * xCoeff, 0);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, 0);
            gl.End();

            // Справа.
            gl.Begin(BeginMode.Quads);
            gl.Vertex(Points[Width - 1].X * xCoeff, -WidthEdgeOfMap * yCoeff);
            gl.Vertex((Points[Width - 1].X + WidthEdgeOfMap) * xCoeff, -WidthEdgeOfMap * yCoeff);
            gl.Vertex((Points[Width - 1].X + WidthEdgeOfMap) * xCoeff, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.Vertex(Points[Width - 1].X * xCoeff, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.End();

            // Низ.
            gl.Begin(BeginMode.Quads);
            gl.Vertex((Points[Width - 1].X + WidthEdgeOfMap) * xCoeff, Points.Last().Y * yCoeff);
            gl.Vertex((Points[Width - 1].X + WidthEdgeOfMap) * xCoeff, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, Points.Last().Y * yCoeff);
            gl.End();

            // Слева.
            gl.Begin(BeginMode.Quads);
            gl.Vertex(0, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, (Points.Last().Y + WidthEdgeOfMap) * yCoeff);
            gl.Vertex(-WidthEdgeOfMap * xCoeff, -WidthEdgeOfMap);
            gl.Vertex(0, -WidthEdgeOfMap * yCoeff);
            gl.End();
        }
        
        /// <summary>
        /// Отрисовка линий широт и долготы.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        private void DrawLinesLatitudeAndLongitude(OpenGL gl, double xCoeff, double yCoeff)
        {
            gl.Color(0.0f, 0.0f, 0.0f);
            gl.LineWidth(0.3f);

            // Отрисовка линий широт.
            for (int i = 0; i < Width; ++i)
            {
                gl.Begin(BeginMode.Lines);
                gl.Vertex(Points[i].X * xCoeff, -WidthEndOfLine * yCoeff, -0.0001d);
                gl.Vertex(Points[Width * (Length - 1) + i].X * xCoeff, (Points[Width * (Length - 1) + i].Y + WidthEndOfLine) * yCoeff, -0.0001d);
                gl.End();
            }

            // Отрисовка линий долготы.
            for (int j = 0; j < Length; ++j)
            {
                gl.Begin(BeginMode.Lines);
                gl.Vertex(-WidthEndOfLine * xCoeff, Points[Width * j].Y * yCoeff, -0.0001d);
                gl.Vertex((Points[Width * (j + 1) - 1].X + WidthEndOfLine) * xCoeff, Points[Width * (j + 1) - 1].Y * yCoeff, -0.0001d);
                gl.End();
            }  
        }

        /// <summary>
        /// Отрисовка цифр градусов на краях карты.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        private void DrawNumbersEdgeOfMap(OpenGL gl, double xCoeff, double yCoeff)
        {
            gl.Color(0f, 0f, 0f);
            // TODO
            
            gl.DrawText(10, 10, 1f, 1f, 1f, "", 12, "HELLO");
        }

        /// <summary>
        /// Отрисовка исходных точек карты.
        /// </summary>
        /// <param name="gl">OpenGl.</param>
        /// <param name="xCoeff">Сжатие по X.</param>
        /// <param name="yCoeff">Сжатие по Y.</param>
        private void DrawSourcePointsOfMap(OpenGL gl, double xCoeff, double yCoeff)
        {
            gl.PointSize(6.0f);
            gl.Color(0f, 0f, 0f);
            
            gl.Begin(BeginMode.Points);
            foreach (var point in Points)
            {
                if (point.IsSource)
                {
                    gl.Vertex(point.X * xCoeff, point.Y * yCoeff, -0.001d);
                }
            }
            gl.End();
        }
    }

    public class Point3DColor
    {
        public bool IsSource { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Depth { get; set; }
        public GLColor Color { get; set; }
    }
}
