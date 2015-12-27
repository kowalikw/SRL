using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Simulation : IXmlSerializable, ISaveable
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Type { get; set; }

        public Map Map { get; set; }

        public Vehicle Vehicle { get; set; }

        public double VehicleSize { get; set; }

        public double InitialVehicleRotation { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
        
        public List<Order> Orders { get; set; }

        #region IXmlSerializable members

        /// <remarks>
        /// Must always return null (as specified by MSDN).
        /// </remarks>
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "svg")
            {
                reader.MoveToAttribute("width");
                Width = reader.ReadContentAsInt();

                reader.MoveToAttribute("height");
                Height = reader.ReadContentAsInt();

                reader.MoveToAttribute("type");
                Type = reader.ReadContentAsString();

                if (Type != "simulation") throw new XmlException();

                reader.MoveToContent();

                reader.ReadToFollowing("g");
                reader.MoveToAttribute("id");
                if (reader.ReadContentAsString() == "map")
                {
                    if (Map == null) Map = new Map();

                    reader.MoveToContent();

                    reader.ReadToDescendant("polygon");
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "polygon")
                    {
                        Polygon obstacle = new Polygon();
                        obstacle.ReadXml(reader);
                        Map.Obstacles.Add(obstacle);
                    }
                }
                else
                    throw new XmlException();

                reader.ReadToFollowing("g");
                reader.MoveToAttribute("id");
                if (reader.ReadContentAsString() == "vehicle")
                {
                    if (Vehicle == null)
                    {
                        Vehicle = new Vehicle();
                        Vehicle.Shape = new Polygon();
                    }

                    reader.MoveToContent();

                    reader.ReadToDescendant("polygon");
                    Vehicle.Shape.ReadXml(reader);
                }
                else
                    throw new XmlException();

                reader.ReadToFollowing("g");
                reader.MoveToAttribute("id");
                if (reader.ReadContentAsString() == "startEndPoint")
                {
                    double x, y;

                    reader.MoveToContent();

                    reader.ReadToDescendant("ellipse");

                    reader.MoveToAttribute("id");
                    if (reader.ReadContentAsString() == "startPoint")
                    {
                        reader.MoveToAttribute("cx");
                        x = reader.ReadContentAsDouble();

                        reader.MoveToAttribute("cy");
                        y = reader.ReadContentAsDouble();

                        StartPoint = new Point(x, y);
                    }

                    reader.ReadToFollowing("ellipse");

                    reader.MoveToAttribute("id");
                    if (reader.ReadContentAsString() == "endPoint")
                    {
                        reader.MoveToAttribute("cx");
                        x = reader.ReadContentAsDouble();

                        reader.MoveToAttribute("cy");
                        y = reader.ReadContentAsDouble();

                        EndPoint = new Point(x, y);
                    }
                }
                else
                    throw new XmlException();

                reader.ReadToFollowing("vehicleSize");
                reader.MoveToContent();
                VehicleSize = reader.ReadElementContentAsDouble();
                InitialVehicleRotation = reader.ReadElementContentAsDouble();

                if (Orders == null) Orders = new List<Order>();
                reader.ReadToFollowing("order");
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "order")
                {
                    Order order = new Order();
                    order.ReadXml(reader);
                    Orders.Add(order);
                }
                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("xmlns", "xlink", "");
            writer.WriteValue("http://www.w3.org/1999/xlink");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("type");
            writer.WriteValue(Type);
            writer.WriteEndAttribute();

            // Background
            writer.WriteStartElement("rect");

            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("rgb(1, 47, 135)");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            // Definitions
            writer.WriteStartElement("defs");

            // Map definition
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("id");
            writer.WriteValue("map");
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Map.Obstacles)
                polygon.WriteXml(writer);

            writer.WriteEndElement();

            // Vehicle definition
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("id");
            writer.WriteValue("vehicle");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("transform");
            writer.WriteValue("scale(" + VehicleSize + ")");
            writer.WriteEndAttribute();

            Vehicle.Shape.WriteXml(writer);

            writer.WriteEndElement();

            // Start point and endpoint definition
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("id");
            writer.WriteValue("startEndPoint");
            writer.WriteEndAttribute();

            writer.WriteStartElement("ellipse");

            writer.WriteStartAttribute("id");
            writer.WriteValue("startPoint");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cx");
            writer.WriteValue(StartPoint.X);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cy");
            writer.WriteValue(StartPoint.Y);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("rx");
            writer.WriteValue(0.02);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("ry");
            writer.WriteValue(0.02 * (Width / 2) / (Height / 2));
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("darkgray");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteStartElement("ellipse");

            writer.WriteStartAttribute("id");
            writer.WriteValue("endPoint");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cx");
            writer.WriteValue(EndPoint.X);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cy");
            writer.WriteValue(EndPoint.Y);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("rx");
            writer.WriteValue(0.02);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("ry");
            writer.WriteValue(0.02 * (Width / 2) / (Height / 2));
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("darkgray");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteEndElement();

            // Path definition
            writer.WriteStartElement("path");

            writer.WriteStartAttribute("id");
            writer.WriteValue("path");
            writer.WriteEndAttribute();

            string points = "M";
            for(int i = 0; i < Orders.Count; i++)
            {
                if (i != 0) points += "L";
                points += Orders[i].Destination.X + " " + Orders[i].Destination.Y + " ";
            }

            writer.WriteStartAttribute("d");
            writer.WriteValue(points);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("darkgray");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue("0.01");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("none");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteEndElement();

            // Draw animation
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + (-Height / 2) + ")");
            writer.WriteEndAttribute();

            writer.WriteStartElement("use");
            writer.WriteAttributeString("xlink", "href", null, "#map");
            writer.WriteEndElement();

            writer.WriteStartElement("use");
            writer.WriteAttributeString("xlink", "href", null, "#startEndPoint");
            writer.WriteEndElement();

            writer.WriteStartElement("use");
            writer.WriteAttributeString("xlink", "href", null, "#path");
            writer.WriteEndElement();

            // Animation of vehicle
            writer.WriteStartElement("use");

            writer.WriteAttributeString("xlink", "href", null, "#vehicle");

            writer.WriteStartElement("animateMotion");

            writer.WriteStartAttribute("dur");
            writer.WriteValue("10s");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("repeatCount");
            writer.WriteValue("indefinite");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("rotate");
            writer.WriteValue("auto");
            writer.WriteEndAttribute();

            writer.WriteStartElement("mpath");
            writer.WriteAttributeString("xlink", "href", null, "#path");
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();

            // Vehicle size&rotation
            writer.WriteStartElement("vehicleSize");
            writer.WriteValue(VehicleSize);
            writer.WriteEndElement();

            writer.WriteStartElement("vehicleInitialRotation");
            writer.WriteValue(InitialVehicleRotation);
            writer.WriteEndElement();

            // Order list
            writer.WriteStartElement("orders");
            foreach (Order order in Orders)
                order.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion
    }
}
