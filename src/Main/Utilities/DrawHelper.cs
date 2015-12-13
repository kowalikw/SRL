using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Model;
using SRL.Model.Model;
using Point = SRL.Model.Model.Point;

namespace SRL.Main.Utilities
{
    internal static class DrawHelper
    {
        #region Private Members

        private static readonly Dictionary<string, List<Point>> CircleCache = new Dictionary<string, List<Point>>();
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
            //float length = Vector2.Distance(new Vector2((float)point1.X, (float)point1.Y), new Vector2((float)point2.X, (float)point2.Y));

            // calculate the angle between the two vectors
            //float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            

            //DrawLine(spriteBatch, point1, distance, angle, color, thickness);

            if (_pixel == null)
                CreateThePixel(spriteBatch);

            /*if (color.R == 0 && color.G == 0 && color.B == 0)
                _pixel.SetData(new[] { Color.White });
            else if(color.R == 255 && color.G == 255 && color.B == 255)
                _pixel.SetData(new[] { Color.White });
            else if (color.R == 255 || color.G == 255 || color.B == 255)
                _pixel.SetData(new[] { Color.White });*/

            // stretch the pixel between the two vectors
            /*spriteBatch.Draw(_pixel,
                             new Vector2((float)point1.X, (float)point1.Y),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);*/

            double x0 = point1.X;
            double y0 = point1.Y;
            double x1 = point2.X;
            double y1 = point2.Y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            

            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }

            if(x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            //Console.WriteLine($"x={x0}, y0={y0}, x1={x1}, y1={y1}, steep={steep}");

            double dx = x1 - x0;
            double dy = y1 - y0;
            double gradient = dy / dx;

            // handle first endpoint
            double xEnd = Math.Round(x0);
            double yEnd = y0 + gradient * (xEnd - x0);
            double xGap = rfpart(x0 + 0.5);
            int xPxl1 = (int)xEnd;
            int yPxl1 = (int)yEnd;

            if(steep)
            {
                spriteBatch.PutPixel(yPxl1, xPxl1, color, (float)(rfpart(yEnd) * xGap));
                spriteBatch.PutPixel(yPxl1 + 1, xPxl1, color, (float)(fpart(yEnd) * xGap));
            }
            else
            {
                spriteBatch.PutPixel(xPxl1, yPxl1, color, (float)(rfpart(yEnd) * xGap));
                spriteBatch.PutPixel(xPxl1, yPxl1 + 1, color, (float)(fpart(yEnd) * xGap));
            }

            double intery = yEnd + gradient;

            // handle second endpoint
            xEnd = Math.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = fpart(x1 + 0.5);
            int xPxl2 = (int)xEnd;
            int yPxl2 = (int)yEnd;

            if (steep)
            {
                spriteBatch.PutPixel(yPxl2, xPxl2, color, (float)(rfpart(yEnd) * xGap));
                spriteBatch.PutPixel(yPxl2 + 1, xPxl2, color, (float)(fpart(yEnd) * xGap));
            }
            else
            {
                spriteBatch.PutPixel(xPxl2, yPxl2, color, (float)(rfpart(yEnd) * xGap));
                spriteBatch.PutPixel(xPxl2, yPxl2 + 1, color, (float)(fpart(yEnd) * xGap));
            }

            // main loop
            for(int x = xPxl1 + 1; x < xPxl2; x++)
            {
                if(steep)
                {
                    spriteBatch.PutPixel((int)intery, x, color, rfpart(intery));
                    spriteBatch.PutPixel((int)intery + 1, x, color, fpart(intery));
                }
                else
                {
                    spriteBatch.PutPixel(x, (int)intery, color, rfpart(intery));
                    spriteBatch.PutPixel(x, (int)intery + 1, color, fpart(intery));
                }
                intery = intery + gradient;
            }


            /*int bWidth = 512;
            int bHeight = 512;

            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            // zmienne pomocnicze
            int d, dx, dy, ai, bi, xi, yi;
            int x = x1, y = y1;
            // ustalenie kierunku rysowania
            if (x1 < x2)
            {
                xi = 1;
                dx = x2 - x1;
            }
            else
            {
                xi = -1;
                dx = x1 - x2;
            }
            // ustalenie kierunku rysowania
            if (y1 < y2)
            {
                yi = 1;
                dy = y2 - y1;
            }
            else
            {
                yi = -1;
                dy = y1 - y2;
            }
            // pierwszy piksel
            if (x < bWidth && x >= 0 && y < bHeight && y >= 0)
                spriteBatch.PutPixel(x, y, color);
            // oś wiodąca OX
            if (dx > dy)
            {
                ai = (dy - dx) * 2;
                bi = dy * 2;
                d = bi - dx;
                // pętla po kolejnych x
                while (x != x2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        x += xi;
                    }
                    if (x < bWidth && x >= 0 && y < bHeight && y >= 0)
                        spriteBatch.PutPixel(x, y, color);
                }
            }
            // oś wiodąca OY
            else
            {
                ai = (dx - dy) * 2;
                bi = dx * 2;
                d = bi - dy;
                // pętla po kolejnych y
                while (y != y2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        y += yi;
                    }
                    if (x < bWidth && x >= 0 && y < bHeight && y >= 0)
                        spriteBatch.PutPixel(x, y, color);
                }
            }*/

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
            /*spriteBatch.Draw(_pixel,
                             new Vector2((float)point.X, (float)point.Y),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);*/
        }

        internal static void PutPixel(this SpriteBatch spriteBatch, int x, int y, Color color, float intense)
        {
            Color newColor = new Color((float)color.R * intense/ 255.0f, (float)color.G * intense / 255.0f, (float)color.B * intense / 255.0f, intense);
            PutPixel(spriteBatch, new Point(x, y), newColor);
            PutPixel(spriteBatch, new Point(x + 1, y), newColor);
            PutPixel(spriteBatch, new Point(x, y + 1), newColor);
            PutPixel(spriteBatch, new Point(x + 1, y + 1), newColor);
        }

        internal static void PutPixel(this SpriteBatch spriteBatch, int x, int y, Color color)
        {
            PutPixel(spriteBatch, new Point(x, y), color);
        }

        /// <summary>
        /// Puts the pixel on the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        /// <param name="position">The position of the pixel.</param>
        /// <param name="color">The color of the pixel.</param>
        internal static void PutPixel(this SpriteBatch spriteBatch, Point position, Color color)
        {
           

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
            {
                spriteBatch.DrawLine(vertices[i], vertices[i + 1], color, thickness);
                //spriteBatch.DrawLine(new Point(vertices[i].X, vertices[i].Y + 1), new Point(vertices[i + 1].X, vertices[i + 1].Y + 1), color, thickness);
                //spriteBatch.DrawLine(new Point(vertices[i].X + 1, vertices[i].Y), new Point(vertices[i + 1].X + 1, vertices[i + 1].Y), color, thickness);
            }

            if (closed)
            {
                spriteBatch.DrawLine(vertices[vertices.Count - 1], vertices[0], color, thickness);
                //spriteBatch.DrawLine(new Point(vertices[vertices.Count - 1].X, vertices[vertices.Count - 1].Y + 1), new Point(vertices[0].X, vertices[0].Y + 1), color, thickness);
            }
        }

        internal static void DrawPolygon(this SpriteBatch spriteBatch, Polygon polygon, Color color, float thickness = 1.0f)
        {
            DrawPath(spriteBatch, polygon.Vertices, true, color, thickness);
        }

        internal static void DrawArrow(this SpriteBatch spriteBatch, Point origin, Point tip, Color color, float thickness = 1.0f, float length = 100.0f)
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

            double axisAngle = GeometryHelper.GetRadAngle(origin, tip);
            Point end = new Point(
                origin.X + Math.Cos(axisAngle) * 100,
                origin.Y + Math.Sin(axisAngle) * 100);

            if (origin.X == tip.X && origin.Y == tip.Y) return;

            // Mirror of arrow
            if (origin.X > tip.X)
            {
                arrowCenter = new Point(arrowCenter.X, arrowCenter.Y);
                arrowTop = new Point(-arrowTop.X, arrowTop.Y);
                arrowBottom = new Point(-arrowBottom.X, arrowBottom.Y);

                end = new Point(
                    origin.X - Math.Cos(axisAngle) * length,
                    origin.Y - Math.Sin(axisAngle) * length);
            }

            // Rotate and translate of arrow
            arrowCenter = new Point(end.X, end.Y);
            arrowTop = new Point(((arrowTop.X * Math.Cos(axisAngle) - arrowTop.Y * Math.Sin(axisAngle)) + end.X),
                ((arrowTop.X * Math.Sin(axisAngle) + arrowTop.Y * Math.Cos(axisAngle)) + end.Y));
            arrowBottom = new Point(((arrowBottom.X * Math.Cos(axisAngle) - arrowBottom.Y * Math.Sin(axisAngle)) + end.X),
                ((arrowBottom.X * Math.Sin(axisAngle) + arrowBottom.Y * Math.Cos(axisAngle)) + end.Y));

            // Draw
            spriteBatch.DrawLine(arrowTop, arrowCenter, color, thickness);
            spriteBatch.DrawLine(arrowBottom, arrowCenter, color, thickness);
            spriteBatch.DrawLine(origin, end, color, thickness);
        }

        internal static void DrawVertex(this SpriteBatch spriteBatch, Point vertex, Color color, float thickness = 1.0f)
        {
            const int circleRadius = 5;
            const int circleSides = 10;

            spriteBatch.DrawCircle(vertex, circleRadius, circleSides, color, thickness);
        }

        private static void Swap(ref double x, ref double y)
        {
            double temp = x;
            x = y;
            y = temp;
        }

        private static int ipart(double val)
        {
            return (int)val;
        }

        private static float fpart(double val)
        {
            if (val < 0)
                return (float)(1 - (val - Math.Floor(val)));
            return (float)(val - Math.Floor(val));
        }

        private static float rfpart(double val)
        {
            return 1 - fpart(val);
        }


        public static void DrawMap(this SpriteBatch spriteBatch, Map map)
        {
            foreach (var obstacle in map.Obstacles)
                DrawPolygon(spriteBatch, obstacle, Color.White);
        }


        public static void DrawVehicle(this SpriteBatch spriteBatch, Vehicle vehicle)
        {
            DrawPolygon(spriteBatch, vehicle.Shape, Color.Aqua);
        }


        public static void DrawFrame(this SpriteBatch spriteBatch, Frame frame, Vehicle vehicle, Map map)
        {
            List<Point> rotatedVehicle = new List<Point>();
            foreach (var point in vehicle.Shape.Vertices)
            {
                Point positionDifference = frame.Position - vehicle.OrientationOrigin;
                rotatedVehicle.Add(GeometryHelper.RotatePoint(point + positionDifference, vehicle.OrientationOrigin + positionDifference, -frame.Rotation));
            }

            /*foreach (var point in vehicle.Shape.Vertices)
            {
                Point positionDifference = order.Destination - vehicle.OrientationOrigin;
                rotatedVehicle.Add(GeometryHelper.RotatePoint(point + positionDifference, vehicle.OrientationOrigin + positionDifference, Math.Abs(order.Rotation)));
            }*/

            DrawMap(spriteBatch, map);
            DrawVehicle(spriteBatch, new Vehicle(new Polygon(rotatedVehicle), vehicle.OrientationOrigin, vehicle.OrientationAngle));
        }
    }
}
