using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
    /// <summary>
    /// Common class for all MonoGame controls embedded in the application.
    /// </summary>
    public abstract class AreaBase : MonoGameControl.MonoGameControl
    {
        /// <summary>
        /// Maximum distance to an object that triggers its special behavior if an action is performed close enough.
        /// </summary>
        protected const double VertexPullRadius = 8;

        /// <summary>
        /// Color of idle or "normal" objects (whatever that means in a given context).
        /// </summary>
        protected static readonly Color RegularColor = new Color(0, 0, 0);
        /// <summary>
        /// Color of objects that are currently manipulated in some way.
        /// </summary>
        protected static readonly Color ActiveColor = new Color(0, 0, 200);
        /// <summary>
        /// Color of invalid objects.
        /// </summary>
        protected static readonly Color InvalidColor = new Color(255, 0, 0);
        /// <summary>
        /// Color of valid objects.
        /// </summary>
        protected static readonly Color ValidColor = new Color(0, 220, 0);

        /// <summary>
        /// Gets the value that indicates whether objects are rendered with anti-aliasing.
        /// </summary>
        protected bool AntialiasingEnabled => Settings.Default.AntialiasingEnabled;

        private MouseButtonEventHandler _mouseUpHandler;
        private MouseButtonEventHandler _mouseDownHandler;
        private MouseEventHandler _mouseMoveHandler;
        private SizeChangedEventHandler _sizeChangedHandler;
        private PropertyChangedEventHandler _propertyChangedHandler;

        private SpriteBatch _spriteBatch;
        private Bitmap _bitmapBuffer;
        /// <summary>
        /// Texture that holds all non-moving and long-lasting elements and objects.
        /// </summary>
        protected Texture2D StaticObjectsTexture;


        /// <summary>
        /// Non-normalized cursor position (that is, in pixel space) relative to the MonoGameControl control.
        /// </summary>
        protected Point MousePosition;
        protected Color BackgroundColor = new Color(219, 240, 251);

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mouseUpHandler = (o, e) => OnMouseUp(e.ChangedButton);
            _mouseDownHandler = (o, e) => OnMouseButtonDown(e.ChangedButton);
            _mouseMoveHandler = (o, e) => MousePosition = e.GetPosition((UIElement)o);
            _sizeChangedHandler = (o, e) => OnSizeChanged();
            _propertyChangedHandler = (o, e) =>
            {
                if (e.PropertyName == nameof(Settings.Default.AntialiasingEnabled))
                    RedrawStaticObjectsTexture();
            };

            MouseUp += _mouseUpHandler;
            MouseDown += _mouseDownHandler;
            MouseMove += _mouseMoveHandler;
            SizeChanged += _sizeChangedHandler;
            Settings.Default.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            _spriteBatch.Dispose();

            MouseUp -= _mouseUpHandler;
            MouseDown -= _mouseDownHandler;
            MouseMove -= _mouseMoveHandler;
            SizeChanged -= _sizeChangedHandler;
            Settings.Default.PropertyChanged -= _propertyChangedHandler;
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
        /// Sets StaticObjectsTexture pixel data.
        /// </summary>
        protected void RedrawStaticObjectsTexture()
        {
            _bitmapBuffer = new Bitmap((int)RenderSize.Width, (int)RenderSize.Height, PixelFormat.Format32bppArgb);
            LockBitmap lockBitmap = new LockBitmap(_bitmapBuffer);
            lockBitmap.LockBits();
            RedrawStaticObjects(lockBitmap);
            lockBitmap.UnlockBits();
            StaticObjectsTexture.SetData(_bitmapBuffer.GetBytes());
        }

        /// <summary>
        /// Says whether current cursor position is close enough to an arbitrary <paramref name="point"/>  to trigger some special action.
        /// </summary>
        /// <param name="point">Non-normalized point.</param>
        /// <returns>True if cursor is close; false otherwise.</returns>
        protected bool IsMousePulledByPoint(Point point)
        {
            return GeometryHelper.GetDistance(MousePosition, point) <= VertexPullRadius;
        }

        /// <summary>
        /// Renders objects that often change or move and should be redrawn from scratch every time.
        /// </summary>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/> used in object rendering.</param>
        /// <param name="time">Time elapsed from the previous invocation.</param>
        protected abstract void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time);
        /// <summary>
        /// Renders objects that change relatively rarely.
        /// </summary>
        /// <param name="lockBitmap"><see cref="LockBitmap"/> used in object rendering.</param>
        protected abstract void RedrawStaticObjects(LockBitmap lockBitmap);

        /// <summary>
        /// Handles mouse button release.
        /// </summary>
        /// <param name="button">Released mouse button.</param>
        protected virtual void OnMouseUp(MouseButton button) { }
        /// <summary>
        /// Handles mouse button press.
        /// </summary>
        /// <param name="button">Pressed mouse button.</param>
        protected virtual void OnMouseButtonDown(MouseButton button) { }
        /// <summary>
        /// Handles screen resize.
        /// </summary>
        protected virtual void OnSizeChanged()
        {
            _bitmapBuffer = new Bitmap((int)RenderSize.Width, (int)RenderSize.Height);
            StaticObjectsTexture = new Texture2D(GraphicsDevice, (int)RenderSize.Width, (int)RenderSize.Height);

            RedrawStaticObjectsTexture();
        }
    }
}
