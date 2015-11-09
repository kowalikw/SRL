using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    [XmlRoot(ElementName = "vmd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Map : IXmlSerializable
    {
        public List<Polygon> Obstacles { get; }

        public Map()
        {
            Obstacles = new List<Polygon>();
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
            //TODO
            throw new System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Default encoding is UTF-8.

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
