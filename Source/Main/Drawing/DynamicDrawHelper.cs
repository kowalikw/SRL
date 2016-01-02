using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static void SetPixel(this SpriteBatch spriteBatch, float x, float y, RgbColor color, float intensity = 1)
        {
            spriteBatch.Draw(_pixel, new Vector2(x, y), color.ToXnaColor(intensity));
        }

        private static void SetBigPixel(this SpriteBatch spriteBatch, float x, float y, RgbColor color, float intensity = 1)
        {
            spriteBatch.SetPixel(x, y, color, intensity);
            spriteBatch.SetPixel(x + 1, y, color, intensity);
            spriteBatch.SetPixel(x, y + 1, color, intensity);
            spriteBatch.SetPixel(x + 1, y + 1, color, intensity);
        }

        private static void BresenhamLine(this SpriteBatch spriteBatch, float aX, float aY, float bX, float bY, RgbColor color)
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

        private static void WuLine(this SpriteBatch spriteBatch, float aX, float aY, float bX, float bY, RgbColor color)
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
            float dy = bY - aY;
            float gradient = dy / dx;

            // calculate and set first point
            float xEnd = (float)Math.Round(aX);
            float yEnd = aY + gradient * (xEnd - aX);
            float xGap = MathHelper.Rfpart(aX + 0.5f);
            
            float intensityTop = MathHelper.Rfpart(yEnd) * xGap;
            float intensityDown = MathHelper.Fpart(yEnd) * xGap;

            int xPxl1 = (int)xEnd;
            int yPxl1 = (int)yEnd;

            if (steep)
            {
                spriteBatch.SetBigPixel(yPxl1, xPxl1, color, intensityTop);
                spriteBatch.SetBigPixel(yPxl1 + 1, xPxl1, color, intensityDown);
            }
            else
            {
                spriteBatch.SetBigPixel(xPxl1, yPxl1, color, intensityTop);
                spriteBatch.SetBigPixel(xPxl1, yPxl1 + 1, color, intensityDown);
            }

            float intery = yEnd + gradient;

            // calculate and set last point
            xEnd = (float)Math.Round(bX);
            yEnd = bY + gradient * (xEnd - bX);
            xGap = MathHelper.Fpart(bX + 0.5f);
            
            intensityTop = MathHelper.Rfpart(yEnd) * xGap;
            intensityDown = MathHelper.Fpart(yEnd) * xGap;

            int xPxl2 = (int)xEnd;
            int yPxl2 = (int)yEnd;

            if (steep)
            {
                spriteBatch.SetBigPixel(yPxl2, xPxl2, color, intensityTop);
                spriteBatch.SetBigPixel(yPxl2 + 1, xPxl2, color, intensityDown);
            }
            else
            {
                spriteBatch.SetBigPixel(xPxl2, yPxl2, color, intensityTop);
                spriteBatch.SetBigPixel(xPxl2, yPxl2 + 1, color, intensityDown);
            }

            // main loop
            for (int x = xPxl1 + 1; x < xPxl2; x++)
            {
                intensityTop = MathHelper.Rfpart(intery);
                intensityDown = MathHelper.Fpart(intery);

                if (steep)
                {
                    spriteBatch.SetBigPixel((int)intery, x, color, intensityTop);
                    spriteBatch.SetBigPixel((int)intery + 1, x, color, intensityDown);
                }
                else
                {
                    spriteBatch.SetBigPixel(x, (int)intery, color, intensityTop);
                    spriteBatch.SetBigPixel(x, (int)intery + 1, color, intensityDown);
                }
                intery = intery + gradient;
            }
        }

        //-------------------------------------------------------------------------------------------------

        #region Line

        public static void DrawLine(this SpriteBatch spriteBatch, Line line, Size renderSize, RgbColor color)
        {
            float aX = (float)line.EndpointA.Denormalize(renderSize).X;
            float aY = (float)line.EndpointA.Denormalize(renderSize).Y;
            float bX = (float)line.EndpointB.Denormalize(renderSize).X;
            float bY = (float)line.EndpointB.Denormalize(renderSize).Y;

            spriteBatch.BresenhamLine(aX, aY, bX, bY, color);
        }

        public static void DrawLineAA(this SpriteBatch spriteBatch, Line line, Size renderSize, RgbColor color)
        {
            float aX = (float)line.EndpointA.Denormalize(renderSize).X;
            float aY = (float)line.EndpointA.Denormalize(renderSize).Y;
            float bX = (float)line.EndpointB.Denormalize(renderSize).X;
            float bY = (float)line.EndpointB.Denormalize(renderSize).Y;

            spriteBatch.WuLine(aX, aY, bX, bY, color);
        }

        #endregion
        
        #region Arrow

        public static void DrawArrow(this SpriteBatch spriteBatch, Point origin, double length, double angle, Size renderSize, RgbColor color)
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

            spriteBatch.BresenhamLine((float)o.X, (float)o.Y, (float)t.X, (float)t.Y, color);
            spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
            spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
        }

        public static void DrawArrowAA(this SpriteBatch spriteBatch, Point origin, double length, double angle, Size renderSize, RgbColor color)
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

            spriteBatch.WuLine((float)o.X, (float)o.Y, (float)t.X, (float)t.Y, color);
            spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
            spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
        }

        public static void DrawArrow(this SpriteBatch spriteBatch, Point origin, Point tip, Size renderSize, RgbColor color)
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

            spriteBatch.BresenhamLine((float)o.X, (float)o.Y, (float)t.X, (float)t.Y, color);
            spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
            spriteBatch.BresenhamLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
        }

        public static void DrawArrowAA(this SpriteBatch spriteBatch, Point origin, Point tip, Size renderSize, RgbColor color)
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

            spriteBatch.WuLine((float)o.X, (float)o.Y, (float)t.X, (float)t.Y, color);
            spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah1.X, (float)ah1.Y, color);
            spriteBatch.WuLine((float)t.X, (float)t.Y, (float)ah2.X, (float)ah2.Y, color);
        }

        #endregion

        #region Path

        public static void DrawPath(this SpriteBatch spriteBatch, Path path, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                spriteBatch.DrawLine(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color);
        }

        public static void DrawPathAA(this SpriteBatch spriteBatch, Path path, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                spriteBatch.DrawLineAA(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color);
        }

        #endregion

        #region Polygon

        public static void DrawPolygon(this SpriteBatch spriteBatch, Polygon polygon, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                spriteBatch.DrawLine(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color);
            spriteBatch.DrawLine(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color);
        }

        public static void DrawPolygonAA(this SpriteBatch spriteBatch, Polygon polygon, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                spriteBatch.DrawLineAA(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color);
            spriteBatch.DrawLineAA(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color);
        }

        #endregion

        #region Vertex

        public static void DrawVertex(this SpriteBatch spriteBatch, Point vertex, Size renderSize, RgbColor color)
        {
            // Draw 7x7 dot

            float vX = (float) vertex.Denormalize(renderSize).X;
            float vY = (float) vertex.Denormalize(renderSize).Y;
            
            if (_pixel == null)
                CreateThePixel(spriteBatch);

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

        public static void DrawVertexAA(this SpriteBatch spriteBatch, Point vertex, Size renderSize, RgbColor color)
        {
            // Draw 7x7 dot

            float vX = (float)vertex.Denormalize(renderSize).X;
            float vY = (float)vertex.Denormalize(renderSize).Y;

            if (_pixel == null)
                CreateThePixel(spriteBatch);


            //TODO
        }

        #endregion
    }
}
