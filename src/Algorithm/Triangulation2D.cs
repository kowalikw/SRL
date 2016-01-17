using SRL.Model;
using SRL.Model.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL.Main.Utilities
{
    public class Triangulation2D
    {
        public static bool PointInPolygon(float X, float Y, Point[] polygon)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = polygon.Length - 1;
            float total_angle = GetAngle(
                (float)polygon[max_point].X, (float)polygon[max_point].Y,
                X, Y,
                (float)polygon[0].X, (float)polygon[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    (float)polygon[i].X, (float)polygon[i].Y,
                    X, Y,
                    (float)polygon[i + 1].X, (float)polygon[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        public static bool PolygonIsOrientedClockwise(ref Polygon polygon)
        {
            return (SignedPolygonArea(ref polygon) < 0);
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
        public static float PolygonArea(ref Polygon polygon)
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
            int num_points = polygon.Vertices.Count;
            Point[] pts = new Point[num_points + 1];
            polygon.Vertices.CopyTo(pts, 0);
            pts[num_points] = polygon.Vertices[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (float)(pts[i + 1].X - pts[i].X) *
                    (float)(pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return area;
        }

        public static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        public static float GetAngle(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float)Math.Atan2(cross_product, dot_product);
        }

        private static void FindEar(ref int A, ref int B, ref int C, ref Polygon polygon)
        {
            int num_points = polygon.Vertices.Count;

            for (A = 0; A < num_points; A++)
            {
                B = (A + 1) % num_points;
                C = (B + 1) % num_points;

                if (FormsEar(polygon.Vertices.ToArray(), A, B, C, ref polygon)) return;
            }

            // We should never get here because there should
            // always be at least two ears.
            Debug.Assert(false);
        }

        // Return true if the three points form an ear.
        private static bool FormsEar(Point[] points, int A, int B, int C, ref Polygon polygon)
        {
            // See if the angle ABC is concave.
            if (GetAngle(
                (float)points[A].X, (float)points[A].Y,
                (float)points[B].X, (float)points[B].Y,
                (float)points[C].X, (float)points[C].Y) > 0)
            {
                // This is a concave corner so the triangle
                // cannot be an ear.
                return false;
            }

            // Make the triangle A, B, C.
            Point[] triangle = new Point[] {
                points[A], points[B], points[C] };

            // Check the other points to see 
            // if they lie in triangle A, B, C.
            for (int i = 0; i < points.Length; i++)
            {
                if ((i != A) && (i != B) && (i != C))
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
            int A = 0, B = 0, C = 0;
            FindEar(ref A, ref B, ref C, ref polygon);

            // Create a new triangle for the ear.
            triangles.Add(new Point[] { polygon.Vertices[A], polygon.Vertices[B], polygon.Vertices[C] });

            // Remove the ear from the polygon.
            RemovePointFromArray(B, ref polygon);
        }

        private static void RemovePointFromArray(int target, ref Polygon polygon)
        {
            Point[] pts = new Point[polygon.Vertices.Count - 1];
            Array.Copy(polygon.Vertices.ToArray(), 0, pts, 0, target);
            Array.Copy(polygon.Vertices.ToArray(), target + 1, pts, target, polygon.Vertices.Count - target - 1);
            polygon.Vertices = pts.ToList();
        }

        // Triangulate the polygon.
        //
        // For a nice, detailed explanation of this method,
        // see Ian Garton's Web page:
        // http://www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/cutting_ears.html
        public static List<Point[]> Triangulate(ref Polygon polygon)
        {
            // Copy the points into a scratch array.
            Point[] pts = new Point[polygon.VertexCount];
            Array.Copy(polygon.Vertices.ToArray(), pts, polygon.VertexCount);

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

















        // From Wikipedia:
        // One way to triangulate a simple polygon is by using the assertion that any simple polygon
        // without holes has at least two so called 'ears'. An ear is a triangle with two sides on the edge
        // of the polygon and the other one completely inside it. The algorithm then consists of finding
        // such an ear, removing it from the polygon (which results in a new polygon that still meets
        // the conditions) and repeating until there is only one triangle left.

        // the algorithm here aims for simplicity over performance. there are other, more performant
        // algorithms that are much more complex.

        // convert a triangle to a list of triangles. each triangle is represented by a PointF array of length 3.
        /*public static List<Point[]> Triangulate(Polygon poly)
        {
            List<Point> fullVertices = new List<Point>(poly.Vertices);
            fullVertices.Add(poly.Vertices[poly.VertexCount - 1]);

            List<Point[]> triangles = new List<Point[]>();  // accumulate the triangles here
            // keep clipping ears off of poly until only one triangle remains
            while (poly.Vertices.Count > 3)  // if only 3 points are left, we have the final triangle
            {
                int midvertex = FindEar(poly);  // find the middle vertex of the next "ear"
                triangles.Add(new Point[] { fullVertices[midvertex - 1], fullVertices[midvertex], fullVertices[midvertex + 1] });
                // create a new polygon that clips off the ear; i.e., all vertices but midvertex
                List<Point> newPts = new List<Point>(poly.Vertices);
                newPts.RemoveAt(midvertex);  // clip off the ear
                poly = new Polygon(newPts);  // poly now has one less point
            }
            // only a single triangle remains, so add it to the triangle list
            triangles.Add(poly.Vertices.ToArray());
            return triangles;
        }

        // find an ear (always a triangle) of the polygon and return the index of the middle (second) vertex in the ear
        public static int FindEar(Polygon poly)
        {
            List<Point> fullVertices = new List<Point>(poly.Vertices);
            fullVertices.Add(poly.Vertices[0]);

            Polygon newPoly = new Polygon(fullVertices);

            for (int i = 0; i < newPoly.Vertices.Count - 2; i++)
            {
                if (GeometryHelper.VertexType(newPoly, i + 1) == PolygonType.Convex)
                {
                    // get the three points of the triangle we are about to test
                    Point a = newPoly.Vertices[i];
                    Point b = newPoly.Vertices[i + 1];
                    Point c = newPoly.Vertices[i + 2];
                    bool foundAPointInTheTriangle = false;  // see if any of the other points in the polygon are in this triangle
                    for (int j = 0; j < poly.Vertices.Count; j++)  // don't check the last point, which is a duplicate of the first
                    {
                        if (j != i && j != i + 1 && j != i + 2)
                            if(PointInTriangle(newPoly.Vertices[j], a, b, c))
                                foundAPointInTheTriangle = true;
                    }
                    if (!foundAPointInTheTriangle)  // the middle point of this triangle is convex and none of the other points in the polygon are in this triangle, so it is an ear
                        return i + 1;  // EXITING HERE!
                }
            }
            //return 0;
            throw new ApplicationException("Improperly formed polygon");
        }

        

        // return true if point p is inside the triangle a,b,c
        public static bool PointInTriangle(Point p, Point a, Point b, Point c)
        {
            // three tests are required.
            // if p and c are both on the same side of the line a,b
            // and p and b are both on the same side of the line a,c
            // and p and a are both on the same side of the line b,c
            // then p is inside the triangle, o.w., not
            return PointsOnSameSide(p, a, b, c) && PointsOnSameSide(p, b, a, c) && PointsOnSameSide(p, c, a, b);
        }

        // if the two points p1 and p2 are both on the same side of the line a,b, return true
        private static bool PointsOnSameSide(Point p1, Point p2, Point a, Point b)
        {
            // these are probably the most interesting three lines of code in the algorithm (probably because I don't fully understand them)
            // the concept is nicely described at http://www.blackpawn.com/texts/pointinpoly/default.html
            float cp1 = CrossProduct(VSub(b, a), VSub(p1, a));
            float cp2 = CrossProduct(VSub(b, a), VSub(p2, a));
            return (cp1 * cp2) >= 0;  // they have the same sign if on the same side of the line
        }

        // subtract the vector (point) b from the vector (point) a
        private static Point VSub(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        // find the cross product of two x,y vectors, which is always a single value, z, representing the three dimensional vector (0,0,z)
        private static float CrossProduct(Point p1, Point p2)
        {
            return (float)((p1.X * p2.Y) - (p1.Y * p2.X));
        }*/
    }
}