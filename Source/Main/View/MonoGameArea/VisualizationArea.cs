using System;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        private readonly SimulationViewModel _context = SimpleIoc.Default.GetInstance<SimulationViewModel>();

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            //throw new NotImplementedException(); TODO
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.Map != null)
                lockBitmap.DrawMap(_context.Map, RenderSize, RegularColor);

            
        }
    }
}
