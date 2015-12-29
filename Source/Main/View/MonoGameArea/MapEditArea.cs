using System;
using System.Collections.Specialized;
using System.ComponentModel;
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
    internal class MapEditArea : AreaBase
    {
        private readonly MapEditorViewModel _context = SimpleIoc.Default.GetInstance<MapEditorViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

        private readonly Line _activeLine = new Line();

        protected override void Initialize()
        {
            base.Initialize();

            _collectionChangedHandler = (o, e) => RedrawStaticObjectsTexture();

            _context.FinishedPolygons.CollectionChanged += _collectionChangedHandler;
            _context.UnfinishedPolygon.CollectionChanged += _collectionChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.FinishedPolygons.CollectionChanged -= _collectionChangedHandler;
            _context.UnfinishedPolygon.CollectionChanged -= _collectionChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            Point normalizedMousePosition = MousePosition.Normalize(RenderSize);

            if (_context.UnfinishedPolygon.Count > 0 && IsMouseOver)
            {
                
                RgbColor color = _context.AddVertexCommand.CanExecute(normalizedMousePosition) ?
                    ValidColor : InvalidColor;

                _activeLine.EndpointA = _context.UnfinishedPolygon.GetLast();
                _activeLine.EndpointB = normalizedMousePosition;

                if (AntialiasingEnabled)
                    spriteBatch.DrawLineAA(_activeLine, RenderSize, color);
                else
                    spriteBatch.DrawLine(_activeLine, RenderSize, color);
            }
            else if (_context.UnfinishedPolygon.Count == 1)
            {
                if (AntialiasingEnabled)
                    spriteBatch.DrawVertexAA(_context.UnfinishedPolygon.GetLast(), RenderSize, ActiveColor);
                else
                    spriteBatch.DrawVertex(_context.UnfinishedPolygon.GetLast(), RenderSize, ActiveColor);
            }
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (AntialiasingEnabled)
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

        protected override void OnMouseUp(MouseButton button)
        {
            if (button == MouseButton.Left)
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
            else if (button == MouseButton.Right)
            {
                if (_context.BackCommand.CanExecute(null))
                    _context.BackCommand.Execute(null);
            }
        }
    }
}
