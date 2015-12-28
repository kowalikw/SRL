using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SRL.Commons.Model.Base;

namespace SRL.Commons.Model
{
    public class Order : SvgSerializable
    {
        public double Rotation { get; set; }
        public Point Destination { get; set; }

        #region IXmlSerializable members

        public override void ReadXml(XmlReader reader)
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

        public override void WriteXml(XmlWriter writer)
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
