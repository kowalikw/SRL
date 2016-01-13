using System;
using System.Globalization;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using SRL.Commons.Model.Base;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "order")]
    public class Order : SvgSerializable, IEquatable<Order>
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
                Destination = new Point(
                    double.Parse(coords[0], CultureInfo.InvariantCulture),
                    double.Parse(coords[1], CultureInfo.InvariantCulture)
                );

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
            writer.WriteValue(Destination.X);
            writer.WriteValue(",");
            writer.WriteValue(Destination.Y);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        #endregion

        #region IEquatable members

        public bool Equals(Order other)
        {
            return Rotation == other.Rotation && 
                Destination == other.Destination;
        }

        public override bool Equals(object obj)
        {
            var order = obj as Order;
            return order != null && Equals(order);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash*29 + Rotation.GetHashCode();
                hash = hash*29 + Destination.GetHashCode();
                return hash;
            }
        }

        #endregion
    }
}
