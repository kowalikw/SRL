using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Model.Model
{
    [XmlRoot(ElementName = "vmd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Map : IXmlSerializable
    {
        public List<Polygon> Obstacles { get; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public int ObstacleCount => Obstacles.Count;

        private Map()
        {
            Obstacles = new List<Polygon>();
        }

        public Map(double width, double height)
        {
            Obstacles = new List<Polygon>();
            Width = width;
            Height = height;
        }

        public Map(double width, double height, List<Polygon> obstacles)
        {
            Obstacles = obstacles;
            Width = width;
            Height = height;
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
            reader.MoveToContent();

            if (reader.MoveToContent() == XmlNodeType.Element &&
                reader.LocalName == "vmd")
            {
                reader.MoveToAttribute("width");
                Width = reader.ReadContentAsDouble();

                reader.MoveToAttribute("height");
                Height = reader.ReadContentAsDouble();

                reader.MoveToContent();
                reader.ReadToDescendant("polygon");
                while (reader.MoveToContent() == XmlNodeType.Element &&
                    reader.LocalName == "polygon")
                {
                    Polygon obstacle = new Polygon();
                    obstacle.ReadXml(reader);
                    Obstacles.Add(obstacle);
                }
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Default encoding is UTF-8.

            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            foreach (Polygon polygon in Obstacles)
            {
                writer.WriteStartElement("polygon");

                for (int i = 0; i < polygon.VertexCount; i++)
                {
                    writer.WriteStartElement("point");

                    writer.WriteStartAttribute("x");
                    writer.WriteValue(polygon.Vertices[i].X);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("y");
                    writer.WriteValue(polygon.Vertices[i].Y);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
