using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Windows.Point;

namespace SRL.Main.View.MonoGameArea
{
    public abstract class AreaBase : MonoGameControl.MonoGameControl
    {
        protected const double VertexPullRadius = 8;

        protected static readonly RgbColor RegularColor = new RgbColor(255, 255, 255);
        protected static readonly RgbColor ActiveColor = new RgbColor(20, 255, 255);
        protected static readonly RgbColor InvalidColor = new RgbColor(255, 20, 20);
        protected static readonly RgbColor ValidColor = new RgbColor(20, 255, 20);

        private MouseButtonEventHandler _mouseUpHandler;
        private MouseButtonEventHandler _mouseDownHandler;
        private MouseEventHandler _mouseMoveHandler;
        private SizeChangedEventHandler _sizeChangedHandler;

        private SpriteBatch _spriteBatch;
        private Bitmap _bitmapBuffer;
        protected Texture2D StaticObjectsTexture;

        /// <summary>
        /// Non-normalized cursor position (that is, in pixel space) relative to the MonoGameControl control.
        /// </summary>
        protected Point MousePosition;
        protected Color BackgroundColor = new Color(1, 47, 135);

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mouseUpHandler = (o, e) => OnMouseUp();
            _mouseDownHandler = (o, e) => OnMouseDown();
            _mouseMoveHandler = (o, e) => MousePosition = e.GetPosition((UIElement) o);
            _sizeChangedHandler = (o, e) => OnSizeChanged();

            MouseUp += _mouseUpHandler;
            MouseDown += _mouseDownHandler;
            MouseMove += _mouseMoveHandler;
            SizeChanged += _sizeChangedHandler;
        }

        protected override void Unitialize()
        {
            _spriteBatch.Dispose();

            MouseUp -= _mouseUpHandler;
            MouseDown -= _mouseDownHandler;
            MouseMove -= _mouseMoveHandler;
            SizeChanged -= _sizeChangedHandler;
        }

        protected sealed override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(BackgroundColor);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _spriteBatch.BeginDraw();
            _spriteBatch.Draw(StaticObjectsTexture, new Vector2(0, 0), Color.White);
            RenderDynamicObjects(_spriteBatch, time);
            _spriteBatch.EndDraw();
        }

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

        /// <summary>
        /// Says whether current cursor position is close enough to an arbitrary <paramref name="point"/> 
        /// to trigger some secondary action.
        /// </summary>
        /// <param name="point">Non-normalized point.</param>
        /// <returns>True if cursor is close; false otherwise.</returns>
        protected bool IsMousePulledByPoint(Point point)
        {
            return GeometryHelper.GetDistance(MousePosition, point) <= VertexPullRadius;
        }

        protected abstract void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time);
        protected abstract void RedrawStaticObjects(LockBitmap lockBitmap);

        protected virtual void OnMouseUp() { }
        protected virtual void OnMouseDown() { }
        protected virtual void OnSizeChanged()
        {
            _bitmapBuffer = new Bitmap((int)RenderSize.Width, (int)RenderSize.Height);
            StaticObjectsTexture = new Texture2D(GraphicsDevice, (int)RenderSize.Width, (int)RenderSize.Height);

            RedrawStaticObjectsTexture();
        }
    }
}
