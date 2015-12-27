using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VehicleEditArea : AreaBase
    {
        private const double ArrowLength = 0.2;

        private readonly VehicleEditorViewModel _context = SimpleIoc.Default.GetInstance<VehicleEditorViewModel>();

        private NotifyCollectionChangedEventHandler _vehicleShapeChangedHandler;
        private PropertyChangedEventHandler _propertyChangedHandler;

        private readonly Line _activeLine = new Line();

        protected override void Initialize()
        {
            base.Initialize();
            _vehicleShapeChangedHandler = (o, e) => RedrawStaticObjectsTexture();
            _propertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.ShapeDone)
                 || e.PropertyName == nameof(_context.AntialiasingEnabled)
                 || e.PropertyName == nameof(_context.Pivot)
                 || e.PropertyName == nameof(_context.Direction))
                    RedrawStaticObjectsTexture();
            };

            _context.VehicleShape.CollectionChanged += _vehicleShapeChangedHandler;
            _context.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.VehicleShape.CollectionChanged -= _vehicleShapeChangedHandler;
            _context.PropertyChanged -= _propertyChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            Point normalizedMousePosition = MousePosition.Normalize(RenderSize);

            if (!_context.ShapeDone)
            {
                RgbColor color = _context.AddShapeVertexCommand.CanExecute(normalizedMousePosition)
                    ? ValidColor
                    : InvalidColor;

                if (IsMouseOver && _context.VehicleShape.Count > 0)
                {
                    _activeLine.EndpointA = _context.VehicleShape.GetLast();
                    _activeLine.EndpointB = normalizedMousePosition;

                    if (_context.AntialiasingEnabled)
                        spriteBatch.DrawLineAA(_activeLine, RenderSize, color);
                    else
                        spriteBatch.DrawLine(_activeLine, RenderSize, color);
                }
                else if (_context.VehicleShape.Count == 1)
                {
                    if (_context.AntialiasingEnabled)
                        spriteBatch.DrawVertexAA(_context.VehicleShape.GetLast(), RenderSize, ActiveColor);
                    else
                        spriteBatch.DrawVertex(_context.VehicleShape.GetLast(), RenderSize, ActiveColor);
                }
            }
            else if (!_context.Pivot.HasValue)
            {
                if (IsMouseOver)
                {
                    RgbColor color = _context.SetPivotCommand.CanExecute(normalizedMousePosition)
                        ? ValidColor
                        : InvalidColor;

                    if (_context.AntialiasingEnabled)
                        spriteBatch.DrawVertexAA(normalizedMousePosition, RenderSize, color);
                    else
                        spriteBatch.DrawVertex(normalizedMousePosition, RenderSize, color);
                }
            }
            else if (!_context.Direction.HasValue)
            {
                if (IsMouseOver)
                {
                    double angle = GeometryHelper.GetAngle(_context.Pivot.Value, normalizedMousePosition);
                    RgbColor color = _context.SetDirectionCommand.CanExecute(angle)
                        ? ValidColor
                        : InvalidColor;

                    if (_context.AntialiasingEnabled)
                        spriteBatch.DrawArrowAA(_context.Pivot.Value, ArrowLength, angle, RenderSize, color);
                    else
                        spriteBatch.DrawArrow(_context.Pivot.Value, ArrowLength, angle, RenderSize, color);
                }
            }
            else
            {
                if (_context.AntialiasingEnabled)
                    spriteBatch.DrawArrowAA(_context.Pivot.Value, ArrowLength, _context.Direction.Value, RenderSize, RegularColor);
                else
                    spriteBatch.DrawArrow(_context.Pivot.Value, ArrowLength, _context.Direction.Value, RenderSize, RegularColor);
            }
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (!_context.ShapeDone)
            {
                if (_context.AntialiasingEnabled)
                    lockBitmap.DrawPathAA(new Path(_context.VehicleShape), RenderSize, ActiveColor);
                else
                    lockBitmap.DrawPath(new Path(_context.VehicleShape), RenderSize, ActiveColor);
            }
            else
            {
                if (_context.AntialiasingEnabled)
                    lockBitmap.DrawPolygonAA(new Polygon(_context.VehicleShape), RenderSize, RegularColor);
                else
                    lockBitmap.DrawPolygon(new Polygon(_context.VehicleShape), RenderSize, RegularColor);
            }

            if (_context.Pivot.HasValue)
            {
                RgbColor color = _context.Direction.HasValue ? RegularColor : ActiveColor;

                if (_context.AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.Pivot.Value, RenderSize, color);
                else
                    lockBitmap.DrawVertex(_context.Pivot.Value, RenderSize, color);
            }

            // Arrow is drawn dynamically only.
        }

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
