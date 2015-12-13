using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Model.Model
{
    public class Order : IXmlSerializable
    {
        public double Rotation { get; set; } // RADIANS
        public Point Destination { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool rotationDone = false;
            bool destinationDone = false;

            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.ReadStartElement();

                while (!rotationDone && !destinationDone)
                {
                    if (reader.LocalName == "rotation" && !rotationDone)
                    {
                        Rotation = reader.ReadElementContentAsDouble();
                        rotationDone = true;
                    }
                    else if (reader.LocalName == "destination" && !destinationDone)
                    {
                        Destination = new Point();
                        Destination.ReadXml(reader);
                        destinationDone = true;
                    }
                    else
                        throw new XmlException();
                }

                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("rotation");
            writer.WriteValue(Rotation);
            writer.WriteEndElement();

            writer.WriteStartElement("destination");
            Destination.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
