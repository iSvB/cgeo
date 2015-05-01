using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CGeo;

namespace CGeoTest
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void CrossProductZ()
        {
            // Arrange.
            var A = new Point(2, 0);
            var B = new Point(3, 2);
            var C = new Point(1, 0);
            var D = new Point(0, 2);            
            // Act.
            double ab_z = Utils.CrossProductZ(A, B, C, D);
            double ba_z = Utils.CrossProductZ(C, D, A, B);
            double bb_z = Utils.CrossProductZ(A, B, A, B);
            // Assert.
            Assert.AreEqual(4, ab_z);
            Assert.AreEqual(-4, ba_z);
            Assert.AreEqual(0, bb_z);
        }

        [TestMethod]
        public void ClockwiseOrder()
        {
            // Arrange.
            var A = new Point(0, 0);
            var B = new Point(2, 2);
            var C = new Point(1, -1);
            // Act.
            bool clockwiseABC = Utils.IsClockwiseOrdered(A, B, C);
            bool clockwiseBCA = Utils.IsClockwiseOrdered(B, C, A);
            bool clockwiseCAB = Utils.IsClockwiseOrdered(C, A, B); ;
            bool counterClockwiseBAC = Utils.IsClockwiseOrdered(B, A, C);
            bool counterClockwiseACB = Utils.IsClockwiseOrdered(A, C, B);
            bool counterClockwiseCBA = Utils.IsClockwiseOrdered(C, B, A);
            // Assert.
            Assert.IsTrue(clockwiseABC);
            Assert.IsTrue(clockwiseBCA);
            Assert.IsTrue(clockwiseCAB);
            Assert.IsFalse(counterClockwiseBAC);
            Assert.IsFalse(counterClockwiseACB);
            Assert.IsFalse(counterClockwiseCBA);
        }

        [TestMethod]
        public void ClockwiseOrder2()
        {
            // Arrange.
            var A = new Point(50, 50);
            var B = new Point(130, 150);
            var C = new Point(250, 250);
            // Act.
            var isClockwise = Utils.IsClockwiseOrdered(A, B, C);
            // Assert.
            Assert.IsTrue(isClockwise);            
        }

        [TestMethod]
        public void IsSeparated()
        {
            // Arrange.
            var A = new Point(1, 2);
            var B = new Point(5, 3);
            var C = new Point(4, 1);
            var D = new Point(-1, 4);
            var O = new Point(3, 0);
            var V = new Point(3, 5);
            // Assert.
            Assert.IsTrue(Utils.IsSeparated(O, V, A, B));
            Assert.IsFalse(Utils.IsSeparated(O, V, B, C));
            Assert.IsFalse(Utils.IsSeparated(O, V, A, D));

            Assert.IsTrue(Utils.IsSeparated(V, O, A, B));
            Assert.IsFalse(Utils.IsSeparated(V, O, B, C));
            Assert.IsFalse(Utils.IsSeparated(V, O, A, D));

            Assert.IsTrue(Utils.IsSeparated(V, O, B, A));
            Assert.IsFalse(Utils.IsSeparated(V, O, C, B));
            Assert.IsFalse(Utils.IsSeparated(V, O, D, A));
        }        

        [TestMethod]
        public void DistanceToLine()
        {
            // Arrange.
            var O = new Point(0, 0);
            var X = new Point(10, 0);
            var P1 = new Point(5, 5);
            var P2 = new Point(-5, -5);
            var P3 = new Point(15, 0);
            // Assert.
            Assert.AreEqual(5, Utils.DistanceToLine(O, X, P1));
            Assert.AreEqual(5, Utils.DistanceToLine(O, X, P2));
            Assert.AreEqual(0, Utils.DistanceToLine(O, X, P3));
        }
    }
}
