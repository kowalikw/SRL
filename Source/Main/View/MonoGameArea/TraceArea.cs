using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class TraceArea : AreaBase
    {
        private readonly TracingViewModel _context = SimpleIoc.Default.GetInstance<TracingViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;
        private PropertyChangedEventHandler _propertyChangedHandler;

        protected override void Initialize()
        {
            base.Initialize();

            _collectionChangedHandler = (o, e) => RedrawStaticObjectsTexture();
            _propertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(_context.AntialiasingEnabled))
                    RedrawStaticObjectsTexture();
            };

            _context.Polygons.CollectionChanged += _collectionChangedHandler;
            _context.SelectedPolygonIndices.CollectionChanged += _collectionChangedHandler;
            _context.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.Polygons.CollectionChanged -= _collectionChangedHandler;
            _context.SelectedPolygonIndices.CollectionChanged -= _collectionChangedHandler;
            _context.PropertyChanged -= _propertyChangedHandler;
        }


        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            // *crickets*
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.AntialiasingEnabled)
            {
                for (int i = 0; i < _context.Polygons.Count; i++)
                {
                    var color = _context.SelectedPolygonIndices.Contains(i) ? 
                        ActiveColor : RegularColor;

                    lockBitmap.DrawPolygonAA(_context.Polygons[i], RenderSize, color);
                }
            }
            else
            {
                for (int i = 0; i < _context.Polygons.Count; i++)
                {
                    var color = _context.SelectedPolygonIndices.Contains(i) ?
                        ActiveColor : RegularColor;

                    lockBitmap.DrawPolygon(_context.Polygons[i], RenderSize, color);
                }
            }
        }

        protected override void OnMouseUp(MouseButton button)
        {
            var normalizedMousePosition = MousePosition.Normalize(RenderSize);

            if (button == MouseButton.Left)
            {
                if (_context.SelectPolygonCommand.CanExecute(normalizedMousePosition))
                    _context.SelectPolygonCommand.Execute(normalizedMousePosition);
            }
            else if (button == MouseButton.Right)
            {
                if (_context.DeselectPolygonCommand.CanExecute(normalizedMousePosition))
                    _context.DeselectPolygonCommand.Execute(normalizedMousePosition);
            }
        }
    }
}
