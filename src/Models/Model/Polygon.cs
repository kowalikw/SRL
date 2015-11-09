using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SRL.Models.Model
{
    public class Polygon
    {
        public Point[] Vertices { get; }
        public int VertexCount => Vertices.Length;

        public Polygon(params Point[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentException("Polygon must have minimum 3 vertices.");

            Vertices = vertices;
        }
    }
}
