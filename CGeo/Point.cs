using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public struct Point
    {        
        public double X;

        public double Y;

        public Point(double X, double Y)
            : this()
        {
            this.X = X;
            this.Y = Y;
        }

        public bool IsInEpsilonArea(Point p)
        {
            if (p.X.IsInEpsilonArea(X) && p.Y.IsInEpsilonArea(Y))
                return true;
            return false;
        }
    }
}
