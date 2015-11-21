using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model;
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

            switch (_context.Stage)
            {
                case EditingStage.NotStarted:
                    break;
                case EditingStage.ShapeStarted:
                    {
                        //Draw new potential shape polygon side.
                        Color segmentColor = _context.AddVertexCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;
                        spriteBatch.DrawLine(LastShapeVertex, MousePosition, segmentColor, LineThickness);
                        break;
                    }
                case EditingStage.ShapeDone:
                    {
                        //Draw new potential orientation origin.
                        Color originColor = _context.SetOrientationOriginCommand.CanExecute(MousePosition)
                            ? ValidColor
                            : InvalidColor;
                        spriteBatch.DrawVertex(MousePosition, originColor, VertexThickness);
                        break;
                    }
                case EditingStage.OrientationOriginSet:
                    {
                        //Draw orientation origin.
                        spriteBatch.DrawVertex(_context.CurrentModel.OrientationOrigin, ActiveColor, VertexThickness);

                        //Draw new potential orientation axis.
                        double angle = GeometryHelper.GetDegAngle(_context.CurrentModel.OrientationOrigin, MousePosition);
                        Color axisColor = _context.SetOrientationAngleCommand.CanExecute(angle) ? ValidColor : InvalidColor;
                        spriteBatch.DrawArrow(_context.CurrentModel.OrientationOrigin, MousePosition, axisColor, LineThickness);
                        break;
                    }
                case EditingStage.OrientationAngleSet:
                    {
                        //Draw orientation origin.
                        spriteBatch.DrawVertex(_context.CurrentModel.OrientationOrigin, RegularColor, VertexThickness);

                        double angle = _context.CurrentModel.OrientationAngle;

                        //TODO draw fixed length arrow (implement draw arrow)
                        spriteBatch.DrawArrow(_context.CurrentModel.OrientationOrigin, AxisLength, (float)(angle * Math.PI / 180),
                            RegularColor, LineThickness);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            if (_context.CloseVehicleShapeCommand.CanExecute(null) 
                && IsMousePulledByPoint(FirstShapeVertex))
            {
                _context.CloseVehicleShapeCommand.Execute(null);
                return;
            }
            if (_context.AddVertexCommand.CanExecute(position))
            {
                _context.AddVertexCommand.Execute(position);
                return;
            }
            if (_context.SetOrientationOriginCommand.CanExecute(position))
            {
                _context.SetOrientationOriginCommand.Execute(position);
                return;
            }
            double angle = GeometryHelper.GetDegAngle(_context.CurrentModel.OrientationOrigin, MousePosition);
            if (_context.SetOrientationAngleCommand.CanExecute(angle))
            {
                _context.SetOrientationAngleCommand.Execute(angle);
                return;
            }
        }
    }
}