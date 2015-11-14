using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    // This class shall remain immutable.
    public class Point : IXmlSerializable, IEquatable<Point>
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

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
                reader.MoveToAttribute("x");
                X = reader.ReadContentAsDouble();

                reader.MoveToAttribute("y");
                Y = reader.ReadContentAsDouble();

                reader.Skip();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("x");
            writer.WriteValue(X);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteValue(Y);
            writer.WriteEndAttribute();
        }

        #endregion
        
        public bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object other)
        {
            return Equals((Point) other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Point a, Point b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !Equals(a, b);
        }

    }
}
