using System;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        private readonly MapEditorViewModel _context = SimpleIoc.Default.GetInstance<MapEditorViewModel>();

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            throw new NotImplementedException();
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            throw new NotImplementedException();
        }
    }
}
