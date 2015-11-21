using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Model.Model;
using SRL.Model.Xml;

namespace SRL.ModelsTests
{
    [TestClass]
    public class MarshallingTests
    {
        private readonly XmlSchemaSet _schemaSet;

        public MarshallingTests()
        {
            _schemaSet = new XmlSchemaSet();

            string schemaString = Model.Resources.XmlSchema;
            var reader = new StringReader(schemaString);
            _schemaSet.Add("pl.pw.mini.KowMisPie.SRL", XmlReader.Create(reader));
        }

        private void AssertVehicleEquality(Vehicle a, Vehicle b)
        {
            Assert.AreEqual(a.OrientationAngle, b.OrientationAngle);
            Assert.AreEqual<Point>(a.OrientationOrigin, b.OrientationOrigin);
            Assert.AreEqual<Polygon>(a.Shape, b.Shape);
        }

        private void AssertMapEquality(Map a, Map b)
        {
            Assert.AreEqual(a.ObstacleCount, b.ObstacleCount);
            Assert.AreEqual(a.Width, b.Width);
            Assert.AreEqual(a.Height, b.Height);
            foreach (var expObstacle in a.Obstacles)
                Assert.IsTrue(b.Obstacles.Contains(expObstacle));
        }

        [TestMethod]
        public void SerializedMapCompliesToXmlSchema()
        {
            var serializer = new XmlSerializer(typeof(Map));
            var output = new XDocument();

            Map map = new Map(10, 12);
            map.Obstacles.Add(new Polygon(
                new Point(1.1, 1.2),
                new Point(2.1, 2.2),
                new Point(3.1, 3.2)
            ));
            map.Obstacles.Add(new Polygon(
                new Point(1.3, 1.4),
                new Point(2.3, 2.4),
                new Point(3.3, 3.4)
            ));

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
                new Polygon(
                    new Point(0, 0),
                    new Point(2, 0),
                    new Point(0, 2)
                ),
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
                new Polygon(new Point(0, 0), new Point(2, 0), new Point(0, 2)),
                new Point(0.45, 0.5),
                45.5);

            TextReader reader = new StringReader(Resources.Vehicle1);
            Vehicle output = (Vehicle)serializer.Deserialize(reader);

            AssertVehicleEquality(expected, output);
        }

        [TestMethod]
        public void ValidMapIsDeserialized()
        {
            var serializer = new XmlSerializer(typeof(Map));

            Map expected = new Map(10, 12);
            expected.Obstacles.Add(new Polygon(
                new Point(1.1, 1.2),
                new Point(2.1, 2.2),
                new Point(3.1, 3.2)
            ));
            expected.Obstacles.Add(new Polygon(
                new Point(1.3, 1.4),
                new Point(2.3, 2.4),
                new Point(3.3, 3.4)
            ));

            TextReader reader = new StringReader(Resources.Map1);
            Map output = (Map)serializer.Deserialize(reader);

            AssertMapEquality(expected, output);
        }

        [TestMethod]
        public void MarshallAndUnmarshallDoesntChangeMap()
        {
            Map expected = new Map(10, 12);
            expected.Obstacles.Add(new Polygon(
                new Point(1.1, 1.2),
                new Point(2.1, 2.2),
                new Point(3.1, 3.2)
            ));
            expected.Obstacles.Add(new Polygon(
                new Point(1.3, 1.4),
                new Point(2.3, 2.4),
                new Point(3.3, 3.4)
            ));

            XDocument marshalled = Marshaller<Map>.Marshall(expected);
            Map output = Marshaller<Map>.Unmarshall(marshalled);

            AssertMapEquality(expected, output);
        }

        [TestMethod]
        public void MarshallAndUnmarshallDoesntChangeVehicle()
        {
            Vehicle expected = new Vehicle(
                new Polygon(new Point(0, 0), new Point(2, 0), new Point(0, 2)),
                new Point(0.45, 0.5),
                45.5);

            XDocument marshalled = Marshaller<Vehicle>.Marshall(expected);
            Vehicle output = Marshaller<Vehicle>.Unmarshall(marshalled);

            AssertVehicleEquality(expected, output);
        }
    }
}
