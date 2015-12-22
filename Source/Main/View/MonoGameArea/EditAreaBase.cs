using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using Color = Microsoft.Xna.Framework.Color;

namespace SRL.Main.View.MonoGameArea
{
    internal abstract class EditAreaBase : AreaBase
    {
        protected static readonly RgbColor RegularColor = new RgbColor(255, 255, 255);
        protected static readonly RgbColor ActiveColor = new RgbColor(20,255,255);
        protected static readonly RgbColor InvalidColor = new RgbColor(255, 20, 20);
        protected static readonly RgbColor ValidColor = new RgbColor(20, 255, 20);

        private Bitmap _bitmapBuffer;
        protected Texture2D StaticObjectsTexture;

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            spriteBatch.Draw(StaticObjectsTexture, new Vector2(0, 0), Color.White);
            RenderDynamicObjects(spriteBatch, time);
        }

        protected abstract void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time);

        /// <summary>
        /// Sets StaticObjectsTexture pixel data based on StaticDrawables list.
        /// </summary>
        protected void RedrawStaticObjectsTexture()
        {
            _bitmapBuffer = new Bitmap((int)RenderSize.Width, (int)RenderSize.Height);
            LockBitmap lockBitmap = new LockBitmap(_bitmapBuffer);
            lockBitmap.LockBits();
            RedrawStaticObjects(lockBitmap);
            lockBitmap.UnlockBits();
            StaticObjectsTexture.SetData(_bitmapBuffer.GetBytes());
        }

        protected abstract void RedrawStaticObjects(LockBitmap lockBitmap);

        protected sealed override void OnSizeChanged()
        {
            _bitmapBuffer = new Bitmap((int)RenderSize.Width, (int)RenderSize.Height);
            StaticObjectsTexture = new Texture2D(GraphicsDevice, (int)RenderSize.Width, (int)RenderSize.Height);
            
            RedrawStaticObjectsTexture();
        }
    }
}
