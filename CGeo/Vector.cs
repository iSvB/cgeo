using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public struct Vector
    {
        public double X;
        public double Y;
        
        public Vector(double X, double Y) : this()
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Initialise vector AB.
        /// </summary>
        /// <param name="A">Origin point.</param>
        /// <param name="B">Terminal point.</param>
        public Vector(Point A, Point B) : this()
        {
            Initialize(A, B);
        }

        public Vector(Rib rib) : this()
        {
            Initialize(rib.A, rib.B);
        }

        private void Initialize(Point A, Point B)
        {
            X = B.X - A.X;
            Y = B.Y - A.Y;
        }
    }
}
