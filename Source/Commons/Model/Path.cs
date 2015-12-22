using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model
{
    public class Path
    {
        public List<Point> Vertices { get; }

        public Path()
        {
            Vertices = new List<Point>();
        }

        public Path(IEnumerable<Point> vertices)
        {
            Vertices = new List<Point>(vertices);
        }
    }
}
