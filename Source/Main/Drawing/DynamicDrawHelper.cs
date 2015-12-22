using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Main.Utilities;
using MathHelper = SRL.Commons.Utilities.MathHelper;

namespace SRL.Main.Drawing
{
    internal static class DynamicDrawHelper
    {
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
        private static void SetPixel(this SpriteBatch spriteBatch, float x, float y, RgbColor color, byte intensity = byte.MaxValue)
        {
            spriteBatch.Draw(_pixel, new Vector2(x, y), color.ToXnaColor(intensity));
        }

        //-------------------------------------------------------------------------------------------------

        #region Line

        public static void DrawLine(this SpriteBatch spriteBatch, Line line, Size renderSize, RgbColor color)
        {
            float aX = (float)line.EndpointA.Denormalize(renderSize).X;
            float aY = (float)line.EndpointA.Denormalize(renderSize).Y;
            float bX = (float)line.EndpointB.Denormalize(renderSize).X;
            float bY = (float)line.EndpointB.Denormalize(renderSize).Y;

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
                    spriteBatch.SetPixel(y, x, color);
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
                    spriteBatch.SetPixel(x, y, color);
                    error -= dy;
                    if (error < 0)
                    {
                        y += yStep;
                        error += dx;
                    }
                }
            }
        }


        public static void DrawLineAA(this SpriteBatch spriteBatch, Line line, Size renderSize, RgbColor color)
        {
            float aX = (float)line.EndpointA.Denormalize(renderSize).X;
            float aY = (float)line.EndpointA.Denormalize(renderSize).Y;
            float bX = (float)line.EndpointB.Denormalize(renderSize).X;
            float bY = (float)line.EndpointB.Denormalize(renderSize).Y;

            throw new NotImplementedException(); //TODO
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
    }
}
