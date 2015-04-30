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

        /// <summary>
        /// Checks order of points. 
        /// </summary>
        /// <returns>True - if points are in clockwise order.</returns>
        public static bool IsClockwiseOrdered(Point A, Point B, Point C)
        {
            var c = CrossProductZ(C, A, C, B);
            return c < 0;
        }

        /// <summary>
        /// Determines the relative position of points with respect to the line.
        /// </summary>
        /// <param name="O">Point that belongs to the line.</param>
        /// <param name="A">Point that belongs to the line.</param>
        /// <returns>True if points are separated by line OA.</returns>
        public static bool IsSeparated(Point O, Point A, Point X, Point Y)
        {
            // Use sign of pseudoscalar vector product - it defines half-plane.
            // If sign of OA ^ OX is same as sign of OA ^ OY - points X & Y lies on same half-plane, otherwise not.
            var OA = new Vector(O, A);
            var oxSign = Math.Sign(PseudoscalarVectorProduct(OA, new Vector(O, X)));
            var oySign = Math.Sign(PseudoscalarVectorProduct(OA, new Vector(O, Y)));
            // If sign is equal to 0 then one of points lies on the line, therefore points are not separated.
            if (oxSign == 0 || oySign == 0)
                return false;
            return oxSign != oySign;
        }

        /// <summary>
        /// Computes pseudoscalar vector product.
        /// </summary>
        /// <returns>Pseudoscalar vector product.</returns>
        public static double PseudoscalarVectorProduct(Vector a, Vector b)
        {
            return a.X * b.Y - a.Y * b.X; 
        }

        /// <summary>
        /// Determines whether y lies in epsilon-neighborhood of x.
        /// </summary>
        /// <returns>True - if y lies in epsilon-neighborhood of x, otherwise - false.</returns>
        public static bool IsInEpsilonArea(this double x, double y, double epsilon = 10e-6)
        {
            if (Math.Abs(x - y) <= epsilon)
                return true;
            return false;
        }

        /// <summary>
        /// Computes distance from point P to the line OX.
        /// </summary>
        /// <returns>Distance from point P to the line OX.</returns>
        public static double DistanceToLine(Point O, Point X, Point P)
        {
            double A, B, C;
            GetLine(O, X, out A, out B, out C);
            return Math.Abs((A * P.X + B * P.Y + C) / Math.Sqrt(A * A + B * B));
        }

        /// <summary>
        /// Computes constants of the general equation of the line.
        /// </summary>
        /// <param name="O">Point on line.</param>
        /// <param name="X">Point on line.</param>
        public static void GetLine(Point O, Point X, out double A, out double B, out double C)
        {
            A = O.Y - X.Y;
            B = X.X - O.X;
            C = O.X * X.Y - X.X * O.Y;
        }
    }
}
