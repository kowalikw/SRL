using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL.Model.Model
{
    public class Simulation
    {
        public Map Map { get; set; }
        public Vehicle Vehicle { get; set; }
        public double InitialVehicleRotation { get; set; }
        
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public List<Order> Orders { get; set; } 
    }
}
