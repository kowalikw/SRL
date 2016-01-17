using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Model;

namespace SRL.CommonsTests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void PolygonWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.PolygonSchema)));

            var serializer = new XmlSerializer(typeof(Polygon));
            var output = new XDocument();

            Polygon polygon = new Polygon(new List<Point>
            {
                new Point(0.2, 0.3),
                new Point(0.7, 0.4),
                new Point(-0.2, 0.9)
            });

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, polygon);

            output.Validate(schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void PolygonReadXmlTest()
        {
            var serializer = new XmlSerializer(typeof(Polygon));

            Polygon expected = new Polygon(new List<Point>
            {
                new Point(-0.54811715481171552,0.20920502092050208),
                new Point(0.7405857740585774,0.32217573221757323),
                new Point(0.55648535564853552,-0.606694560669456)
            });

            TextReader reader = new StringReader(Resources.Polygon);
            Polygon actual = (Polygon)serializer.Deserialize(reader);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OrderWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.OrderSchema)));

            var serializer = new XmlSerializer(typeof(Order));
            var output = new XDocument();

            Order order = new Order(0.5, new Point(-0.3, 0.7));

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, order);

            output.Validate(schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void OrderReadXmlTest()
        {
            var serializer = new XmlSerializer(typeof(Order));

            Order expected = new Order(0.1, new Point(-0.41667, -0.2083));

            TextReader reader = new StringReader(Resources.Order);
            Order actual = (Order)serializer.Deserialize(reader);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void VehicleWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.VehicleSchema)));

            var serializer = new XmlSerializer(typeof(Vehicle));
            var output = new XDocument();

            Vehicle vehicle = new Vehicle
            {
                Shape = new Polygon(new List<Point>
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

            Vehicle expected = new Vehicle
            {
                Shape = new Polygon(new List<Point>
                {
                    new Point(0.14697853332425609,-0.00075762130579515308),
                    new Point(-0.26289459311091157,0.57427694979271182),
                    new Point(-0.553063553230448,0.24774216699500878),
                    new Point(-0.54813901474277948,-0.20645180582917411),
                    new Point(-0.30607900754123418,-0.68640490305039148)
                })
            };

            TextReader reader = new StringReader(Resources.Vehicle);
            Vehicle actual = (Vehicle)serializer.Deserialize(reader);

            Assert.AreEqual(expected, actual);
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
                new Polygon(new List<Point>
                {
                    new Point(0.2, 0.3),
                    new Point(0.7, 0.4),
                    new Point(-0.2, 0.9)
                })
            );
            map.Obstacles.Add(
                new Polygon(new List<Point>
                {
                    new Point(0.2, 0.3),
                    new Point(0.7, 0.4),
                    new Point(-0.2, 0.9),
                    new Point(-0.9,0.0)
                })
            );
            map.Obstacles.Add(
                new Polygon(new List<Point>
                {
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
            var serializer = new XmlSerializer(typeof(Map));

            Map expected = new Map();
            expected.Obstacles.Add(new Polygon(new List<Point>
            {
                new Point(-0.54811715481171552,0.20920502092050208),
                new Point(0.7405857740585774,0.32217573221757323),
                new Point(0.55648535564853552,-0.606694560669456)
            }));
            expected.Obstacles.Add(new Polygon(new List<Point>
            {
                new Point(-0.61087866108786615,-0.32635983263598328),
                new Point(-0.68619246861924688,-0.7615062761506276),
                new Point(0.083682008368200833,-0.80753138075313813),
                new Point(0.062761506276150625,-0.56066945606694563),
                new Point(-0.30543933054393307,-0.67364016736401677),
                new Point(-0.26359832635983266,-0.42259414225941422),
                new Point(-0.37656903765690375,-0.38912133891213391),
                new Point(-0.35146443514644349,-0.23012552301255229),
                new Point(-0.401673640167364,-0.200836820083682),
                new Point(-0.58995815899581594,-0.17154811715481172),
                new Point(-0.77405857740585771,-0.19665271966527198),
                new Point(-0.85774058577405854,-0.31799163179916318),
                new Point(-0.82008368200836823,-0.39330543933054396),
                new Point(-0.71966527196652719,-0.3807531380753138)
            }));
            expected.Obstacles.Add(new Polygon(new List<Point>
            {
                new Point(-0.23430962343096234,0.799163179916318),
                new Point(-0.21338912133891214,0.49372384937238495),
                new Point(0.24267782426778242,0.52301255230125521),
                new Point(0.21338912133891214,0.84937238493723854)
            }));

            TextReader reader = new StringReader(Resources.Map);
            Map actual = (Map)serializer.Deserialize(reader);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SimulationWriteXmlTest()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.SimulationSchema)));
            schemaSet.Add("http://www.w3.org/1999/xlink", XmlReader.Create(new StringReader(Resources.SimulationSchemaXlink)));

            var serializer = new XmlSerializer(typeof(Simulation));
            var output = new XDocument();

            Map map = new Map();
            map.Obstacles.Add(new Polygon(new List<Point> { new Point(-0.0188087774294671, 0.899581589958159), new Point(-0.689655172413793, -0.878661087866109), new Point(0.247648902821317, -0.866108786610879) }));
            map.Obstacles.Add(new Polygon(new List<Point> { new Point(0.366771159874608, 0.267782426778243), new Point(0.351097178683386, -0.121338912133891), new Point(0.821316614420063, -0.158995815899582), new Point(0.855799373040752, 0.330543933054393) }));

            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(new List<Point> { new Point(-0.3, -0.4), new Point(0.2, 0.0), new Point(0.0, 0.2), new Point(-0.3, 0.0) });

            List<Order> orders = new List<Order>();
            orders.Add(new Order(0.1, new Point(-0.41667, -0.2083)));
            orders.Add(new Order(0.84, new Point(0.2083, -0.8333)));
            orders.Add(new Order(2.34, new Point(-0.625, -0.833)));
            orders.Add(new Order(0.1, new Point(-0.833, 0.25)));
            orders.Add(new Order(0.6, new Point(-0.1389, 0.4583)));
            orders.Add(new Order(0.6, new Point(0.3889, 0.6667)));
            orders.Add(new Order(3.14, new Point(0.1111, -0.2083)));

            Simulation simulation = new Simulation();
            simulation.StartPoint = new Point(-0.41667, -0.2083);
            simulation.EndPoint = new Point(0.1111, -0.2083);
            simulation.VehicleSize = 0.5;
            simulation.InitialVehicleRotation = 0;
            simulation.Map = map;
            simulation.Vehicle = vehicle;
            simulation.Orders = orders;

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, simulation);

            output.Validate(schemaSet, (o, e) => Assert.Fail());
        }

        [TestMethod]
        public void SimulationReadXmlTest()
        {
            var serializer = new XmlSerializer(typeof(Simulation));

            Simulation expected = new Simulation();

            Map map = new Map();
            map.Obstacles.Add(new Polygon(new List<Point> { new Point(-0.0188087774294671, 0.899581589958159), new Point(-0.689655172413793, -0.878661087866109), new Point(0.247648902821317, -0.866108786610879) }));
            map.Obstacles.Add(new Polygon(new List<Point> { new Point(0.366771159874608, 0.267782426778243), new Point(0.351097178683386, -0.121338912133891), new Point(0.821316614420063, -0.158995815899582), new Point(0.855799373040752, 0.330543933054393) }));

            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(new List<Point> { new Point(-0.3, -0.4), new Point(0.2, 0.0), new Point(0.0, 0.2), new Point(-0.3, 0.0) });

            List<Order> orders = new List<Order>();
            orders.Add(new Order(0.1, new Point(-0.41667, -0.2083)));
            orders.Add(new Order(0.84, new Point(0.2083, -0.8333)));
            orders.Add(new Order(2.34, new Point(-0.625, -0.833)));
            orders.Add(new Order(0.1, new Point(-0.833, 0.25)));
            orders.Add(new Order(0.6, new Point(-0.1389, 0.4583)));
            orders.Add(new Order(0.6, new Point(0.3889, 0.6667)));
            orders.Add(new Order(3.14, new Point(0.1111, -0.2083)));

            expected.StartPoint = new Point(-0.41667, -0.2083);
            expected.EndPoint = new Point(0.1111, -0.2083);
            expected.VehicleSize = 0.5;
            expected.InitialVehicleRotation = 0;
            expected.Map = map;
            expected.Vehicle = vehicle;
            expected.Orders = orders;

            TextReader reader = new StringReader(Resources.Simulation);
            Simulation actual = (Simulation)serializer.Deserialize(reader);

            Assert.AreEqual(expected, actual);
        }
    }
}
