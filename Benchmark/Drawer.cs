using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CGeo;

namespace Benchmark
{
    public class Drawer
    {
        private Bitmap bmp;
        private Graphics graphics;

        public Drawer(int width, int height)
        {
            bmp = new Bitmap(width, height);
            graphics = Graphics.FromImage(bmp);
        }

        public void SaveFile(string fileName)
        {
            bmp.Save(fileName);
        }

        public void DrawPolyline(IList<CGeo.Point> points, Color color, float thickness)
        {
            for (int i = 1; i < points.Count; ++i)
                DrawLine(points[i - 1], points[i], color, thickness);
        }

        public void Draw(IEnumerable<Triangle> triangles, Color ribColor, float ribThickness, 
            Color nodeColor, float nodeDiameter)
        {
            var ribs = triangles.SelectMany(t => t.Ribs).Distinct();
            var nodes = ribs.SelectMany(n => n.Points).Distinct();
            foreach (var rib in ribs)
                DrawLine(rib.A, rib.B, ribColor, ribThickness);
            foreach (var node in nodes)
                DrawNode(node, nodeColor, nodeDiameter);
        }

        public void Draw(IEnumerable<CGeo.Point> points, Color color, float diameter)
        {
            if (points.Count() > 100)
                diameter = 0;
            foreach (var point in points)
                DrawNode(point, color, diameter);
        }

        public void DrawLine(CGeo.Point A, CGeo.Point B, Color color, float thickness)
        {
            graphics.DrawLine(new Pen(color, thickness), (float)A.X, (float)A.Y, (float)B.X, (float)B.Y);
        }

        public void DrawNode(CGeo.Point node, Color color, float diameter)
        {
            if (diameter == 0)
            {
                bmp.SetPixel((int)Math.Floor(node.X), (int)Math.Floor(node.Y), color);
                return;
            }
            graphics.FillEllipse(new SolidBrush(color), (float)node.X - diameter / 2, (float)node.Y - diameter / 2, 
                diameter, diameter);
        }
    }
}
