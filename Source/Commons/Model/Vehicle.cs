﻿using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SRL.Commons.Model.Base;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "svg", Namespace = "http://www.w3.org/2000/svg")]
    public class Vehicle : SvgSerializable, IEquatable<Vehicle>
    {
        public Polygon Shape { get; set; }

        #region IXmlSerializable members

        public override void ReadXml(XmlReader reader)
        {
            Shape = new Polygon();

            reader.MoveToContent();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "svg")
            {
                reader.MoveToAttribute("width");
                reader.ReadContentAsInt();

                reader.MoveToAttribute("height");
                reader.ReadContentAsInt();

                reader.MoveToContent();

                reader.ReadToDescendant("polygon");
                Shape.ReadXml(reader);
                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("width");
            writer.WriteValue(Width);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("height");
            writer.WriteValue(Height);
            writer.WriteEndAttribute();

            // Background
            WriteBackground(writer);

            // Draw vehicle shape (translate and scale)
            writer.WriteStartElement("g");

            writer.WriteStartAttribute("transform");
            writer.WriteValue("translate(" + Width / 2 + "," + Height / 2 + ") scale(" + Width / 2 + "," + (-Height / 2) + ")");
            writer.WriteEndAttribute();

            Shape.WriteXml(writer);

            writer.WriteEndElement();

            // Draw origin point
            writer.WriteStartElement("circle");

            writer.WriteStartAttribute("cx");
            writer.WriteValue(Width / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("cy");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("r");
            writer.WriteValue(6);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fill");
            writer.WriteValue("white");
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            // Draw origin vector
            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            // Draw arrow
            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 80);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2 - 10);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteStartElement("line");

            writer.WriteStartAttribute("x1");
            writer.WriteValue(Width / 2 + 100);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y1");
            writer.WriteValue(Height / 2);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("x2");
            writer.WriteValue(Width / 2 + 80);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y2");
            writer.WriteValue(Height / 2 + 10);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke");
            writer.WriteValue("black");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("stroke-width");
            writer.WriteValue(3);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        #endregion

        #region IEquatable members

        public bool Equals(Vehicle other)
        {
            return Shape.Equals(other.Shape);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vehicle)
                return Equals((Vehicle)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
