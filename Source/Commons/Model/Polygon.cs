using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;
using SRL.Commons.Model.Base;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "polygon")]
    public class Polygon : SvgSerializable, IEquatable<Polygon>
    {
        public List<Point> Vertices { get; }

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(IEnumerable<Point> vertices)
        {
            Vertices = new List<Point>(vertices);
        }

        #region IXmlSerializable members

        public override void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.MoveToAttribute("points");
                var pointsString = reader.ReadContentAsString();
            
                if(pointsString != null)
                {
                    var points = pointsString.Split(' ');
                    foreach (var pointString in points)
                    {
                        var point = pointString.Split(',');
                        if (point.Length == 2)
                        {
                            Vertices.Add(new Point(
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
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("polygon");

            writer.WriteStartAttribute("points");
            foreach (Point point in Vertices)
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

            writer.WriteEndElement();
        }

        #endregion

        #region IEquatable members

        public bool Equals(Polygon other)
        {
            if (Vertices.Count == other.Vertices.Count)
            {
                int i;
                for (i = 0; i < Vertices.Count; i++)
                {
                    if (Vertices[i] == other.Vertices[0])
                        break;
                }
                if (i < Vertices.Count)
                {
                    for (int j = 0; j < Vertices.Count; j++)
                        if (Vertices[(i + j) % Vertices.Count] != other.Vertices[j])
                            return false;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
