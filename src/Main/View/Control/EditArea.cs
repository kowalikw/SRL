﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Model;
using SRL.Model.Enum;
using SRL.Model.Model;
using SRL.MonoGameControl;

namespace SRL.Main.View.Control
{
    public abstract class EditArea : MonoGameControl.MonoGameControl
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






        protected SpriteBatch SpriteBatch;

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Unitialize()
        {
            SpriteBatch.Dispose();
        }

        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            SpriteBatch.BeginDraw();
            Render(SpriteBatch, time);
            SpriteBatch.EndDraw();
        }

        protected abstract void Render(SpriteBatch spriteBatch, TimeSpan time);

















        public DrawLineState CheckLine(Polygon polygon, Model.Model.Point nextVertice)
        {
            foreach (Model.Model.Point p in polygon.Vertices)
                if (p.X == nextVertice.X && p.Y == nextVertice.Y)
                    return DrawLineState.Done;

            for (int i = 0; i < polygon.VertexCount - 2; i++)
                if (GeometryHelper.SegmentsIntersect(polygon.Vertices[i], polygon.Vertices[i + 1], polygon.Vertices[polygon.VertexCount - 1], nextVertice))
                    return DrawLineState.Incorrect;

            return DrawLineState.Correct;
        }

        public DrawPolygonState CheckPolygon(Polygon polygon, Model.Model.Point cursorPosition = null, bool activeDraw = false)
        {
            if (!activeDraw)
                return DrawPolygonState.Done;
            else
            {
                if (!polygon.IsEmpty())
                {
                    switch (CheckLine(polygon, cursorPosition))
                    {
                        case DrawLineState.Correct:
                            return DrawPolygonState.Correct;
                        case DrawLineState.Incorrect:
                            return DrawPolygonState.Incorrect;
                        case DrawLineState.Done:
                            return DrawPolygonState.Correct;
                    }
                }

                return DrawPolygonState.Empty;
            }
        }


    }
}
