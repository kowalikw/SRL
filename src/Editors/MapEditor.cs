using System;
using SRL.MonoGameControl;
using SRL.Model.Model;
using SRL.Model.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = SRL.Model.Model.Point;

namespace SRL.Editors
{
    /// <summary>
    /// Map Editor Control.
    /// </summary>
    public class MapEditor : Editor
    {
        private SpriteBatch spriteBatch;

        public Polygon CurrentPolygon { get; private set; }
        public Map Map { get; private set; }
        public MapEditorMode Mode { get; set; }
        public Point CursorPosition { get; set; }
        public DrawPolygonState CurrentPolygonState { get; set; }

        /// <summary>
        /// Initialize map editor control.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentPolygon = new Polygon();
            Map = new Map(512, 512); 
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
            CurrentPolygonState = DrawPolygonState.Empty;
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
                    CurrentPolygonState = CheckPolygon(CurrentPolygon, CursorPosition, true);
                    break;
                case MapEditorMode.DrawDone:
                    CurrentPolygonState = CheckPolygon(CurrentPolygon, CursorPosition, false);

                    if (CurrentPolygon.IsFinished())
                        CurrentPolygon.Vertices.RemoveAt(CurrentPolygon.VertexCount - 1);
                    
                    if (CurrentPolygon.IsCorrect())
                        Map.Obstacles.Add(CurrentPolygon);

                    Mode = MapEditorMode.Idle;
                    CurrentPolygon = new Polygon();
                    break;
                case MapEditorMode.Idle:
                    break;
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draws current polygon.
        /// </summary>
        private void DrawCurrentPolygon()
        {
            spriteBatch.DrawPolygon(CurrentPolygon, CurrentPolygonState, CursorPosition);
        }

        /// <summary>
        /// Draws map.
        /// </summary>
        private void DrawMap()
        {
            foreach (Polygon polygon in Map.Obstacles)
                spriteBatch.DrawPolygon(polygon, DrawPolygonState.Done);
            DrawCurrentPolygon();
        }
    }
}
