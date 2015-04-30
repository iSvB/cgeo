using CGeo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ComputationalGeometry
{
    public static class Drawer
    {
        public static BitmapImage ToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static void Draw(this Bitmap bmp, IEnumerable<Triangle> triangles, Color ribColor, Color nodeColor)
        {
            var ribs = triangles.SelectMany(t => t.Ribs).Distinct();
            var nodes = ribs.SelectMany(r => r.Points);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var rib in ribs)
                    g.Draw(rib, ribColor);
                foreach (var node in nodes)
                    bmp.Draw(node, nodeColor);
            }
        }

        public static void Draw(this Graphics g, Rib rib, Color color)
        {
            var A = new System.Drawing.PointF((float)rib.A.X, (float)rib.A.Y);
            var B = new System.Drawing.PointF((float)rib.B.X, (float)rib.B.Y);
            var pen = new Pen(color, 1);
            g.DrawLine(pen, A, B);
        }

        public static void Draw(this Bitmap bmp, CGeo.Point point, Color color)
        {
            bmp.SetPixel((int)Math.Floor(point.X), (int)Math.Floor(point.Y), color);
        }
    }
}
