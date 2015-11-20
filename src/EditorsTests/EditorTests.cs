using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Main.View.Control;
using SRL.Model.Enum;
using SRL.Model.Model;
using System.Collections.Generic;

namespace EditorsTests
{
    [TestClass]
    public class EditorTests
    {
        [TestMethod]
        public void CheckLineCorrect()
        {
            MapEditArea editArea = new MapEditArea();
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100));

            var expected = DrawLineState.Correct;
            var actual = editArea.CheckLine(polygon, new Point(0, 30));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckLineIncorrect()
        {
            MapEditArea editArea = new MapEditArea();
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(50, 100));

            var expected = DrawLineState.Incorrect;
            var actual = editArea.CheckLine(polygon, new Point(50, 0));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckLineDone()
        {
            MapEditArea editArea = new MapEditArea();
            Polygon polygon = new Polygon(new Point(0, 0), new Point(100, 100), new Point(80, 100));

            var expected = DrawLineState.Done;
            var actual = editArea.CheckLine(polygon, new Point(0, 0));

            Assert.AreEqual(expected, actual);
        }

        /*[TestMethod]
        public void DrawPolygonEmpty()
        {
            MapEditArea editor = new MapEditArea();
            Polygon polygon = new Polygon();

            var expected = DrawPolygonState.Empty;
            var actual = editor.CheckPolygon(null, polygon, new Point(0,0), true);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DrawPolygonDone()
        {
            MapEditArea editor = new MapEditArea();
            Polygon polygon = new Polygon();

            var expected = DrawPolygonState.Done;
            var actual = editor.CheckPolygon(null, polygon, new Point(0, 0), false);

            Assert.AreEqual(expected, actual);
        }*/

        /*[TestMethod]
        public void DrawPolygonCorrect()
        {
            MapEditArea editor = new MapEditArea();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0,0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawPolygonState.Correct;
            var actual = editor.DrawPolygon(null, polygon, new Point(0, 30), true);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DrawPolygonIncorrect()
        {
            MapEditArea editor = new MapEditArea();
            Polygon polygon = new Polygon(new List<Point>() { new Point(0, 0), new Point(100, 100), new Point(50, 100) });

            var expected = DrawPolygonState.Incorrect;
            var actual = editor.DrawPolygon(null, polygon, new Point(50, 0), true);

            Assert.AreEqual(expected, actual);
        }*/
    }
}
