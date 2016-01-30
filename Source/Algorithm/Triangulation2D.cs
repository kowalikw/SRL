using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using SRL.Commons.Model;

namespace SRL.Algorithm
{
    /// <summary>
    /// Helper class that slices polygons into triangles.
    /// </summary>
    public class Triangulation2D
    {
        private static bool PointInPolygon(float x, float y, Point[] polygon)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int maxPoint = polygon.Length - 1;
            float totalAngle = GetAngle(
                (float)polygon[maxPoint].X, (float)polygon[maxPoint].Y,
                x, y,
                (float)polygon[0].X, (float)polygon[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < maxPoint; i++)
            {
                totalAngle += GetAngle(
                    (float)polygon[i].X, (float)polygon[i].Y,
                    x, y,
                    (float)polygon[i + 1].X, (float)polygon[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return Math.Abs(totalAngle) > 0.000001;
        }

        private static bool PolygonIsOrientedClockwise(ref Polygon polygon)
        {
            return SignedPolygonArea(ref polygon) < 0;
        }

        // If the polygon is oriented counterclockwise,
        // reverse the order of its points.
        private static void OrientPolygonClockwise(ref Polygon polygon)
        {
            if (!PolygonIsOrientedClockwise(ref polygon))
                polygon.Vertices.Reverse();
        }


        // Return the polygon's area in "square units."
        // Add the areas of the trapezoids defined by the
        // polygon's edges dropped to the X-axis. When the
        // program considers a bottom edge of a polygon, the
        // calculation gives a negative area so the space
        // between the polygon and the axis is subtracted,
        // leaving the polygon's area. This method gives odd
        // results for non-simple polygons.
        private static float PolygonArea(ref Polygon polygon)
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polygon is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea(ref polygon));
        }

        // Return the polygon's area in "square units."
        // Add the areas of the trapezoids defined by the
        // polygon's edges dropped to the X-axis. When the
        // program considers a bottom edge of a polygon, the
        // calculation gives a negative area so the space
        // between the polygon and the axis is subtracted,
        // leaving the polygon's area. This method gives odd
        // results for non-simple polygons.
        //
        // The value will be negative if the polygon is
        // oriented clockwise.
        private static float SignedPolygonArea(ref Polygon polygon)
        {
            // Add the first point to the end.
            int numPoints = polygon.Vertices.Count;
            Point[] pts = new Point[numPoints + 1];
            polygon.Vertices.CopyTo(pts, 0);
            pts[numPoints] = polygon.Vertices[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < numPoints; i++)
            {
                area +=
                    (float)(pts[i + 1].X - pts[i].X) *
                    (float)(pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return area;
        }

        private static float CrossProductLength(float ax, float ay,
            float bx, float @by, float cx, float cy)
        {
            // Get the vectors' coordinates.
            float bAx = ax - bx;
            float bAy = ay - @by;
            float bCx = cx - bx;
            float bCy = cy - @by;

            // Calculate the Z coordinate of the cross product.
            return bAx * bCy - bAy * bCx;
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float ax, float ay,
            float bx, float @by, float cx, float cy)
        {
            // Get the vectors' coordinates.
            float bAx = ax - bx;
            float bAy = ay - @by;
            float bCx = cx - bx;
            float bCy = cy - @by;

            // Calculate the dot product.
            return bAx * bCx + bAy * bCy;
        }

        private static float GetAngle(float ax, float ay, float bx, float @by, float cx, float cy)
        {
            // Get the dot product.
            float dotProduct = DotProduct(ax, ay, bx, @by, cx, cy);

            // Get the cross product.
            float crossProduct = CrossProductLength(ax, ay, bx, @by, cx, cy);

            // Calculate the angle.
            return (float)Math.Atan2(crossProduct, dotProduct);
        }

        private static void FindEar(ref int a, ref int b, ref int c, ref Polygon polygon)
        {
            int numPoints = polygon.Vertices.Count;

            for (a = 0; a < numPoints; a++)
            {
                b = (a + 1) % numPoints;
                c = (b + 1) % numPoints;

                if (FormsEar(polygon.Vertices.ToArray(), a, b, c, ref polygon)) return;
            }

            // We should never get here because there should
            // always be at least two ears.
            Debug.Assert(false);
        }

        // Return true if the three points form an ear.
        private static bool FormsEar(Point[] points, int a, int b, int c, ref Polygon polygon)
        {
            // See if the angle ABC is concave.
            if (GetAngle(
                (float)points[a].X, (float)points[a].Y,
                (float)points[b].X, (float)points[b].Y,
                (float)points[c].X, (float)points[c].Y) > 0)
            {
                // This is a concave corner so the triangle
                // cannot be an ear.
                return false;
            }

            // Make the triangle A, B, C.
            Point[] triangle = new Point[] {
                points[a], points[b], points[c] };

            // Check the other points to see 
            // if they lie in triangle A, B, C.
            for (int i = 0; i < points.Length; i++)
            {
                if ((i != a) && (i != b) && (i != c))
                {
                    if (PointInPolygon((float)points[i].X, (float)points[i].Y, triangle))
                    {
                        // This point is in the triangle 
                        // do this is not an ear.
                        return false;
                    }
                }
            }

            // This is an ear.
            return true;
        }

        // Remove an ear from the polygon and
        // add it to the triangles array.
        private static void RemoveEar(List<Point[]> triangles, ref Polygon polygon)
        {
            // Find an ear.
            int a = 0, b = 0, c = 0;
            FindEar(ref a, ref b, ref c, ref polygon);

            // Create a new triangle for the ear.
            triangles.Add(new Point[] { polygon.Vertices[a], polygon.Vertices[b], polygon.Vertices[c] });

            // Remove the ear from the polygon.
            RemovePointFromArray(b, ref polygon);
        }

        private static void RemovePointFromArray(int target, ref Polygon polygon)
        {
            Point[] pts = new Point[polygon.Vertices.Count - 1];
            Array.Copy(polygon.Vertices.ToArray(), 0, pts, 0, target);
            Array.Copy(polygon.Vertices.ToArray(), target + 1, pts, target, polygon.Vertices.Count - target - 1);
            polygon = new Polygon(pts.ToList());
        }

        /// <summary>
        /// Triangulates the polygon.
        /// </summary>
        /// <param name="polygon">Polygon to triangulate.</param>
        /// <returns>Triangle parts of the polygon.</returns>
        /// <remarks>
        /// For a nice, detailed explanation of this method, see Ian Garton's Web page:
        /// http://www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/cutting_ears.html
        /// </remarks>
        public static List<Point[]> Triangulate(ref Polygon polygon)
        {
            // Copy the points into a scratch array.
            Point[] pts = new Point[polygon.Vertices.Count];
            Array.Copy(polygon.Vertices.ToArray(), pts, polygon.Vertices.Count);

            // Make a scratch polygon.
            Polygon pgon = new Polygon(pts.ToList());

            // Orient the polygon clockwise.
            //pgon.OrientPolygonClockwise();
            OrientPolygonClockwise(ref pgon);

            // Make room for the triangles.
            List<Point[]> triangles = new List<Point[]>();

            // While the copy of the polygon has more than
            // three points, remove an ear.
            while (pgon.Vertices.Count > 3)
            {
                // Remove an ear from the polygon.
                RemoveEar(triangles, ref pgon);
            }

            // Copy the last three points into their own triangle.
            triangles.Add(new Point[] { pgon.Vertices[0], pgon.Vertices[1], pgon.Vertices[2] });

            return triangles;
        }
    }
}