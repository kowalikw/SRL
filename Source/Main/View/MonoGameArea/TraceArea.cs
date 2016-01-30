using System;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    /// <summary>
    /// View class that contains UI logic for the MonoGame control that displays tracing result.
    /// </summary>
    internal class TraceArea : AreaBase
    {
        private readonly TracingViewModel _context = ServiceLocator.Current.GetInstance<TracingViewModel>();

        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

        protected override void Initialize()
        {
            base.Initialize();

            _collectionChangedHandler = (o, e) => RedrawStaticObjectsTexture();

            _context.Polygons.CollectionChanged += _collectionChangedHandler;
            _context.SelectedPolygonIndices.CollectionChanged += _collectionChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.Polygons.CollectionChanged -= _collectionChangedHandler;
            _context.SelectedPolygonIndices.CollectionChanged -= _collectionChangedHandler;
        }


        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            // *crickets*
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            for (int i = 0; i < _context.Polygons.Count; i++)
            {
                var color = _context.SelectedPolygonIndices.Contains(i) ?
                    ActiveColor : RegularColor;

                lockBitmap.DrawPolygon(_context.Polygons[i], RenderSize, color, AntialiasingEnabled);
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
