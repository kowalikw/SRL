using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class MapEditArea : EditAreaBase
    {
        private readonly MapEditorViewModel _context = SimpleIoc.Default.GetInstance<MapEditorViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;
        private PropertyChangedEventHandler _antialiasingPropertyChangedHandler;

        private readonly Line _activeLine = new Line();

        protected override void Initialize()
        {
            base.Initialize();

            _collectionChangedHandler = (o, e) => RedrawStaticObjectsTexture();
            _antialiasingPropertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.AntialiasingEnabled))
                    RedrawStaticObjectsTexture();
            };

            _context.FinishedPolygons.CollectionChanged += _collectionChangedHandler;
            _context.UnfinishedPolygon.CollectionChanged += _collectionChangedHandler;
            _context.PropertyChanged += _antialiasingPropertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.FinishedPolygons.CollectionChanged -= _collectionChangedHandler;
            _context.UnfinishedPolygon.CollectionChanged -= _collectionChangedHandler;
            _context.PropertyChanged -= _antialiasingPropertyChangedHandler;
        }

        protected override void OnMouseUp()
        {
            if (_context.FinishPolygonCommand.CanExecute(null))
            {
                var denormalizedEndpoint = _context.UnfinishedPolygon[0].Denormalize(RenderSize);
                if (IsMousePulledByPoint(denormalizedEndpoint))
                {
                    _context.FinishPolygonCommand.Execute(null);
                    return;
                }
            }

            var normalizedMousePosition = MousePosition.Normalize(RenderSize);
            if (_context.AddVertexCommand.CanExecute(normalizedMousePosition))
                _context.AddVertexCommand.Execute(normalizedMousePosition);
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            // Render active segment (potential polygon side).
            if (_context.UnfinishedPolygon.Count > 0 && IsMouseOver)
            {
                Point normalizedMousePosition = MousePosition.Normalize(RenderSize);
                RgbColor color = _context.AddVertexCommand.CanExecute(normalizedMousePosition) ?
                    ValidColor : InvalidColor;

                _activeLine.EndpointA = _context.UnfinishedPolygon.GetLast();
                _activeLine.EndpointB = normalizedMousePosition;

                if (_context.AntialiasingEnabled)
                    spriteBatch.DrawLineAA(_activeLine, RenderSize, color);
                else
                    spriteBatch.DrawLine(_activeLine, RenderSize, color);
            }
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.AntialiasingEnabled)
            {
                foreach (var obstacle in _context.FinishedPolygons)
                    lockBitmap.DrawPolygonAA(obstacle, RenderSize, RegularColor);
                lockBitmap.DrawPathAA(new Path(_context.UnfinishedPolygon), RenderSize, ActiveColor);
            }
            else
            {
                foreach (var obstacle in _context.FinishedPolygons)
                    lockBitmap.DrawPolygon(obstacle, RenderSize, RegularColor);
                lockBitmap.DrawPath(new Path(_context.UnfinishedPolygon), RenderSize, ActiveColor);
            }
        }
    }
}
