using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public class Triangle
    {
        #region Properties

        public Rib[] Ribs { get; } = new Rib[3];        

        public Point[] Points { get; } = new Point[3];

        #endregion        
        #region Methods

        public Rib GetOppositeRib(Point vertex)
        {
            int i = 0;
            for (; i < 3; ++i)
                if (Points[i].Equals(vertex))
                    break;
            switch (i)
            {
                case 0: return Ribs[1];
                case 1: return Ribs[2];
                case 2: return Ribs[0];
            }
            throw new ArgumentException();
        }

        public Point GetOppositeNode(Rib rib)
        {
            int i = 0;
            for (; i < 3; ++i)
                if (Ribs[i] == rib)
                    break;
            switch (i)
            {
                case 0: return Points[2];
                case 1: return Points[0];
                case 2: return Points[1];
            }
            throw new ArgumentException();
        }        

        public Rib GetAdjacentRib(Triangle T)
        {
            return Ribs.First(r => T.Ribs.Contains(r));
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
            Rib a = Ribs[0];
            Rib b = null;
            Rib c = null;
            #region Nodes
            var A = a.A;
            var B = a.B;
            Point C = new Point();
            for (var i = 1; i < 3; ++i)
                foreach (var node in Ribs[i].Points)
                    if (!node.Equals(A) && !node.Equals(B))
                    {
                        C = node;
                        if (Ribs[i].A.Equals(A) || Ribs[i].B.Equals(A))
                        {
                            c = Ribs[i];
                            if (i == 1)
                                b = Ribs[2];
                            else
                                b = Ribs[1];
                        }
                        else
                        {
                            b = Ribs[i];
                            if (i == 1)
                                c = Ribs[2];
                            else
                                c = Ribs[1];
                        }
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
            #region Ribs            
            Ribs[0] = a;
            Ribs[1] = b;
            Ribs[2] = c;
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
