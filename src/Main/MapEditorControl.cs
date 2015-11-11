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

        public Polygon ActualPolygon { get; private set; }
        public Map Map { get; private set; }
        public MapEditorMode Mode { get; set; }
        public Point CursorPosition { get; set; }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ActualPolygon = new Polygon();
            Map = new Map();
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
        }


        protected override void Unitialize()
        {
            spriteBatch.Dispose();
        }


        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.Begin();

            switch (Mode)
            {
                case MapEditorMode.DrawPolygon:
                    DrawMap();

                    for (int i = 0; i < ActualPolygon.Vertices.Count; i++)
                    {
                        if (i == 0)
                            spriteBatch.DrawCircle(ActualPolygon.Vertices[0], 8, 100, Color.Blue, 3);
                        else
                        {
                            spriteBatch.DrawCircle(ActualPolygon.Vertices[0], 3, 100, Color.Blue, 3);
                            spriteBatch.DrawLine(ActualPolygon.Vertices[i - 1], ActualPolygon.Vertices[i], Color.Blue, 2);
                        }
                    }

                    if (ActualPolygon.Vertices.Count > 0)
                    {
                        if (GeometryHelper.DistanceBetweenPoints(ActualPolygon.Vertices[0], CursorPosition) <= 8 && ActualPolygon.Vertices.Count >= 3)
                        {
                            spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], ActualPolygon.Vertices[0], Color.Blue, 2);
                            spriteBatch.DrawCircle(ActualPolygon.Vertices[0], 8, 100, Color.Yellow, 3);
                        }
                        else
                        {
                            bool isSegmentIntersection = false;

                            for (int i = 0; i < ActualPolygon.Vertices.Count - 2; i++)
                            {
                                if (GeometryHelper.SegmentIntersection(ActualPolygon.Vertices[i], ActualPolygon.Vertices[i + 1], ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition))
                                    isSegmentIntersection = true;
                            }

                            if (isSegmentIntersection)
                                spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition, Color.Red, 2);
                            else
                                spriteBatch.DrawLine(ActualPolygon.Vertices[ActualPolygon.VertexCount - 1], CursorPosition, Color.Green, 2);
                        }
                    }

                    if (ActualPolygon.Vertices.Count >= 3)
                    {
                        if (GeometryHelper.DistanceBetweenPoints(ActualPolygon.Vertices[0], ActualPolygon.Vertices[ActualPolygon.VertexCount - 1]) <= 8)
                        {
                            ActualPolygon.Vertices.RemoveAt(ActualPolygon.VertexCount - 1);
                            Mode = MapEditorMode.DrawDone;
                        }
                    }

                    break;
                case MapEditorMode.DrawDone:
                    Map.Obstacles.Add(ActualPolygon);
                    Mode = MapEditorMode.Idle;
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
            foreach (Polygon p in Map.Obstacles)
                for(int i = 0; i < p.VertexCount; i++)
                    spriteBatch.DrawLine(p.Vertices[i], p.Vertices[(i + 1) % p.VertexCount], Color.Black, 2);
        }
    }
}
