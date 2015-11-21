using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model;
using SRL.Model.Enum;
using SRL.Model.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SRL.Main.View.Control
{
    public class MapEditArea : EditArea
    {
        

        private MapEditorViewModel _context;

        public Polygon CurrentPolygon { get; private set; }
        public Map Map { get; private set; }
        public MapEditorMode Mode { get; set; }
        public DrawPolygonState CurrentPolygonState { get; set; }


        private SrlPoint LastPolygonVertex => _context.CurrentPolygon[_context.CurrentPolygon.Count - 1];
        private SrlPoint FirstPolygonVertex => _context.CurrentPolygon[0];

        protected override void Initialize()
        {
            base.Initialize();
            _context = (MapEditorViewModel)DataContext;




            CurrentPolygon = new Polygon();
            Map = new Map(512, 512);
            Mode = MapEditorMode.Idle;
            CurrentPolygonState = DrawPolygonState.Empty;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            //Draw placed, valid polygons.
            foreach (var polygon in _context.CurrentModel.Obstacles)
            {
                spriteBatch.DrawPolygon(polygon, RegularColor, LineThickness);
            }

            spriteBatch.DrawPath(_context.CurrentPolygon, false, RegularColor, LineThickness);

            if (!_context.IsCurrentModelValid)
            {
                Color color = _context.PlacePointCommand.CanExecute(MousePosition) ? ActiveColor : InvalidColor;
                spriteBatch.DrawLine(LastPolygonVertex, MousePosition, color, LineThickness);
            }

            
            /*
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
            */
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            if (!_context.IsCurrentModelValid)
            {
                if (GeometryHelper.GetDistance(position, FirstPolygonVertex) < VertexPullRadius)
                {
                    if (_context.CloseCurrentPolygonCommand.CanExecute(null))
                    {
                        _context.CloseCurrentPolygonCommand.Execute(null);
                        return;
                    }
                }
            }

            if (_context.PlacePointCommand.CanExecute(position))
                _context.PlacePointCommand.Execute(position);
        }

    }
}
