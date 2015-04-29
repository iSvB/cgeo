using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public static class Utils
    {
        /// <summary>
        /// Computes z-component of cross product of vectors [a,b] in right-handed coordinate system.        
        /// </summary>
        /// <param name="aStart">Initial point of vector a.</param>
        /// <param name="aEnd">Terminal point of vector a.</param>
        /// <param name="bStart">Initial point of vector b.</param>
        /// <param name="bEnd">Terminal point of vector b.</param>
        /// <returns>Z-component of cross product.</returns>        
        public static double CrossProductZ(Point aStart, Point aEnd, Point bStart, Point bEnd)
        {
            return (aEnd.X - aStart.X) * (bEnd.Y - bStart.Y) - (aEnd.Y - aStart.Y) * (bEnd.X - bStart.X);
        }
    }
}
