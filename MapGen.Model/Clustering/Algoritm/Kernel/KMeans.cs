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
        public int K { get; set; }

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
        public Cluster[] Clusters { get; set; }

        /// <summary>
        /// Настройка выбора центроидов на первом шаге алгоритма.
        /// </summary>
        public Seedings Seeding { get; set; } = Seedings.Random;

        /// <summary>
        /// Создает объект для выполнения метода кластеризации k - средних.
        /// </summary>
        /// <param name="k">Количество кластеров.</param>
        public KMeans(int k)
        {
            K = k;
        }

        /// <summary>
        /// Создает объект для выполнения метода кластеризации.
        /// </summary>
        public KMeans()
        {
        }

        /// <summary>
        /// Выполнить кластеризацию.
        /// </summary>
        /// <param name="data">Исходные данные.</param>
        /// <param name="outData">Множество точек, которые являются представителями кластеров.</param>
        public void Learn(Point[] data, out Point[] outData)
        {
            outData = new Point[K];

            // 1. Инициализируем центроиды.
            FirstInitClusters(data);

            // 2. Запускаем кластеризацию пока не сработает критерий останова.
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

            // 3. Формируем выходные даные.
            for (int i = 0; i < K; ++i)
            {
                for (int j = 0; j < Clusters[i].Count; ++j)
                {
                    if (j == 0)
                    {
                        outData[i] = data[Clusters[i][j]];
                        Clusters[i].MapGenCentroid = Clusters[i][j];
                    }
                    else
                    {
                        if (data[Clusters[i][j]].Depth < outData[i].Depth)
                        {
                            outData[i] = data[Clusters[i][j]];
                            Clusters[i].MapGenCentroid = Clusters[i][j];
                        }
                    }
                }
            }
        }

        private void SeedingCentroids(Point[] data)
        {
            Clusters = new Cluster[K];

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
                Clusters[i] = new Cluster
                {
                    MiddleCentroid =
                    {
                        [0] = data[ind[i]].X,
                        [1] = data[ind[i]].Y,
                        [2] = data[ind[i]].Depth
                    }
                };
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
        
        private int FindNearestCluster(Point point)
        {
            int minIndex = 0;
            double[] pointDouble = { point.X, point.Y };

            double distance = 0;
            for (int i = 0; i < K; ++i)
            {
                if (i == 0)
                {
                    distance = Methods.DistanceBetweenTwoPoints2D(pointDouble, Clusters[0].MiddleCentroid);
                }
                else
                {
                    var dist = Methods.DistanceBetweenTwoPoints2D(pointDouble, Clusters[i].MiddleCentroid);
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
}
