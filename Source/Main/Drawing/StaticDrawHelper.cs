using System;
using System.Windows;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;

namespace SRL.Main.Drawing
{
    internal static class StaticDrawHelper
    {
        private static void SetBigPixel(this LockBitmap bitmap, int x, int y, RgbColor color, double intensity)
        {
            //color = new Color(color.B, color.G, color.R, color.A);

            var wartosc = 24; // TODO: Wyznaczyć empirycznie.

            //while (x > (int)RenderSize.Width - 2) x--;
            //while (y > (int)RenderSize.Height - 2) y--;
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            var x0 = x;
            var x1 = x + 1;
            var y0 = y;
            var y1 = y + 1;

            var pixel1 = bitmap.GetPixel(x0, y0);
            var pixel2 = bitmap.GetPixel(x1, y0);
            var pixel3 = bitmap.GetPixel(x0, y1);
            var pixel4 = bitmap.GetPixel(x1, y1);

            var pixel1A = (intensity + ((double)pixel1.A / 255.0) * (1 - intensity));
            var pixel1B = (byte)((((color.B / 255.0 * intensity) + (pixel1.B / 255.0 * (pixel1.A / 255.0) * (1 - intensity)))) * 255);
            var pixel1G = (byte)((((color.G / 255.0 * intensity) + (pixel1.G / 255.0 * (pixel1.A / 255.0) * (1 - intensity)))) * 255);
            var pixel1R = (byte)((((color.R / 255.0 * intensity) + (pixel1.R / 255.0 * (pixel1.A / 255.0) * (1 - intensity)))) * 255);

            var pixel2A = (intensity + ((double)pixel2.A / 255.0) * (1 - intensity));
            var pixel2B = (byte)((((color.B / 255.0 * intensity) + (pixel2.B / 255.0 * (pixel2.A / 255.0) * (1 - intensity)))) * 255);
            var pixel2G = (byte)((((color.G / 255.0 * intensity) + (pixel2.G / 255.0 * (pixel2.A / 255.0) * (1 - intensity)))) * 255);
            var pixel2R = (byte)((((color.R / 255.0 * intensity) + (pixel2.R / 255.0 * (pixel2.A / 255.0) * (1 - intensity)))) * 255);

            var pixel3A = (intensity + ((double)pixel3.A / 255.0) * (1 - intensity));
            var pixel3B = (byte)((((color.B / 255.0 * intensity) + (pixel3.B / 255.0 * (pixel3.A / 255.0) * (1 - intensity)))) * 255);
            var pixel3G = (byte)((((color.G / 255.0 * intensity) + (pixel3.G / 255.0 * (pixel3.A / 255.0) * (1 - intensity)))) * 255);
            var pixel3R = (byte)((((color.R / 255.0 * intensity) + (pixel3.R / 255.0 * (pixel3.A / 255.0) * (1 - intensity)))) * 255);

            var pixel4A = (intensity + ((double)pixel4.A / 255.0) * (1 - intensity));
            var pixel4B = (byte)((((color.B / 255.0 * intensity) + (pixel4.B / 255.0 * (pixel4.A / 255.0) * (1 - intensity)))) * 255);
            var pixel4G = (byte)((((color.G / 255.0 * intensity) + (pixel4.G / 255.0 * (pixel4.A / 255.0) * (1 - intensity)))) * 255);
            var pixel4R = (byte)((((color.R / 255.0 * intensity) + (pixel4.R / 255.0 * (pixel4.A / 255.0) * (1 - intensity)))) * 255);

            RgbColor pixel1Color = new RgbColor(pixel1R, pixel1G, pixel1B);
            RgbColor pixel2Color = new RgbColor(pixel2R, pixel2G, pixel2B);
            RgbColor pixel3Color = new RgbColor(pixel3R, pixel3G, pixel3B);
            RgbColor pixel4Color = new RgbColor(pixel4R, pixel4G, pixel4B);

            /*bitmap.SetPixel(x0, y0, pixel1Color.ToXnaColor((float)pixel1A));
            bitmap.SetPixel(x1, y0, pixel2Color.ToXnaColor((float)pixel2A));
            bitmap.SetPixel(x0, y1, pixel3Color.ToXnaColor((float)pixel3A));
            bitmap.SetPixel(x1, y1, pixel4Color.ToXnaColor((float)pixel4A));*/

            bitmap.SetPixel(x0, y0, System.Drawing.Color.FromArgb(
                (int)(pixel1A * 255),
                pixel1R < 232 ? pixel1R + wartosc : 255,
                pixel1G < 232 ? pixel1G + wartosc : 255,
                pixel1B < 232 ? pixel1B + wartosc : 255
            ));

            bitmap.SetPixel(x1, y0, System.Drawing.Color.FromArgb(
                (int)(pixel2A * 255),
                pixel2R < 232 ? pixel2R + wartosc : 255,
                pixel2G < 232 ? pixel2G + wartosc : 255,
                pixel2B < 232 ? pixel2B + wartosc : 255
            ));

            bitmap.SetPixel(x0, y1, System.Drawing.Color.FromArgb(
                (int)(pixel3A * 255),
                pixel3R < 232 ? pixel3R + wartosc : 255,
                pixel3G < 232 ? pixel3G + wartosc : 255,
                pixel3B < 232 ? pixel3B + wartosc : 255
            ));

            bitmap.SetPixel(x1, y1, System.Drawing.Color.FromArgb(
                (int)(pixel4A * 255),
                pixel4R < 232 ? pixel4R + wartosc : 255,
                pixel4G < 232 ? pixel4G + wartosc : 255,
                pixel4B < 232 ? pixel4B + wartosc : 255
            ));
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
                    //lockBitmap.SetPixel(y, x, color.ToXnaColor());
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
                    //lockBitmap.SetPixel(x, y, color.ToXnaColor());
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

            double dx = bX - aX;
            double dy = bY - aY;
            double gradient = dy / dx;

            // handle first endpoint
            double xEnd = aX;
            double yEnd = aY + gradient * (xEnd - aX);
            double xGap = MathHelper.Rfpart(aX + 0.5);
            int xPxl1 = (int)xEnd;
            int yPxl1 = (int)yEnd;
            float intensityTop = (float)(MathHelper.Rfpart(yEnd) * xGap);
            float intensityDown = (float)(MathHelper.Fpart(yEnd) * xGap);

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

            double intery = yEnd + gradient;

            // handle second endpoint
            xEnd = bX;
            yEnd = bY + gradient * (xEnd - bX);
            xGap = MathHelper.Fpart(bX + 0.5);
            int xPxl2 = (int)xEnd;
            int yPxl2 = (int)yEnd;
            intensityTop = (float)(MathHelper.Rfpart(yEnd) * xGap);
            intensityDown = (float)(MathHelper.Fpart(yEnd) * xGap);

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
                intensityTop = (float)MathHelper.Rfpart(intery);
                intensityDown = (float)MathHelper.Fpart(intery);

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

            var xnaColor = color.ToXnaColor();

            /*lockBitmap.SetPixel(vX - 1, vY - 3, xnaColor);
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
            lockBitmap.SetPixel(vX + 1, vY + 3, xnaColor);*/
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
