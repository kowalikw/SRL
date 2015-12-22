using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model
{
    public class Polygon
    {
        public List<Point> Vertices { get; }

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(IEnumerable<Point> vertices)
        {
            Vertices = new List<Point>(vertices);
        }
    }
}
