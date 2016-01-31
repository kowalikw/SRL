using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using Point = System.Windows.Point;

namespace SRL.Main.View.MonoGameArea
{
    /// <summary>
    /// View class that contains UI logic for the MonoGame control that displays vehicle edit area.
    /// </summary>
    internal class VehicleEditArea : AreaBase
    {
        private const double ArrowLength = 0.2;

        private readonly VehicleEditorViewModel _context = ServiceLocator.Current.GetInstance<VehicleEditorViewModel>();

        private NotifyCollectionChangedEventHandler _vehicleShapeChangedHandler;
        private PropertyChangedEventHandler _propertyChangedHandler;

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            _vehicleShapeChangedHandler = (o, e) => RedrawStaticObjectsTexture();
            _propertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.ShapeDone)
                 || e.PropertyName == nameof(_context.Pivot)
                 || e.PropertyName == nameof(_context.Direction))
                    RedrawStaticObjectsTexture();
            };

            _context.VehicleShape.CollectionChanged += _vehicleShapeChangedHandler;
            _context.PropertyChanged += _propertyChangedHandler;
        }

        /// <inheritdoc />
        protected override void Unitialize()
        {
            base.Unitialize();

            _context.VehicleShape.CollectionChanged -= _vehicleShapeChangedHandler;
            _context.PropertyChanged -= _propertyChangedHandler;
        }

        /// <inheritdoc />
        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            Point normalizedMousePosition = MousePosition.Normalize(RenderSize);

            if (!_context.ShapeDone)
            {
                Color color = _context.AddShapeVertexCommand.CanExecute(normalizedMousePosition)
                    ? ValidColor
                    : InvalidColor;

                if (IsMouseOver && _context.VehicleShape.Count > 0)
                {
                    // Draw active line.
                    Point endpointA = _context.VehicleShape.GetLast();
                    Point endpointB = normalizedMousePosition;
                    spriteBatch.DrawLine(endpointA, endpointB, RenderSize, color, AntialiasingEnabled);
                }
                else if (_context.VehicleShape.Count == 1)
                {
                    spriteBatch.DrawVertex(_context.VehicleShape.GetLast(), RenderSize, ActiveColor, AntialiasingEnabled);
                }
            }
            else if (!_context.Pivot.HasValue)
            {
                if (IsMouseOver)
                {
                    Color color = _context.SetPivotCommand.CanExecute(normalizedMousePosition)
                        ? ValidColor
                        : InvalidColor;

                    spriteBatch.DrawVertex(normalizedMousePosition, RenderSize, color, AntialiasingEnabled);
                }
            }
            else if (!_context.Direction.HasValue)
            {
                if (IsMouseOver)
                {
                    double angle = GeometryHelper.GetAngle(_context.Pivot.Value, normalizedMousePosition);
                    Color color = _context.SetDirectionCommand.CanExecute(angle)
                        ? ValidColor
                        : InvalidColor;

                    spriteBatch.DrawArrow(_context.Pivot.Value, ArrowLength, angle, RenderSize, color, AntialiasingEnabled);
                }
            }
            else
            {
                spriteBatch.DrawArrow(_context.Pivot.Value, ArrowLength, _context.Direction.Value, RenderSize, RegularColor, AntialiasingEnabled);
            }
        }
        /// <inheritdoc />
        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (!_context.ShapeDone)
            {
                lockBitmap.DrawPath(_context.VehicleShape, RenderSize, ActiveColor, AntialiasingEnabled);
            }
            else
            {
                lockBitmap.DrawPolygon(new Polygon(_context.VehicleShape), RenderSize, RegularColor, AntialiasingEnabled);
            }

            if (_context.Pivot.HasValue)
            {
                Color color = _context.Direction.HasValue ? RegularColor : ActiveColor;

                lockBitmap.DrawVertex(_context.Pivot.Value, RenderSize, color, AntialiasingEnabled);
            }

            // Arrow is drawn dynamically only.
        }
        /// <inheritdoc />
        protected override void OnMouseUp(MouseButton button)
        {
            var normalizedMousePosition = MousePosition.Normalize(RenderSize);
            if (button == MouseButton.Left)
            {
                if (!_context.ShapeDone)
                {
                    if (_context.FinishShapeCommand.CanExecute(null))
                    {
                        var denormalizedEndpoint = _context.VehicleShape[0].Denormalize(RenderSize);
                        if (IsMousePulledByPoint(denormalizedEndpoint))
                        {
                            _context.FinishShapeCommand.Execute(null);
                            return;
                        }
                    }

                    if (_context.AddShapeVertexCommand.CanExecute(normalizedMousePosition))
                        _context.AddShapeVertexCommand.Execute(normalizedMousePosition);
                }
                else if (!_context.Pivot.HasValue)
                {
                    if (_context.SetPivotCommand.CanExecute(normalizedMousePosition))
                        _context.SetPivotCommand.Execute(normalizedMousePosition);
                }
                else if (!_context.Direction.HasValue)
                {
                    double angle = GeometryHelper.GetAngle(_context.Pivot.Value, normalizedMousePosition);

                    if (_context.SetDirectionCommand.CanExecute(angle))
                        _context.SetDirectionCommand.Execute(angle);
                }
            }
            else if (button == MouseButton.Right)
            {
                if (_context.BackCommand.CanExecute(null))
                    _context.BackCommand.Execute(null);
            }
        }
    }
}
