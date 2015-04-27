using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public static class Triangulation
    {
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

        public static ICollection<Triangle> FindDelaunayTriangulation(IEnumerable<Point> points)
        {
            // Step 1. Create superstructure.
            // Step 2. Perform step 2-3 for each node from input.
            // Step 2. Add node to triangulation.
            //     A) Find triangle in which falls this node (or on rib).
            //     B) If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.
            //     C) If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib
            //        also splits in two new.
            //     D) If node falls in triangle - split this triangle in three new.
            // Step 3. Check Delaunay condition for new triangles and perform required changes.
            throw new NotImplementedException();
        }

        public static void AddNode(this ICollection<Triangle> triangulation, Point node)
        {
            throw new NotImplementedException();
        }

        public static bool SatisfiesDelaunayCondition(Triangle T, Point node)
        {
            throw new NotImplementedException();
        }
    }
}
