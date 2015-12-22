using System.Windows;

namespace SRL.Commons.Model
{
    public class Line
    {
        public Point EndpointA { get; set; }
        public Point EndpointB { get; set; }

        public Line()
        {
            
        }

        public Line(Point endpointA, Point endpointB)
        {
            EndpointA = endpointA;
            EndpointB = endpointB;
        }
    }
}
