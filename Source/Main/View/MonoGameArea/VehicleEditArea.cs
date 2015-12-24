using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VehicleEditArea : AreaBase
    {
        private readonly VehicleEditorViewModel _context = SimpleIoc.Default.GetInstance<VehicleEditorViewModel>();

        private NotifyCollectionChangedEventHandler _vehicleShapeChangedHandler;
        private PropertyChangedEventHandler _shapeDonePropertyChangedHandler;
        private PropertyChangedEventHandler _antialiasingPropertyChangedHandler;

        private readonly Line _activeLine = new Line();

        protected override void Initialize()
        {
            base.Initialize();
            _vehicleShapeChangedHandler = (o, e) => RedrawStaticObjectsTexture();
            _shapeDonePropertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.ShapeDone))
                    RedrawStaticObjectsTexture();
            };
            _antialiasingPropertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.AntialiasingEnabled))
                    RedrawStaticObjectsTexture();
            };

            _context.VehicleShape.CollectionChanged += _vehicleShapeChangedHandler;
            _context.PropertyChanged += _shapeDonePropertyChangedHandler;
            _context.PropertyChanged += _antialiasingPropertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.VehicleShape.CollectionChanged -= _vehicleShapeChangedHandler;
            _context.PropertyChanged -= _shapeDonePropertyChangedHandler;
            _context.PropertyChanged -= _antialiasingPropertyChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            if (!_context.ShapeDone && _context.VehicleShape.Count > 0 & IsMouseOver)
            {
                Point normalizedMousePosition = MousePosition.Normalize(RenderSize);
                RgbColor color = _context.AddShapeVertexCommand.CanExecute(normalizedMousePosition)
                    ? ValidColor
                    : InvalidColor;

                _activeLine.EndpointA = _context.VehicleShape.GetLast();
                _activeLine.EndpointB = normalizedMousePosition;

                if (_context.AntialiasingEnabled)
                    spriteBatch.DrawLineAA(_activeLine, RenderSize, color);
                else
                    spriteBatch.DrawLine(_activeLine, RenderSize, color);

                return;
            }
            
            if (_context.Direction.HasValue)
            {
                //TODO draw arrow
            }
            if (_context.Pivot.HasValue)
            {
                //TODO draw vertex
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
        }

        protected override void OnMouseUp(MouseButton button)
        {
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

                    var normalizedMousePosition = MousePosition.Normalize(RenderSize);
                    if (_context.AddShapeVertexCommand.CanExecute(normalizedMousePosition))
                        _context.AddShapeVertexCommand.Execute(normalizedMousePosition);
                }
                else if (!_context.Pivot.HasValue)
                {
                    //TODO
                }
                else if (!_context.Direction.HasValue)
                {
                    //TODO
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
