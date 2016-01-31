using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using SRL.Commons.Model.Base;
using SRL.Commons.Utilities;

namespace SRL.Commons.Model
{
    /// <summary>
    /// Model class that represents a collection of <see cref="Polygon"/> obstacles that must avoided by the <see cref="Vehicle"/>.
    /// </summary>
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Map : SvgSerializable, IEquatable<Map>
    {
        /// <summary>
        /// List of contained obstacles.
        /// </summary>
        public List<Polygon> Obstacles { get; }

        public Map()
        {
            Obstacles = new List<Polygon>();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                    obstacle = reader.ReadContentAsPolygon();
                    Obstacles.Add(obstacle);
                }
                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        /// <inheritdoc />
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
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + -Height / 2 + ")");
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Obstacles)
            {
                writer.WriteStartElement("polygon");
                writer.WritePolygon(polygon);
                writer.WriteEndElement();
            }


            writer.WriteEndElement();
        }

        #endregion

    }
}
