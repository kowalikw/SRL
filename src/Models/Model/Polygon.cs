using SRL.Models.Enum;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    public class Polygon : IXmlSerializable, IEquatable<Polygon>
    {
        public const int MinVerticesCount = 3;
        public const int StartPointRadius = 8;
        public List<Point> Vertices { get; }
        public int VertexCount => Vertices.Count;

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(params Point[] vertices)
        {
            Vertices = new List<Point>();
        }

        public Polygon(List<Point> vertices)
        {
            if (vertices.Count < MinVerticesCount)
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

        public bool IsCorrect()
        {
            if (VertexCount < MinVerticesCount) return false;

            for (int i = 1; i < VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(Vertices[i], Vertices[i + 1], Vertices[VertexCount - 1], Vertices[0]))
                    return false;

            return true;
        }

        public bool IsCorrect(Point nextVertice)
        {
            if (VertexCount < MinVerticesCount) return false;

            for (int i = 1; i < VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(Vertices[i], Vertices[i + 1], nextVertice, Vertices[0]))
                    return false;

            return true;
        }

        public bool IsFinished()
        {
            if (GeometryHelper.DistanceBetweenPoints(Vertices[0], Vertices[VertexCount - 1]) <= StartPointRadius && VertexCount >= MinVerticesCount && IsCorrect())
                return true;

            return false;
        }

        public bool IsFinished(Point nextVertice)
        {
            if (Vertices.Count >= MinVerticesCount && GeometryHelper.DistanceBetweenPoints(Vertices[0], nextVertice) <= StartPointRadius)
                return true;

            return false;
        }

        public bool IsEmpty()
        {
            return VertexCount == 0;
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
                        if (this.Vertices[(i + j) % VertexCount] != other.Vertices[j])
                            return false;
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals((Polygon)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}
