using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CGeoTest")]

namespace CGeo
{
    /// <summary>
    /// Computes Delaunay triangulation.        
    /// </summary>
    /// <remarks>
    /// Algorithm:
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
    public class Triangulation : IEnumerable<Triangle>
    {
        #region Fields

        /// <summary>
        /// Set of triangles that represent Delaunay triangulation.
        /// </summary>
        private ICollection<Triangle> triangles;

        #endregion
        #region Methods
        #region Private & internal

        /// <summary>
        /// Creates superstructure in defined rectangle.
        /// </summary>
        /// <param name="topLeft">Top left vertex of rectangle.</param>
        /// <param name="bottomRight">Bottom right vertex of rectangle.</param>
        /// <returns>Superstructure represented as set of triangles.</returns>
        private static IEnumerable<Triangle> CreateSuperstructure(Point topLeft, Point bottomRight)
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
        /// Check Delaunay condition for this triangle and perform flip if required.
        /// </summary>
        /// <param name="triangle">Unchecked on Delaunay condition triangle.</param>
        /// <param name="uncheckedTriangles">Set of unchecked on Delaunay condition triangles.</param>
        internal static void CheckAndFlip(Triangle triangle, HashSet<Triangle> uncheckedTriangles)
        {
            // Triangle that doesn't satisfies Delaunay condition.
            Triangle T;
            if (!FlipRequired(triangle, out T))
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
        /// Check this triangle and adjacent nodes on Delaunay condition. 
        /// Returns value indicating is flip required or not.
        /// </summary>
        /// <param name="T">Checked on Delaunay condition triangle.</param>
        /// <param name="Flip">Triangle, which violates Delaunay condition.</param>
        /// <returns>True - if flip required, otherwise - false.</returns>
        internal static bool FlipRequired(Triangle T, out Triangle Flip)
        {
            Flip = null;
            foreach (var rib in T.Ribs)
            {
                // Get adjacent by rib triangle.
                Flip = rib.GetAdjacent(T);
                // If there are no adjacent by this rib triangle - go to next rib.
                if (Flip == null)
                    continue;
                var node = Flip.GetOppositeNode(rib);
                var p1 = rib.A;
                var p2 = T.GetOppositeNode(rib);                
                var p3 = rib.B;
                // Vertices should be in clockwise order.
                if (!Utils.IsClockwiseOrdered(p1, p2, p3))
                {
                    // Swap vertices p1 & p3 to sort them in clockwise order.
                    var buf = p1;
                    p1 = p3;
                    p3 = buf;
                }
                // If triangle and node doesn't satisfy Delaunay condition - flip required;
                if (!SatisfiesDelaunayCondition(p1, p2, p3, node))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks Delaunay condition for triangle and node <code>node</code>.
        /// </summary>        
        /// <param name="node">Adjacent node.</param>
        /// <returns>True - if satisfies, otherwise - false.</returns>
        /// <remarks>
        /// Nodes (p1, p2, p3) - should be in clockwise order.
        /// Node <code>node</code> - should be opposite to p2 and have adjacent ribs with p1 & p3 
        /// in rectangle (p1, p2, p3, node).
        /// </remarks>
        internal static bool SatisfiesDelaunayCondition(Point p1, Point p2, Point p3, Point node)
        {
            double sa, sb;
            ModifiedCheckOfOppositeAnglesSum(node, p1, p2, p3, out sa, out sb);
            // If sa && sb < 0 => a & b > pi/2 => doesn't satisfies.
            if (sa < 0 && sb < 0) return false;
            // If sa && sb >= 0 => a & b < pi/2 => satisfies.
            if (sa >= 0 && sb >= 0) return true;
            // If sin(a+b) >= 0 - satisfies, otherwise not.
            if (OppositeAnglesSum(node, p1, p2, p3, sa, sb) >= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Performs full check of opposite angles sum.
        /// </summary>
        /// <returns>
        /// Sinus of sum of angles (p1,p2,p3) & (p1, p0, p3) multiplied by some constant positive value.
        /// </returns>
        /// <remarks>
        /// Nodes (p1, p2, p3) - should be in clockwise order.
        /// Node <code>node</code> - should be opposite to p2 and have adjacent ribs with p1 & p3 
        /// in rectangle (p1, p2, p3, node).
        /// </remarks>
        private static double OppositeAnglesSum(Point p0, Point p1, Point p2, Point p3, double sa, double sb)
        {
            #region Math
            /*
             * a + b <= pi    <=>    sin(a + b) >= 0    <=>    sin A * cos B + cos A * sin B >= 0
             *              
             * 
             *                    (x0 - x1) * (x0 - x3) + (y0 - y1) * (y0 - y3)
             * cos A = ---------------------------------------------------------------------
             *         sqrt( (x0 - x1)^2 + (y0 - y1)^2 ) * sqrt( (x0 - x3)^2 + (y0 - y3)^2 )
             * 
             *                    (x2 - x1) * (x2 - x3) + (y2 - y1) * (y2 - y3)
             * cos B = ---------------------------------------------------------------------
             *         sqrt( (x2 - x1)^2 + (y2 - y1)^2 ) * sqrt( (x2 - x3)^2 + (y2 - y3)^2 )
             *         
             *                    (x0 - x1) * (y0 - y3) - (x0 - x3) * (y0 - y1)
             * sin A = ---------------------------------------------------------------------
             *         sqrt( (x0 - x1)^2 + (y0 - y1)^2 ) * sqrt( (x0 - x3)^2 + (y0 - y3)^2 )
             *         
             *                    (x2 - x1) * (y2 - y3) - (x2 - x3) * (y2 - y1)
             * sin B = ---------------------------------------------------------------------
             *         sqrt( (x2 - x1)^2 + (y2 - y1)^2 ) * sqrt( (x2 - x3)^2 + (y2 - y3)^2 )
             *                      
             * 
             * ( (x0 - x1)*(y0 - y3) - (x0 - x3)*(y0 - y1) ) * ( (x2 - x1)*(x2 - x3) + (y2 - y1)*(y2 - y3) ) +
             * ( (x0 - x1)*(x0 - x3) + (y0 - y1)*(y0 - y3) ) * ( (x2 - x1)*(y2 - y3) - (x2 - x3)*(y2 - y1) ) >= 0 
             * 
             * sa = (x0 - x1) * (x0 - x3)  +  (y0 - y1) * (y0 - y3)
             * sb = (x2 - x1) * (x2 - x3)  +  (y2 - y1) * (y2 - y3)
             * 
             * x0 - x1 = exp1
             * y0 - y3 = exp2
             * x0 - x3 = exp3
             * y0 - y1 = exp4
             * 
             * x2 - x1 = exp5
             * x2 - x3 = exp6
             * y2 - y1 = exp7
             * y2 - y3 = exp8
             * 
             * ( exp1 * exp2 - exp3 * exp4 ) * ( exp5 * exp6 + exp7 * exp8 ) +
             * ( exp1 * exp3 + exp4 * exp2 ) * ( exp5 * exp8 - exp6 * exp7 ) >= 0 
             * 
             * sa = exp1 * exp3  +  exp4 * exp2
             * sb = exp5 * exp6  +  exp7 * exp8
             * 
             * ( exp1 * exp2 - exp3 * exp4 ) * sb  +  sa * ( exp5 * exp8 - exp6 * exp7 ) >= 0 
             * 
             */
            #endregion

            return ((p0.X - p1.X) * (p0.Y - p3.Y) - (p0.X - p3.X) * (p0.Y - p1.Y)) * sb + 
                sa * ((p2.X - p1.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p2.Y - p1.Y));
        }

        /// <summary>
        /// Performs modified check of opposite angles sum.
        /// </summary>
        /// <param name="sa">Coefficient of angle A.</param>
        /// <param name="sb">Coefficient of angle B.</param>
        /// <remarks>
        /// Nodes (p1, p2, p3) - should be in clockwise order.
        /// Node <code>node</code> - should be opposite to p2 and have adjacent ribs with p1 & p3 
        /// in rectangle (p1, p2, p3, node).
        /// </remarks>
        private static void ModifiedCheckOfOppositeAnglesSum(Point p0, Point p1, Point p2, Point p3, 
            out double sa, out double sb)
        {            
            /*
             * sa = (x0 - x1) * (x0 - x3)  +  (y0 - y1) * (y0 - y3)
             * sb = (x2 - x1) * (x2 - x3)  +  (y2 - y1) * (y2 - y3)
             */
            sa = (p0.X - p1.X) * (p0.X - p3.X) + (p0.Y - p1.Y) * (p0.Y - p3.Y);
            sb = (p2.X - p1.X) * (p2.X - p3.X) + (p2.Y - p1.Y) * (p2.Y - p3.Y);
        }

        /// <summary>
        /// Performs flip of two triangles.
        /// ABC & ABD => ACD & BCD
        /// </summary>        
        internal static void Flip(Triangle T1, Triangle T2)
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
            T1.UpdateRib(BC, AD);
            T2.UpdateRib(AD, BC);
        }

        /// <summary>
        /// Add node to triangulation.
        /// </summary>
        /// <returns>Set of new/modified triangles.</returns>
        /// <remarks>
        /// 1) Find triangle in which falls this node (or on rib).
        /// 2) If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.
        /// 3) If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib
        ///    also splits in two new.
        /// 4) If node falls in triangle - split this triangle in three new.
        /// </remarks>
        private HashSet<Triangle> AddNode(Point node)
        {
            var initTriangle = triangles.First();
            Triangle targetTriangle = FindTriangleBySeparatingRibs(node, initTriangle);
            // If there are vertex that lies in epsilon-neighborhood of node - then ignore this node.
            if (targetTriangle.Points.Any(p => p.IsInEpsilonArea(node)))
                return null;
            // List of new and modified triangles.
            var newAndModifiedTriangles = new List<Triangle>() { targetTriangle };
            // If node fall on rib - rib splits on two new, 
            // and each triangle adjacent with this rib also splits in 2 new.
            foreach (var rib in targetTriangle.Ribs)
                if (Utils.DistanceToLine(rib.A, rib.B, node).IsInEpsilonArea(0))
                    newAndModifiedTriangles.AddRange(PutPointOnRib(targetTriangle, rib, node));
            // If in list of triangles only one element then it doesn't falls on rib and then it falls in triangle.            
            if (newAndModifiedTriangles.Count == 1)
                newAndModifiedTriangles.AddRange(PutPointInTriangle(targetTriangle, node));                        
            // Return set of new and modified triangles.
            return new HashSet<Triangle>(newAndModifiedTriangles);
        }

        /// <summary>
        /// Puts node on triangles rib.
        /// Rib splits on two new, and each triangle adjacent with this rib also splits in two new
        /// </summary>        
        /// <returns>New triangles.</returns>
        private static IEnumerable<Triangle> PutPointOnRib(Triangle T, Rib rib, Point node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Puts node in triangle.
        /// Split this triangle in three new.
        /// </summary>
        /// <returns>New triangles.</returns>
        private static IEnumerable<Triangle> PutPointInTriangle(Triangle T, Point node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find triangle in which falls <code>node</code>.
        /// </summary>        
        /// <param name="T">
        /// Initial triangle for search algorithm.
        /// Works faster with triangles located closer to node.
        /// </param>
        /// <returns>Triangle in which falls passed node.</returns>
        private static Triangle FindTriangleBySeparatingRibs(Point node, Triangle T)
        {
            Rib separatingRib = GetSeparatingRib(T, node);
            while (separatingRib != null)
            {                
                T = separatingRib.GetAdjacent(T);
                separatingRib = GetSeparatingRib(T, node);
            }
            return T;
        }

        /// <summary>
        /// Find rib that separates target node and any vertex of triangle.
        /// </summary>
        /// <returns>
        /// Rib that separates target node and any vertex of triangle.
        /// Returns null if target node located within triangle.
        /// </returns>
        internal static Rib GetSeparatingRib(Triangle T, Point targetNode)
        {
            foreach (var rib in T.Ribs)
                if (Utils.IsSeparated(rib.A, rib.B, targetNode, T.GetOppositeNode(rib)))
                    return rib;
            return null;
        }        

        #endregion
        #region Public        

        /// <summary>
        /// Add node to triangulation.
        /// </summary>
        public void Add(Point node)
        {
            // Add node to triangulation.
            var uncheckedTriangles = AddNode(node);
            if (uncheckedTriangles == null)
                return;
            // Check Delaunay condition for new/modified triangles and perform required changes.
            while (uncheckedTriangles.Count > 0)
            {
                // Take first element of set.
                var triangle = uncheckedTriangles.First();
                // Check Delaunay condition for this triangle and perform flip if required.
                CheckAndFlip(triangle, uncheckedTriangles);
            }
        }

        /// <summary>
        /// Add set of nodes to triangulation.
        /// </summary>
        public void Add(IEnumerable<Point> nodes)
        {
            foreach (var node in nodes)
                Add(node);
        }

        #endregion
        #endregion
        #region Constructors 

        /// <summary>
        /// Initialize instance of <code>Triangulation</code> class.
        /// </summary>
        /// <param name="topLeft">
        /// Top left vertex of rectangle, which occupy all possible nodes of triangulation.
        /// </param>
        /// <param name="bottomRight">
        /// Bottom right vertex of rectangle, which occupy all possible nodes of triangulation.
        /// </param>
        /// <param name="nodes">Nodes of triangulation.</param>
        public Triangulation(Point topLeft, Point bottomRight, IEnumerable<Point> nodes = null) 
        {
            triangles = CreateSuperstructure(topLeft, bottomRight).ToList();
            // If we don't add any nodes then superstructure might not satisfy Delaunay condition 
            // therefore we should check it.            
            if (nodes == null)
                CheckAndFlip(this.First(), new HashSet<Triangle>(this));
            // Add nodes to triangulation.
            Add(nodes);
        }

        #endregion                                       
        #region IEnumerable<Triangle> implementation

        public IEnumerator<Triangle> GetEnumerator()
        {
            return triangles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
