using System;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model.Model;
using SRL.Main.Utilities;

namespace SRL.Main.View.Control
{
    public class SimulationArea : EditArea
    {
        private VisualizationModuleViewModel _context;

        protected override void Initialize()
        {
            base.Initialize();
            _context = (VisualizationModuleViewModel) DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            //spriteBatch.DrawMap(_context.Map);

            //spriteBatch.DrawVehicle(_context.Vehicle);

            //._context.CurrentFrame.

            //throw new NotImplementedException();

            spriteBatch.DrawFrame(_context.CurrentFrame, _context.Vehicle, _context.Map);
        }

        protected override void OnMouseUp(Point position)
        {
            //throw new NotImplementedException();
        }

        
    }
}
