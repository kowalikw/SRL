using System;
using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model
{
    /// <summary>
    /// Model class that represents a polygon in [-1,1]x[-1,1] space.
    /// </summary>
    public struct Polygon : IEquatable<Polygon>
    {
        /// <summary>
        /// Ordered vertices that make up the polygon.
        /// </summary>
        public List<Point> Vertices { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="vertices">Ordered polygon's vertices.</param>
        public Polygon(IEnumerable<Point> vertices)
        {
            Vertices = new List<Point>(vertices);
        }

        public bool Equals(Polygon other)
        {
            if (Vertices.Count == other.Vertices.Count)
            {
                int i;
                for (i = 0; i < Vertices.Count; i++)
                {
                    if (Vertices[i] == other.Vertices[0])
                        break;
                }
                if (i < Vertices.Count)
                {
                    for (int j = 0; j < Vertices.Count; j++)
                        if (Vertices[(i + j) % Vertices.Count] != other.Vertices[j])
                            return false;
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            var polygon = obj as Polygon?;
            return polygon != null && Equals(polygon.Value);
        }

        public override int GetHashCode()
        {
            return Vertices.GetHashCode();
        }
    }
}
