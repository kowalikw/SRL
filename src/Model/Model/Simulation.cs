using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Model.Model
{
    [XmlRoot(ElementName = "simd", Namespace = "pl.pw.mini.KowMisPie.SRL")]
    public class Simulation : IXmlSerializable
    {
        public Map Map { get; set; }
        public Vehicle Vehicle { get; set; }
        public double InitialVehicleRotation { get; set; }
        
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public List<Order> Orders { get; set; }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool mapDone, vehicleDone, initRotDone, 
                startpointDone, endpointDone, ordersDone;

            mapDone = vehicleDone = initRotDone = 
                startpointDone = endpointDone = ordersDone = false;

            if (reader.MoveToContent() == XmlNodeType.Element &&
                reader.LocalName == "simd")
            {
                reader.ReadStartElement();


                while (!mapDone || !vehicleDone || !initRotDone
                       || !startpointDone || !endpointDone || !ordersDone)
                {
                    
                    reader.MoveToContent();


                    if (reader.LocalName == "vmd" && !mapDone)
                    {
                        Map = new Map();
                        Map.ReadXml(reader);
                        mapDone = true;
                    }
                    else if (reader.LocalName == "vvd" && !vehicleDone)
                    {
                        Vehicle = new Vehicle();
                        Vehicle.ReadXml(reader);
                        vehicleDone = true;
                    }
                    else if (reader.LocalName == "initialRotation" && !initRotDone)
                    {
                        InitialVehicleRotation = reader.ReadElementContentAsDouble();
                        initRotDone = true;
                    }
                    else if (reader.LocalName == "startPoint" && !startpointDone)
                    {
                        StartPoint = new Point();
                        StartPoint.ReadXml(reader);
                        startpointDone = true;
                    }
                    else if (reader.LocalName == "endPoint" && !endpointDone)
                    {
                        EndPoint = new Point();
                        EndPoint.ReadXml(reader);
                        endpointDone = true;
                    }
                    else if (reader.LocalName == "orderList" && !ordersDone)
                    {
                        Orders = new List<Order>();

                        reader.MoveToContent();
                        reader.ReadToDescendant("order");
                        while (reader.MoveToContent() == XmlNodeType.Element &&
                            reader.LocalName == "order")
                        {
                            Order order = new Order();
                            order.ReadXml(reader);
                            Orders.Add(order);
                        }
                        reader.ReadEndElement();
                        ordersDone = true;
                    }
                }

                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
                throw new XmlException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("vmd");
            Map.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("vvd");
            Vehicle.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("initialRotation");
            writer.WriteValue(InitialVehicleRotation);
            writer.WriteEndElement();

            writer.WriteStartElement("startPoint");
            StartPoint.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("endPoint");
            EndPoint.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("orderList");
            foreach (Order t in Orders)
            {
                writer.WriteStartElement("order");
                t.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }


}
