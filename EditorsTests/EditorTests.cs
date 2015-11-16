using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Editors;
using SRL.Models.Enum;
using SRL.Models.Model;
using System.Collections.Generic;

namespace EditorsTests
{
    [TestClass]
    public class EditorTests
    {
        [TestMethod]
        public void CheckLineCorrect()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0, 0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawLineState.Correct;
            var actual = editor.CheckLine(polygon, new Point(0, 30));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckLineIncorrect()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0, 0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawLineState.Incorrect;
            var actual = editor.CheckLine(polygon, new Point(50, 0));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckLineDone()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0, 0), new Point(100, 100), new Point(80, 100) });

            var expected = DrawLineState.Done;
            var actual = editor.CheckLine(polygon, new Point(0, 0));

            Assert.AreEqual(expected, actual);
        }

        /*[TestMethod]
        public void DrawPolygonEmpty()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon();

            var expected = DrawPolygonState.Empty;
            var actual = editor.CheckPolygon(null, polygon, new Point(0,0), true);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DrawPolygonDone()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon();

            var expected = DrawPolygonState.Done;
            var actual = editor.CheckPolygon(null, polygon, new Point(0, 0), false);

            Assert.AreEqual(expected, actual);
        }*/

        /*[TestMethod]
        public void DrawPolygonCorrect()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0,0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawPolygonState.Correct;
            var actual = editor.DrawPolygon(null, polygon, new Point(0, 30), true);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DrawPolygonIncorrect()
        {
            MapEditor editor = new MapEditor();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0, 0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawPolygonState.Incorrect;
            var actual = editor.DrawPolygon(null, polygon, new Point(50, 0), true);

            Assert.AreEqual(expected, actual);
        }*/
    }
}
