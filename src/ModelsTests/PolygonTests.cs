using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Model;
using SRL.Model.Model;
using System.Collections.Generic;

namespace SRL.ModelsTests
{
    [TestClass]
    public class PolygonTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PolygonException()
        {
            List<Point> vertices = new List<Point>() { new Point(0, 0) };
         //   Polygon polygon = new Polygon(vertices);
            Assert.Fail();
        }

        [TestMethod]
        public void IsCorrectNoMinVertices()
        {
            Polygon polygon = new Polygon();

            var expected = false;
            var actual = polygon.IsCorrect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCorrectTrue1()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(20, 20), new Point(80, 100));

            var expected = true;
            var actual = polygon.IsCorrect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCorrectFalse1()
        {
            Polygon polygon = new Polygon(new Point(0,0), new Point(100, 100), new Point(20, 100), new Point(80, 100));

            var expected = false;
            var actual = polygon.IsCorrect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCorrectTrue2()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100), new Point(25, 125), new Point(25, 150));

            var expected = true;
            var actual = polygon.IsCorrect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsCorrectFalse2()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100), new Point(25, 125), new Point(75, 125));

            var expected = false;
            var actual = polygon.IsCorrect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFinishedTrue1()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(20, 20), new Point(80, 100), new Point(2, 3));

            var expected = true;
            var actual = polygon.IsFinished();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFinishedFalse1()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(20, 20), new Point(80, 100));

            var expected = false;
            var actual = polygon.IsFinished();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFinishedTrue2()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100), new Point(25, 125));

            var expected = true;
            var actual = polygon.IsFinished(new Point(3, 2));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFinishedFalse2()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100), new Point(25, 125));

            var expected = false;
            var actual = polygon.IsFinished(new Point(25, 150));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsEmptyTrue()
        {
            Polygon polygon = new Polygon();

            var expected = true;
            var actual = polygon.IsEmpty();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsEmptyFalse()
        {
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100), new Point(25, 125));

            var expected = false;
            var actual = polygon.IsEmpty();

            Assert.AreEqual(expected, actual);
        }
    }
}
