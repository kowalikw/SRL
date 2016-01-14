using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using SRL.Commons.Model.Base;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Map : SvgSerializable, IEquatable<Map>
    {
        public List<Polygon> Obstacles { get; }

        public Map()
        {
            Obstacles = new List<Polygon>();
        }

        public bool Equals(Map other)
        {
            if (Obstacles.Count == other.Obstacles.Count)
            {
                foreach (Polygon polygon in Obstacles)
                    if (!other.Obstacles.Contains(polygon))
                        return false;
                return true;
            }
            return false;
        }

        #region IXmlSerializable members



        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "svg")
            {
                reader.MoveToAttribute("width");
                reader.ReadContentAsInt();

                reader.MoveToAttribute("height");
                reader.ReadContentAsInt();

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

        #endregion

    }
}
