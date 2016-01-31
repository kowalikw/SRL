using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model.Base
{
    /// <summary>
    /// Base class for models that can be serialized to a SVG file.
    /// </summary>
    public abstract class SvgSerializable : IXmlSerializable
    {
        /// <summary>
        /// Output image width.
        /// </summary>
        protected static readonly int Width = 480;
        /// <summary>
        /// Output image height.
        /// </summary>
        protected static readonly int Height = 480;
        /// <summary>
        /// Background color.
        /// </summary>
        protected static readonly Color BackgroundFill = Color.FromArgb(1, 47, 135);
        /// <summary>
        /// Shape stroke width.
        /// </summary>
        protected static readonly double StrokeWidth = 3; // only height of scale transform affects stroke.

        /// <summary>
        /// Always returns null (as specified by MSDN).
        /// </summary>
        /// <returns>Null.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML (SVG) representation.
        /// </summary>
        /// <param name="reader">Stream from which the object is deserialized.</param>
        public abstract void ReadXml(XmlReader reader);
        /// <summary>
        /// Converts an object into its XML (SVG) representation.
        /// </summary>
        /// <param name="writer">Stream to which the object is serialized.</param>
        public abstract void WriteXml(XmlWriter writer);

        /// <summary>
        /// Writes SVG rectangle to the stream to act as image background.
        /// </summary>
        /// <param name="writer">Stream to which background definition is added.</param>
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

        /// <summary>
        /// Serializes model object to SVG file.
        /// </summary>
        /// <typeparam name="TR">Serializable model type.</typeparam>
        /// <param name="model">Model instance.</param>
        /// <param name="filename">Filename of the SVG file.</param>
        public static void Serialize<TR>(TR model, string filename)
            where TR : SvgSerializable
        {
            var serializer = new XmlSerializer(typeof(TR));
            var output = new XDocument();

            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, model);

            output.Save(filename);
        }

        /// <summary>
        /// Gets a value that indicates whether the file can be deserialized to the model type.
        /// </summary>
        /// <typeparam name="TR">Type of the model.</typeparam>
        /// <param name="filename">Serialized object's filename.</param>
        /// <returns>True if the file contains a valid model object's definition; false otherwise.</returns>
        public static bool CanDeserialize<TR>(string filename)
            where TR : SvgSerializable
        {
            bool canDeserialize = true;

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            if (typeof(TR) == typeof(Map))
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.MapSchema)));
            else if (typeof(TR) == typeof(Vehicle))
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.VehicleSchema)));
            else if (typeof(TR) == typeof(Simulation))
            {
                schemaSet.Add("http://www.w3.org/1999/xlink", XmlReader.Create(new StringReader(Resources.SimulationSchemaXlink)));
                schemaSet.Add("http://www.w3.org/2000/svg", XmlReader.Create(new StringReader(Resources.SimulationSchema)));
            }

            XDocument.Load(filename).Validate(schemaSet, (o, e) => { canDeserialize = false; });

            var serializer = new XmlSerializer(typeof(TR));

            using (var reader = XmlReader.Create(filename))
                return canDeserialize && serializer.CanDeserialize(reader);
        }

        /// <summary>
        /// Deserializes SVG file into a model instance.
        /// </summary>
        /// <typeparam name="TR">Type of the model.</typeparam>
        /// <param name="filename">Serialized object's filename.</param>
        /// <returns>Deserialized model instance.</returns>
        public static TR Deserialize<TR>(string filename)
            where TR : SvgSerializable
        {
            var serializer = new XmlSerializer(typeof(TR));

            using (var reader = XmlReader.Create(filename))
            {
                if (serializer.CanDeserialize(reader))
                    return (TR)serializer.Deserialize(reader);
            }
            throw new InvalidOperationException($"Can't deserialize {filename}.");
        }
    }
}
