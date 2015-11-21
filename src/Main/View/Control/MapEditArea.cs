using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SRL.Main.View.Control
{
    public class MapEditArea : EditArea
    {
        private MapEditorViewModel _context;


        private SrlPoint LastPolygonVertex => _context.CurrentPolygon[_context.CurrentPolygon.Count - 1];
        private SrlPoint FirstPolygonVertex => _context.CurrentPolygon[0];


        protected override void Initialize()
        {
            base.Initialize();
            _context = (MapEditorViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            //Draw placed, valid polygons.
            foreach (var polygon in _context.CurrentModel.Obstacles)
            {
                spriteBatch.DrawPolygon(polygon, RegularColor, LineThickness);
            }

            //Draw currently constructed obstacle.
            spriteBatch.DrawPath(_context.CurrentPolygon, false, ActiveColor, LineThickness);

            //Draw new potential obstacle polygon side.
            if (!_context.IsCurrentModelValid)
            {
                Color segmentColor = _context.AddVertexCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;
                spriteBatch.DrawLine(LastPolygonVertex, MousePosition, segmentColor, LineThickness);
            }
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            if (!_context.IsCurrentModelValid)
            {
                if (IsMousePulledByPoint(FirstPolygonVertex))
                {
                    if (_context.CloseCurrentPolygonCommand.CanExecute(null))
                    {
                        _context.CloseCurrentPolygonCommand.Execute(null);
                        return;
                    }
                }

            }

            if (_context.AddVertexCommand.CanExecute(position))
                _context.AddVertexCommand.Execute(position);
        }

    }
}
