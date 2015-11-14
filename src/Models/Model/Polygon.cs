using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    public class Polygon : IXmlSerializable, IEquatable<Polygon>
    {
        public List<Point> Vertices { get; }
        public int VertexCount => Vertices.Count;

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(params Point[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentException("Polygon must have minimum 3 vertices.");

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
                reader.ReadStartElement();

                while (reader.MoveToContent() == XmlNodeType.Element 
                    && reader.LocalName == "point")
                {
                    Point point = new Point();
                    point.ReadXml(reader);
                    Vertices.Add(point);
                }

                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            for (int i = 0; i < VertexCount; i++)
            {
                writer.WriteStartElement("point");
                Vertices[i].WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion

        /// <remarks>
        /// Only the relative order of vertices matters.
        /// </remarks>
        public bool Equals(Polygon other)
        {
            if (this.VertexCount == other.VertexCount)
            {
                int i;
                for (i = 0; i < VertexCount; i++)
                {
                    if (this.Vertices[i] == other.Vertices[0])
                        break;
                }
                if (i < VertexCount)
                {
                    for (int j = 0; j < VertexCount; j++)
                        if (this.Vertices[(i + j)%VertexCount] != other.Vertices[j])
                            return false;
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals((Polygon) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
