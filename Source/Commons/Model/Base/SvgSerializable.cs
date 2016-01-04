using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model.Base
{
    public abstract class SvgSerializable : IXmlSerializable
    {
        protected static readonly int Width = 480;
        protected static readonly int Height = 480;
        protected static readonly Color BackgroundFill = Color.FromArgb(1, 47, 135);
        protected static readonly double StrokeWidth = 3; // only height of scale transform affects stroke.

        protected string Type;

        /// <remarks>
        /// Must always return null (as specified by MSDN).
        /// </remarks>
        public XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);


        protected void WriteBackground(XmlWriter writer)
        {
            writer.WriteStartElement("rect");

            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue($"rgb({BackgroundFill.R}, {BackgroundFill.G}, {BackgroundFill.B})");
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        public static void Serialize<R>(R model, string filename)
            where R : SvgSerializable
        {
            var serializer = new XmlSerializer(typeof(R));
            var output = new XDocument();

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, model);

            output.Save(filename);
        }

        public static R Deserialize<R>(string filename)
            where R : SvgSerializable
        {
            var serializer = new XmlSerializer(typeof(R));

            using (var reader = XmlReader.Create(filename))
            {
                if (serializer.CanDeserialize(reader))
                    return (R)serializer.Deserialize(reader);
            }
            return null;
        }
    }
}
