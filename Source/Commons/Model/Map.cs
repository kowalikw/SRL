using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Map : IXmlSerializable, ISaveable
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Type { get; set; }

        public List<Polygon> Obstacles { get; }

        public Map()
        {
            Obstacles = new List<Polygon>();
            Width = 720; // TODO: Is it good place?
            Height = 480; // TODO: Is it good place?
            Type = "map"; // TODO: Is it good place?
        }

        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("type");
            writer.WriteValue(Type);
            writer.WriteEndAttribute();

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

            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + Height / 2 + ")");
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Obstacles)
            {
                string points = "";

                foreach (Point point in polygon.Vertices)
                    points += point.X.ToString() + "," + point.Y.ToString() + " ";

                writer.WriteStartElement("polygon");

                writer.WriteStartAttribute("points");
                writer.WriteValue(points);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("stroke");
                writer.WriteValue("black");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("stroke-width");
                writer.WriteValue("0.01");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("fill");
                writer.WriteValue("white");
                writer.WriteEndAttribute();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
