using System;
using System.Collections.Generic;

namespace CGeo
{
    /// <summary>
    /// Represents rib of triangle.
    /// </summary>
    public sealed class Rib
    {
        #region Properties

        /// <summary>
        /// Vertex of this rib.
        /// </summary>
        public Point A { get; set; }

        /// <summary>
        /// Vertex of this rib.
        /// </summary>
        public Point B { get; set; }

        /// <summary>
        /// Link to adjacent triangle.
        /// </summary>        
        public Triangle T1 { get; set; }

        /// <summary>
        /// Link to adjacent triangle.
        /// </summary>
        public Triangle T2 { get; set; }
        
        /// <summary>
        /// Set of vertices of this rib.
        /// </summary>
        public IEnumerable<Point> Points { get { yield return A; yield return B; } }

        /// <summary>
        /// Set of adjacent with this rib triangles.
        /// </summary>
        public IEnumerable<Triangle> Triangles { get { yield return T1; yield return T2; } }

        #endregion
        #region Methods

        /// <summary>
        /// Update link to adjacent triangle.
        /// </summary>
        /// <param name="oldTriangle">Triangle, that already not adjacent and link to it will be updated.</param>
        /// <param name="newTriangle">Triangle that adjacent with this rib.</param>
        public void Update(Triangle oldTriangle, Triangle newTriangle)
        {
            if (T1 == oldTriangle)
                T1 = newTriangle;
            else if (T2 == oldTriangle)
                T2 = newTriangle;
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Returns adjacent by this rib to <code>T</code> triangle.
        /// </summary>
        /// <param name="T">Triangle, adjacent with this rib.</param>
        /// <returns>Triangle adjacent by this rib to <code>T</code> triangle.</returns>
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
