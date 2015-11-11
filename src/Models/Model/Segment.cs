using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = SRL.Models.Model.Point;

namespace SRL.Models.Model
{
    public class Segment
    {
        public Point Start { get; }
        public Point End { get; }

        public Segment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}
