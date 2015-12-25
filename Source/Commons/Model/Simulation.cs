using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public class Simulation : IXmlSerializable, ISaveable
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Type { get; set; }

        public Map Map { get; set; }

        public Vehicle Vehicle { get; set; }

        public double VehicleSize { get; set; }

        public double InitialVehicleRotation { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
        
        public List<Order> Orders { get; set; }

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
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
