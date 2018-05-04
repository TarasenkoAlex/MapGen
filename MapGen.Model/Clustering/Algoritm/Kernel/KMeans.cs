using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

namespace MapGen.Model.Clustering.Algoritm.Kernel
{
    public class KMeans
    {
        public int K { get; }

        public int Itarations { get; private set; } = 0;

        public int MaxItarations { get; set; } = -1;

        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions();

        public KMeansCluster[] Clusters { get; private set; }

        public KMeans(int k)
        {
            K = k;
        }

        public void Learn(Point[] data, out Point[] outData, out string message)
        {
            outData = new Point[K];
            message = string.Empty;

            FirstInitClusters(data);

            object sync = new object();
            int movements = 1;
            while (movements > 0)
            {
                movements = 0;

                //for (int i = 0; i < _clusters.Length; ++i)
                Parallel.For(0, K, ParallelOptions, i =>
                {
                    Clusters[i].UpdateCentroid(data);
                });

                //for (int i = 0; i < _clusters.Length; ++i)
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

                Itarations++;

                if (Itarations == MaxItarations && MaxItarations != -1)
                {
                    break;
                }
            }

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
                        if (data[Clusters[i][j]].Depth > outData[i].Depth)
                        {
                            outData[i] = data[Clusters[i][j]];
                        }
                    }
                }
            }
        }

        private void FirstInitClusters(Point[] data)
        {
            Clusters = new KMeansCluster[K];

            int interval = data.Length;
            int range = interval % K == 0 ? interval / K : interval / (K - 1);

            Random rand = new Random();

            for (int i = 0; i < K; ++i)
            {
                var index = (i == K - 1)
                    ? rand.Next(i * range, interval)
                    : rand.Next(i * range, (i + 1) * range);

                Clusters[i] = new KMeansCluster();
                Clusters[i].Centroid[0] = data[index].X;
                Clusters[i].Centroid[1] = data[index].Y;
                Clusters[i].Centroid[2] = data[index].Depth;
            }

            for (int i = 0; i < interval; ++i)
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
