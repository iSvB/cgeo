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

        public Rib R1 { get; set; }
        public Rib R2 { get; set; }
        public Rib R3 { get; set; }

        public IEnumerable<Rib> Ribs 
        { 
            get 
            { 
                yield return R1; 
                yield return R2; 
                yield return R3; 
            } 
        }

        public IEnumerable<Point> Points { get { return Ribs.SelectMany(r => r.Points).Distinct(); } }

        #endregion        
        #region Methods

        public Rib GetOppositeRib(Point vertex)
        {
            return Ribs.Single(r => !r.Points.Contains(vertex));
        }

        public Point GetOppositeNode(Rib rib)
        {
            return Points.Distinct().Single(p => !rib.Points.Contains(p));
        }        

        public Rib GetAdjacentRib(Triangle T)
        {
            return Ribs.Single(r => T.Ribs.Contains(r));
        }

        public Rib GetRib(Point A, Point B)
        {
            return Ribs.Single(r => r.Points.Contains(A) && r.Points.Contains(B));
        }

        public void UpdateRib(Rib oldRib, Rib newRib)
        {
            if (R1 == oldRib)
                R1 = newRib;
            else if (R2 == oldRib)
                R2 = newRib;
            else if (R3 == oldRib)
                R3 = newRib;
            else
                throw new ArgumentException();
        }

        public bool IsAdjacent(Triangle T)
        {
            return Ribs.Any(r => r.Triangles.Contains(T));
        }

        #endregion
        #region Constructor

        public void SetRibs(Rib R1, Rib R2, Rib R3)
        {
            this.R1 = R1;
            this.R2 = R2;
            this.R3 = R3;
        }

        #endregion
    }
}
