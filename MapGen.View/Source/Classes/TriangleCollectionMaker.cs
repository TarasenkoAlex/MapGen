using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MapGen.View.Source.Classes
{
    public class TriangleCollectionMaker
    {
        public ConcurrentBag<DrawingObjects.Triangle> CreateTriangleCollectionMap(RegMatrixView regMatrix)
        {
            if (regMatrix != null)
            {
                // Вычисляем количество вертикальны и горизонтальных полос поверхности.
                int countHorizStrips = Convert.ToInt32(regMatrix.Width / regMatrix.Step);
                int countVerticStrip = Convert.ToInt32(regMatrix.Length / regMatrix.Step);

                // Размер коллекции.
                int sizeCollection = 2 * countHorizStrips * countVerticStrip;

                // Выделение памяти под коллекцию треугольников.
                ConcurrentBag<DrawingObjects.Triangle> collection = new ConcurrentBag<DrawingObjects.Triangle>();

                // (i, j) - квадрат в i - ой строке и j - ом столбце.
                Parallel.For(0, countVerticStrip, i =>
                {
                    Parallel.For(0, countVerticStrip, j =>
                    {
                        // Разбиваем квадраты на трегуольники, проводя диагональ из верхнего левого угла в нижний правый.
                        // Первый треугольник.
                        DrawingObjects.Point3d A1 = new DrawingObjects.Point3d()
                        {
                            X = j * regMatrix.Step,
                            Y = -i * regMatrix.Step,
                            Depth = regMatrix.Points[i * countVerticStrip + j]
                        };
                        DrawingObjects.Point3d B1 = new DrawingObjects.Point3d()
                        {
                            X = j * regMatrix.Step,
                            Y = -(i + 1) * regMatrix.Step,
                            Depth = regMatrix.Points[(i + 1) * countVerticStrip + j]
                        };
                        DrawingObjects.Point3d C1 = new DrawingObjects.Point3d()
                        {
                            X = (j + 1) * regMatrix.Step,
                            Y = -(i + 1) * regMatrix.Step,
                            Depth = regMatrix.Points[(i + 1) * countVerticStrip + (j + 1)]
                        };
                        collection.Add(new DrawingObjects.Triangle()
                        {
                            A = A1,
                            B = B1,
                            C = C1
                        });

                        // Второй треугольник.
                        DrawingObjects.Point3d A2 = new DrawingObjects.Point3d()
                        {
                            X = j * regMatrix.Step,
                            Y = -i * regMatrix.Step,
                            Depth = regMatrix.Points[i * countVerticStrip + j]
                        };
                        DrawingObjects.Point3d B2 = new DrawingObjects.Point3d()
                        {
                            X = (j + 1) * regMatrix.Step,
                            Y = -i * regMatrix.Step,
                            Depth = regMatrix.Points[i * countVerticStrip + (j + 1)]
                        };
                        DrawingObjects.Point3d C2 = new DrawingObjects.Point3d()
                        {
                            X = (j + 1) * regMatrix.Step,
                            Y = -(i + 1) * regMatrix.Step,
                            Depth = regMatrix.Points[(i + 1) * countVerticStrip + (j + 1)]
                        };
                        collection.Add(new DrawingObjects.Triangle()
                        {
                            A = A2,
                            B = B2,
                            C = C2
                        });
                    });
                });

                return collection;
            }
            else
            {
                return new ConcurrentBag<DrawingObjects.Triangle>();
            }
        }
    }
}
