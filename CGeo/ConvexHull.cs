using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGeo
{
    public static class ConvexHull
    {
        #region Nested data structures
        
        private class GrahamPoint
        {
            public GrahamPoint(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }

            #region Properties
            
            public double X { get; set; }
            public double Y { get; set; }
            public double PolarAngle { get; set; }
            public double Length { get; set; }

            #endregion
            #region Conversion operators

            public static implicit operator GrahamPoint(Point p)
            {
                return new GrahamPoint(p.X, p.Y);
            }

            public static implicit operator Point(GrahamPoint p)
            {
                return new Point(p.X, p.Y);
            }

            #endregion
        }    
        
        #endregion

        public static IList<Point> GrahamScan(IList<Point> points)
        {
            if (points.Count <= 3)
                return points.ToArray();            
            // Points with additional properties of polar angle and position vector length.
            var gPoints = new List<GrahamPoint>(points.Count);
            foreach (Point p in points)
                gPoints.Add(p);
            // Bottom left point index.
            int p0Index = BottomLeft(points);
            // Compute polar angle for each point.
            // Actually only cosinus of angle, but because all points lays in interval (0; pi) it is enough.
            Parallel.ForEach(gPoints, point =>
            {
                double Vx = point.X - points[p0Index].X;
                double Vy = point.Y - points[p0Index].Y;
                point.Length = Math.Sqrt(Vx * Vx + Vy * Vy);
                point.PolarAngle = Vx / point.Length;
            });
            // Sort by polar angle.
            // If angles equals then by distance to p0.
            List<GrahamPoint> stack =
                gPoints.AsParallel().OrderByDescending(p => p.PolarAngle).ThenBy(p => p.Length).ToList();
            // Convex hull.
            List<GrahamPoint> hull = new List<GrahamPoint>(points.Count);
            // Bottom left node definitely belongs to hull.
            // Node with minimal polar angle belongs too.
            hull.Add(stack[0]);
            hull.Add(stack[1]);
            // Scan.
            for (int i = 2; i < stack.Count; ++i)
            {
                // While axis that defined by penultimate and last added to hull node, and axis that defined by
                // current node from input and last node from hull forms left-handed coordinate system
                // delete last point from hull.
                while (hull.Count > 1 && Utils.IsLeftHanded(hull[hull.Count - 2], hull[hull.Count - 1], stack[i]))
                    hull.RemoveAt(hull.Count - 1);
                // Now these nodes forms right-handed coordinate system, therefore add this node to hull.
                hull.Add(stack[i]);
            }
            // Converts back to points.
            var resultHull = new List<Point>(hull.Count);
            foreach (GrahamPoint gPoint in hull)
                resultHull.Add(gPoint);
            return resultHull;
        }

        // Находит нижнюю точку
        // Если таких точек несколько, то возвращает самую левую из них
        private static int BottomLeft(IList<Point> points)
        {
            int result = 0;
            for (int i = 1; i < points.Count; ++i)
            {
                // Если точка находится ниже, 
                // либо на одной координате по Y, но левее - запоминаем её                
                if (points[i].Y < points[result].Y ||
                   (points[i].Y == points[result].Y && points[i].X < points[result].X))
                    result = i;
            }
            return result;
        }
    }
}
