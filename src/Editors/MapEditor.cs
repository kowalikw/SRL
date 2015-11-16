using System;
using SRL.MonoGameControl;
using SRL.Models.Model;
using SRL.Models.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = SRL.Models.Model.Point;

namespace SRL.Editors
{
    /// <summary>
    /// Map Editor Control.
    /// </summary>
    public class MapEditor : Editor
    {
        private SpriteBatch spriteBatch;

        public Polygon ActualPolygon { get; private set; }
        public Map Map { get; private set; }
        public MapEditorMode Mode { get; set; }
        public Point CursorPosition { get; set; }
        public bool IsSegmentIntersection { get; set; }

        /// <summary>
        /// Initialize map editor control.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ActualPolygon = new Polygon();
            Map = new Map();
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
            IsSegmentIntersection = false;
        }

        /// <summary>
        /// Uninitialize map editor control.
        /// </summary>
        protected override void Unitialize()
        {
            spriteBatch.Dispose();
        }

        /// <summary>
        /// Render map editor control content.
        /// </summary>
        /// <param name="time">Time of rendering.</param>
        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            IsSegmentIntersection = false;

            spriteBatch.BeginDraw();

            DrawMap();

            switch (Mode)
            {
                case MapEditorMode.DrawPolygon:
                    if (DrawPolygon(spriteBatch, ActualPolygon, CursorPosition, true) == DrawPolygonState.Incorrect)
                        IsSegmentIntersection = true;
                    break;
                case MapEditorMode.DrawDone:
                    if (ActualPolygon.IsFinished())
                        ActualPolygon.Vertices.RemoveAt(ActualPolygon.VertexCount - 1);
                    
                    if (ActualPolygon.IsCorrect())
                        Map.Obstacles.Add(ActualPolygon);

                    Mode = MapEditorMode.Idle;
                    ActualPolygon = new Polygon();
                    break;
                case MapEditorMode.Idle:
                    break;
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draw map.
        /// </summary>
        private void DrawMap()
        {
            foreach (Polygon polygon in Map.Obstacles)
                DrawPolygon(spriteBatch, polygon);
        }
    }
}
