using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClipperLib;
using SRL.Commons.Model;

namespace SRL.Commons.Utilities
{
    /// <summary>
    /// <see cref="GeometryHelper"/> class contains helper geometry methods.
    /// </summary>
    public static class GeometryHelper
    {
        private static double CrossProduct(double pX, double pY, double qX, double qY)
        {
            return pX * qY - qX * pY;
        }

        /// <summary>
        /// Calculates cross product of two vectors.
        /// </summary>
        /// <param name="p1">End <see cref="Point"/> of first vector.</param>
        /// <param name="p2">End <see cref="Point"/> of second vector.</param>
        /// <returns>Cross product of two vectors.</returns>
        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        /// <summary>
        /// Calculates dot product of two vectors.
        /// </summary>
        /// <param name="p1">End <see cref="Point"/> of first vector.</param>
        /// <param name="p2">End <see cref="Point"/> of second vector.</param>
        /// <returns>Dot product of two vectors.</returns>
        public static double DotProduct(Point p1, Point p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        /// <summary>
        /// Determines whether two segments intersect each other.
        /// </summary>
        /// <param name="p1">Start <see cref="Point"/> of first segment.</param>
        /// <param name="p2">End <see cref="Point"/> of first segment.</param>
        /// <param name="q1">Start <see cref="Point"/> of second segment.</param>
        /// <param name="q2">End <see cref="Point"/> of second segment.</param>
        /// <returns>True if two segments intersect each other, false otherwise.</returns>
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

        /// <summary>
        /// Gets distance between two points.
        /// </summary>
        /// <param name="p">First <see cref="Point"/>.</param>
        /// <param name="q">Second <see cref="Point"/>.</param>
        /// <returns>Distance between two points.</returns>
        public static double GetDistance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        /// <summary>
        /// Gets angle in radians between segment and OX axis.
        /// </summary>
        /// <param name="pivot">Pivot <see cref="Point"/>.</param>
        /// <param name="source">Source <see cref="Point"/>.</param>
        /// <param name="dest">Destination <see cref="Point"/>.</param>
        /// <returns>Angle (in radians) between segment and axis.</returns>
        public static double GetAngle(Point pivot, Point source, Point dest)
        {
            Point s = new Point(source.X - pivot.X, source.Y - pivot.Y);
            Point d = new Point(dest.X - pivot.X, dest.Y - pivot.Y);

            return Math.Atan2(
                CrossProduct(s, d),
                DotProduct(s, d));
        }

        /// <summary>
        /// Gets angle in radians between segment and OX axis.
        /// Source point of segment is (0,0).
        /// </summary>
        /// <param name="pivot">Pivot <see cref="Point"/>.</param>
        /// <param name="dest">Destination <see cref="Point"/>.</param>
        /// <returns>Angle (in radians) between segment and axis.</returns>
        public static double GetAngle(Point pivot, Point dest)
        {
            return GetAngle(pivot, new Point(pivot.X + 1, pivot.Y), dest);
        }

        /// <summary>
        /// Rotates <see cref="Point"/>.
        /// </summary>
        /// <param name="point"><see cref="Point"/> to rotate.</param>
        /// <param name="pivot">Pivot <see cref="Point"/> of rotation.</param>
        /// <param name="angle">Angle of rotation.</param>
        /// <returns>Rotated <see cref="Point"/>.</returns>
        public static Point Rotate(Point point, Point pivot, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);
            return new Point(
                    cosTheta * (point.X - pivot.X) -
                    sinTheta * (point.Y - pivot.Y) + pivot.X,
                    sinTheta * (point.X - pivot.X) +
                    cosTheta * (point.Y - pivot.Y) + pivot.Y);
        }

        /// <summary>
        /// Rotate <see cref="Polygon"/>.
        /// </summary>
        /// <param name="polygon"><see cref="Polygon"/> to rotate.</param>
        /// <param name="pivot">Pivot <see cref="Point"/> of rotation.</param>
        /// <param name="angle">Angle of rotation.</param>
        /// <returns>Rotated <see cref="Polygon"/>.</returns>
        public static Polygon Rotate(Polygon polygon, Point pivot, double angle)
        {
            return new Polygon(polygon.Vertices.Select(vertex => Rotate(vertex, pivot, angle)));
        }

        /// <summary>
        /// Rotate <see cref="Polygon"/>.
        /// Pivot of rotation is point (0,0).
        /// </summary>
        /// <param name="polygon"><see cref="Polygon"/> to rotate.</param>
        /// <param name="angle">Angle of rotation.</param>
        /// <returns>Rotated <see cref="Polygon"/>.</returns>
        public static Polygon Rotate(Polygon polygon, double angle)
        {
            return Rotate(polygon, new Point(0, 0), angle);
        }

        /// <summary>
        /// Translate <see cref="Polygon"/>.
        /// </summary>
        /// <param name="polygon"><see cref="Polygon"/> to translate.</param>
        /// <param name="position">Vector of translation.</param>
        /// <returns>Translated <see cref="Polygon"/>.</returns>
        public static Polygon Move(Polygon polygon, Point position)
        {
            return new Polygon(polygon.Vertices.Select(vertex => new Point(
                    vertex.X + position.X,
                    vertex.Y + position.Y)));
        }

        /// <summary>
        /// Resizes <see cref="Polygon"/>.
        /// </summary>
        /// <param name="polygon"><see cref="Polygon"/> to resize.</param>
        /// <param name="factor">Fctor of resizing.</param>
        /// <returns>Resized <see cref="Polygon"/>.</returns>
        public static Polygon Resize(Polygon polygon, double factor)
        {
            return new Polygon(polygon.Vertices.Select(vertex => new Point(
                    vertex.X * factor,
                    vertex.Y * factor)));
        }

        /// <summary>
        /// Transforms <see cref="Polygon"/>.
        /// </summary>
        /// <param name="polygon"><see cref="Polygon"/> to transform.</param>
        /// <param name="size">Factor of resizing.</param>
        /// <param name="rotation">Angle of rotation.</param>
        /// <param name="position">Vector of translation.</param>
        /// <returns>Transformed <see cref="Polygon"/>.</returns>
        public static Polygon Transform(this Polygon polygon, double? size = null, double? rotation = null, Point? position = null)
        {
            if (size != null)
                polygon = Resize(polygon, size.Value);
            if (rotation != null)
                polygon = Rotate(polygon, rotation.Value);
            if (position != null)
                polygon = Move(polygon, position.Value);
            return polygon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pivot">Pivot <see cref="Point"/>.</param>
        /// <param name="source">Source <see cref="Point"/>.</param>
        /// <param name="dest">Destination <see cref="Point"/>.</param>
        /// <returns></returns>
        public static bool IsCounterClockwiseTurn(Point pivot, Point source, Point dest)
        {
            return CrossProduct(source.X - pivot.X, source.Y - pivot.Y, dest.X - pivot.X, dest.Y - pivot.Y) > 0;
        }

        /// <summary>
        /// Determines whether subject polygon intersects at least one of the polygons from the list.
        /// </summary>
        /// <param name="subject">Subject <see cref="Polygon"/>.</param>
        /// <param name="polygons">List of <see cref="Polygon"/> objects.</param>
        /// <returns>True if subject polygon intersects at least one of the polygons form the list, false otherwise.</returns>
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

        /// <summary>
        /// Determines whether subject point is enclosed by polygon.
        /// </summary>
        /// <param name="subject">Subject <see cref="Point"/>.</param>
        /// <param name="polygon">Enclosure <see cref="Polygon"/>.</param>
        /// <returns>True if subject point is enclosed by polygon, false otherwise.</returns>
        public static bool IsEnclosed(Point subject, Polygon polygon)
        {
            // If subject it's on polygon edge, it's inside of polygon.

            double totalAngle = GetAngle(subject, polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]);

            for (int i = 0; i < polygon.Vertices.Count - 1; i++)
                totalAngle += GetAngle(subject, polygon.Vertices[i], polygon.Vertices[i + 1]);

            return !totalAngle.EpsilonEquals(0);
        }

        /// <summary>
        /// Determines whether subject polygon is enclosed by one fo the polygon form the list.
        /// </summary>
        /// <param name="subject">Subject <see cref="Polygon"/>.</param>
        /// <param name="polygons">List of <see cref="Polygon"/> objects.</param>
        /// <returns>True if subject polygon is enclosed by one of the polygon from list, false otherwise.</returns>
        public static bool IsEnclosed(Point subject, IEnumerable<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                if (IsEnclosed(subject, polygon))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether subject polygon is enclosed by enclosure polygon.
        /// </summary>
        /// <param name="subject">Subject <see cref="Polygon"/>.</param>
        /// <param name="enclosure">Enclosure <see cref="Polygon"/>.</param>
        /// <returns>True if subject polygon is enclosed by enclosure polygon, false otherwise.</returns>
        public static bool IsEnclosed(Polygon subject, Polygon enclosure)
        {
            foreach (var vertex in subject.Vertices)
                if (!IsEnclosed(vertex, enclosure))
                    return false;

            if (IsIntersected(subject, new List<Polygon>() { enclosure }))
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether point is enclosed by rectangle.
        /// </summary>
        /// <param name="point"><see cref="Point"/> object.</param>
        /// <param name="cornerA"><see cref="Point"/> object, rectangle corner.</param>
        /// <param name="cornerB"><see cref="Point"/> object, rectangle corner.</param>
        /// <returns>True if point is enclosed by rectangle, false otherwise.</returns>
        public static bool IsEnclosedByRect(Point point, Point cornerA, Point cornerB)
        {
            return Math.Min(cornerA.X, cornerB.X) <= point.X
                && point.X <= Math.Max(cornerA.X, cornerB.X)
                && Math.Min(cornerA.Y, cornerB.Y) <= point.Y
                && point.Y <= Math.Max(cornerA.Y, cornerB.Y);
        }

        /// <summary>
        /// Merges polygons from list into new list of polygons.
        /// Clipper source: http://www.angusj.com/delphi/clipper.php
        /// </summary>
        /// <param name="polygons">List of <see cref="Polygon"/> objects.</param>
        /// <returns>List of polygons.</returns>
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

        /// <summary>
        /// Determines whether point is out of bounds.
        /// </summary>
        /// <param name="p"><see cref="Point"/> object.</param>
        /// <returns>True if point is out of bounds, false otherwise.</returns>
        public static bool IsOutOfBounds(Point p)
        {
            return p.X > 1 || p.X < -1 || p.Y > 1 || p.Y < -1;
        }
    }
}
