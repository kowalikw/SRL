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
    public class VehicleEditArea : EditArea
    {
        private const float AxisLength = 100;
        
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

            // TODO
            /*if (GeometryHelper.IsPointInPolygon(MousePosition, new Model.Model.Polygon(_context.VehicleShape)))
                shapeColor = Color.Yellow;*/

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
                        shapeColor = MousePosition != null && GeometryHelper.IsPointInPolygon(MousePosition, _context.CurrentModel.Shape)
                            ? ValidColor
                            : InvalidColor;
                        spriteBatch.DrawPath(_context.VehicleShape, _context.Stage > EditingStage.ShapeStarted, shapeColor, LineThickness);
                        //spriteBatch.DrawVertex(MousePosition, originColor, VertexThickness);
                        break;
                    }
                case EditingStage.OrientationOriginSet:
                    {
                        //Draw orientation origin.
                        spriteBatch.DrawVertex(_context.CurrentModel.OrientationOrigin, ActiveColor, VertexThickness);

                        //Draw new potential orientation axis.
                        spriteBatch.DrawArrow(_context.CurrentModel.OrientationOrigin, MousePosition, ActiveColor, LineThickness, AxisLength);
                        break;
                    }
                case EditingStage.OrientationAngleSet:
                    {
                        //Draw orientation origin.
                        spriteBatch.DrawVertex(_context.CurrentModel.OrientationOriginOld, RegularColor, VertexThickness);

                        //Draw orientation axis.
                        spriteBatch.DrawArrow(_context.CurrentModel.OrientationOriginOld, _context.CurrentModel.OrientationOriginEnd, RegularColor, LineThickness, AxisLength);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            if (_context.CloseVehicleShapeCommand.CanExecute(null) && IsMousePulledByPoint(FirstShapeVertex))
            {
                _context.CloseVehicleShapeCommand.Execute(null);
            }
            else if (_context.AddVertexCommand.CanExecute(position))
            {
                _context.AddVertexCommand.Execute(position);
            }
            else if (_context.SetOrientationOriginCommand.CanExecute(position))
            {
                _context.SetOrientationOriginCommand.Execute(position);
            }
            else if (_context.Stage == EditingStage.OrientationOriginSet) //Checking stage is necessary here due to angle calculation.
            {
                //double angle = GeometryHelper.GetDegAngle(_context.CurrentModel.OrientationOrigin, MousePosition);
                //if (_context.SetOrientationAngleCommand.CanExecute(angle))
                _context.SetOrientationAngleCommand.Execute(position);
            }
        }
    }
}