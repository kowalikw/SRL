using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons
{
    public abstract class SvgSerializable : IXmlSerializable
    {
        protected int Width = 480;
        protected int Height = 480;

        protected string Type;

        /// <remarks>
        /// Must always return null (as specified by MSDN).
        /// </remarks>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}
