using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Model;
using SRL.Model.Model;
using Point = SRL.Model.Model.Point;

namespace SRL.Main
{
    public static class DrawHelper
    {
        #region Private Members

        private static readonly Dictionary<String, List<Point>> CircleCache = new Dictionary<string, List<Point>>();
        private static Texture2D _pixel;

        #endregion

        /// <summary>
        /// Creates the pixel
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        private static void CreateThePixel(SpriteBatch spriteBatch)
        {
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Draws a list of connecting points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// /// <param name="position">Where to position the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        private static void DrawPoints(SpriteBatch spriteBatch, Point position, List<Point> points, Color color, float thickness)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
        }

        /// <summary>
        /// Creates a list of vectors that represents a circle
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <returns>A list of vectors that, if connected, will create a circle</returns>
        private static List<Point> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = radius + "x" + sides;
            if (CircleCache.ContainsKey(circleKey))
            {
                return CircleCache[circleKey];
            }

            List<Point> vectors = new List<Point>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Point((radius * Math.Cos(theta)), (radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Point((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

            // Cache this circle so that it can be quickly drawn next time
            CircleCache.Add(circleKey, vectors);

            return vectors;
        }

        /// <summary>
        /// Begins drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        internal static void BeginDraw(this SpriteBatch spriteBatch)
        {
            _pixel = null;
            spriteBatch.Begin();
        }

        /// <summary>
        /// Ends drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        internal static void EndDraw(this SpriteBatch spriteBatch)
        {
            _pixel = null;
            spriteBatch.End();
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        internal static void DrawLine(this SpriteBatch spriteBatch, Point point1, Point point2, Color color, float thickness = 1.0f)
        {
            // calculate the distance between the two vectors
            float length = Vector2.Distance(new Vector2((float)point1.X, (float)point1.Y), new Vector2((float)point2.X, (float)point2.Y));

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            //DrawLine(spriteBatch, point1, distance, angle, color, thickness);

            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(_pixel,
                             new Vector2((float)point1.X, (float)point1.Y),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        /// <param name="point">The starting point.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="angle">The angle of this line from the starting point.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="thickness">The thickness of the line.</param>
        internal static void DrawLine(this SpriteBatch spriteBatch, Point point, float length, float angle, Color color, float thickness)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(_pixel,
                             new Vector2((float)point.X, (float)point.Y),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);
        }

        /// <summary>
        /// Puts the pixel on the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        /// <param name="position">The position of the pixel.</param>
        /// <param name="color">The color of the pixel.</param>
        internal static void PutPixel(this SpriteBatch spriteBatch, Point position, Color color)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            spriteBatch.Draw(_pixel, new Vector2((float)position.X, (float)position.Y), color);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="sides">The number of sides to generate.</param>
        /// <param name="color">The color of the circle.</param>
        /// <param name="thickness">The thickness of the lines used.</param>
        internal static void DrawCircle(this SpriteBatch spriteBatch, Point center, float radius, int sides, Color color, float thickness = 1.0f)
        {
            DrawPoints(spriteBatch, center, CreateCircle(radius, sides), color, thickness);
        }

        internal static void DrawPath(this SpriteBatch spriteBatch, List<Point> vertices, bool closed, Color color, float thickness = 1.0f)
        {
            for (int i = 0; i < vertices.Count - 1; i++)
                spriteBatch.DrawLine(vertices[i], vertices[i + 1], color, thickness);

            if (closed)
                spriteBatch.DrawLine(vertices[vertices.Count - 1], vertices[0], color, thickness);
        }

        internal static void DrawPolygon(this SpriteBatch spriteBatch, Polygon polygon, Color color, float thickness = 1.0f)
        {
            DrawPath(spriteBatch, polygon.Vertices, true, color, thickness);
        }

        internal static void DrawArrow(this SpriteBatch spriteBatch, Point origin, Point tip, Color color, float thickness = 1.0f)
        {
            const int arrowTopX = -12;
            const int arrowTopY = -6;
            const int arrowCenterX = 0;
            const int arrowCenterY = 0;
            const int arrowBottomX = -12;
            const int arrowBottomY = 6;

            Point arrowCenter = new Point(arrowCenterX, arrowCenterY);
            Point arrowTop = new Point(arrowTopX, arrowTopY);
            Point arrowBottom = new Point(arrowBottomX, arrowBottomY);

            // Mirror of arrow
            if (origin.X > tip.X)
            {
                arrowCenter = new Point(arrowCenter.X, arrowCenter.Y);
                arrowTop = new Point(-arrowTop.X, arrowTop.Y);
                arrowBottom = new Point(-arrowBottom.X, arrowBottom.Y);
            }

            // Rotate and translate of arrow
            double axisAngle = GeometryHelper.GetRadAngle(origin, tip);
            arrowCenter = new Point(tip.X, tip.Y);
            arrowTop = new Point(((arrowTop.X * Math.Cos(axisAngle) - arrowTop.Y * Math.Sin(axisAngle)) + tip.X),
                ((arrowTop.X * Math.Sin(axisAngle) + arrowTop.Y * Math.Cos(axisAngle)) + tip.Y));
            arrowBottom = new Point(((arrowBottom.X * Math.Cos(axisAngle) - arrowBottom.Y * Math.Sin(axisAngle)) + tip.X),
                ((arrowBottom.X * Math.Sin(axisAngle) + arrowBottom.Y * Math.Cos(axisAngle)) + tip.Y));

            // Draw
            spriteBatch.DrawLine(arrowTop, arrowCenter, color, thickness);
            spriteBatch.DrawLine(arrowBottom, arrowCenter, color, thickness);
            spriteBatch.DrawLine(origin, tip, color, thickness);
        }

        internal static void DrawArrow(this SpriteBatch spriteBatch, Point origin, float length, float angle, Color color,
            float thickness = 1.0f)
        {
            Point tip = new Point(
                origin.X + Math.Cos(angle) * length,
                origin.Y + Math.Sin(angle) * length);

            spriteBatch.DrawLine(origin, tip, color, thickness);

            //TODO fix
        }

        internal static void DrawVertex(this SpriteBatch spriteBatch, Point vertex, Color color, float thickness = 1.0f)
        {
            const int circleRadius = 5;
            const int circleSides = 10;

            spriteBatch.DrawCircle(vertex, circleRadius, circleSides, color, thickness);
        }
    }
}
