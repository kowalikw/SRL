using System;
using System.Collections.Generic;

namespace SRL.Models.Model
{
    public class Polygon
    {
        public List<Point> Vertices { get; }
        public int VertexCount => Vertices.Count;

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(List<Point> vertices)
        {
            if (vertices.Count < 3)
                throw new ArgumentException("Polygon must have minimum 3 vertices.");

            Vertices = vertices;
        }
    }
}
