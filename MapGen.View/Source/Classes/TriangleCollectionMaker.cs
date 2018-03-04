using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SharpGL.SceneGraph;

namespace MapGen.View.Source.Classes
{
    /// <summary>
    /// Класс, который отвечает за создание коллекции треугольников.
    /// </summary>
    public class TriangleCollectionMaker
    {
        /// <summary>
        /// Триангуляция поверхности.
        /// </summary>
        /// <param name="regMatrix">Регуряная матрица глубин.</param>
        /// <param name="triangleCollection">Коллекция треугольников.</param>
        /// <param name="message">Сообщение ошибки.</param>
        /// <returns>Успешна ли прошла триангуляция.</returns>
        public bool CreateTriangleCollectionMap(RegMatrixView regMatrix, out DrawingObjects.Triangle[] triangleCollection, out string message)
        {
            triangleCollection = new DrawingObjects.Triangle[] {};
            message = string.Empty;

            try
            {
                // Вычисляем количество вертикальны и горизонтальных полос поверхности.
                int countHorizStrips = Convert.ToInt32(regMatrix.Width / regMatrix.Step);
                int countVerticStrip = Convert.ToInt32(regMatrix.Length / regMatrix.Step);

                // Размер коллекции.
                int sizeCollection = 2 * countHorizStrips * countVerticStrip;

                // Выделение памяти под коллекцию треугольников.
                DrawingObjects.Triangle[] collection = new DrawingObjects.Triangle[sizeCollection];

                // (i, j) - квадрат в i - ой строке и j - ом столбце.
                Parallel.For(0, countHorizStrips, i =>
                //for (int i = 0; i < countHorizStrips; ++i)
                {
                    Parallel.For(0, countVerticStrip, j =>
                    //for (int j = 0; j < countVerticStrip; ++j)
                    {
                        // Разбиваем квадраты на трегуольники, проводя диагональ из верхнего левого угла в нижний правый.
                        //     (i,j)--(i,j+1)                    a1----a2
                        //       | \    |                        | \    |
                        //       |  \   |      =========>        |  \   |  
                        //       |   \  |                        |   \  |
                        //       |    \ |                        |    \ |
                        //   (i+1,j)--(i+1,j+1)                  a4----a3
                        
                        // Вычисляем вершины.
                        Vertex a1 = new Vertex(
                            (float)(j * regMatrix.Step), 
                            (float)(i * regMatrix.Step), 
                            (float)regMatrix.Points[i * countVerticStrip + j]);
                        Vertex a2 = new Vertex(
                            (float)((j + 1) * regMatrix.Step),
                            (float)(i * regMatrix.Step),
                            (float)regMatrix.Points[i * countVerticStrip + (j + 1)]);
                        Vertex a3 = new Vertex(
                            (float)((j + 1) * regMatrix.Step),
                            (float)((i + 1) * regMatrix.Step),
                            (float)regMatrix.Points[(i + 1) * countVerticStrip + (j + 1)]);
                        Vertex a4 = new Vertex(
                            (float)(j * regMatrix.Step), 
                            (float)((i + 1) * regMatrix.Step), 
                            (float)regMatrix.Points[(i + 1) * countVerticStrip + j]);
                        
                        // Строим треугольники и добавляем в коллекцию.
                        collection[i * countVerticStrip * 2 + j * 2] = new DrawingObjects.Triangle(a1, a4, a3);
                        collection[i * countVerticStrip * 2 + (j * 2 + 1)] = new DrawingObjects.Triangle(a1, a2, a3);
                    });
                });

                triangleCollection = collection;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                message = ex.Message;
                return false;
            }

            return true;
        }
    }
}
