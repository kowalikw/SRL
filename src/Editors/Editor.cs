﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Models;
using SRL.Models.Enum;
using SRL.Models.Model;
using SRL.MonoGameControl;
using Point = SRL.Models.Model.Point;

namespace SRL.Editors
{
    public abstract class Editor : MonoGameControl.MonoGameControl
    {
        protected const int StartCircleRadius = 8;
        protected const int StartCircleThickness = 3;
        protected const int PointRadius = 3;
        protected const int PointThickness = 3;
        protected const int LineThickness = 2;
        protected const int CircleSegments = 100;

        protected Color normalDrawColor = Color.Black;
        protected Color activeDrawColor = Color.Blue;
        protected Color correctActiveDrawColor = Color.Green;
        protected Color incorrectActiveDrawColor = Color.Red;
        protected Color activeStartCircleColor = Color.Orange;

        public DrawLineState CheckLine(Polygon polygon, Point nextVertice)
        {
            if (polygon.Vertices.Contains(nextVertice))
                return DrawLineState.Done;

            for (int i = 0; i < polygon.VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(polygon.Vertices[i], polygon.Vertices[i + 1], polygon.Vertices[polygon.VertexCount - 1], nextVertice))
                    return DrawLineState.Incorrect;

            return DrawLineState.Correct;
        }

        public DrawPolygonState DrawPolygon(SpriteBatch spriteBatch, Polygon polygon, Point cursorPosition = null, bool activeDraw = false)
        {
            if (!activeDraw)
            {
                for (int i = 0; i < polygon.VertexCount; i++)
                    spriteBatch.DrawLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.VertexCount], normalDrawColor, LineThickness);

                return DrawPolygonState.Done;
            }
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

                if (!polygon.IsEmpty())
                {
                    switch (CheckLine(polygon, cursorPosition))
                    {
                        case DrawLineState.Correct:
                            if (polygon.IsFinished(cursorPosition))
                            {
                                spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], polygon.Vertices[0], correctActiveDrawColor, LineThickness);
                                spriteBatch.DrawCircle(polygon.Vertices[0], StartCircleRadius, CircleSegments, activeStartCircleColor, StartCircleThickness);
                            }
                            else
                                spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], cursorPosition, correctActiveDrawColor, LineThickness);
                            return DrawPolygonState.Correct;
                        case DrawLineState.Incorrect:
                            spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], cursorPosition, incorrectActiveDrawColor, LineThickness);
                            return DrawPolygonState.Incorrect;
                        case DrawLineState.Done:
                            spriteBatch.DrawLine(polygon.Vertices[polygon.VertexCount - 1], cursorPosition, activeDrawColor, LineThickness);
                            return DrawPolygonState.Correct;
                    }
                }

                return DrawPolygonState.Empty;
            }
        }
    }
}
