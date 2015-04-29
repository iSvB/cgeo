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
            // Arrange
            var A = new Point(2, 0);
            var B = new Point(3, 2);
            var C = new Point(1, 0);
            var D = new Point(0, 2);            
            // Act
            double ab_z = Utils.CrossProductZ(A, B, C, D);
            double ba_z = Utils.CrossProductZ(C, D, A, B);
            // Assert
            Assert.AreEqual(4, ab_z);
            Assert.AreEqual(-4, ba_z);
        }
    }
}
