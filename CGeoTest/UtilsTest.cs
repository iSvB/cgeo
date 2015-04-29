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
    }
}
