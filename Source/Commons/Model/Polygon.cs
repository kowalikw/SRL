using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public class Polygon : IXmlSerializable
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
                reader.MoveToAttribute("points");
                var pointsString = reader.ReadContentAsString();
            
                if(pointsString != null)
                {
                    var points = pointsString.Split(' ');
                    foreach (var pointString in points)
                    {
                        var point = pointString.Split(',');
                        if(point.Length == 2)
                            Vertices.Add(new Point(double.Parse(point[0]), double.Parse(point[1])));
                    }
                }
                else
                    throw new XmlException();

                reader.Skip();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            var points = "";

            foreach (Point point in Vertices)
                points += point.X.ToString() + "," + point.Y.ToString() + " ";

            writer.WriteStartElement("polygon");

            writer.WriteStartAttribute("points");
            writer.WriteValue(points);
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
    }
}
