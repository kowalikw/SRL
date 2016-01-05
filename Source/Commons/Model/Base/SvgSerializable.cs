using System;
using System.Drawing;
using System.IO;
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

        public static bool CanDeserialize<R>(string filename)
        {
            bool canDeserialize = true;

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            if(typeof(R) == typeof(Map))
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.MapSchema)));
            else if(typeof(R) == typeof(Vehicle))
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.VehicleSchema)));
            else if(typeof(R) == typeof(Simulation))
            {
                schemaSet.Add("http://www.w3.org/1999/xlink", XmlReader.Create(new StringReader(Resources.SimulationSchemaXlink)));
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.SimulationSchema)));
            }

            XDocument.Load(filename).Validate(schemaSet, (o, e) => { canDeserialize = false; });

            var serializer = new XmlSerializer(typeof(R));

            using (var reader = XmlReader.Create(filename))
                return canDeserialize && serializer.CanDeserialize(reader);
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
            throw new InvalidOperationException($"Can't deserialize {filename}.");
        }
    }
}
