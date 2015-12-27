using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Map : SvgSerializable
    {
        public List<Polygon> Obstacles { get; }

        public Map()
        {
            Obstacles = new List<Polygon>();
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "svg")
            {
                reader.MoveToAttribute("width");
                reader.ReadContentAsInt();

                reader.MoveToAttribute("height");
                reader.ReadContentAsInt();

                reader.MoveToAttribute("type");
                Type = reader.ReadContentAsString();

                if (Type != "map") throw new XmlException();

                reader.MoveToContent();

                reader.ReadToDescendant("polygon");
                while(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "polygon")
                {
                    Polygon obstacle = new Polygon();
                    obstacle.ReadXml(reader);
                    Obstacles.Add(obstacle);
                }
                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public override void WriteXml(XmlWriter writer)
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

            // Background
            WriteBackground(writer);

            // Draw obstacles (translate and scale)
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + (-Height / 2) + ")");
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Obstacles)
                polygon.WriteXml(writer);

            writer.WriteEndElement();
        }
    }
}
