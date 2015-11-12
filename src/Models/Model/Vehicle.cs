using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    [XmlRoot(ElementName = "vvd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Vehicle : IXmlSerializable
    {
        public Polygon Shape { get; set; }
        public Point Origin { get; set; }
        public double FrontAngle { get; set; }

        /// <remarks>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// </remarks>
        public Vehicle()
        {
            Shape = new Polygon();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// </summary>
        /// <param name="shape"><see cref="Polygon"/> that constitutes the vehicle's boundary.</param>
        /// <param name="origin">Origin of the axis defining the vehicle's front.</param>
        /// <param name="angle">Angle of the axis defining the vehicle's front.</param>
        public Vehicle(Polygon shape, Point origin, double angle)
        {
            Shape = shape;
            Origin = origin;
            FrontAngle = angle;
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

            writer.WriteStartElement("orientation");
            {
                writer.WriteStartElement("point");
                {
                    writer.WriteStartAttribute("x");
                    writer.WriteValue(Origin.X);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("y");
                    writer.WriteValue(Origin.Y);
                    writer.WriteEndAttribute();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("angle");
                writer.WriteValue(FrontAngle);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("polygon");
            for (int i = 0; i < Shape.VertexCount; i++)
            {
                writer.WriteStartElement("point");
                {
                    writer.WriteStartAttribute("x");
                    writer.WriteValue(Shape.Vertices[i].X);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("y");
                    writer.WriteValue(Shape.Vertices[i].Y);
                    writer.WriteEndAttribute();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
