using System;
using System.Collections.Concurrent;
using SharpGL;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    public class DrawingObjects
    {
        public class DepthScale
        {
            private double _range;
            private double _stepScale;

            public DepthScale(double maxDepth)
            {
                _stepScale = Math.Truncate(maxDepth / int.Parse(ResourcesView.CountDepthScale));
                _range = _stepScale * int.Parse(ResourcesView.CountDepthScale);
            }

            public GLColor GetColorDepth(double depth)
            {
                float g = 0.0f;
                float b = 1.0f;

                int numColor = (int)Math.Truncate(depth / _stepScale) + 1;
                
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

        public class Point3d
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Depth { get; set; }
        }

        public class Triangle
        {
            public Point3d A { get; set; }
            public Point3d B { get; set; }
            public Point3d C { get; set; }
        }

        public class SurfaceMaker
        {
            private DepthScale _depthScale;

            public SurfaceMaker(double maxDepth)
            {
                _depthScale = new DepthScale(maxDepth);
            }

            public void DrawSurface(OpenGL gl, ref ConcurrentBag<Triangle> collection)
            {
                gl.PointSize(0.3f);
                gl.LineWidth(0.3f);


                foreach (Triangle triangle in collection)
                {
                    // рисуем треугольники
                    gl.Begin(OpenGL.GL_TRIANGLES);

                    gl.Color(_depthScale.GetColorDepth(triangle.A.Depth));
                    gl.Vertex((float)triangle.A.X, (float)triangle.A.Y, 0.0f);
                    gl.Color(_depthScale.GetColorDepth(triangle.B.Depth));
                    gl.Vertex((float)triangle.B.X, (float)triangle.B.Y, 0.0f);
                    gl.Color(_depthScale.GetColorDepth(triangle.C.Depth));
                    gl.Vertex((float)triangle.C.X, (float)triangle.C.Y, 0.0f);

                    gl.End();


                    gl.Color(0.0f, 0.0f, 0.0f);

                    // рисуем ребра
                    gl.Begin(OpenGL.GL_TRIANGLES);
                    // ребро между точками А и В
                    gl.Vertex((float)triangle.A.X, (float)triangle.A.Y, 0.0f);
                    gl.Vertex((float)triangle.B.X, (float)triangle.B.Y, 0.0f);
                    // ребро между точками В и С
                    gl.Vertex((float)triangle.B.X, (float)triangle.B.Y, 0.0f);
                    gl.Vertex((float)triangle.C.X, (float)triangle.C.Y, 0.0f);
                    // ребро между точками А и С
                    gl.Vertex((float)triangle.A.X, (float)triangle.A.Y, 0.0f);
                    gl.Vertex((float)triangle.C.X, (float)triangle.C.Y, 0.0f);
                    gl.End();
                }               
                                
            }
        }

    }
}
