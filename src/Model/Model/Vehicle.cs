using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Model.Model
{
    [XmlRoot(ElementName = "vvd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Vehicle : IXmlSerializable
    {
        public Polygon Shape { get; set; }
        public Point OrientationOrigin { get; set; }
        public Point OrientationOriginEnd { get; set; }

        public double OrientationAngle
        {
            get { return _orientationAngle;}
            set
            {
                value = value % 360;
                _orientationAngle = value < 0 ? 360 + value : value;
            }
        }

        private double _orientationAngle;

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
        /// <param name="orientationOrigin">OrientationOrigin of the axis defining the vehicle's front.</param>
        /// <param name="angle">Angle of the axis defining the vehicle's front.</param>
        public Vehicle(Polygon shape, Point orientationOrigin, double angle)
        {
            Shape = shape;
            OrientationOrigin = orientationOrigin;
            OrientationAngle = angle;

            //TODO check if OrientationOrigin lies inside the shape polygon. Throw argument exception otherwise.
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
                reader.LocalName == "vvd")
            {
                bool orientationDone = false;
                bool shapeDone = false;

                reader.ReadStartElement();

                while (!orientationDone || !shapeDone)
                {
                    if (reader.LocalName == "orientation" && !orientationDone)
                    {
                        bool angleDone = false;
                        bool originDone = false;

                        reader.ReadStartElement();

                        while (!angleDone || !originDone)
                        {
                            if (reader.LocalName == "angle" && !angleDone)
                            {
                                OrientationAngle = reader.ReadElementContentAsDouble();
                                angleDone = true;
                            }
                            else if (reader.LocalName == "point" && !originDone)
                            {
                                OrientationOrigin = new Point();
                                OrientationOrigin.ReadXml(reader);
                                originDone = true;
                            }
                            else
                                throw new XmlException();
                        }

                        reader.ReadEndElement();
                        orientationDone = true;
                    }
                    else if (reader.LocalName == "polygon" && !shapeDone)
                    {
                        Shape = new Polygon();
                        Shape.ReadXml(reader);
                        shapeDone = true;
        }
                    else
                        throw new XmlException();
                }

                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Default encoding is UTF-8.

            writer.WriteStartElement("orientation");
            {
                writer.WriteStartElement("point");
                OrientationOrigin.WriteXml(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("angle");
                writer.WriteValue(OrientationAngle);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("polygon");
            Shape.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion
    }
}
