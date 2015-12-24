using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public class Simulation : IXmlSerializable
    {
        public Map Map { get; set; }

        public Vehicle Vehicle { get; set; }

        public double VehicleSize { get; set; }

        public double InitialVehicleRotation { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
        
        public List<Order> Orders { get; set; }


        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
