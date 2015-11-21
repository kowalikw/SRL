using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model;
using Point = SRL.Model.Model.Point;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;
using EditingStage = SRL.Main.ViewModel.VehicleEditorViewModel.EditingStage;

namespace SRL.Main.View.Control
{
    public class VehicleEditArea : EditArea
    {
        private const float AxisLength = 50;


        private VehicleEditorViewModel _context;
        

        private SrlPoint LastShapeVertex => _context.VehicleShape[_context.VehicleShape.Count - 1];
        private SrlPoint FirstShapeVertex => _context.VehicleShape[0];


        protected override void Initialize()
        {
            base.Initialize();
            _context = (VehicleEditorViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            //Draw shape of the vehicle.
            Color shapeColor = _context.Stage > EditingStage.ShapeStarted ? RegularColor : ActiveColor;
            spriteBatch.DrawPath(_context.VehicleShape, _context.Stage > EditingStage.ShapeStarted, shapeColor, LineThickness);

            if (_context.Stage == EditingStage.ShapeStarted)
            {
                //Draw new potential shape polygon side.
                Color segmentColor = _context.AddVertexCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;
                spriteBatch.DrawLine(LastShapeVertex, MousePosition, segmentColor, LineThickness);
            }
            else if (_context.Stage == EditingStage.ShapeDone)
            {
                //Draw new potential orientation origin.
                Color originColor = _context.SetOrientationOriginCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;
                spriteBatch.DrawVertex(MousePosition, originColor, VertexThickness);
            }
            else if (_context.Stage == EditingStage.OrientationOriginSet)
            {
                //Draw orientation origin.
                spriteBatch.DrawVertex(_context.CurrentModel.Origin, ActiveColor, VertexThickness);

                //Draw new potential orientation axis.
                double angle = GeometryHelper.GetDegAngle(_context.CurrentModel.Origin, MousePosition);
                Color axisColor = _context.SetOrientationAngleCommand.CanExecute(angle) ? ValidColor : InvalidColor;
                spriteBatch.DrawArrow(_context.CurrentModel.Origin, MousePosition, axisColor, LineThickness);

                //TODO draw fixed length arrow instead of dynamic length arrow (implement draw arrow)
                spriteBatch.DrawArrow(_context.CurrentModel.Origin, MousePosition, axisColor, LineThickness);
                //spriteBatch.DrawArrow(_context.CurrentModel.Origin, AxisLength, (float)(angle * Math.PI / 180), RegularColor, LineThickness);
            }
            else if (_context.Stage == EditingStage.OrientationAngleSet)
            {
                //Draw orientation origin.
                spriteBatch.DrawVertex(_context.CurrentModel.Origin, RegularColor, VertexThickness);

                double angle = _context.CurrentModel.DirectionAngle;

                //TODO draw fixed length arrow (implement draw arrow)
                spriteBatch.DrawArrow(_context.CurrentModel.Origin, AxisLength, (float)(angle * Math.PI / 180), RegularColor, LineThickness);
            }

            //TODO remove stage checking here?
        }

        protected override void OnMouseUp(Point position)
        {
            if (_context.Stage < EditingStage.ShapeDone)
            {
                if (_context.CloseVehicleShapeCommand.CanExecute(null) && IsMousePulledByPoint(FirstShapeVertex))
                    _context.CloseVehicleShapeCommand.Execute(null);
                else if (_context.AddVertexCommand.CanExecute(position))
                    _context.AddVertexCommand.Execute(position);
            }
            else if (_context.Stage == EditingStage.ShapeDone)
            {
                if (_context.SetOrientationOriginCommand.CanExecute(position))
                    _context.SetOrientationOriginCommand.Execute(position);
            }
            else if (_context.Stage == EditingStage.OrientationOriginSet)
            {
                double angle = GeometryHelper.GetDegAngle(_context.CurrentModel.Origin, MousePosition);
                if (_context.SetOrientationAngleCommand.CanExecute(angle))
                    _context.SetOrientationAngleCommand.Execute(angle);
                //TODO pass angle
            }

            //TODO remove stage checking here?
        }
    }
}
