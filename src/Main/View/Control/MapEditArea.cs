using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Model.Enum;
using SRL.Model.Model;
using SRL.MonoGameControl;
using Point = SRL.Model.Model.Point;

namespace SRL.Main.View.Control
{
    public class MapEditArea : EditArea
    {
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
            base.Initialize();

            CurrentPolygon = new Polygon();
            Map = new Map(512, 512);
            Mode = MapEditorMode.Idle;
            CursorPosition = new Point(0, 0);
            CurrentPolygonState = DrawPolygonState.Empty;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            DrawMap(spriteBatch);

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
        }

        /// <summary>
        /// Draws current polygon.
        /// </summary>
        private void DrawCurrentPolygon(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawPolygon(CurrentPolygon, CurrentPolygonState, CursorPosition);
        }

        /// <summary>
        /// Draws map.
        /// </summary>
        private void DrawMap(SpriteBatch spriteBatch)
        {
            foreach (Polygon polygon in Map.Obstacles)
                spriteBatch.DrawPolygon(polygon, DrawPolygonState.Done);
            DrawCurrentPolygon(spriteBatch);
        }
    }
}
