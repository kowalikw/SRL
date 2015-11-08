using System;

namespace SRL.Models.Model
{
    public class Polygon
    {
        public Point[] Vertices { get; private set; }

        public Polygon(params Point[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentException("Polygon must have minimum 3 vertices.");

            Vertices = vertices;
        }
    }
}
