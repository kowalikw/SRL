using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SRL.Models.Marshalling
{
    public static class Marshaller<T> where T : IXmlSerializable
    {
        public static XDocument Marshall(T obj)
        {
            throw new NotImplementedException();
        }

        public static T Unmarshall(XDocument obj)
        {
            throw new NotImplementedException();
        }
    }
}
