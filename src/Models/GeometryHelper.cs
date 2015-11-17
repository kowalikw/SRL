using System;
using Point = SRL.Models.Model.Point;

namespace SRL.Models
{

    public static class GeometryHelper
    {
        public static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
        }

        public static bool DoSegmentsIntersect(Point p1, Point p2, Point q1, Point q2)
        {
            double d1 = CrossProduct((q2 - q1), (p1 - q1));
            double d2 = CrossProduct((q2 - q1), (p2 - q1));
            double d3 = CrossProduct((p2 - p1), (q1 - p1));
            double d4 = CrossProduct((p2 - p1), (q2 - p1));

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0) return false;
            if (d12 < 0 || d34 < 0) return true;

            return OnRectangle(p1, q1, q2) || OnRectangle(p2, q1, q2) ||
                OnRectangle(q1, p1, p2) || OnRectangle(q2, p1, p2);
        }

        private static bool OnRectangle(Point q, Point p1, Point p2)
        {
            return Math.Min(p1.X, p2.X) <= q.X && q.X <= Math.Max(p1.X, p2.X) &&
                Math.Min(p1.Y, p2.Y) <= q.Y && q.Y <= Math.Max(p1.Y, p2.Y);
        }

        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }
    }
}
