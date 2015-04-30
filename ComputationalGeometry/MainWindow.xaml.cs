using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CGeo;
using DColor = System.Drawing.Color;

namespace ComputationalGeometry
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var bmp = new Bitmap(300, 300);

            var t = new Triangulation(new CGeo.Point(0, 0), new CGeo.Point(299, 299));

            var p1 = new CGeo.Point(50, 50);
            var p2 = new CGeo.Point(250, 250);
            var p3 = new CGeo.Point(250, 50);
            var p4 = new CGeo.Point(50, 250);

            t.Add(p1);
            t.Add(p2);
            t.Add(p3);
            //t.Add(p4);

            var p5 = new CGeo.Point(130, 150);
            t.Add(p5);

            //var points = GeneratePoints(4, 299, 299);
            //foreach (var point in points)
                //t.Add(point);

            bmp.Draw(t, DColor.Aqua, DColor.Green);
            image.Source = bmp.ToImageSource();            
        }        

        public IEnumerable<CGeo.Point> GeneratePoints(int count, int maxX, int maxY)
        {
            var result = new List<CGeo.Point>();
            var prng = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < count; ++i)
            {
                double x = prng.NextDouble() * maxX;
                double y = prng.NextDouble() * maxY;
                result.Add(new CGeo.Point(x, y));
            }
            return result;
        }
    }
}
