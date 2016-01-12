﻿using System;
using System.Windows;
using Microsoft.Xna.Framework;
using SRL.Commons.Model;
using SRL.Main.Utilities;
using MathHelper = SRL.Commons.Utilities.MathHelper;
using Point = System.Windows.Point;

namespace SRL.Main.Drawing
{
    internal static class StaticDrawHelper
    {
        private static void SetBigPixel(this LockBitmap bitmap, int x, int y, Color color)
        {
            bitmap.SetPixel(x, y, color);
            bitmap.SetPixel(x + 1, y, color);
            bitmap.SetPixel(x, y + 1, color);
            bitmap.SetPixel(x + 1, y + 1, color);
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

            int xStart = aX;
            float xEnd = bX;

            int x = xStart;
            float y = aY + gradient * (xStart - aX);

            if (steep)
            {
                for (; x <= xEnd; x++)
                {
                    float intensityTop = MathHelper.Rfpart(y);
                    float intensityDown = MathHelper.Fpart(y);

                    lockBitmap.SetBigPixel((int)y, x, color * intensityTop);
                    lockBitmap.SetBigPixel((int)y + 1, x, color * intensityDown);

                    y = y + gradient;
                }
            }
            else
            {
                for (; x <= xEnd; x++)
                {
                    float intensityTop = MathHelper.Rfpart(y);
                    float intensityDown = MathHelper.Fpart(y);

                    lockBitmap.SetBigPixel(x, (int)y, color * intensityTop);
                    lockBitmap.SetBigPixel(x, (int)y + 1, color * intensityDown);

                    y = y + gradient;
                }
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
