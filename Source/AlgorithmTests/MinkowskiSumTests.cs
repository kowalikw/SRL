using System.Collections.Generic;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Model;

namespace SRL.AlgorithmTests
{
    [TestClass]
    public class MinkowskiSumTests
    {
        [TestMethod]
        public void MinkowskiTest()
        {
            Algorithm.Algorithm a = new Algorithm.Algorithm();
            Map map = new Map();
            List<Point> lst = new List<Point>() { new Point(0.5, 0.5), new Point(0.5, -0.5), new Point(-0.5, -0.1), new Point(-0.5, 0.1) };
            map.Obstacles.Add(new Polygon(lst));
            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(lst);
            List<Polygon>[] minkowskiSum = a.MinkowskiSum(map, vehicle, 20, new System.Threading.CancellationToken());
            Assert.AreEqual(minkowskiSum.Length, 20);
            foreach (List<Polygon> list in minkowskiSum)
            {
                Assert.AreEqual(list.Count, 1);
            }
            lst = new List<Point>() { new Point(1, 1), new Point(-1, 1), new Point(0, 0.9) };
            map.Obstacles.Add(new Polygon(lst));
            minkowskiSum = a.MinkowskiSum(map, vehicle, 20, new System.Threading.CancellationToken());
            foreach (List<Polygon> list in minkowskiSum)
            {
                Assert.AreEqual(list.Count, 2);
            }
            foreach (List<Polygon> list in minkowskiSum)
            {
                Assert.IsTrue(list[0].Vertices.Count <= 16);
                Assert.IsTrue(list[1].Vertices.Count <= 16);
            }
        }
    }
}
