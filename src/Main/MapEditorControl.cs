using System;
using System.Collections.Generic;
using SRL.Main.Resources;
using SRL.MonoGameControl;
using SRL.Models.Model;
using SRL.Models.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = SRL.Models.Model.Point;

namespace SRL.Main
{
    class MapEditorControl : D3D11Host
    {
        private SpriteBatch spriteBatch;
        private Color normalDrawColor = Color.Black;
        private Color activeDrawColor = Color.Blue;
        private Color correctActiveDrawColor = Color.Green;
        private Color incorrectActiveDrawColor = Color.Red;
        private Color activeStartCircleColor = Color.Orange;

        public Polygon ActualPolygon { get; private set; }
        public Map Map { get; private set; }
        public MapEditorMode Mode { get; set; }
        public Point CursorPosition { get; set; }
        public bool IsSegmentIntersection { get; set; }
        public bool IsSegmentIntersectionUnchecked { get; private set; }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ActualPolygon = new Polygon();
            Map = new Map();
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
            IsSegmentIntersection = false;
            IsSegmentIntersectionUnchecked = false;
        }

        protected override void Unitialize()
        {
            spriteBatch.Dispose();
        }

        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.BeginDraw();

            switch (Mode)
            {
                case MapEditorMode.DrawPolygon:
                    DrawMap();

                    for (int i = 0; i < ActualPolygon.VertexCount; i++)
                    {
                        if (i == 0)
                            spriteBatch.DrawCircle(ActualPolygon.Vertices[0], int.Parse(Number.PolygonStartCircleRadius), int.Parse(Number.CircleSegments), activeDrawColor, int.Parse(Number.PolygonStartCircleThickness));
                        else
                            spriteBatch.DrawLine(ActualPolygon.Vertices[i - 1], ActualPolygon.Vertices[i], activeDrawColor, int.Parse(Number.PolygonLineThickness));
                        
                        spriteBatch.DrawCircle(ActualPolygon.Vertices[0], int.Parse(Number.PolygonPointRadius), int.Parse(Number.CircleSegments), activeDrawColor, int.Parse(Number.PolygonPointThickness));
                    }

                    if (ActualPolygon.VertexCount > 0)
                    {
                        if (GeometryHelper.DistanceBetweenPoints(ActualPolygon.Vertices[0], CursorPosition) <= int.Parse(Number.PolygonStartCircleRadius) && 
                            ActualPolygon.Vertices.Count > int.Parse(Number.MinimumPolygonVertices) && !IsSegmentIntersection)
                        {
                            spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], ActualPolygon.Vertices[0], activeDrawColor, int.Parse(Number.PolygonLineThickness));
                            spriteBatch.DrawCircle(ActualPolygon.Vertices[0], int.Parse(Number.PolygonStartCircleRadius), int.Parse(Number.CircleSegments), activeStartCircleColor, int.Parse(Number.PolygonStartCircleThickness));
                        }
                        else
                        {
                            IsSegmentIntersection = false;

                            for (int i = 0; i < ActualPolygon.VertexCount - 2; i++)
                                if (GeometryHelper.SegmentIntersection(ActualPolygon.Vertices[i], ActualPolygon.Vertices[i + 1], ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition))
                                    IsSegmentIntersection = true;

                            if (IsSegmentIntersection)
                                spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition, incorrectActiveDrawColor, int.Parse(Number.PolygonLineThickness));
                            else
                                spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition, correctActiveDrawColor, int.Parse(Number.PolygonLineThickness));
                        }
                    }

                    for (int i = 1; i < ActualPolygon.VertexCount - 2; i++)
                        if (GeometryHelper.SegmentIntersection(ActualPolygon.Vertices[i], ActualPolygon.Vertices[i + 1], ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], ActualPolygon.Vertices[0]))
                            IsSegmentIntersectionUnchecked = true;

                    break;
                case MapEditorMode.DrawDone:
                    if (ActualPolygon.Vertices.Count > int.Parse(Number.MinimumPolygonVertices))
                        if (GeometryHelper.DistanceBetweenPoints(ActualPolygon.Vertices[0], ActualPolygon.Vertices[ActualPolygon.VertexCount - 1]) <= int.Parse(Number.PolygonStartCircleRadius))
                            ActualPolygon.Vertices.RemoveAt(ActualPolygon.VertexCount - 1);

                    if (!IsSegmentIntersectionUnchecked)
                        Map.Obstacles.Add(ActualPolygon);

                    Mode = MapEditorMode.Idle;
                    IsSegmentIntersection = false;
                    IsSegmentIntersectionUnchecked = false;
                    ActualPolygon = new Polygon();
                    break;
                case MapEditorMode.Idle:
                    DrawMap();
                    break;
            }

            spriteBatch.End();
        }

        private void DrawMap()
        {
            foreach (Polygon polygon in Map.Obstacles)
                for(int i = 0; i < polygon.VertexCount; i++)
                    spriteBatch.DrawLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.VertexCount], normalDrawColor, int.Parse(Number.PolygonLineThickness));
        }
    }
}
