using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using SRL.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;
using EditingStage = SRL.Main.ViewModel.VehicleEditorViewModel.EditingStage;
using SRL.Model.Model;

namespace SRL.Main.View.Control
{
    public class TraceArea : EditArea
    {
        private TracingViewModel _context;

        protected override void Initialize()
        {
            base.Initialize();
            _context = (TracingViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            foreach (var polygon in _context.CurrentModel)
            {
                if (GeometryHelper.IsPointInPolygon(MousePosition, polygon))
                    spriteBatch.DrawPolygon(polygon, EditArea.HoverColor, EditArea.LineThickness);
                else if (_context.CurrentShape.Equals(polygon))
                    spriteBatch.DrawPolygon(polygon, EditArea.ActiveColor, EditArea.LineThickness);
                else
                    spriteBatch.DrawPolygon(polygon, EditArea.RegularColor, EditArea.LineThickness);
            }
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            foreach (var polygon in _context.CurrentModel)
                if(GeometryHelper.IsPointInPolygon(MousePosition, polygon))
                    _context.CurrentShape = polygon;
        }
    }
}