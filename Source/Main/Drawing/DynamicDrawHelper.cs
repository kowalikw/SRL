using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using MathHelper = SRL.Commons.Utilities.MathHelper;
using Point = System.Windows.Point;

namespace SRL.Main.Drawing
{
    internal static class DynamicDrawHelper
    {
        private const float ArrowTipLength = 15;
        private const float ArrowTipHalfWidth = 10;

        private static Texture2D _pixel;

        /// <summary>
        /// Begins drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        public static void BeginDraw(this SpriteBatch spriteBatch)
        {
            _pixel = null;
            spriteBatch.Begin();
        }

        /// <summary>
        /// Ends drawing in SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        public static void EndDraw(this SpriteBatch spriteBatch)
        {
            _pixel = null;
            spriteBatch.End();
        }

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
        /// Puts the pixel on the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface.</param>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <param name="color">The color of the pixel.</param>
        /// <param name="intensity">The intensity of the color.</param>
        private static void SetPixel(this SpriteBatch spriteBatch, float x, float y, Color color)
        {
            spriteBatch.Draw(_pixel, new Vector2(x, y), color);
        }

        private static void SetBigPixel(this SpriteBatch spriteBatch, float x, float y, Color color)
        {
            spriteBatch.SetPixel(x, y, color);
            spriteBatch.SetPixel(x + 1, y, color);
            spriteBatch.SetPixel(x, y + 1, color);
            spriteBatch.SetPixel(x + 1, y + 1, color);
        }

        private static void BresenhamLine(this SpriteBatch spriteBatch, float aX, float aY, float bX, float bY, Color color)
        {
            if (_pixel == null)
                CreateThePixel(spriteBatch);

            bool steep = Math.Abs(bY - aY) > Math.Abs(bX - aX);

            if (steep)
            {
                MathHelper.Swap(ref aX, ref aY);
                MathHelper.Swap(ref bX, ref bY);
            }

            if (aX > bX)
            {
                MathHelper.Swap(ref aX, ref bX);
                MathHelper.Swap(ref aY, ref bY);
            }

            float dx = bX - aX;
            float dy = Math.Abs(bY - aY);

            float error = dx / 2.0f;
            float yStep = (aY < bY) ? 1 : -1;
            float y = aY;

            float maxX = bX;

            if (steep)
            {
                for (float x = aX; x < maxX; x += 1)
                {
                    spriteBatch.SetBigPixel(y, x, color);
                    error -= dy;
                    if (error < 0)
                    {
                        y += yStep;
                        error += dx;
                    }
                }
            }
            else
            {
                for (float x = aX; x < maxX; x += 1)
                {
                    spriteBatch.SetBigPixel(x, y, color);
                    error -= dy;
                    if (error < 0)
                    {
                        y += yStep;
                        error += dx;
                    }
                }
            }
        }

        private static void WuLine(this SpriteBatch spriteBatch, float aX, float aY, float bX, float bY, Color color)
        {
            if (_pixel == null)
                CreateThePixel(spriteBatch);

            bool steep = Math.Abs(bY - aY) > Math.Abs(bX - aX);

            if (steep)
            {
                MathHelper.Swap(ref aX, ref aY);
                MathHelper.Swap(ref bX, ref bY);
            }

            if (aX > bX)
            {
                MathHelper.Swap(ref aX, ref bX);
                MathHelper.Swap(ref aY, ref bY);
            }
            
            float gradient = (bY - aY) / (bX - aX);
            
            float xStart = (float)Math.Round(aX);
            float xEnd = (float)Math.Round(bX);

            float x = xStart;
            float y = aY + gradient * (xStart - aX);

            if (steep)
            {
                for (; x <= xEnd; x++)
                {
                    float intensityTop = MathHelper.Rfpart(y);
                    float intensityDown = MathHelper.Fpart(y);

                    spriteBatch.SetBigPixel((int)y, x, color * intensityTop);
                    spriteBatch.SetBigPixel((int)y + 1, x, color * intensityDown);

                    y = y + gradient;
                }
            }
            else
            {
                for (; x <= xEnd; x++)
                {
                    float intensityTop = MathHelper.Rfpart(y);
                    float intensityDown = MathHelper.Fpart(y);

                    spriteBatch.SetBigPixel(x, (int)y, color * intensityTop);
                    spriteBatch.SetBigPixel(x, (int)y + 1, color * intensityDown);

                    y = y + gradient;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------

        #region Line

        public static void DrawLine(this SpriteBatch spriteBatch, Line line, Size renderSize, Color color, bool antialiasing)
        {
            float aX = (float)line.EndpointA.Denormalize(renderSize).X;
            float aY = (float)line.EndpointA.Denormalize(renderSize).Y;
            float bX = (float)line.EndpointB.Denormalize(renderSize).X;
            float bY = (float)line.EndpointB.Denormalize(renderSize).Y;

            if (antialiasing)
                spriteBatch.WuLine(aX, aY, bX, bY, color);
            else
                spriteBatch.BresenhamLine(aX, aY, bX, bY, color);
        }

        #endregion
        
        #region Arrow

        public static void DrawArrow(this SpriteBatch spriteBatch, Point origin, double length, double angle, Size renderSize, Color color, bool antialiasing)
        {
            double cosA = Math.Cos(angle);
            double sinA = Math.Sin(angle);

            length *= Math.Sqrt(renderSize.Height * renderSize.Width);

            Point o = origin.Denormalize(renderSize);
            Point t = new Point(
                o.X + length * cosA,
                o.Y - length * sinA);

            Point ah1 = new Point(
                t.X - ArrowTipLength * cosA - ArrowTipHalfWidth * sinA,
                t.Y + ArrowTipLength * sinA - ArrowTipHalfWidth * cosA);
            Point ah2 = new Point(
                t.X - ArrowTipLength * cosA + ArrowTipHalfWidth * sinA,
                t.Y + ArrowTipLength * sinA + ArrowTipHalfWidth * cosA);

            if (antialiasing)
            {
                spriteBatch.WuLine((float)t.X, (float)t.Y, (float)o.X, (float)o.Y, color);
                spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
                spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
            }
            else
            {
                spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)o.X, (float)o.Y, color);
                spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
                spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
            }
        }

        public static void DrawArrow(this SpriteBatch spriteBatch, Point origin, Point tip, Size renderSize, Color color, bool antialiasing)
        {
            double angle = GeometryHelper.GetAngle(origin, tip);
            double cosA = Math.Cos(angle);
            double sinA = Math.Sin(angle);

            Point o = origin.Denormalize(renderSize);
            Point t = tip.Denormalize(renderSize);

            Point ah1 = new Point(
                t.X - ArrowTipLength * cosA - ArrowTipHalfWidth * sinA,
                t.Y + ArrowTipLength * sinA - ArrowTipHalfWidth * cosA);
            Point ah2 = new Point(
                t.X - ArrowTipLength * cosA + ArrowTipHalfWidth * sinA,
                t.Y + ArrowTipLength * sinA + ArrowTipHalfWidth * cosA);

            if (antialiasing)
            {
                spriteBatch.WuLine((float)o.X, (float)o.Y, (float)t.X, (float)t.Y, color);
                spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
                spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
            }
            else
            {
                spriteBatch.BresenhamLine((float) o.X, (float) o.Y, (float) t.X, (float) t.Y, color);
                spriteBatch.BresenhamLine((float) t.X, (float) t.Y, (float) ah1.X, (float) ah1.Y, color);
                spriteBatch.BresenhamLine((float) t.X, (float) t.Y, (float) ah2.X, (float) ah2.Y, color);
            }
        }

        #endregion

        #region Path

        public static void DrawPath(this SpriteBatch spriteBatch, Path path, Size renderSize, Color color, bool antialiasing)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                spriteBatch.DrawLine(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color, antialiasing);
        }

        #endregion

        #region Polygon

        public static void DrawPolygon(this SpriteBatch spriteBatch, Polygon polygon, Size renderSize, Color color, bool antialiasing)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                spriteBatch.DrawLine(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color, antialiasing);
            spriteBatch.DrawLine(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color, antialiasing);
        }

        #endregion

        #region Vertex

        public static void DrawVertex(this SpriteBatch spriteBatch, Point vertex, Size renderSize, Color color, bool antialiasing)
        {
            // Draw 7x7 dot

            float vX = (float) vertex.Denormalize(renderSize).X;
            float vY = (float) vertex.Denormalize(renderSize).Y;
            
            if (_pixel == null)
                CreateThePixel(spriteBatch);

            if (antialiasing)
            {
                spriteBatch.SetPixel(vX - 2, vY - 3, color * 0.255f);
                spriteBatch.SetPixel(vX - 1, vY - 3, color * 0.863f);
                spriteBatch.SetPixel(vX, vY - 3, color * 1);
                spriteBatch.SetPixel(vX + 1, vY - 3, color * 0.863f);
                spriteBatch.SetPixel(vX + 2, vY - 3, color * 0.255f);
                spriteBatch.SetPixel(vX - 3, vY - 2, color * 0.255f);
                spriteBatch.SetPixel(vX - 2, vY - 2, color * 1);
                spriteBatch.SetPixel(vX - 1, vY - 2, color * 1);
                spriteBatch.SetPixel(vX, vY - 2, color * 1);
                spriteBatch.SetPixel(vX + 1, vY - 2, color * 1);
                spriteBatch.SetPixel(vX + 2, vY - 2, color * 1);
                spriteBatch.SetPixel(vX + 3, vY - 2, color * 0.255f);
                spriteBatch.SetPixel(vX - 3, vY - 1, color * 0.863f);
                spriteBatch.SetPixel(vX - 2, vY - 1, color * 1);
                spriteBatch.SetPixel(vX - 1, vY - 1, color * 1);
                spriteBatch.SetPixel(vX, vY - 1, color * 1);
                spriteBatch.SetPixel(vX + 1, vY - 1, color * 1);
                spriteBatch.SetPixel(vX + 2, vY - 1, color * 1);
                spriteBatch.SetPixel(vX + 3, vY - 1, color * 0.863f);
                spriteBatch.SetPixel(vX - 3, vY, color * 1);
                spriteBatch.SetPixel(vX - 2, vY, color * 1);
                spriteBatch.SetPixel(vX - 1, vY, color * 1);
                spriteBatch.SetPixel(vX, vY, color * 1);
                spriteBatch.SetPixel(vX + 1, vY, color * 1);
                spriteBatch.SetPixel(vX + 2, vY, color * 1);
                spriteBatch.SetPixel(vX + 3, vY, color * 1);
                spriteBatch.SetPixel(vX - 3, vY + 1, color * 0.863f);
                spriteBatch.SetPixel(vX - 2, vY + 1, color * 1);
                spriteBatch.SetPixel(vX - 1, vY + 1, color * 1);
                spriteBatch.SetPixel(vX, vY + 1, color * 1);
                spriteBatch.SetPixel(vX + 1, vY + 1, color * 1);
                spriteBatch.SetPixel(vX + 2, vY + 1, color * 1);
                spriteBatch.SetPixel(vX + 3, vY + 1, color * 0.863f);
                spriteBatch.SetPixel(vX - 3, vY + 2, color * 0.255f);
                spriteBatch.SetPixel(vX - 2, vY + 2, color * 1);
                spriteBatch.SetPixel(vX - 1, vY + 2, color * 1);
                spriteBatch.SetPixel(vX, vY + 2, color * 1);
                spriteBatch.SetPixel(vX + 1, vY + 2, color * 1);
                spriteBatch.SetPixel(vX + 2, vY + 2, color * 1);
                spriteBatch.SetPixel(vX + 3, vY + 2, color * 0.255f);
                spriteBatch.SetPixel(vX - 2, vY + 3, color * 0.255f);
                spriteBatch.SetPixel(vX - 1, vY + 3, color * 0.863f);
                spriteBatch.SetPixel(vX, vY + 3, color * 1);
                spriteBatch.SetPixel(vX + 1, vY + 3, color * 0.863f);
                spriteBatch.SetPixel(vX + 2, vY + 3, color * 0.255f);
            }
            else
            {
                spriteBatch.SetPixel(vX - 1, vY - 3, color);
                spriteBatch.SetPixel(vX, vY - 3, color);
                spriteBatch.SetPixel(vX + 1, vY - 3, color);
                spriteBatch.SetPixel(vX - 2, vY - 2, color);
                spriteBatch.SetPixel(vX - 1, vY - 2, color);
                spriteBatch.SetPixel(vX, vY - 2, color);
                spriteBatch.SetPixel(vX + 1, vY - 2, color);
                spriteBatch.SetPixel(vX + 2, vY - 2, color);
                spriteBatch.SetPixel(vX - 3, vY - 1, color);
                spriteBatch.SetPixel(vX - 2, vY - 1, color);
                spriteBatch.SetPixel(vX - 1, vY - 1, color);
                spriteBatch.SetPixel(vX, vY - 1, color);
                spriteBatch.SetPixel(vX + 1, vY - 1, color);
                spriteBatch.SetPixel(vX + 2, vY - 1, color);
                spriteBatch.SetPixel(vX + 3, vY - 1, color);
                spriteBatch.SetPixel(vX - 3, vY, color);
                spriteBatch.SetPixel(vX - 2, vY, color);
                spriteBatch.SetPixel(vX - 1, vY, color);
                spriteBatch.SetPixel(vX, vY, color);
                spriteBatch.SetPixel(vX + 1, vY, color);
                spriteBatch.SetPixel(vX + 2, vY, color);
                spriteBatch.SetPixel(vX + 3, vY, color);
                spriteBatch.SetPixel(vX - 3, vY + 1, color);
                spriteBatch.SetPixel(vX - 2, vY + 1, color);
                spriteBatch.SetPixel(vX - 1, vY + 1, color);
                spriteBatch.SetPixel(vX, vY + 1, color);
                spriteBatch.SetPixel(vX + 1, vY + 1, color);
                spriteBatch.SetPixel(vX + 2, vY + 1, color);
                spriteBatch.SetPixel(vX + 3, vY + 1, color);
                spriteBatch.SetPixel(vX - 2, vY + 2, color);
                spriteBatch.SetPixel(vX - 1, vY + 2, color);
                spriteBatch.SetPixel(vX, vY + 2, color);
                spriteBatch.SetPixel(vX + 1, vY + 2, color);
                spriteBatch.SetPixel(vX + 2, vY + 2, color);
                spriteBatch.SetPixel(vX - 1, vY + 3, color);
                spriteBatch.SetPixel(vX, vY + 3, color);
                spriteBatch.SetPixel(vX + 1, vY + 3, color);
            }
        }

        #endregion
    }
}
