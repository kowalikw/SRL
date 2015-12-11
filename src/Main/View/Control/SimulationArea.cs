using System;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model.Model;

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
            
            throw new NotImplementedException();
        }

        protected override void OnMouseUp(Point position)
        {
            //throw new NotImplementedException();
        }

        
    }
}
