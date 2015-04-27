using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public static class Triangulation
    {
        /// <summary>
        /// Creates superstructure in defined rectangle.
        /// </summary>
        /// <param name="topLeft">Top left vertex of rectangle.</param>
        /// <param name="bottomRight">Bottom right vertex of rectangle.</param>
        /// <returns>Superstructure represented as set of triangles.</returns>
        public static IEnumerable<Triangle> CreateSuperstructure(Point topLeft, Point bottomRight)
        {
            // Initial triangles.
            var left = new Triangle();
            var right = new Triangle();
            // Define another two points of rectangle with diagonal defined by points topLeft & bottomRight.
            var bottomLeft = new Point(topLeft.X, bottomRight.Y);
            var topRight = new Point(bottomRight.X, topLeft.Y);                        
            // Define ribs of superstructure.
            var diagonal = new Rib(topLeft, bottomRight, left, right);
            var leftRib = new Rib(topLeft, bottomLeft, left, null);
            var rightRib = new Rib(topRight, bottomRight, right, null);
            var topRib = new Rib(topLeft, topRight, right, null);
            var bottomRib = new Rib(bottomLeft, bottomRight, left, null);                       
            // Set ribs for triangles.
            left.SetRibs(leftRib, bottomRib, diagonal);
            right.SetRibs(rightRib, topRib, diagonal);
            // Return superstructure.
            return new Triangle[] { left, right };
        }

        /// <summary>
        /// Computes Delaunay triangulation for set of points.
        /// </summary>
        /// <param name="points">Set of points.</param>
        /// <param name="topLeft">
        /// Such node, that any other node in input set of points would have greater or equal X & GOE Y components.
        /// </param>
        /// <param name="bottomRight">
        /// Such node, that any other node in input set of points would have lesser or equal X & LOE Y components.
        /// </param>
        /// <returns>Delaunay triangulation of input set of points.</returns>
        /// <remarks>
        /// Step 1. Create superstructure.
        /// Step 2. Perform step 2-3 for each node from input.
        /// Step 2. Add node to triangulation.
        ///     A) Find triangle in which falls this node (or on rib).
        ///     B) If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.
        ///     C) If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib
        ///        also splits in two new.
        ///     D) If node falls in triangle - split this triangle in three new.
        /// Step 3. Check Delaunay condition for new triangles and perform required changes.
        /// </remarks>
        public static ICollection<Triangle> FindDelaunayTriangulation(IEnumerable<Point> points, 
            Point topLeft, Point bottomRight)
        {            
            // Create superstructure.             
            ICollection<Triangle> triangulation = CreateSuperstructure(topLeft, bottomRight).ToList();
            foreach (var node in points)
            {
                // Add node to triangulation.
                var uncheckedTriangles = triangulation.AddNode(node);
                // Check Delaunay condition for new/modified triangles and perform required changes.
                while (uncheckedTriangles.Count > 0)
                {
                    // Take first element of set.
                    var triangle = uncheckedTriangles.First();
                    // Check Delaunay condition for this triangle and perform flip if required.
                    triangle.CheckAndFlip(uncheckedTriangles);
                }
            }
            return triangulation;
        }

        /// <summary>
        /// Check Delaunay condition for this triangle and perform flip if required.
        /// </summary>
        /// <param name="triangle">Unchecked on Delaunay condition triangle.</param>
        /// <param name="uncheckedTriangles">Set of unchecked on Delaunay condition triangles.</param>
        public static void CheckAndFlip(this Triangle triangle, HashSet<Triangle> uncheckedTriangles)
        {                        
            // Triangle that doesn't satisfies Delaunay condition.
            Triangle T;
            if (!triangle.FlipRequired(out T))
            {
                // Remove taken triangle from set because it satisfies Delaunay condition.
                uncheckedTriangles.Remove(triangle);
                return;
            }
            // Perform flip.
            Flip(triangle, T);
            // If another triangle is not in set - add this triangle.
            if (!uncheckedTriangles.Contains(T))
                uncheckedTriangles.Add(T);
            // Attention! Do not remove taken from set triangle because flip was performed
            // and we have to check it again.            
        }

        /// <summary>
        /// Add node to existing triangulation.
        /// </summary>
        /// <returns>Set of new/modified triangles.</returns>
        public static HashSet<Triangle> AddNode(this ICollection<Triangle> triangulation, Point node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check this triangle on Delaunay condition. Returns value indicating is flip required or not.
        /// </summary>
        /// <param name="T">Checked on Delaunay condition triangle.</param>
        /// <param name="Flip">Triangle, which violates Delaunay condition.</param>
        /// <returns>True - if flip required.</returns>
        public static bool FlipRequired(this Triangle T, out Triangle Flip)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs flip of two triangles.
        /// ABC & ABD => ACD & BCD
        /// </summary>
        public static void Flip(Triangle T1, Triangle T2)
        {
            var adjacentRib = T1.GetAdjacentRib(T2);
            // Vertices of the adjacent rib.
            Point A = adjacentRib.A;
            Point B = adjacentRib.B;
            // Vertices, opposite to adjacent rib.
            Point C = T1.GetOppositeNode(adjacentRib);
            Point D = T2.GetOppositeNode(adjacentRib);
            // Update of links to adjacent triangles required for next ribs:
            Rib BC = T1.GetRib(B, C);
            Rib AD = T2.GetRib(A, D);            
            // New adjacent rib.
            var CD = new Rib(C, D, T1, T2);
            // Update links to triangles.
            BC.Update(T1, T2);
            AD.Update(T2, T1);
            // Update triangles' ribs.
            T1.UpdateRib(adjacentRib, CD);
            T2.UpdateRib(adjacentRib, CD);
        }
    }
}
