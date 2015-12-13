using System;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model.Model;
using SRL.Main.Utilities;
using Microsoft.Xna.Framework;
using Point = SRL.Model.Model.Point;
using System.Windows;

namespace SRL.Main.View.Control
{
    public class SimulationArea : EditArea
    {
        private VisualizationModuleViewModel _context;
        int i = 0;

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

            //if(i < _context.fr.Count)
                spriteBatch.DrawFrame(_context.CurrentFrame, _context.Vehicle, _context.Map, _context.orders[_context.i % _context.orders.Count]);

            for(int i = 1; i < _context.orders.Count; i++)
                spriteBatch.DrawLine(_context.orders[i - 1].Destination, _context.orders[i].Destination, Color.Red);
            spriteBatch.DrawLine(_context.Startpoint, _context.orders[0].Destination, Color.Red);
            spriteBatch.DrawLine(_context.Endpoint, _context.orders[_context.orders.Count - 1].Destination, Color.Red);

            //i++;
            //MessageBox.Show("ok");
        }

        protected override void OnMouseUp(Point position)
        {
            //throw new NotImplementedException();
        }

        
    }
}
