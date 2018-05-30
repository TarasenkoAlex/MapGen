using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;

namespace MapGen.Model.Clustering.Algoritm.Kernel
{
    public class AGNES
    {
        /// <summary>
        /// Количество кластеров.
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// Настройка параллельного выполнения.
        /// </summary>
        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions();

        /// <summary>
        /// Кластера.
        /// </summary>
        public Cluster[] Clusters { get; private set; }

        /// <summary>
        /// Коичество итераций.
        /// </summary>
        public int Itarations { get; private set; } = 0;
        
        /// <summary>
        /// Создает объект для выполнения иерархического агломеративного метода кластеризации.
        /// </summary>
        /// <param name="k">Количество кластеров.</param>
        public AGNES(int k)
        {
            K = k;
        }

        /// <summary>
        /// Создает объект для выполнения иерархического агломеративного метода кластеризации.
        /// </summary>
        public AGNES()
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

            List<Cluster> clusters = new List<Cluster>();
            for (int i = 0; i < data.Length; ++i)
            {
                clusters.Add(new Cluster {i});
            }

            while (clusters.Count != K)
            {
                Itarations++;

                double[] minDistBetweenClusters = new double[clusters.Count - 1];
                int[] numCluster = new int[clusters.Count - 1];
                for (int i = 0; i < clusters.Count - 1; ++i)
                {
                    for (int j = i + 1; j < clusters.Count; ++j)
                    {
                        double dist = clusters[i].DistanceTo(clusters[j], data);
                        if (j == i + 1)
                        {
                            minDistBetweenClusters[i] = dist;
                            numCluster[i] = j;
                        }
                        else if (dist < minDistBetweenClusters[i])
                        {
                            minDistBetweenClusters[i] = dist;
                            numCluster[i] = j;
                        }
                    }
                }
                
                double min = minDistBetweenClusters.Min();
                int index = Array.FindIndex(minDistBetweenClusters, el => Math.Abs(el - min) < double.Epsilon);

                int firstCluster = index;
                int secondCluster = numCluster[index];
                
                clusters[firstCluster].AddRange(clusters[secondCluster]);
                clusters.RemoveAt(secondCluster);
            }

            Clusters = clusters.ToArray();

            // Формируем выходные данные.
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
    }
}
