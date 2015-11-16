using System;
using Point = SRL.Models.Model.Point;

namespace SRL.Models
{

    public static class GeometryHelper
    {
        public static double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
        }

        public static bool SegmentIntersection(Point p1, Point p2, Point p3, Point p4)
        {
            double d1 = VectorProduct((p4 - p3), (p1 - p3));
            double d2 = VectorProduct((p4 - p3), (p2 - p3));
            double d3 = VectorProduct((p2 - p1), (p3 - p1));
            double d4 = VectorProduct((p2 - p1), (p4 - p1));

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0) return false;
            if (d12 < 0 || d34 < 0) return true;

            return OnRectangle(p1, p3, p4) || OnRectangle(p2, p3, p4) ||
                OnRectangle(p3, p1, p2) || OnRectangle(p4, p1, p2);
        }

        private static bool OnRectangle(Point q, Point p1, Point p2)
        {
            return Math.Min(p1.X, p2.X) <= q.X && q.X <= Math.Max(p1.X, p2.X) &&
                Math.Min(p1.Y, p2.Y) <= q.Y && q.Y <= Math.Max(p1.Y, p2.Y);
        }

        private static double VectorProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }
    }
}
