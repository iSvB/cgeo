using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CGeoTest")]

namespace CGeo
{
    /// <summary>
    /// Represents Delaunay triangulation.        
    /// </summary>
    /// <remarks>
    /// Algorithm:
    /// Step 1. Create superstructure.
    /// Step 2. Perform step 2-3 for each node from input.
    /// Step 3. Add node to triangulation.
    ///     A) Find triangle in which falls this node (or on rib).
    ///     B) If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.
    ///     C) If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib
    ///        also splits in two new.
    ///     D) If node falls in triangle - split this triangle in three new.
    /// Step 4. Check Delaunay condition for new triangles and perform required changes.
    /// </remarks>
    public class Triangulation : IEnumerable<Triangle>
    {
        #region Fields

        /// <summary>
        /// Set of triangles that represent Delaunay triangulation.
        /// </summary>
        private ICollection<Triangle> triangles;
        /// <summary>
        /// Dynamic cache for fast search of closest to passed node triangle.
        /// </summary>
        private DynamicCache cache;

        #endregion
        #region Methods
        #region Private & internal
        #region Triangle search

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
        #region Add node

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
            var initTriangle = cache.Get(node);
            Triangle targetTriangle = FindTriangleBySeparatingRibs(node, initTriangle);
            // If there are vertex that lies in epsilon-neighborhood of node - then ignore this node.
            if (targetTriangle.Vertices.Any(p => p.IsInEpsilonArea(node)))
                return null;
            // List of new and modified triangles.
            IEnumerable<Triangle> newAndModifiedTriangles = null;
            IEnumerable<Triangle> newTriangles = null;
            // If node fall on rib - rib splits on two new, 
            // and each triangle adjacent with this rib also splits in 2 new.
            foreach (var rib in targetTriangle.Ribs)
                if (Utils.DistanceToLine(rib.A, rib.B, node).IsInEpsilonArea(0))
                {
                    newAndModifiedTriangles = PutPointOnRib(rib, node, out newTriangles);
                    break;
                }
            // If list of triangles is null then it doesn't falls on rib and then it falls in triangle.            
            if (newAndModifiedTriangles == null)
                newAndModifiedTriangles = PutPointInTriangle(targetTriangle, node, out newTriangles);
            // Increase cache's counter of nodes.
            cache.IncrementNodeCount(newTriangles.Count());
            // Add new triangles to cache.
            foreach (var T in newTriangles)
                cache.Update(T);
            // Return set of new and modified triangles.
            return new HashSet<Triangle>(newAndModifiedTriangles);
        }

        /// <summary>
        /// Puts node on triangles rib.
        /// Rib splits on two new, and each triangle adjacent with this rib also splits in two new
        /// </summary>        
        /// <returns>New and modified triangles.</returns>
        private IEnumerable<Triangle> PutPointOnRib(Rib rib, Point node, out IEnumerable<Triangle> newTriangles)
        {
            newTriangles = null;
            IEnumerable<Triangle> modifiedTriangles = null;

            if (rib.Triangles.Contains(null))
                modifiedTriangles = PutPointOnOutsideRib(rib, node, out newTriangles);
            else
                modifiedTriangles = PutPointOnInnerRib(rib, node, out newTriangles);
            // Add new triangles to triangulation.
            foreach (var t in newTriangles)
                triangles.Add(t);
            // Return new and modified triangles.
            return newTriangles.Union(modifiedTriangles);
        }

        /// <summary>
        /// Put node on rib that lies on the bounds of superstructure.
        /// </summary>
        /// <param name="newTriangles">Collection of new triangles.</param>
        /// <returns>Modified triangles.</returns>
        private static IEnumerable<Triangle> PutPointOnOutsideRib(Rib rib, Point node,
            out IEnumerable<Triangle> newTriangles)
        {
            // Triangles.
            var T = rib.Triangles.Single(t => t != null);
            var NT = new Triangle();
            // Vertices of rib.
            var A = rib.A;
            var B = rib.B;
            // Third vertex of left triangle.
            var C = T.GetOppositeNode(rib);
            // Ribs that requires update.
            var BC = T.GetOppositeRib(A);
            // New ribs.
            var OC = new Rib(node, C, T, NT);
            var OB = new Rib(node, B, NT, null);
            // Update ribs links.
            BC.Update(T, NT);
            // Set ribs to new triangle.
            NT.SetRibs(OC, OB, BC);
            // Update rib of existing triangle.
            T.UpdateRib(BC, OC);
            // Change vertex B of old adjacent rib to passed node.
            rib.B = node;
            // Update triangles.
            T.Update();
            NT.Update();
            // Set newTriangles out parameter.
            newTriangles = new Triangle[] { NT };
            // Return modified triangles.
            return new Triangle[] { T };
        }

        /// <summary>
        /// Put node on rib that located somewhere in the bounds of superstructure 
        /// and it has adjacent triangles from both sides.
        /// </summary>
        /// <param name="newTriangles">Collection of new triangles.</param>
        /// <returns>Modified triangles.</returns>
        private static IEnumerable<Triangle> PutPointOnInnerRib(Rib rib, Point node,
            out IEnumerable<Triangle> newTriangles)
        {
            // Triangles.
            var LT = rib.T1;
            var RT = rib.T2;
            var NLT = new Triangle();
            var NRT = new Triangle();
            // Vertices of rib.
            var A = rib.A;
            var B = rib.B;
            // Third vertex of left triangle.
            var C = LT.GetOppositeNode(rib);
            // Third vertex of right triangle.
            var D = RT.GetOppositeNode(rib);
            // Ribs that requires update.
            var BC = LT.GetOppositeRib(A);
            var BD = RT.GetOppositeRib(A);
            // New ribs.
            var OC = new Rib(node, C, LT, NLT);
            var OD = new Rib(node, D, RT, NRT);
            var OB = new Rib(node, B, NLT, NRT);
            // Update ribs links.
            BC.Update(LT, NLT);
            BD.Update(RT, NRT);
            // Set ribs to new triangles.
            NLT.SetRibs(OC, OB, BC);
            NRT.SetRibs(OD, OB, BD);
            // Update ribs of existing triangles.
            LT.UpdateRib(BC, OC);
            RT.UpdateRib(BD, OD);
            // Change vertex B of old adjacent rib to passed node.
            rib.B = node;
            // Update triangles.
            LT.Update();
            RT.Update();
            NLT.Update();
            NRT.Update();
            // Set newTriangles out parameter.
            newTriangles = new Triangle[] { NLT, NRT };
            // Return modified triangles.
            return new Triangle[] { LT, RT };
        }

        /// <summary>
        /// Puts node in triangle.
        /// Split this triangle in three new.
        /// </summary>
        /// <returns>New and modified triangles.</returns>
        private IEnumerable<Triangle> PutPointInTriangle(Triangle T, Point node, 
            out IEnumerable<Triangle> newTriangles)
        {   
            // Vertices.
            // node == O
            var A = T.Vertices[0];
            var B = T.Vertices[1];
            var C = T.Vertices[2];
            // Triangles.
            var LT = new Triangle();
            var RT = new Triangle();
            // Set new triangles.
            newTriangles = new Triangle[] { LT, RT };
            // Ribs.
            var AB = T.GetRib(A, B);
            var BC = T.GetRib(B, C);
            var AC = T.GetRib(A, C);
            // New ribs.
            var OA = new Rib(node, A, LT, T);
            var OB = new Rib(node, B, LT, RT);
            var OC = new Rib(node, C, RT, T);
            // Update links to triangles of old ribs of triangle T.
            AB.Update(T, LT);
            BC.Update(T, RT);            
            // Upadte ribs of triangle T.
            T.UpdateRib(AB, OA);
            T.UpdateRib(BC, OC);
            // Set ribs to new triangles.
            LT.SetRibs(AB, OB, OA);
            RT.SetRibs(BC, OB, OC);
            // Add new triangles to triangulation.
            triangles.Add(LT);
            triangles.Add(RT);
            // Update triangles.
            T.Update();
            LT.Update();
            RT.Update();
            // Return new and modified triangles.
            return new Triangle[] { T, LT, RT };
        }

        #endregion        
        #region Delaunay condition & flip

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

            return Math.Abs( ((p0.X - p1.X) * (p0.Y - p3.Y) - (p0.X - p3.X) * (p0.Y - p1.Y)) )* sb +
                sa * Math.Abs( ((p2.X - p1.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p2.Y - p1.Y)) );
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
                if (rib == null)
                    continue;
                // Get adjacent by rib triangle.
                Flip = rib.GetAdjacent(T);
                // If there are no adjacent by this rib triangle - go to next rib.
                if (Flip == null)
                    continue;
                var node = Flip.GetOppositeNode(rib);
                var p1 = rib.A;
                var p2 = T.GetOppositeNode(rib);
                var p3 = rib.B;
                // If triangle and node doesn't satisfy Delaunay condition - flip required;
                if (!SatisfiesDelaunayCondition(p1, p2, p3, node))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Performs flip of two triangles.
        /// ABC & ABD => ACD & BCD
        /// </summary>        
        internal static void Flip(Triangle T1, Triangle T2)
        {
            int t1AdjacentRibIndex = T1.GetAdjacentRibIndex(T2);
            Rib adjacentRib = T1.Ribs[t1AdjacentRibIndex];
            int t2AdjacentRibIndex = T2.GetIndex(adjacentRib);            
            // Vertices of the adjacent rib.
            int A = T1.GetIndex(T1.Ribs[t1AdjacentRibIndex].A);
            int A2 = T2.GetIndex(T1.Vertices[A]);
            int B = T1.GetIndex(T1.Ribs[t1AdjacentRibIndex].B);
            // Vertices, opposite to adjacent rib.
            int C = T1.GetOppositeNodeIndex(t1AdjacentRibIndex);
            Point CPoint = T1.Vertices[C];
            int D = T2.GetOppositeNodeIndex(t2AdjacentRibIndex);
            Point DPoint = T2.Vertices[D];
            // Update of links to adjacent triangles required for next ribs:                                           
            int t1BC = T1.GetRibIndex(B, C);
            Rib BC = T1.Ribs[t1BC];
            int t2AD = T2.GetRibIndex(A2, D);
            Rib AD = T2.Ribs[t2AD];
            // New adjacent rib.
            Rib CD = new Rib(CPoint, DPoint, T1, T2);
            // Update links to triangles.
            BC.Update(T1, T2);
            AD.Update(T2, T1);
            // Update triangles' ribs.
            T1.UpdateRib(t1AdjacentRibIndex, CD);
            T2.UpdateRib(t2AdjacentRibIndex, CD);
            T1.UpdateRib(t1BC, AD);
            T2.UpdateRib(t2AD, BC);
            // Update triangles.
            T1.Update();
            T2.Update();
        }

        #endregion        
        #region Utils

        /// <summary>
        /// Creates superstructure in defined rectangle.
        /// </summary>
        /// <param name="topLeft">Top left vertex of rectangle.</param>
        /// <param name="bottomRight">Bottom right vertex of rectangle.</param>
        /// <returns>Superstructure represented as set of triangles.</returns>
        private IEnumerable<Triangle> CreateSuperstructure(Point topLeft, Point bottomRight)
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
            // Update triangles.
            left.Update();
            right.Update();
            // Add triangles to cache.
            cache.Initialize(left, right, left, right);
            // Return superstructure.
            return new Triangle[] { left, right };
        }                                

        #endregion
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
            if (nodes == null)
                return;
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
            cache = new DynamicCache(6, topLeft.X, bottomRight.X, topLeft.Y, bottomRight.Y);
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
