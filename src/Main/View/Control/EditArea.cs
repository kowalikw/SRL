using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Utilities;
using SRL.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SRL.Main.View.Control
{
    public abstract class EditArea : MonoGameControl.MonoGameControl
    {
        protected const double VertexPullRadius = 8;

        protected const int VertexThickness = 2;
        protected const int LineThickness = 2;

        protected static readonly Color RegularColor = new Color(255, 255, 255);
        protected static readonly Color ActiveColor = Color.Yellow;
        protected static readonly Color HoverColor = new Color(255, 255, 20);
        protected static readonly Color InvalidColor = new Color(255, 20, 20);
        protected static readonly Color ValidColor = new Color(20, 255, 20);

        protected SrlPoint MousePosition { get; private set; }

        protected SpriteBatch SpriteBatch;

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            MouseUp += (o, e) =>
            {
                WinPoint position = e.GetPosition((UIElement) o);
                OnMouseUp((SrlPoint)position);
            };
            MouseDown += (o, e) =>
            {
                WinPoint position = e.GetPosition((UIElement)o);
                OnMouseDown((SrlPoint)position);
            };
            MouseMove += (o, e) =>
            {
                WinPoint position = e.GetPosition((UIElement)o);
                MousePosition = ((SrlPoint)position);
            };
        }

        protected override void Unitialize()
        {
            SpriteBatch.Dispose();
        }

        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(new Color(1, 47, 135));
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            SpriteBatch.BeginDraw();
            Render(SpriteBatch, time);
            SpriteBatch.EndDraw();
        }

        protected abstract void Render(SpriteBatch spriteBatch, TimeSpan time);
        
        protected abstract void OnMouseUp(SrlPoint position);
        protected abstract void OnMouseDown(SrlPoint position);

        protected bool IsMousePulledByPoint(SrlPoint point)
        {
            return GeometryHelper.GetDistance(MousePosition, point) <= VertexPullRadius;
        }

    }
}
