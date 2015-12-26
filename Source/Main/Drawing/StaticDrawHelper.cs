using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;

namespace SRL.Main.Drawing
{
    internal static class StaticDrawHelper
    {
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
                    lockBitmap.SetPixel(y, x, color.ToXnaColor());
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
                    lockBitmap.SetPixel(x, y, color.ToXnaColor());
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

            throw new NotImplementedException(); //TODO
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
