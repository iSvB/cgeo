using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public class Triangle
    {
        #region Fields

        private Dictionary<Rib, Point> oppositeNodes = new Dictionary<Rib, Point>(3);
        private Dictionary<Point, Rib> oppositeRibs = new Dictionary<Point, Rib>(3);        

        #endregion
        #region Properties

        public Rib[] Ribs { get; } = new Rib[3];        

        public Point[] Points { get; } = new Point[3];

        #endregion        
        #region Methods

        public Rib GetOppositeRib(Point vertex)
        {
            Rib result;
            if (oppositeRibs.TryGetValue(vertex, out result))
                return result;
            throw new ArgumentException();
        }

        public Point GetOppositeNode(Rib rib)
        {
            Point result;
            if (oppositeNodes.TryGetValue(rib, out result))
                return result;
            throw new ArgumentException();
        }        

        public Rib GetAdjacentRib(Triangle T)
        {
            try
            {
                return Ribs.First(r => T.Ribs.Contains(r));
            }
            catch (Exception)
            {
                int x = 0;
                throw;
            }
        }

        public Rib GetRib(Point A, Point B)
        {
            foreach (var rib in Ribs)
            {
                if (rib.A.Equals(A))
                    if (rib.B.Equals(B))
                        return rib;
                    else
                        continue;
                if (rib.A.Equals(B))
                    if (rib.B.Equals(A))
                        return rib;
            }
            throw new ArgumentException();
        }

        public void UpdateRib(Rib oldRib, Rib newRib)
        {
            if (Ribs[0] == oldRib)
                Ribs[0] = newRib;
            else if (Ribs[1] == oldRib)
                Ribs[1] = newRib;
            else if (Ribs[2] == oldRib)
                Ribs[2] = newRib;
            else
                throw new ArgumentException();
        }

        public bool IsAdjacent(Triangle T)
        {
            return Ribs.Any(r => r.Triangles.Contains(T));
        }

        public unsafe void Update()
        {            
            #region Nodes
            var A = Ribs[0].A;
            var B = Ribs[0].B;
            Point C = new Point();
            for (var i = 1; i < 3; ++i)
                foreach (var node in Ribs[i].Points)
                    if (!node.Equals(A) && !node.Equals(B))
                    {
                        C = node;
                        // I know that using goto is bad style, but in this case it is alriht. 
                        // It makes code much cleaner.
                        goto main;
                    }
                main:
            fixed (Point* points = Points)
            {
                points[0] = A;
                points[1] = B;
                points[2] = C;
            }
            #endregion
            #region Dictionaries with opposit nodes/ribs
            Rib oppositeToA, oppositeToB;
            if (Ribs[1].Points.Contains(A))
            {
                oppositeToA = Ribs[2];
                oppositeToB = Ribs[1];
            }
            else
            {
                oppositeToA = Ribs[1];
                oppositeToB = Ribs[2];
            }
            oppositeNodes = new Dictionary<Rib, Point>()
            {
                { oppositeToA, A },
                { oppositeToB, B },
                { Ribs[0], C }
            };
                oppositeRibs = new Dictionary<Point, Rib>()
            {
                { A, oppositeToA },
                { B, oppositeToB },
                { C, Ribs[0] }
            };
            #endregion
        }

        public static void Update(params Triangle[] triangles)
        {
            foreach (var t in triangles)
                t.Update();
        }

        #endregion
        #region Constructor

        public void SetRibs(Rib R1, Rib R2, Rib R3)
        {
            Ribs[0] = R1;
            Ribs[1] = R2;
            Ribs[2] = R3;
        }

        #endregion
    }
}
