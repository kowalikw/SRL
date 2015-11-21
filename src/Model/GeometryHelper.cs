using System;
using SRL.Model.Model;

namespace SRL.Model
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

            if (d12 > 0 || d34 > 0)
                return false;
            if (d12 < 0 || d34 < 0)
                return true;

            return IsInsideRect(p1, q1, q2)
                || IsInsideRect(p2, q1, q2)
                || IsInsideRect(q1, p1, p2)
                || IsInsideRect(q2, p1, p2);
        }

        private static bool IsInsideRect(Point point, Point corner1, Point corner2)
        {
            return Math.Min(corner1.X, corner2.X) <= point.X
                && point.X <= Math.Max(corner1.X, corner2.X)
                && Math.Min(corner1.Y, corner2.Y) <= point.Y
                && point.Y <= Math.Max(corner1.Y, corner2.Y);
        }

        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        public static double GetDegAngle(Point start, Point end)
        {
            return GetRadAngle(start, end) * 180 / Math.PI;
        }

        public static double GetRadAngle(Point start, Point end)
        {
            return Math.Atan((end.Y - start.Y) / (end.X - start.X));
        }
    }
}
