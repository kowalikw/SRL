using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Model.Xml
{
    public static class Marshaller<T> where T : IXmlSerializable
    {
        private static readonly XmlSchemaSet _schemaSet;

        static Marshaller()
        {
            _schemaSet = new XmlSchemaSet();

            string schemaString = Resources.XmlSchema;
            var reader = new StringReader(schemaString);
            _schemaSet.Add("pl.pw.mini.KowMisPie.SRL", XmlReader.Create(reader));
        }

        public static XDocument Marshall(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var output = new XDocument();
            
            using (XmlWriter writer = output.CreateWriter())
                serializer.Serialize(writer, obj);

            output.Validate(_schemaSet, (o, e) =>
            {
                throw new XmlSchemaValidationException();
            });
            
            return output;
        }

        public static T Unmarshall(XDocument doc)
        {
            doc.Validate(_schemaSet, (o, e) =>
            {
                throw new XmlSchemaValidationException();
            });

            var serializer = new XmlSerializer(typeof(T));
            T output;

            using (XmlReader reader = doc.CreateReader())
                output = (T)serializer.Deserialize(reader);

            return output;
        }
    }
}
