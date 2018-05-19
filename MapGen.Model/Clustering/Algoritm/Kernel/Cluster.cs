using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.Database.EDM;

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
    }
}
