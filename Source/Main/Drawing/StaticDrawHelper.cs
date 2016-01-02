using System;
using System.Windows;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;

namespace SRL.Main.Drawing
{
    internal static class StaticDrawHelper
    {
        private static RgbColor BlendColors(RgbColor color1, RgbColor color2)
        {
            float rgbMaxValue = 255.0f;
            float color1Intensity = color1.A / rgbMaxValue;

            var A = (color1Intensity + color2.A / rgbMaxValue * (1 - color1Intensity));
            var B = (byte)((((color1.B / rgbMaxValue * color1Intensity) + (color2.B / rgbMaxValue * (color2.A / rgbMaxValue) * (1 - color1Intensity)))) * byte.MaxValue);
            var G = (byte)((((color1.G / rgbMaxValue * color1Intensity) + (color2.G / rgbMaxValue * (color2.A / rgbMaxValue) * (1 - color1Intensity)))) * byte.MaxValue);
            var R = (byte)((((color1.R / rgbMaxValue * color1Intensity) + (color2.R / rgbMaxValue * (color2.A / rgbMaxValue) * (1 - color1Intensity)))) * byte.MaxValue);

            return new RgbColor(R, G, B, (byte)(A * byte.MaxValue));
        }

        private static RgbColor ColorAmendment(RgbColor color, int amendment = 0)
        {
            byte R = (byte)(color.R < byte.MaxValue - amendment ? color.R + amendment : byte.MaxValue);
            byte G = (byte)(color.G < byte.MaxValue - amendment ? color.G + amendment : byte.MaxValue);
            byte B = (byte)(color.B < byte.MaxValue - amendment ? color.B + amendment : byte.MaxValue);
            byte A = color.A;

            return new RgbColor(R, G, B, A);
        }

        private static void SetBigPixel(this LockBitmap bitmap, int x, int y, RgbColor color, float intensity = 1)
        {
            color.A = (byte)(intensity * byte.MaxValue);

            int amendmentAA = 16; // wyznaczone empirycznie

            // TODO: Check it.
            //while (x > (int)RenderSize.Width - 2) x--;
            //while (y > (int)RenderSize.Height - 2) y--;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            var pixel1Color = BlendColors(color, new RgbColor(bitmap.GetPixel(x, y)));
            var pixel2Color = BlendColors(color, new RgbColor(bitmap.GetPixel(x + 1, y)));
            var pixel3Color = BlendColors(color, new RgbColor(bitmap.GetPixel(x, y + 1)));
            var pixel4Color = BlendColors(color, new RgbColor(bitmap.GetPixel(x + 1, y + 1)));

            bitmap.SetPixel(x, y, ColorAmendment(pixel1Color, amendmentAA).ToWinColor());
            bitmap.SetPixel(x + 1, y, ColorAmendment(pixel2Color, amendmentAA).ToWinColor());
            bitmap.SetPixel(x, y + 1, ColorAmendment(pixel3Color, amendmentAA).ToWinColor());
            bitmap.SetPixel(x + 1, y + 1, ColorAmendment(pixel4Color, amendmentAA).ToWinColor());
        }

        #region Line

        public static void DrawLine(this LockBitmap lockBitmap, Line line, Size renderSize, RgbColor color)
        {
            int aX = (int)line.EndpointA.Denormalize(renderSize).X;
            int aY = (int)line.EndpointA.Denormalize(renderSize).Y;
            int bX = (int)line.EndpointB.Denormalize(renderSize).X;
            int bY = (int)line.EndpointB.Denormalize(renderSize).Y;

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

        public static void DrawLineAA(this LockBitmap lockBitmap, Line line, Size renderSize, RgbColor color)
        {
            int aX = (int)line.EndpointA.Denormalize(renderSize).X;
            int aY = (int)line.EndpointA.Denormalize(renderSize).Y;
            int bX = (int)line.EndpointB.Denormalize(renderSize).X;
            int bY = (int)line.EndpointB.Denormalize(renderSize).Y;

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

            // calculate and set last point
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

            // main loop
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

        #endregion

        #region Path

        public static void DrawPath(this LockBitmap lockBitmap, Path path, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                lockBitmap.DrawLine(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color);
        }

        public static void DrawPathAA(this LockBitmap lockBitmap, Path path, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < path.Vertices.Count; i++)
                lockBitmap.DrawLineAA(new Line(path.Vertices[i - 1], path.Vertices[i]), renderSize, color);
        }

        #endregion

        #region Polygon

        public static void DrawPolygon(this LockBitmap lockBitmap, Polygon polygon, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                lockBitmap.DrawLine(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color);
            lockBitmap.DrawLine(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color);
        }

        public static void DrawPolygonAA(this LockBitmap lockBitmap, Polygon polygon, Size renderSize, RgbColor color)
        {
            for (int i = 1; i < polygon.Vertices.Count; i++)
                lockBitmap.DrawLineAA(new Line(polygon.Vertices[i - 1], polygon.Vertices[i]), renderSize, color);
            lockBitmap.DrawLineAA(new Line(polygon.Vertices[polygon.Vertices.Count - 1], polygon.Vertices[0]), renderSize, color);
        }

        #endregion

        #region Map

        public static void DrawMap(this LockBitmap lockBitmap, Map map, Size renderSize, RgbColor color)
        {
            foreach (var obstacle in map.Obstacles)
                lockBitmap.DrawPolygon(obstacle, renderSize, color);
        }

        public static void DrawMapAA(this LockBitmap lockBitmap, Map map, Size renderSize, RgbColor color)
        {
            foreach (var obstacle in map.Obstacles)
                lockBitmap.DrawPolygonAA(obstacle, renderSize, color);
        }

        #endregion

        #region Vertex

        public static void DrawVertex(this LockBitmap lockBitmap, Point vertex, Size renderSize, RgbColor color)
        {
            // Draw 7x7 dot

            int vX = (int)vertex.Denormalize(renderSize).X;
            int vY = (int)vertex.Denormalize(renderSize).Y;

            var winColor = color.ToWinColor();

            lockBitmap.SetPixel(vX - 1, vY - 3, winColor);
            lockBitmap.SetPixel(vX, vY - 3, winColor);
            lockBitmap.SetPixel(vX + 1, vY - 3, winColor);
            lockBitmap.SetPixel(vX - 2, vY - 2, winColor);
            lockBitmap.SetPixel(vX - 1, vY - 2, winColor);
            lockBitmap.SetPixel(vX, vY - 2, winColor);
            lockBitmap.SetPixel(vX + 1, vY - 2, winColor);
            lockBitmap.SetPixel(vX + 2, vY - 2, winColor);
            lockBitmap.SetPixel(vX - 3, vY - 1, winColor);
            lockBitmap.SetPixel(vX - 2, vY - 1, winColor);
            lockBitmap.SetPixel(vX - 1, vY - 1, winColor);
            lockBitmap.SetPixel(vX, vY - 1, winColor);
            lockBitmap.SetPixel(vX + 1, vY - 1, winColor);
            lockBitmap.SetPixel(vX + 2, vY - 1, winColor);
            lockBitmap.SetPixel(vX + 3, vY - 1, winColor);
            lockBitmap.SetPixel(vX - 3, vY, winColor);
            lockBitmap.SetPixel(vX - 2, vY, winColor);
            lockBitmap.SetPixel(vX - 1, vY, winColor);
            lockBitmap.SetPixel(vX, vY, winColor);
            lockBitmap.SetPixel(vX + 1, vY, winColor);
            lockBitmap.SetPixel(vX + 2, vY, winColor);
            lockBitmap.SetPixel(vX + 3, vY, winColor);
            lockBitmap.SetPixel(vX - 3, vY + 1, winColor);
            lockBitmap.SetPixel(vX - 2, vY + 1, winColor);
            lockBitmap.SetPixel(vX - 1, vY + 1, winColor);
            lockBitmap.SetPixel(vX, vY + 1, winColor);
            lockBitmap.SetPixel(vX + 1, vY + 1, winColor);
            lockBitmap.SetPixel(vX + 2, vY + 1, winColor);
            lockBitmap.SetPixel(vX + 3, vY + 1, winColor);
            lockBitmap.SetPixel(vX - 2, vY + 2, winColor);
            lockBitmap.SetPixel(vX - 1, vY + 2, winColor);
            lockBitmap.SetPixel(vX, vY + 2, winColor);
            lockBitmap.SetPixel(vX + 1, vY + 2, winColor);
            lockBitmap.SetPixel(vX + 2, vY + 2, winColor);
            lockBitmap.SetPixel(vX - 1, vY + 3, winColor);
            lockBitmap.SetPixel(vX, vY + 3, winColor);
            lockBitmap.SetPixel(vX + 1, vY + 3, winColor);
        }

        public static void DrawVertexAA(this LockBitmap spriteBatch, Point vertex, Size renderSize, RgbColor color)
        {
            // Draw 7x7 dot

            int vX = (int)vertex.Denormalize(renderSize).X;
            int vY = (int)vertex.Denormalize(renderSize).Y;

            //TODO
        }

        #endregion
    }
}
