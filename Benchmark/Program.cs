using System;
using System.Collections.Generic;
using System.Diagnostics;
using CGeo;
using System.IO;
using System.Drawing;

using CGPoint = CGeo.Point;

namespace Benchmark
{
    class Program
    {        
        private static int maxX = 1000;
        private static int maxY = 1000;
        private static CGPoint topLeft = new CGPoint(0, 0);
        private static CGPoint bottomRight = new CGPoint(maxX, maxY);
        private static int loopCount = 1;
        private static int[] sizes = new int[] { 100, 1000, 10000, 100000, 1000000 };

        private static Color ribColor = Color.Black;
        private static float ribThickness = 1;
        private static Color nodeColor = Color.Red;
        private static float nodeDiameter = 4;
        private static Color hullColor = Color.Green;
        private static float hullThickness = 2;

        private static string dirName = "Drawings";        

        static void Main(string[] args)
        {                        
            if (args.Length > 0)
            {
                loopCount = int.Parse(args[0]);
                var sizeList = new List<int>();
                for (int i = 1; i < args.Length; ++i)
                    sizeList.Add(int.Parse(args[i]));
                sizes = sizeList.ToArray();                 
            }
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            LoopTest(loopCount, sizes);            
            if (args.Length == 0)
                Console.ReadKey(false);
        }

        private static void LoopTest(int count, int[] sizes)
        {
            for (int i = 1; i <= count; ++i)
            {
                Console.WriteLine("Triangulation:");
                foreach (var size in sizes)
                    RunTriangulationTest(size, i);
                Console.WriteLine("Convex hull:");
                foreach (var size in sizes)
                    RunConvexHullTest(size, i);
                Console.WriteLine($"\t#{i}: Done!\n");
            }            
        }

        private static void RunTriangulationTest(int size, int iteration)
        {
            var nodes = GeneratePoints(size); 
            var sw = Stopwatch.StartNew();
            var triangulation = new Triangulation(topLeft, bottomRight, nodes);
            sw.Stop();
            var logMsg = $"{DateTime.Now}   10^{Math.Log10(size)}: {sw.Elapsed}";
            Console.WriteLine(logMsg);            
            File.AppendAllText("log.txt", DateTime.Now.ToString() + "\t" + logMsg + "\r\n");
            DrawTriangulation(triangulation, size, sw.Elapsed, iteration);
            triangulation = null; 
            GC.Collect(2);
        }

        private static void DrawTriangulation(Triangulation triangulation, int size, TimeSpan elapsed, int iteration)
        {
            var drawer = new Drawer(maxX + 1, maxY + 1);
            drawer.Draw(triangulation, ribColor, ribThickness, nodeColor, nodeDiameter);
            var filename = $@"triangulation_{iteration}_{size}_{DateTime.Now:hh-mm-ss}_in_{elapsed:mm\-ss\.fff}.bmp";
            drawer.SaveFile(filename);
            var current = Directory.GetCurrentDirectory();
            File.Move($@"{current}\{filename}", $@"{current}\{dirName}\{filename}");
        }

        private static void RunConvexHullTest(int size, int iteration)
        {
            var nodes = GeneratePoints(size);
            var sw = Stopwatch.StartNew();
            var convexHull = ConvexHull.GrahamScan(nodes);
            sw.Stop();
            var logMsg = $"{DateTime.Now}   10^{Math.Log10(size)}: {sw.Elapsed}";
            Console.WriteLine(logMsg);
            File.AppendAllText("log.txt", DateTime.Now.ToString() + "\t" + logMsg + "\r\n");
            DrawConvexHull(convexHull, nodes, size, sw.Elapsed, iteration);
            convexHull = null;
            GC.Collect(2);            
        }

        private static void DrawConvexHull(IList<CGPoint> convexHull, IEnumerable<CGPoint> nodes,
            int size, TimeSpan elapsed, int iteration)
        {
            var drawer = new Drawer(maxX + 1, maxY + 1);
            drawer.DrawPolyline(convexHull, hullColor, hullThickness);
            drawer.Draw(nodes, nodeColor, nodeDiameter);
            var filename = $@"hull_{iteration}_{size}_{DateTime.Now:hh-mm-ss}_in_{elapsed:mm\-ss\.fff}.bmp";
            drawer.SaveFile(filename);
            var current = Directory.GetCurrentDirectory();
            File.Move($@"{current}\{filename}", $@"{current}\{dirName}\{filename}");
        }

        private static IList<CGPoint> GeneratePoints(int count)
        {
            var prng = new Random((int)DateTime.Now.Ticks);
            var result = new List<CGPoint>(count);
            for (int i = 0; i < count; ++i)
            {
                var x = prng.NextDouble() * maxX;
                var y = prng.NextDouble() * maxY;
                result.Add(new CGPoint(x, y));
            }
            return result;
        }
    }
}
