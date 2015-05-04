using System;
using System.Linq;

namespace CGeo
{
    public class Triangle
    {
        #region Properties

        /// <summary>
        /// Ribs of triangle.
        /// </summary>
        /// <remarks>
        /// First rib matches vertices 0 & 1.
        /// Second rib matches vertices 1 & 2.
        /// Third rib matches vertices 0 & 2.
        /// </remarks>
        public Rib[] Ribs { get; } = new Rib[3];        

        /// <summary>
        /// Vertices of triangle.
        /// </summary>
        /// <remarks>
        /// First vertex belongs to ribs 0 & 2.
        /// Second vertex belongs to ribs 0 & 1.
        /// Third vertex belongs to ribs 1 & 2.
        /// </remarks>
        public Point[] Vertices { get; } = new Point[3];

        #endregion        
        #region Methods

        /// <summary>
        /// Returns opposite to passed vertex rib.
        /// </summary>
        /// <param name="vertex">Vertex of triangle.</param>
        /// <returns>Opposite to passed vertex rib.</returns>
        public Rib GetOppositeRib(Point vertex)
        {
            int i = 0;
            for (; i < 3; ++i)
                if (Vertices[i].Equals(vertex))
                    break;
            switch (i)
            {
                case 0: return Ribs[1];
                case 1: return Ribs[2];
                case 2: return Ribs[0];
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns opposite to passsed rib node.
        /// </summary>
        /// <param name="rib">Rib that belongs to this triangle.</param>
        /// <returns>Opposite to passed rib vertex.</returns>
        public Point GetOppositeNode(Rib rib)
        {
            int i = 0;
            for (; i < 3; ++i)
                if (Ribs[i] == rib)
                    break;
            switch (i)
            {
                case 0: return Vertices[2];
                case 1: return Vertices[0];
                case 2: return Vertices[1];
            }
            throw new ArgumentException();
        }        
        
        /// <summary>
        /// Returns index of adjacent rib.
        /// </summary>
        /// <param name="T">Adjacent triangle.</param>
        /// <returns>Index of adjacent rib.</returns>
        public int GetAdjacentRibIndex(Triangle T)
        {
            for (int i = 0; i < 3; ++i)
            {
                var rib = Ribs[i];
                if (rib.T1 == T || rib.T2 == T)
                    return i;                                    
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns index of opposite to passed rib node.
        /// </summary>
        /// <param name="ribIndex">Index of rib.</param>
        /// <returns>Index of opposite to passed rib node.</returns>
        public int GetOppositeNodeIndex(int ribIndex)
        {            
            switch (ribIndex)
            {
                case 0: return 2;
                case 1: return 0;
                case 2: return 1;
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns index of vertex in array of vertices.
        /// </summary>
        /// <returns>Index of passed vertex.</returns>
        public int GetIndex(Point vertex)
        {            
            for (int i = 0; i < 3; ++i)
                if (Vertices[i].Equals(vertex))
                    return i;
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns index of passed rib in array of ribs.
        /// </summary>
        /// <returns>Index of passed rib.</returns>
        public int GetIndex(Rib rib)
        {            
            for (int i = 0; i < 3; ++i)
                if (Ribs[i] == rib)
                    return i;
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns index of rib that contains passed nodes.
        /// </summary>
        /// <returns>Index of rib that contains passed nodes.</returns>
        public int GetRibIndex(int A, int B)
        {
            int a = Math.Min(A, B);
            int b = Math.Max(A, B);
            // a == 0.
            // b == 1 | 2.
            if (a == 0)
            {
                if (b == 1)
                    return 0;
                // b == 2.
                return 2;
            }
            // a == 1.
            // b == 2.
            return 1;
        }

        /// <summary>
        /// Update link to rib by index <code>index</code> with a link to rib <code>newRib</code>.
        /// </summary>
        /// <param name="index">Index of updated rib.</param>
        /// <param name="newRib">
        /// New rib that will be accessible by index <code>index</code> ufter update.
        /// </param>
        public void UpdateRib(int index, Rib newRib)
        {            
            Ribs[index] = newRib;
        }

        /// <summary>
        /// Returns rib that contains passed points.
        /// </summary>
        /// <returns>Rib that contains passed points.</returns>
        public Rib GetRib(Point A, Point B)
        {
            foreach (var rib in Ribs)
                if (rib.A.Equals(A) && rib.B.Equals(B) || rib.A.Equals(B) && rib.B.Equals(A))
                    return rib;
            throw new ArgumentException();
        }

        /// <summary>
        /// Updates rib <code>oldRib</code> with a <code>newRib</code>.
        /// </summary>
        public void UpdateRib(Rib oldRib, Rib newRib)
        {
            if (Ribs[0] == oldRib)
                Ribs[0] = newRib;
            else if (Ribs[1] == oldRib)
                Ribs[1] = newRib;
            else if (Ribs[2] == oldRib)
                Ribs[2] = newRib;
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Determines whether this triangle adjacent with <code>T</code> or not.
        /// </summary>
        /// <returns>True - if <code>T</code> is adjacent with this triangle, otherwise - false.</returns>
        public bool IsAdjacent(Triangle T)
        {
            return Ribs.Any(r => r.Triangles.Contains(T));
        }

        /// <summary>
        /// Update triangle's internal representation.
        /// Sorting ribs and vertices in such way that first rib would match rib that lies on first & second
        /// vertices, second rib would match rib that lies on second & third vertices, and third rib would match
        /// rib that lies on firs & third vertices.
        /// </summary>
        public unsafe void Update()
        {
            Rib a = Ribs[0];
            Rib b = null;
            Rib c = null;
            var A = a.A;
            var B = a.B;
            Point C = new Point();
            var r1 = Ribs[1];
            if (r1.A.Equals(A))
            {
                b = Ribs[2];
                c = r1;
                C = r1.B;
            }
            else if (r1.B.Equals(A))
            {
                b = Ribs[2];
                c = r1;
                C = r1.A;
            }
            else if (r1.A.Equals(B))
            {
                b = r1;
                c = Ribs[2];
                C = r1.B;
            }
            else if (r1.B.Equals(B))
            {
                b = r1;
                c = Ribs[2];
                C = r1.A;
            }
            fixed (Point* points = Vertices)
            {
                points[0] = A;
                points[1] = B;
                points[2] = C;
            }          
            Ribs[0] = a;
            Ribs[1] = b;
            Ribs[2] = c;
        }

        /// <summary>
        /// Update collection of triangles.
        /// </summary>
        /// <param name="triangles">Collection that will be updated.</param>
        public static void Update(params Triangle[] triangles)
        {
            foreach (var t in triangles)
                t.Update();
        }

        /// <summary>
        /// Set ribs to triangle.
        /// </summary>
        public void SetRibs(Rib R1, Rib R2, Rib R3)
        {
            Ribs[0] = R1;
            Ribs[1] = R2;
            Ribs[2] = R3;
        }

        #endregion
    }
}
