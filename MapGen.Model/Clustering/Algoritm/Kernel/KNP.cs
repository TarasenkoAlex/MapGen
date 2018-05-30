using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;
using QuickGraph;
using QuickGraph.Algorithms.ConnectedComponents;
using QuickGraph.Data;

namespace MapGen.Model.Clustering.Algoritm.Kernel
{
    public class KNP
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
        /// Создает объект для выполнения кластеризации алгоритмом минимального покрывающего дерева.
        /// </summary>
        /// <param name="k">Количество кластеров.</param>
        public KNP(int k)
        {
            K = k;
        }

        /// <summary>
        /// Создает объект для выполнения кластеризации алгоритмом минимального покрывающего дерева.
        /// </summary>
        public KNP()
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

            // 1. Построим граф.
            UndirectedGraph<int, WeightedEdge<int>> graph = new UndirectedGraph<int, WeightedEdge<int>>(true);
            InitDistMatrix(data);
            double min = _minDistBetweenTwoPoints.Min();
            int index = Array.FindIndex(_minDistBetweenTwoPoints, el => Math.Abs(el - min) < double.Epsilon);
            int firstVertex = index;
            int secondVertex = _numVertex[index];

            graph.AddVertex(firstVertex);
            graph.AddVertex(secondVertex);
            graph.AddEdge(new WeightedEdge<int>(firstVertex, secondVertex, min));
            _points.Remove(firstVertex);
            _points.Remove(secondVertex);

            while (_points.Count != 0)
            {
                double minDist = double.MaxValue;
                foreach (int vertexGraph in graph.Vertices)
                {
                    foreach (int point in _points)
                    {
                        double currDist = _distMatrix[vertexGraph, point];
                        if (Math.Abs(currDist) < double.Epsilon) currDist = _distMatrix[point, vertexGraph];
                        if (currDist < minDist)
                        {
                            firstVertex = vertexGraph;
                            secondVertex = point;
                            minDist = _distMatrix[vertexGraph, point];
                        }
                    }
                }

                graph.AddVertex(firstVertex);
                graph.AddVertex(secondVertex);
                graph.AddEdge(new WeightedEdge<int>(firstVertex, secondVertex, minDist));

                _points.Remove(firstVertex);
                _points.Remove(secondVertex);
            }

            // 2. Удалим K − 1 самых длинных рёбер.
            Func<WeightedEdge<int>, double> edgeCost = edge => edge.Weight;
            for (int i = 0; i < K - 1; ++i)
            {
                var minWeight = graph.Edges.Max(edgeCost);
                var edge = graph.Edges.First(el => Math.Abs(el.Weight - minWeight) < double.Epsilon);
                graph.RemoveEdge(edge);
            }
            
            // 3. Выделенение компонентов связности.
            ConnectedComponentsAlgorithm<int, WeightedEdge<int>> algorithm = new ConnectedComponentsAlgorithm<int, WeightedEdge<int>>(graph);
            algorithm.Compute();

            // 4. Сформируем выходные данные.
            Clusters = new Cluster[K];
            for (int i = 0; i < data.Length; ++i)
            {
                var numCluster = algorithm.Components[i];
                if (Clusters[numCluster] == null) Clusters[numCluster] = new Cluster();
                Clusters[numCluster].Add(i);
            }
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

        private double[,] _distMatrix;
        private double[] _minDistBetweenTwoPoints;
        private int[] _numVertex;
        private List<int> _points;

        private void InitDistMatrix(Point[] data)
        {
            _distMatrix = new double[data.Length, data.Length];
            _minDistBetweenTwoPoints = new double[data.Length - 1];
            _numVertex = new int[data.Length - 1];
            _points = new List<int>();

            for (int i = 0; i < data.Length; ++i)
            {
                _points.Add(i);
            }

            Parallel.For(0, data.Length - 1, ParallelOptions, i =>
                //for (int i = 0; i < data.Length - 1; ++i)
            {
                _minDistBetweenTwoPoints[i] = double.MaxValue;
                for (int j = i + 1; j < data.Length; ++j)
                {
                    if (i != j)
                    {
                        double dist = Methods.DistanceBetweenTwoPoints2D(data[i], data[j]);
                        _distMatrix[i, j] = dist;
                        if (dist < _minDistBetweenTwoPoints[i])
                        {
                            _minDistBetweenTwoPoints[i] = dist;
                            _numVertex[i] = j;
                        }
                    }
                }
            });
        }
    }

    public class WeightedEdge<TVertex> : Edge<TVertex>
    {
        public double Weight { get; private set; }

        public WeightedEdge(TVertex source, TVertex target)
            : this(source, target, 1) { }

        public WeightedEdge(TVertex source, TVertex target, double weight)
            : base(source, target)
        {
            this.Weight = weight;
        }
    }
}
