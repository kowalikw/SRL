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

        public static double CrossProduct(Point p1, Point p2, Point? pivot = null) //TODO remove pivot parameter!
        {
            if (pivot.HasValue)
                return (p1.X - pivot.Value.X) * (p2.Y - pivot.Value.Y) - (p2.X - pivot.Value.X) * (p1.Y - pivot.Value.Y);

            return p1.X * p2.Y - p2.X * p1.Y;
        }

        public static double DotProduct(Point p1, Point p2, Point? pivot = null) //TODO remove pivot parameter!
        {
            if (pivot.HasValue)
                return (p1.X - pivot.Value.X) * (p2.X - pivot.Value.X) + (p1.Y - pivot.Value.Y) * (p2.Y - pivot.Value.Y);

            return p1.X * p2.X + p1.Y * p2.Y;
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

            return IsEnclosedByRect(p1, q1, q2)
                || IsEnclosedByRect(p2, q1, q2)
                || IsEnclosedByRect(q1, p1, p2)
                || IsEnclosedByRect(q2, p1, p2);
        }

        public static double GetDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow((p.X - q.X), 2) + Math.Pow((p.Y - q.Y), 2));
        }

        public static double GetAngle(Point pivot, Point source, Point dest)
        {
            return Math.Atan2(
                CrossProduct(source, dest, pivot),
                DotProduct(source, dest, pivot));
        }

        public static double GetAngle(Point pivot, Point dest)
        {
            return GetAngle(pivot, new Point(pivot.X + 1, 0), dest);
        }

        public static Point Rotate(Point point, Point pivot, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);
            return new Point(
                    (cosTheta * (point.X - pivot.X) -
                    sinTheta * (point.Y - pivot.Y) + pivot.X),
                    (sinTheta * (point.X - pivot.X) +
                    cosTheta * (point.Y - pivot.Y) + pivot.Y));
        }

        public static Polygon Rotate(Polygon polygon, Point pivot, double angle)
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
                output.Vertices.Add(Rotate(polygon.Vertices[i], pivot, angle));
            return output;
        }

        public static Polygon Rotate(Polygon polygon, double angle)
        {
            return Rotate(polygon, new Point(0, 0), angle);
        }

        public static Polygon Move(Polygon polygon, double dx, double dy)
        {
            var output = new Polygon();
            for (int i = 0; i < polygon.Vertices.Count; i++)
            {
                output.Vertices.Add(new Point(
                    polygon.Vertices[i].X + dx,
                    polygon.Vertices[i].Y + dy));
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

        public static bool IsCounterClockwiseTurn(Point pivot, Point source, Point dest)
        {
            return CrossProduct(source.X - pivot.X, source.Y - pivot.Y, dest.X - pivot.X, dest.Y - pivot.Y) > 0;
        }

        public static bool IsIntersected(Polygon subject, IEnumerable<Polygon> polygons)
        {
            foreach (Polygon polygon in polygons)
            {
                for (int i = 0; i < polygon.Vertices.Count; i++)
                    for (int j = 0; j < subject.Vertices.Count; j++)
                        if (DoSegmentsIntersect(
                            polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count],
                            subject.Vertices[j], subject.Vertices[(j + 1) % subject.Vertices.Count]))
                            return true;
            }

            return false;
        }

        public static bool IsEnclosed(Point point, Polygon polygon)
        {
            // If point it's on polygon edge, it's inside of polygon.

            double totalAngle = GetAngle(point, polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]);

            for (int i = 0; i < polygon.Vertices.Count - 1; i++)
                totalAngle += GetAngle(point, polygon.Vertices[i], polygon.Vertices[i + 1]);

            return Math.Abs(totalAngle) > MathHelper.DoubleComparisonEpsilon;
        }

        public static bool IsEnclosed(Point point, IEnumerable<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                if (IsEnclosed(point, polygon))
                    return true;
            }
            return false;
        }

        public static bool IsEnclosed(Polygon subject, Polygon enclosure)
        {
            foreach (var vertex in subject.Vertices)
                if (!IsEnclosed(vertex, enclosure))
                    return false;

            if (IsIntersected(subject, new List<Polygon>() { enclosure }))
                return false;

            return true;
        }

        public static bool IsEnclosedByRect(Point point, Point cornerA, Point cornerB)
        {
            return Math.Min(cornerA.X, cornerB.X) <= point.X
                && point.X <= Math.Max(cornerA.X, cornerB.X)
                && Math.Min(cornerA.Y, cornerB.Y) <= point.Y
                && point.Y <= Math.Max(cornerA.Y, cornerB.Y);
        }

    }
}
