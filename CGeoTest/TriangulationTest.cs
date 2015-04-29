using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGeo;

namespace CGeoTest
{
    [TestClass]
    public class TriangulationTest
    {
        #region Fields

        Point A, B, C, D, E, F, G, H, J;
        Triangle T, T_D;
        Rib AB, BC, AC;
        Rib BD, CD;

        #endregion

        private void Initialize()
        {
            // Nodes
            // Main triangle.
            A = new Point(0, 37);
            B = new Point(57, 64);
            C = new Point(60, 8);
            // Right nodes.            
            D = new Point(67, 38);
            E = new Point(119, 39);
            // Bottom nodes.
            F = new Point(27, 12);
            G = new Point(10, -10);
            // Top nodes.
            H = new Point(22, 61);
            J = new Point(19, 89);

            T = new Triangle();
            // Ribs.
            AB = new Rib(A, B, T, null);
            BC = new Rib(B, C, T, null);
            AC = new Rib(A, C, T, null);
            T.SetRibs(AB, BC, AC);
        }

        private void CreateT_D()
        {
            T_D = new Triangle();
            BD = new Rib(B, D, T_D, null);
            CD = new Rib(C, D, T_D, null);
            T_D.SetRibs(BC, BD, CD);
            BC.Update(null, T_D);         
        }

        [TestMethod]
        public void Flip()
        {
            #region Arrange
            // Nodes.
            var A = new Point(0, 5);
            var B = new Point(5, 0);
            var C = new Point(0, 0);
            var D = new Point(5, 5);
            var L = new Point(-3, 2);
            var T = new Point(3, 7);
            var R = new Point(7, 2);
            var Q = new Point(3, -2);
            // Triangles.
            var LT = new Triangle();
            var TT = new Triangle();
            var RT = new Triangle();
            var QT = new Triangle();
            var T1 = new Triangle();
            var T2 = new Triangle();
            // Ribs.
            // Left triangle.
            var LA = new Rib(L, A, LT, null);
            var LC = new Rib(L, C, LT, null);
            var AC = new Rib(A, C, LT, T1);
            // Top triangle.
            var TA = new Rib(T, A, TT, null);
            var TD = new Rib(T, D, TT, null);
            var AD = new Rib(A, D, TT, T2);
            // Right triangle.
            var RD = new Rib(R, D, RT, null);
            var RB = new Rib(R, B, RT, null);
            var BD = new Rib(B, D, RT, T2);
            // Bottom triangle.
            var QC = new Rib(Q, C, QT, null);
            var QB = new Rib(Q, B, QT, null);
            var BC = new Rib(B, C, QT, T1);
            // Diagonal.
            var AB = new Rib(A, B, T1, T2);
            // Set ribs for triangles.
            LT.SetRibs(LA, LC, AC);
            TT.SetRibs(TA, TD, AD);
            RT.SetRibs(RD, RB, BD);
            QT.SetRibs(QC, QB, BC);
            T1.SetRibs(AC, BC, AB);
            T2.SetRibs(AD, BD, AB);
            #endregion
            #region Act
            Triangulation.Flip(T1, T2);
            #endregion            
            #region Assert
            // Left.
            Assert.IsTrue(LT.IsAdjacent(T1));
            Assert.IsFalse(LT.IsAdjacent(T2));
            Assert.IsTrue(LT.IsAdjacent(null));
            // Top.
            Assert.IsTrue(TT.IsAdjacent(T1));
            Assert.IsFalse(TT.IsAdjacent(T2));
            Assert.IsTrue(TT.IsAdjacent(null));
            // Right.
            Assert.IsTrue(RT.IsAdjacent(T2));
            Assert.IsFalse(RT.IsAdjacent(T1));
            Assert.IsTrue(RT.IsAdjacent(null));
            // Bottom.
            Assert.IsTrue(QT.IsAdjacent(T2));
            Assert.IsFalse(QT.IsAdjacent(T1));
            Assert.IsTrue(QT.IsAdjacent(null));
            // T1 & T2.
            Assert.IsTrue(T1.IsAdjacent(T2));
            Assert.IsTrue(T2.IsAdjacent(T1));
            Assert.IsFalse(T1.IsAdjacent(null));
            Assert.IsFalse(T2.IsAdjacent(null));
            #endregion
        }

        [TestMethod]
        public void DelaunayCondition()
        {
            // Arrange.
            Initialize();
            // Assert.            
            // Right.
            Assert.IsFalse(Triangulation.SatisfiesDelaunayCondition(C, A, B, D));
            Assert.IsTrue(Triangulation.SatisfiesDelaunayCondition(C, A, B, E));
            // Bottom.
            Assert.IsFalse(Triangulation.SatisfiesDelaunayCondition(A, B, C, F));
            Assert.IsTrue(Triangulation.SatisfiesDelaunayCondition(A, B, C, G));
            // Top.
            Assert.IsFalse(Triangulation.SatisfiesDelaunayCondition(B, C, A, H));
            Assert.IsTrue(Triangulation.SatisfiesDelaunayCondition(B, C, A, J));
        }

        [TestMethod]
        public void FlipRequired()
        {
            // Arrange.
            Initialize();
            CreateT_D();
            // Act.
            Triangle outFlip;
            var isRequiredBefore = Triangulation.FlipRequired(T, out outFlip);
            Triangulation.Flip(T, T_D);
            var isRequiredAfter = Triangulation.FlipRequired(T, out outFlip);
            // Assert.            
            Assert.IsTrue(isRequiredBefore);
            Assert.IsFalse(isRequiredAfter);
        }

        [TestMethod]
        public void CheckAndFlip()
        {
            // Arrange.
            Initialize();
            CreateT_D();
            // Act.
            Triangulation.CheckAndFlip(T, new System.Collections.Generic.HashSet<Triangle>());
            // Assert.            
            var tribs = T.Ribs.Any(r => r.Points.Contains(A) && r.Points.Contains(B));
            tribs &= T.Ribs.Any(r => r.Points.Contains(B) && r.Points.Contains(D));
            tribs &= T.Ribs.Any(r => r.Points.Contains(A) && r.Points.Contains(D));
            tribs &= T.Points.Count() == 3;
            var t_dribs = T_D.Ribs.Any(r => r.Points.Contains(C) && r.Points.Contains(D));
            t_dribs &= T_D.Ribs.Any(r => r.Points.Contains(A) && r.Points.Contains(C));
            t_dribs &= T_D.Ribs.Any(r => r.Points.Contains(A) && r.Points.Contains(D));
            t_dribs &= T_D.Points.Count() == 3;

            Assert.IsTrue(tribs);
            Assert.IsTrue(t_dribs);
        }

        [TestMethod]
        public void GetSeparatingRib()
        {
            #region Arrange
            var A = new Point(0, 0);
            var B = new Point(1, 2);
            var C = new Point(2, 0);
            var D = new Point(3, 1);
            var E = new Point(0, 1);
            var F = new Point(1, -1);

            var ABC = new Triangle();
            var BCD = new Triangle();
            var ABE = new Triangle();
            var ACF = new Triangle();

            var AB = new Rib(A, B, ABC, ABE);
            var BC = new Rib(B, C, ABC, BCD);
            var AC = new Rib(A, C, ABC, ACF);

            var BD = new Rib(B, D, BCD, null);
            var CD = new Rib(C, D, BCD, null);

            var AE = new Rib(A, E, ABE, null);
            var BE = new Rib(B, E, ABE, null);

            var AF = new Rib(A, F, ACF, null);
            var CF = new Rib(C, F, ACF, null);

            ABC.SetRibs(AB, BC, AC);
            BCD.SetRibs(BC, BD, CD);
            ABE.SetRibs(AB, AE, BE);
            ACF.SetRibs(AC, AF, CF);
            #endregion
            // Act.
            var bc = Triangulation.GetSeparatingRib(ABC, D);
            var ab = Triangulation.GetSeparatingRib(ABC, E);
            var ac = Triangulation.GetSeparatingRib(ABC, F);
            var inv_bc = Triangulation.GetSeparatingRib(BCD, A);
            var inv_ab = Triangulation.GetSeparatingRib(ABE, C);
            var inv_ac = Triangulation.GetSeparatingRib(ACF, B);
            // Assert.
            Assert.AreSame(BC, bc);
            Assert.AreSame(BC, inv_bc);
            Assert.AreSame(AB, ab);
            Assert.AreSame(AB, inv_ab);
            Assert.AreSame(AC, ac);
            Assert.AreSame(AC, inv_ac);
        }
    }
}
