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

            // Draw obstacles (translate and scale)
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + (-Height / 2) + ")");
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Obstacles)
                polygon.WriteXml(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}
