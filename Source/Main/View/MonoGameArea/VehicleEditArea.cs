using System;
using System.Collections.Specialized;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VehicleEditArea : EditAreaBase
    {
        private readonly VehicleEditorViewModel _context = SimpleIoc.Default.GetInstance<VehicleEditorViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

        private readonly Line _activeLine = new Line();

        protected override void Initialize()
        {
            base.Initialize();
            _collectionChangedHandler = (o, e) => RedrawStaticObjectsTexture();

            _context.VehicleShape.CollectionChanged += _collectionChangedHandler;
            //TODO antialiasing enabled/disabled event handler
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.VehicleShape.CollectionChanged -= _collectionChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            if (!_context.ShapeDone && _context.VehicleShape.Count > 0)
            {
                Point normalizedMousePosition = MousePosition.Normalize(RenderSize);
                RgbColor color = _context.AddShapeVertexCommand.CanExecute(normalizedMousePosition) ?
                    ValidColor : InvalidColor;

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
                    lockBitmap.DrawPathAA(new Path(_context.VehicleShape), RenderSize, RegularColor);
                else
                    lockBitmap.DrawPath(new Path(_context.VehicleShape), RenderSize, RegularColor);
            }
            else
            {
                if (_context.AntialiasingEnabled)
                    lockBitmap.DrawPolygonAA(new Polygon(_context.VehicleShape), RenderSize, RegularColor);
                else
                    lockBitmap.DrawPolygon(new Polygon(_context.VehicleShape), RenderSize, RegularColor);
            }
        }

        protected override void OnMouseUp()
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
    }
}
