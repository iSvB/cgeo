using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGeo;
using System.IO;

namespace Benchmark
{
    class Program
    {
        private static int maxX = 1000;
        private static int maxY = 1000;
        private static Point topLeft = new Point(0, 0);
        private static Point bottomRight = new Point(maxX, maxY);

        static void Main(string[] args)
        {
            var sizes = new int[] { 100, 1000, 10000, 100000, 1000000 };
            foreach (var size in sizes)
                RunTest(size);
            Console.WriteLine("Done!");
            Console.ReadKey(false);
        }

        private static void RunTest(int size)
        {
            var nodes = GeneratePoints(size);
            var sw = Stopwatch.StartNew();
            var triangulation = new Triangulation(topLeft, bottomRight, nodes);
            sw.Stop();
            var logMsg = string.Format("{0}   10^{1}: {2}", DateTime.Now, Math.Log10(size), sw.Elapsed);
            Console.WriteLine(logMsg);            
            File.AppendAllText("log.txt", DateTime.Now.ToString() + "\t" + logMsg + "\r\n");
            triangulation = null;
            GC.Collect(2);
        }

        private static IEnumerable<Point> GeneratePoints(int count)
        {
            var prng = new Random((int)DateTime.Now.Ticks);
            var result = new Point[count];
            for (int i = 0; i < count; ++i)
            {
                var x = prng.NextDouble() * maxX;
                var y = prng.NextDouble() * maxY;
                result[i] = new Point(x, y);
            }
            return result;
        }
    }
}
