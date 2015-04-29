using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public sealed class Rib
    {
        #region Properties

        public Point A;
        public Point B;

        public Triangle T1;
        public Triangle T2;

        public IEnumerable<Point> Points { get { yield return A; yield return B; } }
        public IEnumerable<Triangle> Triangles { get { yield return T1; yield return T2; } }

        #endregion
        #region Methods

        public void Update(Triangle oldTriangle, Triangle newTriangle)
        {
            if (T1 == oldTriangle)
                T1 = newTriangle;
            else if (T2 == oldTriangle)
                T2 = newTriangle;
            else
                throw new ArgumentException();
        }

        public Triangle GetAdjacent(Triangle T)
        {
            if (T == T1)
                return T2;
            if (T == T2)
                return T1;
            throw new ArgumentException();
        }

        #endregion
        #region Constructors

        public Rib() { }

        public Rib(Point A, Point B, Triangle T1, Triangle T2)
        {
            this.A = A;
            this.B = B;
            this.T1 = T1;
            this.T2 = T2;
        }

        #endregion
    }
}
