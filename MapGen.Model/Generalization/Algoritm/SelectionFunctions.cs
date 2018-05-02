using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.Generalization.Algoritm
{
    public static class SelectionFunctions
    {
        /// <summary>
        /// Формула Топфера для определения норм отбора.
        /// </summary>
        /// <param name="sourceScale">Масштаб исходной карты.</param>
        /// <param name="distScale">Масштаб составляемой карты.</param>
        /// <param name="countSourcePoints">Количество точек на исходной карте.</param>
        /// <returns>Количество точек на составляемой карте.</returns>
        public static int FunctionTopfer(long sourceScale, long distScale, int countSourcePoints)
        {

            // countSourcePoints = Na
            // countDistPoints = Nb
            // sourceScale = 1 / Ma
            // distScale = 1 / Mb
            // Nb = Na * sqrt(Mb / Ma)

            int countDistPoints = (int)(countSourcePoints * Math.Sqrt(sourceScale / (double)distScale));
            
            return countDistPoints;
        }
    }
}
