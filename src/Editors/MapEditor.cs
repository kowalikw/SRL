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
        public DrawPolygonState ActualPolygonState { get; set; }

        /// <summary>
        /// Initialize map editor control.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ActualPolygon = new Polygon();
            Map = new Map(512, 512); 
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
            ActualPolygonState = DrawPolygonState.Empty;
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

            spriteBatch.BeginDraw();

            DrawMap();

            switch (Mode)
            {
                case MapEditorMode.DrawPolygon:
                    ActualPolygonState = CheckPolygon(ActualPolygon, CursorPosition, true);
                    break;
                case MapEditorMode.DrawDone:
                    ActualPolygonState = CheckPolygon(ActualPolygon, CursorPosition, false);

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
        /// Draws current polygon.
        /// </summary>
        private void DrawActualPolygon()
        {
            spriteBatch.DrawPolygon(ActualPolygon, ActualPolygonState, CursorPosition);
        }

        /// <summary>
        /// Draws map.
        /// </summary>
        private void DrawMap()
        {
            foreach (Polygon polygon in Map.Obstacles)
                spriteBatch.DrawPolygon(polygon, DrawPolygonState.Done);
            DrawActualPolygon();
        }
    }
}
