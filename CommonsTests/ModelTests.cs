using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonsTests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void VehicleWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.VehicleSchema)));

            var serializer = new XmlSerializer(typeof(Vehicle));
            var output = new XDocument();

            Vehicle vehicle = new Vehicle()
            {
                Shape = new Polygon(new List<Point>()
                {
                    new Point(0.2, 0.3),
                    new Point(0.7, 0.4),
                    new Point(-0.2, 0.9)
                })
            };

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, vehicle);

            output.Validate(schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void VehicleReadXmlTest()
        {
            var serializer = new XmlSerializer(typeof(Vehicle));

            Vehicle expected = new Vehicle()
            {
                Shape = new Polygon(new List<Point>()
                {
                    new Point(0.14697853332425609,-0.00075762130579515308),
                    new Point(-0.26289459311091157,0.57427694979271182),
                    new Point(-0.553063553230448,0.24774216699500878),
                    new Point(-0.54813901474277948,-0.20645180582917411),
                    new Point(-0.30607900754123418,-0.68640490305039148)
                })
            };

            TextReader reader = new StringReader(Resources.Vehicle.ToString());
            Vehicle actual = (Vehicle)serializer.Deserialize(reader);

            Assert.AreEqual(expected.Shape, actual.Shape);
        }

        [TestMethod]
        public void MapWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.MapSchema)));

            var serializer = new XmlSerializer(typeof(Map));
            var output = new XDocument();

            Map map = new Map();
            map.Obstacles.Add(
                new Polygon(new List<Point>() {
                    new Point(0.2, 0.3),
                    new Point(0.7, 0.4),
                    new Point(-0.2, 0.9)
                })
            );
            map.Obstacles.Add(
                new Polygon(new List<Point>() {
                    new Point(0.2, 0.3),
                    new Point(0.7, 0.4),
                    new Point(-0.2, 0.9),
                    new Point(-0.9,0.0)
                })
            );
            map.Obstacles.Add(
                new Polygon(new List<Point>() {
                    new Point(0.1, 0.3),
                    new Point(0.3, 0.7),
                    new Point(-0.2, 0.0)
                })
            );

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, map);

            output.Validate(schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void MapReadXmlTest()
        {

        }

        [TestMethod]
        public void SimulationWriteXmlTest()
        {

        }

        [TestMethod]
        public void SimulationReadXmlTest()
        {

        }
    }
}
