using System;
using System.Collections.Generic;
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

        public static double CrossProduct(Point p1, Point p2, Point? pivot = null)
        {
            if (pivot.HasValue)
                return (p1.X - pivot.Value.X) * (p2.Y - pivot.Value.Y) - (p2.X - pivot.Value.X) * (p1.Y - pivot.Value.Y);

            return p1.X * p2.Y - p2.X * p1.Y;
        }

        public static double DotProduct(Point p1, Point p2, Point? pivot = null)
        {
            if (pivot.HasValue)
                return (p1.X - pivot.Value.X) * (p2.X - pivot.Value.X) + (p1.Y - pivot.Value.Y) * (p2.Y - pivot.Value.Y);

            return p1.X * p2.X + p1.Y * p2.Y;
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
            // If point it's on polygon edge, it's inside of polygon.

            double epsilon = 0.000001;

            double totalAngle = GetAngle(point, polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]);

            for (int i = 0; i < polygon.Vertices.Count - 1; i++)
                totalAngle += GetAngle(point, polygon.Vertices[i], polygon.Vertices[i + 1]);

            return Math.Abs(totalAngle) > epsilon;
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

        public static double GetAngle(Point pivot, Point point)
        {
            double angle = Math.Atan((point.Y - pivot.Y) / (point.X - pivot.X));

            if (point.X < pivot.X)
                angle += Math.PI;

            return angle;
        }

        public static double GetAngle(Point pivot, Point origin, Point point)
        {
            return Math.Atan2(
                CrossProduct(origin, point, pivot), 
                DotProduct(origin, point, pivot));
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

        public static Polygon Rotate(Point pivot, Polygon polygon, double angle) //TODO change parameter order (pivot should be second), add ref to point and remove return value.
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
                output.Vertices.Add(RotatePoint(polygon.Vertices[i], pivot, angle));
            return output;
        }

        public static Polygon Rotate(Polygon polygon, double angle)
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
                output.Vertices.Add(RotatePoint(polygon.Vertices[i], new Point(0, 0), angle));
            return output;
        }

        public static Polygon Move(Polygon polygon, double x, double y)
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
            {
                output.Vertices.Add(new Point(
                    polygon.Vertices[i].X + x,
                    polygon.Vertices[i].Y + y));
            }
            return output;
        }

        public static Polygon Resize(Polygon polygon, double factor)
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
            {
                output.Vertices.Add(new Point(
                    polygon.Vertices[i].X * factor,
                    polygon.Vertices[i].Y * factor));
            }
            return output;
        }

        public static bool IsCounterClockwiseTurn(Point pivot, Point init, Point target)
        {
            return CrossProduct(
                new Point(init.X - pivot.X, init.Y - pivot.Y),
                new Point(target.X - pivot.X, target.Y - pivot.Y)) > 0; // TODO Doesn't return value == 0 mean that no there's no turn? Fix me
        }

        public static bool IsIntersected(Polygon subject, IEnumerable<Polygon> polygons)
        {
            foreach (Polygon polygon in polygons)
                for (int i = 0; i < polygon.Vertices.Count; i++)
                    for (int j = 0; j < subject.Vertices.Count; j++)
                        if (DoSegmentsIntersect(
                            polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count],
                            subject.Vertices[j], subject.Vertices[(j + 1) % subject.Vertices.Count]))
                            return true;

            return false;
        }

        public static bool IsEnclosed(Polygon subject, Polygon enclosure)
        {
            bool enclosed = false;

            for (int i = 0; i < subject.Vertices.Count; i++)
                if (!IsInsidePolygon(subject.Vertices[i], enclosure))
                    return false;

            return enclosed;
        }

    }
}
