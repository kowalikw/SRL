using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Utilities;

namespace SRL.CommonsTests
{
    [TestClass]
    public class MathHelperTests
    {
        [TestMethod]
        public void SwapTest()
        {
            int num1 = 3;
            int num2 = 10;

            MathHelper.Swap<int>(ref num1, ref num2);

            Assert.AreEqual(10, num1);
            Assert.AreEqual(3, num2);
        }

        [TestMethod]
        public void RfpartTest()
        {
            float val = 1.567f;

            var actual = MathHelper.Rfrac(val);
            var expected = 0.433;

            Assert.AreEqual(expected, Math.Round(actual, 3));
        }

        [TestMethod]
        public void FpartTest()
        {
            float val = 1.567f;

            var actual = MathHelper.Frac(val);
            var expected = 0.567;

            Assert.AreEqual(expected, Math.Round(actual, 3));
        }

        [TestMethod]
        public void ClampDoubleTest()
        {
            Assert.AreEqual(12.01, 12.01.Clamp(10, 20));
            Assert.AreEqual(10, 9.22.Clamp(10, 20));
            Assert.AreEqual(20, 33.34.Clamp(10, 20));
        }

        [TestMethod]
        public void ClampIntTest()
        {
            Assert.AreEqual(12, 12.Clamp(10, 20));
            Assert.AreEqual(10, 9.Clamp(10, 20));
            Assert.AreEqual(20, 33.Clamp(10, 20));
        }

        [TestMethod]
        public void MaxTest()
        {
            var expected = 99;
            var actual = MathHelper.Max(2, 4, 99, 24, 12, 53, 63, 86);

            Assert.AreEqual(expected, actual);
        }
    }
}
