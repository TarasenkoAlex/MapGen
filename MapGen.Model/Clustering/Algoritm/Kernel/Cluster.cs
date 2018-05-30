using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;

namespace MapGen.Model.Clustering.Algoritm.Kernel
{
    public class Cluster : List<int>
    {
        public double[] MiddleCentroid { get; private set; } = new double[3];

        public int MapGenCentroid { get; set; } = -1;

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

            MiddleCentroid = tmp;
        }

        public double DistanceTo(Cluster cluster, Point[] data)
        {
            double distance = -1;

            foreach (var indexFirst in this)
            {
                foreach (var indexSecond in cluster)
                {
                    var dist = Methods.DistanceBetweenTwoPoints2D(data[indexFirst], data[indexSecond]);

                    if (Math.Abs(distance - (-1)) < double.Epsilon)
                    {
                        distance = dist;
                    }
                    else
                    {
                        if (dist < distance)
                        {
                            distance = dist;
                        }
                    }
                } 
            }

            return distance;
        }
    }
}
