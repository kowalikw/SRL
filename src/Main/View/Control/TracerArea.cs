using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using SRL.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;
using EditingStage = SRL.Main.ViewModel.VehicleEditorViewModel.EditingStage;
using SRL.Model.Model;

namespace SRL.Main.View.Control
{
    public class TracerArea : EditArea
    {
        private TracingViewModel _context;

        protected override void Initialize()
        {
            base.Initialize();
            _context = (TracingViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            foreach (var polygon in _context.CurrentModel)
            {
                if (GeometryHelper.IsPointInPolygon(MousePosition, polygon))
                    spriteBatch.DrawPolygon(polygon, EditArea.HoverColor, EditArea.LineThickness);
                else if (_context.CurrentShape.Equals(polygon))
                    spriteBatch.DrawPolygon(polygon, EditArea.ActiveColor, EditArea.LineThickness);
                else
                    spriteBatch.DrawPolygon(polygon, EditArea.RegularColor, EditArea.LineThickness);
            }

            /*for(int i = 0; i < 255; i++)
            {
                spriteBatch.PutPixelIntense(i, 100, (float)((float)i / (float)255));
            }

            spriteBatch.DrawLine(new SrlPoint(100, 100), new SrlPoint(300, 187), Color.AliceBlue, 1);*/

            /*Game game = new Game();

            GraphicsDeviceManager device = new GraphicsDeviceManager(game);
            device.PreferMultiSampling = true;*/

            /*BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            VertexPositionColor[] vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(+0.5f, 0, 0), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 0, 0), Color.Blue);
            vertices[3] = new VertexPositionColor(new Vector3(+0.5f, 0.9f, 0), Color.Blue);

            VertexBuffer vertexBuffer = new VertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 3);
            }*/

            /*Texture2D _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(_pixel, new Vector2(100, 100), Color.Red);*/

            //GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 3);
        }

        protected override void OnMouseUp(SrlPoint position)
        {
            foreach (var polygon in _context.CurrentModel)
                if(GeometryHelper.IsPointInPolygon(MousePosition, polygon))
                    _context.CurrentShape = polygon;
        }
    }
}