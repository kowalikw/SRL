﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClipperLib;
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

        public static double DotProduct(Point p1, Point p2)
        {
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
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        public static double GetAngle(Point pivot, Point source, Point dest)
        {
            Point s = new Point(source.X - pivot.X, source.Y - pivot.Y);
            Point d = new Point(dest.X - pivot.X, dest.Y - pivot.Y);

            return Math.Atan2(
                CrossProduct(s, d),
                DotProduct(s, d));
        }

        public static double GetAngle(Point pivot, Point dest)
        {
            return GetAngle(pivot, new Point(pivot.X + 1, pivot.Y), dest);
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
            return new Polygon(polygon.Vertices.Select(vertex => Rotate(vertex, pivot, angle)));
        }

        public static Polygon Rotate(Polygon polygon, double angle)
        {
            return Rotate(polygon, new Point(0, 0), angle);
        }

        public static Polygon Move(Polygon polygon, double dx, double dy)
        {
            return new Polygon(polygon.Vertices.Select(vertex => new Point(
                    vertex.X + dx,
                    vertex.Y + dy)));
        }

        public static Polygon Resize(Polygon polygon, double factor)
        {
            return new Polygon(polygon.Vertices.Select(vertex => new Point(
                    vertex.X * factor,
                    vertex.Y * factor)));
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

        public static bool IsEnclosed(Point subject, Polygon polygon)
        {
            // If subject it's on polygon edge, it's inside of polygon.

            double totalAngle = GetAngle(subject, polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]);

            for (int i = 0; i < polygon.Vertices.Count - 1; i++)
                totalAngle += GetAngle(subject, polygon.Vertices[i], polygon.Vertices[i + 1]);

            return !totalAngle.EpsilonEquals(0);
        }

        public static bool IsEnclosed(Point subject, IEnumerable<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                if (IsEnclosed(subject, polygon))
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

        public static List<Polygon> Union(List<Polygon> polygons)
        {
            const long multiply = long.MaxValue / 8;

            if (polygons == null)
                throw new ArgumentNullException(nameof(polygons));

            var input = new List<List<IntPoint>>(polygons.Count);
            foreach (Polygon polygon in polygons)
                input.Add(polygon.Vertices.Select(t => new IntPoint(t.X * multiply, t.Y * multiply)).ToList());

            var output = new List<List<IntPoint>>();
            Clipper c = new Clipper();
            c.AddPaths(input, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, output, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (var polygon in output)
            {
                var vertices = polygon.Select(v => new Point(v.X / (double)multiply, v.Y / (double)multiply));
                result.Add(new Polygon(vertices));
            }

            return result;
        }

        public static bool IsOutOfBounds(Point p)
        {
            return p.X > 1 || p.X < -1 || p.Y > 1 || p.Y < -1;
        }
    }
}
