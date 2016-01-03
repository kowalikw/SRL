using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Model;
using System.Collections.Generic;
using System.Windows;
using SRL.Commons.Utilities;

namespace CommonsTests
{
    [TestClass]
    public class GeometryHelperTests
    {
        [TestMethod]
        public void DoSegmentsIntersectTest()
        {
            var expected = false;
            var actual = GeometryHelper.DoSegmentsIntersect(new Point(0, 0), new Point(0, 1), new Point(0, 0), new Point(1, 0));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsInsidePolygonTestTrue()
        {
            Polygon polygon = new Polygon(new List<Point>()
            {
                new Point(-0.2, 0.0),
                new Point(0.0, 0.5),
                new Point(0.2, 0.1),
                new Point(0.0, -0.2)
            });

            Point point1 = new Point(-0.1, 0.01);
            Point point2 = new Point(0, 0);
            Point point3 = new Point(0, 0.1);

            Assert.IsTrue(GeometryHelper.IsInsidePolygon(point1, polygon));
            Assert.IsTrue(GeometryHelper.IsInsidePolygon(point2, polygon));
            Assert.IsTrue(GeometryHelper.IsInsidePolygon(point3, polygon));
        }

        [TestMethod]
        public void IsInsidePolygonTestFalse()
        {
            Polygon polygon = new Polygon(new List<Point>()
            {
                new Point(-0.2, 0.0),
                new Point(0.0, 0.5),
                new Point(0.2, 0.1),
                new Point(0.0, -0.2)
            });

            Point point1 = new Point(0.5, 0.5);
            Point point2 = new Point(0.21, 0.1);

            Assert.IsFalse(GeometryHelper.IsInsidePolygon(point1, polygon));
            Assert.IsFalse(GeometryHelper.IsInsidePolygon(point2, polygon));
        }
    }
}
