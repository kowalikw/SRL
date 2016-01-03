using System;
using System.Windows;
using Microsoft.Xna.Framework;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using MathHelper = SRL.Commons.Utilities.MathHelper;
using Point = System.Windows.Point;

namespace SRL.Main.Drawing
{
    internal static class StaticDrawHelper
    {
        private static Color BlendColors(Color color1, Color color2)
        {
            float rgbMaxValue = 255.0f;
            float color1Intensity = color1.A / rgbMaxValue;
            float color2Intensity = color2.A / rgbMaxValue;

            float a = color1Intensity + color2Intensity * (1 - color1Intensity);
            float b = color1.B / rgbMaxValue * color1Intensity + color2.B / rgbMaxValue * color2Intensity * (1 - color1Intensity);
            float g = color1.G / rgbMaxValue * color1Intensity + color2.G / rgbMaxValue * color2Intensity * (1 - color1Intensity);
            float r = color1.R / rgbMaxValue * color1Intensity + color2.R / rgbMaxValue * color2Intensity * (1 - color1Intensity);

            return new Color(r, g, b, a);
        }

        private static Color ColorAmendment(Color color, int amendment = 0)
        {
            byte R = (byte)(color.R < byte.MaxValue - amendment ? color.R + amendment : byte.MaxValue);
            byte G = (byte)(color.G < byte.MaxValue - amendment ? color.G + amendment : byte.MaxValue);
            byte B = (byte)(color.B < byte.MaxValue - amendment ? color.B + amendment : byte.MaxValue);
            byte A = color.A;

            return new Color(R, G, B, A);
        }

        private static void SetBigPixel(this LockBitmap bitmap, int x, int y, Color color, float intensity = 1)
        {
            color.A = (byte)(intensity * byte.MaxValue);

            int amendmentAA = 16; // wyznaczone empirycznie

            var pixel1Color = BlendColors(color, bitmap.GetPixel(x, y));
            var pixel2Color = BlendColors(color, bitmap.GetPixel(x + 1, y));
            var pixel3Color = BlendColors(color, bitmap.GetPixel(x, y + 1));
            var pixel4Color = BlendColors(color, bitmap.GetPixel(x + 1, y + 1));

            bitmap.SetPixel(x, y, ColorAmendment(pixel1Color, amendmentAA));
            bitmap.SetPixel(x + 1, y, ColorAmendment(pixel2Color, amendmentAA));
            bitmap.SetPixel(x, y + 1, ColorAmendment(pixel3Color, amendmentAA));
            bitmap.SetPixel(x + 1, y + 1, ColorAmendment(pixel4Color, amendmentAA));
        }

        private static void BresenhamLine(this LockBitmap lockBitmap, int aX, int aY, int bX, int bY, Color color)
        {
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

            int dx = bX - aX;
            int dy = Math.Abs(bY - aY);

            int error = dx / 2;
            int yStep = (aY < bY) ? 1 : -1;
            int y = aY;

            int maxX = bX;

            if (steep)
            {
                for (int x = aX; x < maxX; x++)
                {
                    lockBitmap.SetBigPixel(y, x, color);
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
                for (int x = aX; x < maxX; x++)
                {
                    lockBitmap.SetBigPixel(x, y, color);
                    error -= dy;
                    if (error < 0)
                    {
                        y += yStep;
                        error += dx;
                    }
                }
            }
        }

        private static void WuLine(this LockBitmap lockBitmap, int aX, int aY, int bX, int bY, Color color)
        {
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

            // Calculate and set first point
            float xEnd = aX;
            float yEnd = aY + gradient * (xEnd - aX);
            float xGap = MathHelper.Rfpart(aX + 0.5f);

            float intensityTop = MathHelper.Rfpart(yEnd) * xGap;
            float intensityDown = MathHelper.Fpart(yEnd) * xGap;

            int xPxl1 = (int)xEnd;
            int yPxl1 = (int)yEnd;

            if (steep)
            {
                lockBitmap.SetBigPixel(yPxl1, xPxl1, color, intensityTop);
                lockBitmap.SetBigPixel(yPxl1 + 1, xPxl1, color, intensityDown);
            }
            else
            {
                lockBitmap.SetBigPixel(xPxl1, yPxl1, color, intensityTop);
                lockBitmap.SetBigPixel(xPxl1, yPxl1 + 1, color, intensityDown);
            }

            float intery = yEnd + gradient;

            // Calculate and set last point
            xEnd = bX;
            yEnd = bY + gradient * (xEnd - bX);
            xGap = MathHelper.Fpart(bX + 0.5f);

            intensityTop = MathHelper.Rfpart(yEnd) * xGap;
            intensityDown = MathHelper.Fpart(yEnd) * xGap;

            int xPxl2 = (int)xEnd;
            int yPxl2 = (int)yEnd;

            if (steep)
            {
                lockBitmap.SetBigPixel(yPxl2, xPxl2, color, intensityTop);
                lockBitmap.SetBigPixel(yPxl2 + 1, xPxl2, color, intensityDown);
            }
            else
            {
                lockBitmap.SetBigPixel(xPxl2, yPxl2, color, intensityTop);
                lockBitmap.SetBigPixel(xPxl2, yPxl2 + 1, color, intensityDown);
            }

            // Main loop
            for (int x = xPxl1 + 1; x < xPxl2; x++)
            {
                intensityTop = MathHelper.Rfpart(intery);
                intensityDown = MathHelper.Fpart(intery);

                if (steep)
                {
                    lockBitmap.SetBigPixel((int)intery, x, color, intensityTop);
                    lockBitmap.SetBigPixel((int)intery + 1, x, color, intensityDown);
                }
                else
                {
                    lockBitmap.SetBigPixel(x, (int)intery, color, intensityTop);
                    lockBitmap.SetBigPixel(x, (int)intery + 1, color, intensityDown);
                }
                intery = intery + gradient;
            }
        }

        //-------------------------------------------------------------------------------------------------

            #region Line

        public static void DrawLine(this LockBitmap lockBitmap, Line line, Size renderSize, Color color, bool antialiasing)
        {
            int aX = (int)line.EndpointA.Denormalize(renderSize).X;
            int aY = (int)line.EndpointA.Denormalize(renderSize).Y;
            int bX = (int)line.EndpointB.Denormalize(renderSize).X;
            int bY = (int)line.EndpointB.Denormalize(renderSize).Y;

            if (antialiasing)
                lockBitmap.WuLine(aX, aY, bX, bY, color);
            else
                lockBitmap.BresenhamLine(aX, aY, bX, bY, color);
        }

        #endregion

        #region Path

        public static void DrawPath(this LockBitmap lockBitmap, Path path, Size renderSize, Color color, bool antialiasing)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                lockBitmap.DrawLine(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color, antialiasing);
        }

        #endregion

        #region Polygon

        public static void DrawPolygon(this LockBitmap lockBitmap, Polygon polygon, Size renderSize, Color color, bool antialiasing)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                lockBitmap.DrawLine(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color, antialiasing);
            lockBitmap.DrawLine(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color, antialiasing);
        }

        #endregion

        #region Map

        public static void DrawMap(this LockBitmap lockBitmap, Map map, Size renderSize, Color color, bool antialiasing)
        {
            foreach (var obstacle in map.Obstacles)
                lockBitmap.DrawPolygon(obstacle, renderSize, color, antialiasing);
        }

        #endregion

        #region Vertex

        public static void DrawVertex(this LockBitmap lockBitmap, Point vertex, Size renderSize, Color color, bool antialiasing)
        {
            // Draw 7x7 dot

            int vX = (int)vertex.Denormalize(renderSize).X;
            int vY = (int)vertex.Denormalize(renderSize).Y;

            var xnaColor = new Color(color.R, color.G, color.B);

            if (antialiasing)
            {
                lockBitmap.SetPixel(vX - 2, vY - 3, color * 0.255f);
                lockBitmap.SetPixel(vX - 1, vY - 3, color * 0.863f);
                lockBitmap.SetPixel(vX, vY - 3, color * 1);
                lockBitmap.SetPixel(vX + 1, vY - 3, color * 0.863f);
                lockBitmap.SetPixel(vX + 2, vY - 3, color * 0.255f);
                lockBitmap.SetPixel(vX - 3, vY - 2, color * 0.255f);
                lockBitmap.SetPixel(vX - 2, vY - 2, color * 1);
                lockBitmap.SetPixel(vX - 1, vY - 2, color * 1);
                lockBitmap.SetPixel(vX, vY - 2, color * 1);
                lockBitmap.SetPixel(vX + 1, vY - 2, color * 1);
                lockBitmap.SetPixel(vX + 2, vY - 2, color * 1);
                lockBitmap.SetPixel(vX + 3, vY - 2, color * 0.255f);
                lockBitmap.SetPixel(vX - 3, vY - 1, color * 0.863f);
                lockBitmap.SetPixel(vX - 2, vY - 1, color * 1);
                lockBitmap.SetPixel(vX - 1, vY - 1, color * 1);
                lockBitmap.SetPixel(vX, vY - 1, color * 1);
                lockBitmap.SetPixel(vX + 1, vY - 1, color * 1);
                lockBitmap.SetPixel(vX + 2, vY - 1, color * 1);
                lockBitmap.SetPixel(vX + 3, vY - 1, color * 0.863f);
                lockBitmap.SetPixel(vX - 3, vY, color * 1);
                lockBitmap.SetPixel(vX - 2, vY, color * 1);
                lockBitmap.SetPixel(vX - 1, vY, color * 1);
                lockBitmap.SetPixel(vX, vY, color * 1);
                lockBitmap.SetPixel(vX + 1, vY, color * 1);
                lockBitmap.SetPixel(vX + 2, vY, color * 1);
                lockBitmap.SetPixel(vX + 3, vY, color * 1);
                lockBitmap.SetPixel(vX - 3, vY + 1, color * 0.863f);
                lockBitmap.SetPixel(vX - 2, vY + 1, color * 1);
                lockBitmap.SetPixel(vX - 1, vY + 1, color * 1);
                lockBitmap.SetPixel(vX, vY + 1, color * 1);
                lockBitmap.SetPixel(vX + 1, vY + 1, color * 1);
                lockBitmap.SetPixel(vX + 2, vY + 1, color * 1);
                lockBitmap.SetPixel(vX + 3, vY + 1, color * 0.863f);
                lockBitmap.SetPixel(vX - 3, vY + 2, color * 0.255f);
                lockBitmap.SetPixel(vX - 2, vY + 2, color * 1);
                lockBitmap.SetPixel(vX - 1, vY + 2, color * 1);
                lockBitmap.SetPixel(vX, vY + 2, color * 1);
                lockBitmap.SetPixel(vX + 1, vY + 2, color * 1);
                lockBitmap.SetPixel(vX + 2, vY + 2, color * 1);
                lockBitmap.SetPixel(vX + 3, vY + 2, color * 0.255f);
                lockBitmap.SetPixel(vX - 2, vY + 3, color * 0.255f);
                lockBitmap.SetPixel(vX - 1, vY + 3, color * 0.863f);
                lockBitmap.SetPixel(vX, vY + 3, color * 1);
                lockBitmap.SetPixel(vX + 1, vY + 3, color * 0.863f);
                lockBitmap.SetPixel(vX + 2, vY + 3, color * 0.255f);
            }
            else
            {
                lockBitmap.SetPixel(vX - 1, vY - 3, xnaColor);
                lockBitmap.SetPixel(vX, vY - 3, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY - 3, xnaColor);
                lockBitmap.SetPixel(vX - 2, vY - 2, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY - 2, xnaColor);
                lockBitmap.SetPixel(vX, vY - 2, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY - 2, xnaColor);
                lockBitmap.SetPixel(vX + 2, vY - 2, xnaColor);
                lockBitmap.SetPixel(vX - 3, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX - 2, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX + 2, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX + 3, vY - 1, xnaColor);
                lockBitmap.SetPixel(vX - 3, vY, xnaColor);
                lockBitmap.SetPixel(vX - 2, vY, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY, xnaColor);
                lockBitmap.SetPixel(vX, vY, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY, xnaColor);
                lockBitmap.SetPixel(vX + 2, vY, xnaColor);
                lockBitmap.SetPixel(vX + 3, vY, xnaColor);
                lockBitmap.SetPixel(vX - 3, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX - 2, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX + 2, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX + 3, vY + 1, xnaColor);
                lockBitmap.SetPixel(vX - 2, vY + 2, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY + 2, xnaColor);
                lockBitmap.SetPixel(vX, vY + 2, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY + 2, xnaColor);
                lockBitmap.SetPixel(vX + 2, vY + 2, xnaColor);
                lockBitmap.SetPixel(vX - 1, vY + 3, xnaColor);
                lockBitmap.SetPixel(vX, vY + 3, xnaColor);
                lockBitmap.SetPixel(vX + 1, vY + 3, xnaColor);
            }
        }

        #endregion
    }
}
