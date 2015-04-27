﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGeo;

namespace CGeoTest
{
    [TestClass]
    public class TriangulationTest
    {        
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
            // Top.
            Assert.IsTrue(TT.IsAdjacent(T1));
            Assert.IsFalse(TT.IsAdjacent(T2));
            // Right.
            Assert.IsTrue(RT.IsAdjacent(T2));
            Assert.IsFalse(RT.IsAdjacent(T1));
            // Bottom.
            Assert.IsTrue(QT.IsAdjacent(T2));
            Assert.IsFalse(QT.IsAdjacent(T1));
            // T1 & T2.
            Assert.IsTrue(T1.IsAdjacent(T2));
            Assert.IsTrue(T2.IsAdjacent(T1));
            #endregion
        }
    }
}