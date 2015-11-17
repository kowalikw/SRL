using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Models.Model;
using System.Collections.Generic;

namespace SRL.ModelsTests
{
    [TestClass]
    public class MarshallingTests
    {
        private readonly XmlSchemaSet _schemaSet;

        public MarshallingTests()
        {
            _schemaSet = new XmlSchemaSet();

            string schemaString = Models.Resources.XmlSchema;
            var reader = new StringReader(schemaString);
            _schemaSet.Add("pl.pw.mini.KowMisPie.SRL", XmlReader.Create(reader));
        }

        [TestMethod]
        public void SerializedMapCompliesToXmlSchema()
        {
            var serializer = new XmlSerializer(typeof(Map));
            var output = new XDocument();

            Map map = new Map(10, 12);
            map.Obstacles.Add(new Polygon(new Point[]
            {
                new Point(1.1,1.2),
                new Point(2.1,2.2),
                new Point(3.1,3.2),
            }));
            map.Obstacles.Add(new Polygon(new List<Point>()
            {
                new Point(1.3,1.4),
                new Point(2.3,2.4),
                new Point(3.3,3.4),
            }));

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, map);

            output.Validate(_schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void SerializedVehicleCompliesToXmlSchema()
        {
            var serializer = new XmlSerializer(typeof(Vehicle));
            var output = new XDocument();

            Vehicle vehicle = new Vehicle(
                new Polygon(new List<Point>()
                {
                    new Point(0, 0),
                    new Point(2, 0),
                    new Point(0, 2),
                }),
                new Point(0.5, 0.5),
                45.5);

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, vehicle);

            output.Validate(_schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void ValidVehicleIsDeserialized()
        {
            var serializer = new XmlSerializer(typeof(Vehicle));

            Vehicle expected = new Vehicle(
                new Polygon(new Point[]
                {
                                new Point(0, 0),
                                new Point(2, 0),
                                new Point(0, 2),
                }),
                new Point(0.45, 0.5),
                45.5);

            TextReader reader = new StringReader(Resources.Vehicle1);
            Vehicle output = (Vehicle)serializer.Deserialize(reader);

            Assert.AreEqual(expected.DirectionAngle, output.DirectionAngle);
            Assert.AreEqual<Point>(expected.Origin, output.Origin);
            Assert.AreEqual<Polygon>(expected.Shape, output.Shape);
        }

        [TestMethod]
        public void ValidMapIsDeserialized()
        {
            var serializer = new XmlSerializer(typeof(Map));

            Map expected = new Map(10, 12);
            expected.Obstacles.Add(new Polygon(new Point[]
            {
                new Point(1.1,1.2),
                new Point(2.1,2.2),
                new Point(3.1,3.2),
            }));
            expected.Obstacles.Add(new Polygon(new Point[]
            {
                new Point(1.3,1.4),
                new Point(2.3,2.4),
                new Point(3.3,3.4),
            }));

            TextReader reader = new StringReader(Resources.Map1);
            Map output = (Map)serializer.Deserialize(reader);

            Assert.AreEqual(expected.ObstacleCount, output.ObstacleCount);
            Assert.AreEqual(expected.Width, output.Width);
            Assert.AreEqual(expected.Height, output.Height);
            foreach (var expObstacle in expected.Obstacles)
                Assert.IsTrue(output.Obstacles.Contains(expObstacle));

        }
    }
}
