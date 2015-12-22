using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model
{
    public class Simulation
    {
        public Map Map { get; set; }

        public Vehicle Vehicle { get; set; }

        public double VehicleSize { get; set; }

        public double InitialVehicleRotation { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
        
        public List<Order> Orders { get; set; }
    }
}
