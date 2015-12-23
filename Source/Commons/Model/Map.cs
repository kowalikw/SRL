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

            foreach (Polygon polygon in Obstacles)
            {
                string points = "";

                foreach (Point point in polygon.Vertices)
                    points += point.X + point.Y + " ";

                writer.WriteStartElement("polygon");

                

                writer.WriteEndElement();
            }

            //throw new System.NotImplementedException();
        }
    }
}
