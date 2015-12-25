using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public class Vehicle : IXmlSerializable, ISaveable
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Type { get; set; }

        public Polygon Shape { get; set; }

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

                if (Type != "vehicle") throw new XmlException();

                reader.MoveToContent();

                reader.ReadToDescendant("polygon");
                Shape.ReadXml(reader);
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

            // Draw vehicle shape (translate and scale)
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + (-Height / 2) + ")");
            writer.WriteEndAttribute();

            Shape.WriteXml(writer);

            writer.WriteEndElement();

            // Draw origin point
            writer.WriteStartElement("circle");

            writer.WriteStartAttribute("cx");
            writer.WriteValue(Width / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cy");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("r");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("white");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            // Draw origin vector
            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            // Draw arrow
            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 80);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2 - 10);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 80);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2 + 10);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        #endregion
    }
}
