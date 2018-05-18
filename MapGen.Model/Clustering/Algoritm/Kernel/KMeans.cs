using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;

namespace MapGen.Model.Clustering.Algoritm.Kernel
{
    public class KMeans
    {
        /// <summary>
        /// Количество кластеров.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// Коичество итераций.
        /// </summary>
        public int Itarations { get; private set; } = 0;

        /// <summary>
        /// Максимальное количество итераций.
        /// </summary>
        public int MaxItarations { get; set; } = -1;

        /// <summary>
        /// Настройка параллельного выполнения.
        /// </summary>
        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions();

        /// <summary>
        /// Кластера.
        /// </summary>
        public KMeansCluster[] Clusters { get; private set; }

        /// <summary>
        /// Настройка выбора центроидов на первом шаге алгоритма.
        /// </summary>
        public Seedings Seeding { get; set; } = Seedings.Random;

        /// <summary>
        /// Создает объект для выполнения метода кластеризации.
        /// </summary>
        /// <param name="k">Количество кластеров.</param>
        public KMeans(int k)
        {
            K = k;
        }

        /// <summary>
        /// Выполнить кластеризацию.
        /// </summary>
        /// <param name="data">Исходные данные.</param>
        /// <param name="outData">Множество точек, которые являются представителями кластеров.</param>
        public void Learn(Point[] data, out Point[] outData)
        {
            outData = new Point[K];

            // Инициализируем центроиды.
            FirstInitClusters(data);

            // Запускаем кластеризацию пока н сработает критерий останова.
            object sync = new object();
            int movements = 1;
            while (movements > 0 && !(Itarations == MaxItarations && MaxItarations != -1))
            {
                movements = 0;

                // Пересчитываем центроиды о каждого кластера.
                Parallel.For(0, K, ParallelOptions, i =>
                {
                    Clusters[i].UpdateCentroid(data);
                });

                // Вычисляем ближайшие центроиды для каждой точки.
                // Перемещаем точки в другие кластера, если это необходимо.
                Parallel.For(0, K, ParallelOptions, i =>
                {
                    for (int pointIndex = 0; pointIndex < Clusters[i].Count; ++pointIndex)
                    {
                        var point = Clusters[i][pointIndex];

                        int nearestCluster = FindNearestCluster(data[point]);

                        if (nearestCluster != Array.IndexOf(Clusters, Clusters[i]))
                        {
                            if (Clusters[i].Count > 1)
                            {
                                lock (sync)
                                {
                                    Clusters[i].Remove(point);
                                    Clusters[nearestCluster].Add(point);
                                    movements++;
                                }
                            }
                        }
                    }
                });

                // Увеличиваем количество итераций.
                Itarations++;
            }

            // Формируем выходной 
            for (int i = 0; i < K; ++i)
            {
                for (int j = 0; j < Clusters[i].Count; ++j)
                {
                    if (j == 0)
                    {
                        outData[i] = data[Clusters[i][j]];
                    }
                    else
                    {
                        if (data[Clusters[i][j]].Depth < outData[i].Depth)
                        {
                            outData[i] = data[Clusters[i][j]];
                        }
                    }
                }
            }
        }

        private void SeedingCentroids(Point[] data)
        {
            Clusters = new KMeansCluster[K];

            switch (Seeding)
            {
                case Seedings.Random:
                {
                    int[] ind = GetSortedRandVector(K, 0, data.Length);
                    InitCentroidsByData(ind, data);
                    break;
                }
                default:
                {
                    int[] ind = GetSortedRandVector(K, 0, data.Length);
                    InitCentroidsByData(ind, data);
                    break;
                }
            }
        }

        private void InitCentroidsByData(int[] ind, Point[] data)
        {
            for (int i = 0; i < K; ++i)
            {
                Clusters[i] = new KMeansCluster();
                Clusters[i].Centroid[0] = data[ind[i]].X;
                Clusters[i].Centroid[1] = data[ind[i]].Y;
                Clusters[i].Centroid[2] = data[ind[i]].Depth;
            }
        }

        private void FirstInitClusters(Point[] data)
        {
            SeedingCentroids(data);

            for (int i = 0; i < data.Length; ++i)
            {
                var index = FindNearestCluster(data[i]);
                Clusters[index].Add(i);
            }
        }

        private double FindDistance(double[] pt1, double[] pt2)
        {
            return Math.Sqrt(Math.Pow(pt2[0] - pt1[0], 2.0) + Math.Pow(pt2[1] - pt1[1], 2.0)/* + Math.Pow(pt2[2] - pt1[2], 2.0)*/);
        }

        private int FindNearestCluster(Point point)
        {
            int minIndex = 0;
            double[] pointDouble = { point.X, point.Y };

            double distance = 0;
            for (int i = 0; i < K; ++i)
            {
                if (i == 0)
                {
                    distance = FindDistance(pointDouble, Clusters[0].Centroid);
                }
                else
                {
                    var dist = FindDistance(pointDouble, Clusters[i].Centroid);
                    if (dist < distance)
                    {
                        distance = dist;
                        minIndex = i;
                    }
                }
            }

            return minIndex;
        }

        private int[] GetSortedRandVector(int size, int startIndex, int endIndex)
        {
            Random random = new Random(startIndex);

            var randVector = new int[size];

            for (int i = 0; i < size; ++i)
            {
                var value = random.Next(endIndex);
                if (!randVector.Contains(value))
                {
                    randVector[i] = value;
                }
                else
                {
                    i--;
                }
            }

            Array.Sort(randVector);

            return randVector;
        }
    }

    public class KMeansCluster : List<int>
    {
        public double[] Centroid { get; private set; } = new double[3];

        public void UpdateCentroid(Point[] data)
        {
            double[] tmp = new double[3];
            foreach (var element in this)
            {
                tmp[0] += data[element].X;
                tmp[1] += data[element].Y;
                tmp[2] += data[element].Depth;
            }

            tmp[0] /= Count;
            tmp[1] /= Count;
            tmp[2] /= Count;

            Centroid = tmp;
        }
    }
}
