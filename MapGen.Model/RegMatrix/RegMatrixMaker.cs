using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.RegMatrix
{
    public class RegMatrixMaker
    {
        public RegMatrix CreateRegMatrix()
        {
            RegMatrix regMatrix = new RegMatrix();

            regMatrix.Step = 1.0;
            regMatrix.Width = 2;
            regMatrix.Length = 2;
            regMatrix.Points = new double[]
            {
                500, 600, 500, 400, 1000, 400, 500, 600, 500
            };

            return regMatrix;
        }
    }
}
