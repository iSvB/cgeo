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
        
        public Point(double X, double Y) : this() 
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class Rib
    {
        public Point A;
        public Point B;

        public Triangle T1;
        public Triangle T2;

        public IEnumerable<Point> Points { get { yield return A; yield return B; } }
        public IEnumerable<Triangle> Triangles { get { yield return T1; yield return T2; } }

        public Rib() { }

        public Rib(Point A, Point B, Triangle T1, Triangle T2)
        {
            this.A = A;
            this.B = B;
            this.T1 = T1;
            this.T2 = T2;
        }
    }

    public class Triangle
    {
        public Rib R1;
        public Rib R2;
        public Rib R3;

        public IEnumerable<Rib> Ribs { get { yield return R1; yield return R2; yield return R3; } }

        public void SetRibs(Rib R1, Rib R2, Rib R3)
        {
            this.R1 = R1;
            this.R2 = R2;
            this.R3 = R3;
        }
    }
}
