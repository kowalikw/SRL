using System;
using System.Windows;
using SRL.Commons.Model;

namespace SRL.Commons.Utilities
{
    public static class GeometryHelper
    {
        private static double CrossProduct(double pX, double pY, double qX, double qY)
        {
            return pX * qY - qX * pY;
        }

        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        private static bool IsInsideRectangle(Point point, Point cornerA, Point cornerB)
        {
            return Math.Min(cornerA.X, cornerB.X) <= point.X
                && point.X <= Math.Max(cornerA.X, cornerB.X)
                && Math.Min(cornerA.Y, cornerB.Y) <= point.Y
                && point.Y <= Math.Max(cornerA.Y, cornerB.Y);
        }

        public static bool IsInsidePolygon(Point point, Polygon polygon)
        {
            throw new NotImplementedException(); //TODO
        }

        public static bool DoSegmentsIntersect(Point p1, Point p2, Point q1, Point q2)
        {
            double d1 = CrossProduct(q2.X - q1.X, q2.Y - q1.Y, p1.X - q1.X, p1.Y - q1.Y);
            double d2 = CrossProduct(q2.X - q1.X, q2.Y - q1.Y, p2.X - q1.X, p2.Y - q1.Y);
            double d3 = CrossProduct(p2.X - p1.X, p2.Y - p1.Y, q1.X - p1.X, q1.Y - p1.Y);
            double d4 = CrossProduct(p2.X - p1.X, p2.Y - p1.Y, q2.X - p1.X, q2.Y - p1.Y);

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0)
                return false;
            if (d12 < 0 || d34 < 0)
                return true;

            return IsInsideRectangle(p1, q1, q2)
                || IsInsideRectangle(p2, q1, q2)
                || IsInsideRectangle(q1, p1, p2)
                || IsInsideRectangle(q2, p1, p2);
        }

        public static double GetDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow((p.X - q.X), 2) + Math.Pow((p.Y - q.Y), 2));
        }

        public static Point RotatePoint(Point point, Point pivot, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);
            return new Point(
                    (cosTheta * (point.X - pivot.X) -
                    sinTheta * (point.Y - pivot.Y) + pivot.X),
                    (sinTheta * (point.X - pivot.X) +
                    cosTheta * (point.Y - pivot.Y) + pivot.Y));
        }

        public static bool IsCounterClockwiseTurn(Point pivot, Point init, Point target)
        {
            return CrossProduct(
                new Point(init.X - pivot.X, init.Y - pivot.Y), 
                new Point(target.X - pivot.X, target.Y - pivot.Y)) > 0;
        }
    }
}
