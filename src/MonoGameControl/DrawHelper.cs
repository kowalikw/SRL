using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Models.Enum;
using SRL.Models.Model;
using System;
using System.Collections.Generic;
using Point = SRL.Models.Model.Point;

namespace SRL.MonoGameControl
{
    public static class DrawHelper
    {
        public const int StartCircleRadius = 8;
        public const int StartCircleThickness = 3;
        public const int PointRadius = 3;
        public const int PointThickness = 3;
        public const int LineThickness = 2;
        public const int CircleSegments = 100;

        private static readonly Color normalDrawColor = Color.Black;
        private static readonly Color activeDrawColor = Color.Blue;
        private static readonly Color correctActiveDrawColor = Color.Green;
        private static readonly Color incorrectActiveDrawColor = Color.Red;
        private static readonly Color activeStartCircleColor = Color.Orange;

        #region Private Members

        private static readonly Dictionary<String, List<Point>> circleCache = new Dictionary<string, List<Point>>();
        private static Texture2D pixel;

        #endregion

        /// <summary>
        /// Creates the pixel
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        private static void CreateThePixel(SpriteBatch spriteBatch)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
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

        public static void DrawPolygon(this SpriteBatch spriteBatch, Polygon polygon, DrawPolygonState state, Point cursorPosition = null)
        {
            if (state == DrawPolygonState.Done)
            {
                for (int i = 0; i < polygon.VertexCount; i++)
                    spriteBatch.DrawLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.VertexCount], normalDrawColor, LineThickness);
            }
            else if (state == DrawPolygonState.Empty) return;
            else
            {
                for (int i = 0; i < polygon.VertexCount; i++)
                {
                    if (i == 0)
                        spriteBatch.DrawCircle(polygon.Vertices[0], StartCircleRadius, CircleSegments, activeDrawColor, StartCircleThickness);
                    else
                        spriteBatch.DrawLine(polygon.Vertices[i - 1], polygon.Vertices[i], activeDrawColor, LineThickness);

                    spriteBatch.DrawCircle(polygon.Vertices[0], PointRadius, CircleSegments, activeDrawColor, PointThickness);
                }

                if (state == DrawPolygonState.Correct)
                {
                    if (polygon.IsFinished(cursorPosition))
                    {
                        spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], polygon.Vertices[0], correctActiveDrawColor, LineThickness);
                        spriteBatch.DrawCircle(polygon.Vertices[0], StartCircleRadius, CircleSegments, activeStartCircleColor, StartCircleThickness);
                    }
                    else
                        spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], cursorPosition, correctActiveDrawColor, LineThickness);
                }
                else if (state == DrawPolygonState.Incorrect)
                {
                    spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], cursorPosition, incorrectActiveDrawColor, LineThickness);
                }
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
            if (circleCache.ContainsKey(circleKey))
            {
                return circleCache[circleKey];
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
            circleCache.Add(circleKey, vectors);

            return vectors;
        }

        /// <summary>
        /// Begins drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        public static void BeginDraw(this SpriteBatch spriteBatch)
        {
            pixel = null;
            spriteBatch.Begin();
        }

        /// <summary>
        /// Ends drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        public static void EndDraw(this SpriteBatch spriteBatch)
        {
            pixel = null;
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
        public static void DrawLine(this SpriteBatch spriteBatch, Point point1, Point point2, Color color, float thickness = 1.0f)
        {
            // calculate the distance between the two vectors
            float length = Vector2.Distance(new Vector2((float)point1.X, (float)point1.Y), new Vector2((float)point2.X, (float)point2.Y));

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            //DrawLine(spriteBatch, point1, distance, angle, color, thickness);

            if (pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(pixel,
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
        public static void DrawLine(this SpriteBatch spriteBatch, Point point, float length, float angle, Color color, float thickness)
        {
            if (pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(pixel,
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
        public static void PutPixel(this SpriteBatch spriteBatch, Point position, Color color)
        {
            if (pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            spriteBatch.Draw(pixel, new Vector2((float)position.X, (float)position.Y), color);
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
        public static void DrawCircle(this SpriteBatch spriteBatch, Point center, float radius, int sides, Color color, float thickness = 1.0f)
        {
            DrawPoints(spriteBatch, center, CreateCircle(radius, sides), color, thickness);
        }
    }
}