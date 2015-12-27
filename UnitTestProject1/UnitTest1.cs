using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRL.Commons.Model;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        /*[TestMethod]
        public void SimulationWriteXmlTest()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Map map = new Map();
            map.Obstacles.Add(new Polygon(new List<Point>() { new Point(-0.0188087774294671, 0.899581589958159), new Point(-0.689655172413793, -0.878661087866109), new Point(0.247648902821317, -0.866108786610879) }));
            map.Obstacles.Add(new Polygon(new List<Point>() { new Point(0.366771159874608, 0.267782426778243), new Point(0.351097178683386, -0.121338912133891), new Point(0.821316614420063, -0.158995815899582), new Point(0.855799373040752, 0.330543933054393) }));

            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(new List<Point>() { new Point(-0.3, -0.4), new Point(0.2, 0.0), new Point(0.0, 0.2), new Point(- 0.3, 0.0) });

            List<Order> orders = new List<Order>();
            orders.Add(new Order { Destination = new Point(-0.41667, -0.2083), Rotation = 0.1 });
            orders.Add(new Order { Destination = new Point(0.2083, -0.8333), Rotation = 0.84 });
            orders.Add(new Order { Destination = new Point(-0.625, -0.833), Rotation = 2.34 });
            orders.Add(new Order { Destination = new Point(-0.833, 0.25), Rotation = 0.1 });
            orders.Add(new Order { Destination = new Point(-0.1389, 0.4583), Rotation = 0.6 });
            orders.Add(new Order { Destination = new Point(0.3889, 0.6667), Rotation = 0.6 });
            orders.Add(new Order { Destination = new Point(0.1111, -0.2083), Rotation = 3.14 });

            Simulation simulation = new Simulation();

            simulation.Width = 720;
            simulation.Height = 480;
            simulation.Type = "simulation";

            simulation.StartPoint = new Point(-0.41667,-0.2083);
            simulation.EndPoint = new Point(0.1111, -0.2083);
            simulation.VehicleSize = 0.5;
            simulation.InitialVehicleRotation = 0;

            simulation.Map = map;
            simulation.Vehicle = vehicle;
            simulation.Orders = orders;

            var serializer = new XmlSerializer(typeof(Simulation));
            var output = new XDocument();

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, simulation);

            File.WriteAllText("test.svg", output.ToString());
        }*/

        /*[TestMethod]
        public void SimulationReadXmlTest()
        {
            
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            XDocument doc = XDocument.Load("test.svg");
            var serializer = new XmlSerializer(typeof(Simulation));

            Simulation output;

            using (XmlReader reader = doc.CreateReader())
                output = (Simulation)serializer.Deserialize(reader);
            
        }*/
    }
}
