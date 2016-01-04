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
        public void CrossProductTest()
        {
            var actual1 = GeometryHelper.CrossProduct(new Point(-1, 1), new Point(5.2, 9));
            var actual2 = GeometryHelper.CrossProduct(new Point(-1, 1), new Point(5.2, 9), new Point(-3, 4));

            var expected1 = -14.2;
            var expected2 = 34.6;

            Assert.AreEqual(expected1, Math.Round(actual1, 1));
            Assert.AreEqual(expected2, Math.Round(actual2, 1));
        }

        [TestMethod]
        public void DotProductTest()
        {
            var actual1 = GeometryHelper.DotProduct(new Point(-1, 1), new Point(5.2, 9));
            var actual2 = GeometryHelper.DotProduct(new Point(-1, 1), new Point(5.2, 9), new Point(-3, 4));

            var expected1 = 3.8;
            var expected2 = 1.4;

            Assert.AreEqual(expected1, Math.Round(actual1, 1));
            Assert.AreEqual(expected2, Math.Round(actual2, 1));
        }

        [TestMethod]
        public void IsInsideRectangle()
        {
            Point cornerA = new Point(2, 3);
            Point cornerB = new Point(10, 10);

            Point point1 = new Point(0, 0);
            Point point2 = new Point(5, 5);
            Point point3 = new Point(2, 8);

            Assert.IsFalse(GeometryHelper.IsInsideRectangle(point1, cornerA, cornerB));
            Assert.IsTrue(GeometryHelper.IsInsideRectangle(point2, cornerA, cornerB));
            Assert.IsTrue(GeometryHelper.IsInsideRectangle(point3, cornerA, cornerB));
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

        [TestMethod]
        public void DoSegmentsIntersectTest()
        {
            var segmentIntersection1 = GeometryHelper.DoSegmentsIntersect(new Point(0, 0), new Point(0, 1), new Point(0, 0), new Point(1, 0));
            var segmentIntersection2 = GeometryHelper.DoSegmentsIntersect(new Point(0, 0), new Point(1, 0), new Point(0.5, -0.5), new Point(1, 1));
            var segmentIntersection3 = GeometryHelper.DoSegmentsIntersect(new Point(0, 0), new Point(0, 1), new Point(-0.2, 0), new Point(-1, 1));

            Assert.IsTrue(segmentIntersection1);
            Assert.IsTrue(segmentIntersection2);
            Assert.IsFalse(segmentIntersection3);
        }

        [TestMethod]
        public void GetDistanceTest()
        {
            var actual = GeometryHelper.GetDistance(new Point(-3, -4), new Point(2, 1));
            var expected = 7.0711;

            Assert.AreEqual(expected, Math.Round(actual, 4));
        }

        [TestMethod]
        public void GetAngleTest()
        {
            var actual = GeometryHelper.GetAngle(new Point(1, 2), new Point(4, 5));
            var expected = Math.PI / 4;

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void GetAngleWithOriginTest()
        {
            var actual = GeometryHelper.GetAngle(new Point(0, 1), new Point(1, 2), new Point(1, 1));
            var expected = -Math.PI / 4;

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void RotatePointTest()
        {
            var actual = GeometryHelper.RotatePoint(new Point(1, 2), new Point(0, 1), Math.PI / 2);
            var expected = new Point(-1, 2);

            Assert.AreEqual(expected.X, Math.Round(actual.X));
            Assert.AreEqual(expected.Y, Math.Round(actual.Y));
        }

        [TestMethod]
        public void RotateTest()
        {
            Polygon polygon = new Polygon(new List<Point>()
            {
                new Point(1, 1),
                new Point(4, 1),
                new Point(4, 4),
                new Point(1, 4)
            });

            Polygon actual1 = GeometryHelper.Rotate(polygon, Math.PI / 2);
            Polygon expected1 =  new Polygon(new List<Point>()
            {
                new Point(-1, 1),
                new Point(-1, 4),
                new Point(-4, 4),
                new Point(-4, 1)
            });

            Polygon actual2 = GeometryHelper.Rotate(new Point(0.2, 0.5), polygon, Math.PI / 2);
            Polygon expected2 = new Polygon(new List<Point>()
            {
                new Point(-0.3, 1.3),
                new Point(-0.3, 4.3),
                new Point(-3.3, 4.3),
                new Point(-3.3, 1.3)
            });

            Polygon actual1Rounded = new Polygon();
            for (int i = 0; i < actual1.Vertices.Count; i++)
                actual1Rounded.Vertices.Add(new Point(Math.Round(actual1.Vertices[i].X, 0), Math.Round(actual1.Vertices[i].Y, 0)));

            Polygon actual2Rounded = new Polygon();
            for (int i = 0; i < actual2.Vertices.Count; i++)
                actual2Rounded.Vertices.Add(new Point(Math.Round(actual2.Vertices[i].X, 1), Math.Round(actual2.Vertices[i].Y, 1)));

            Assert.AreEqual(expected1, actual1Rounded);
            Assert.AreEqual(expected2, actual2Rounded);
        }

        [TestMethod]
        public void MoveTest()
        {
            Polygon polygon = new Polygon(new List<Point>()
            {
                new Point(1, 1),
                new Point(4, 1),
                new Point(4, 4),
                new Point(1, 4)
            });

            Polygon actual = GeometryHelper.Move(polygon, 3, -4);
            Polygon expected = new Polygon(new List<Point>()
            {
                new Point(4, -3),
                new Point(7, -3),
                new Point(7, 0),
                new Point(4, 0)
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResizeTest()
        {
            Polygon polygon = new Polygon(new List<Point>()
            {
                new Point(1, 1),
                new Point(4, 1),
                new Point(4, 4),
                new Point(1, 4)
            });

            Polygon actual = GeometryHelper.Resize(polygon, 2);
            Polygon expected = new Polygon(new List<Point>()
            {
                new Point(2, 2),
                new Point(8, 2),
                new Point(8, 8),
                new Point(2, 8)
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCounterClockwiseTurnTest()
        {
            var test1 = GeometryHelper.IsCounterClockwiseTurn(new Point(1, 1), new Point(4, 1), new Point(2, 4));
            var test2 = GeometryHelper.IsCounterClockwiseTurn(new Point(1, 1), new Point(2, 4), new Point(4, 1));

            Assert.IsTrue(test1);
            Assert.IsFalse(test2);
        }

        [TestMethod]
        public void IsIntersectedTest()
        {
            Polygon subject = new Polygon(new List<Point>()
            {
                new Point(1, 1),
                new Point(4, 1),
                new Point(4, 4),
                new Point(1, 4)
            });

            Polygon polygon1 = new Polygon(new List<Point>()
            {
                new Point(5, 5),
                new Point(7, 5),
                new Point(7, 7)
            });

            Polygon polygon2 = new Polygon(new List<Point>()
            {
                new Point(0, 0),
                new Point(5, 0),
                new Point(5, 5),
                new Point(3, 5),
                new Point(3, 3),
                new Point(2, 3),
                new Point(2, 5),
                new Point(0, 5)
            });

            Assert.IsFalse(GeometryHelper.IsIntersected(subject, new List<Polygon>() { polygon1 }));
            Assert.IsTrue(GeometryHelper.IsIntersected(subject, new List<Polygon>() { polygon2 }));
        }

        [TestMethod]
        public void IsEnclosedTest()
        {
            Polygon subject = new Polygon(new List<Point>()
            {
                new Point(1, 1),
                new Point(4, 1),
                new Point(4, 4),
                new Point(1, 4)
            });

            Polygon enclosure1 = new Polygon(new List<Point>()
            {
                new Point(0, 0),
                new Point(5, 0),
                new Point(5, 4),
                new Point(0, 5)
            });

            Polygon enclosure2 = new Polygon(new List<Point>()
            {
                new Point(0, 0),
                new Point(5, 0),
                new Point(5, 5),
                new Point(3, 5),
                new Point(3, 3),
                new Point(2, 3),
                new Point(2, 5),
                new Point(0, 5)
            });

            Assert.IsTrue(GeometryHelper.IsEnclosed(subject, enclosure1));
            Assert.IsFalse(GeometryHelper.IsEnclosed(subject, enclosure2));
        }

    }
}
