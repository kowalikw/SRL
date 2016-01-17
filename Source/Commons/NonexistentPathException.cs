using System;
using System.Windows;
using SRL.Commons.Model;

namespace SRL.Commons
{
    public class NonexistentPathException : Exception
    {
        public Map Map { get; set; }
        public Vehicle Vehicle { get; set; }
        public Point PathStart { get; set; }
        public Point PathEnd { get; set; }
        public double VehicleSize { get; set; }
        public double VehicleRotation { get; set; }
    }
}
