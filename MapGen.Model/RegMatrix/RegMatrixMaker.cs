using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.General;
using MapGen.Model.Maps;

namespace MapGen.Model.RegMatrix
{
    public class RegMatrixMaker
    {

        public bool CreateRegMatrix(DbMap map, double step, out RegMatrix regMatrix, out string message)
        {
            regMatrix = new RegMatrix();
            message = string.Empty;

            try
            {
                regMatrix.Step = step;
                regMatrix.Width = 2;
                regMatrix.Length = 2;
                regMatrix.Points = new double[9]
                {
                    400, 500, 600, 700, 800, 900, 1000, 1100, 1200
                };
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время создания регулярной матрицы. {Methods.CalcMessageException(ex)}";
                return false;
            }
            return true; 
        }
    }
}
