using System;
using System.Collections.Specialized;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using Point = System.Windows.Point;

namespace SRL.Main.View.MonoGameArea
{
    internal class MapEditArea : AreaBase
    {
        private readonly MapEditorViewModel _context = SimpleIoc.Default.GetInstance<MapEditorViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

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
                Color color = _context.AddVertexCommand.CanExecute(normalizedMousePosition) ?
                    ValidColor : InvalidColor;

                // Draw active line.
                Point endpointA = _context.UnfinishedPolygon.GetLast();
                Point endpointB = normalizedMousePosition;
                spriteBatch.DrawLine(endpointA, endpointB, RenderSize, color, AntialiasingEnabled);
            }
            else if (_context.UnfinishedPolygon.Count == 1)
            {
                spriteBatch.DrawVertex(_context.UnfinishedPolygon.GetLast(), RenderSize, ActiveColor, AntialiasingEnabled);
            }
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            foreach (var obstacle in _context.FinishedPolygons)
                lockBitmap.DrawPolygon(obstacle, RenderSize, RegularColor, AntialiasingEnabled);
            lockBitmap.DrawPath(_context.UnfinishedPolygon, RenderSize, ActiveColor, AntialiasingEnabled);
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
