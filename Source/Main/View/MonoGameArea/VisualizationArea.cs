using System;
using System.ComponentModel;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        private readonly SimulationViewModel _context = SimpleIoc.Default.GetInstance<SimulationViewModel>();
        private PropertyChangedEventHandler _propertyChangedHandler;

        protected override void Initialize()
        {
            base.Initialize();

            _propertyChangedHandler = (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_context.Map):
                    case nameof(_context.StartPoint):
                    case nameof(_context.EndPoint):
                        RedrawStaticObjectsTexture();
                        break;
                }
            };

            _context.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.PropertyChanged -= _propertyChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            if (_context.CurrentFrame != null)
            {

                return;
            }
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.Map != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawMapAA(_context.Map, RenderSize, RegularColor);
                else
                    lockBitmap.DrawMap(_context.Map, RenderSize, RegularColor);
            }
            if (_context.StartPoint.HasValue)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.StartPoint.Value, RenderSize, InvalidColor); // TODO Some other color
                else
                    lockBitmap.DrawVertex(_context.StartPoint.Value, RenderSize, InvalidColor); // TODO Some other color
            }
            if (_context.EndPoint.HasValue)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.EndPoint.Value, RenderSize, InvalidColor); // TODO Some other color
                else
                    lockBitmap.DrawVertex(_context.EndPoint.Value, RenderSize, InvalidColor); // TODO Some other color
            }
        }
    }
}
