using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public class Order : IXmlSerializable
    {
        public double Rotation { get; set; }
        public Point Destination { get; set; }

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
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.MoveToAttribute("rotation");
                Rotation = reader.ReadContentAsDouble();

                reader.MoveToAttribute("destination");
                var coords = reader.ReadContentAsString().Split(',');
                Destination = new Point(double.Parse(coords[0]), double.Parse(coords[1]));

                reader.Skip();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("order");

            writer.WriteStartAttribute("rotation");
            writer.WriteValue(Rotation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("destination");
            writer.WriteValue(Destination.X + "," + Destination.Y);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        #endregion
    }
}
