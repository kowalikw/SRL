 using System;
 using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;
using SRL.Commons.Model;

namespace SRL.Commons.Utilities
{
    /// <summary>
    /// <see cref="SvgSerializationHelper"/> class contains helper methods
    /// to read Xml content as certain type object and write objects to Xml.
    /// </summary>
    public static class SvgSerializationHelper
    {
        /// <summary>
        /// Reads the XML content as a <see cref="Polygon"/> object.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> object.</param>
        /// <returns><see cref="Polygon"/> object.</returns>
        /// <exception cref="XmlException">Thrown if the object is not properly <see cref="Polygon"/>.</exception>
        public static Polygon ReadContentAsPolygon(this XmlReader reader)
        {
            var vertices = new List<Point>();

            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.MoveToAttribute("points");
                var pointsString = reader.ReadContentAsString();

                if (pointsString != null)
                {
                    var points = pointsString.Split(' ');
                    foreach (var pointString in points)
                    {
                        var point = pointString.Split(',');
                        if (point.Length == 2)
                        {
                            vertices.Add(new Point(
                                double.Parse(point[0], CultureInfo.InvariantCulture),
                                double.Parse(point[1], CultureInfo.InvariantCulture)));
                        }
                    }
                }
                else
                    throw new XmlException();

                reader.Skip();
            }
            else
                throw new XmlException();

            return new Polygon(vertices);
        }

        /// <summary>
        /// Writes <see cref="Polygon"/> object to XML.
        /// </summary>
        /// <param name="writer"><see cref="XmlWriter"/> object.</param>
        /// <param name="polygon"><see cref="Polygon"/> object.</param>
        public static void WritePolygon(this XmlWriter writer, Polygon polygon)
        {
            writer.WriteStartAttribute("points");
            foreach (var point in polygon.Vertices)
            {
                writer.WriteValue(point.X);
                writer.WriteValue(",");
                writer.WriteValue(point.Y);
                writer.WriteValue(" ");
            }
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue("0.01");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("white");
            writer.WriteEndAttribute();
        }

        /// <summary>
        /// Reads the XML content as a <see cref="Order"/> object.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> object.</param>
        /// <returns><see cref="Order"/> object.</returns>
        /// <exception cref="XmlException">Thrown if the object is not properly <see cref="Order"/>.</exception>
        public static Order ReadContentAsOrder(this XmlReader reader)
        {
            double rotation;
            Point destination;

            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.MoveToAttribute("rotation");
                rotation = reader.ReadContentAsDouble();

                reader.MoveToAttribute("destination");
                var coords = reader.ReadContentAsString().Split(',');
                destination = new Point(
                    double.Parse(coords[0], CultureInfo.InvariantCulture),
                    double.Parse(coords[1], CultureInfo.InvariantCulture)
                );

                reader.Skip();
            }
            else
                throw new XmlException();

            return new Order(rotation, destination);
        }

        /// <summary>
        /// Writes <see cref="Order"/> object to XML.
        /// </summary>
        /// <param name="writer"><see cref="XmlWriter"/> object.</param>
        /// <param name="order"><see cref="Order"/> object.</param>
        public static void WriteOrder(this XmlWriter writer, Order order)
        {
            writer.WriteStartAttribute("rotation");
            writer.WriteValue(order.Rotation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("destination");
            writer.WriteValue(order.Destination.X);
            writer.WriteValue(",");
            writer.WriteValue(order.Destination.Y);
            writer.WriteEndAttribute();
        }

        /// <summary>
        /// Gets <see cref="Option"/> values from XML content.
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> object.</param>
        /// <param name="key">Option key out parameter.</param>
        /// <param name="value">Option value out parameter.</param>
        /// <exception cref="XmlException">Thrown if the object is not properly <see cref="Polygon"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value type is incorrect.</exception>
        public static void GetOptionValue(this XmlReader reader, out string key, out object value)
        {
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.MoveToAttribute("type");
                Option.ValueType type;
                Enum.TryParse(reader.ReadContentAsString(), out type);

                reader.MoveToAttribute("key");
                key = reader.ReadContentAsString();

                reader.MoveToAttribute("value");
                switch (type)
                {
                    case Option.ValueType.Integer:
                        value = reader.ReadContentAsInt();
                        break;
                    case Option.ValueType.Double:
                        value = reader.ReadContentAsDouble();
                        break;
                    case Option.ValueType.Boolean:
                        value = reader.ReadContentAsBoolean();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                reader.Skip();
            }
            else
                throw new XmlException();
        }

        /// <summary>
        /// Writes <see cref="Option"/> to XML.
        /// </summary>
        /// <param name="writer"><see cref="XmlWriter"/> object.</param>
        /// <param name="option"><see cref="Option"/> object.</param>
        public static void WriteOption(this XmlWriter writer, Option option)
        {
            writer.WriteStartAttribute("type");
            writer.WriteValue(option.Type.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("key");
            writer.WriteValue(option.Key);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("value");
            switch (option.Type)
            {
                case Option.ValueType.Integer:
                    writer.WriteValue((int)option.Value);
                    break;
                case Option.ValueType.Double:
                    writer.WriteValue((double)option.Value);
                    break;
                case Option.ValueType.Boolean:
                    writer.WriteValue((bool)option.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            writer.WriteEndAttribute();
        }
    }
}
