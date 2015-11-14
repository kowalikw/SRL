﻿using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    [XmlRoot(ElementName = "vvd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Vehicle : IXmlSerializable
    {
        public Polygon Shape { get; private set; }
        public Point Origin { get; private set; }
        public double DirectionAngle { get; private set; }

        /// <remarks>
        /// Visibility set to private as it's intended for deserialization only.
        /// </remarks>
        private Vehicle() { }

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
            DirectionAngle = angle % 360;

            //TODO check if origin lies inside the shape polygon. Throw argument exception otherwise.
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
                                DirectionAngle = reader.ReadElementContentAsDouble();
                                angleDone = true;
                            }
                            else if (reader.LocalName == "point" && !originDone)
                            {
                                Origin = new Point();
                                Origin.ReadXml(reader);
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
                Origin.WriteXml(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("angle");
                writer.WriteValue(DirectionAngle);
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
