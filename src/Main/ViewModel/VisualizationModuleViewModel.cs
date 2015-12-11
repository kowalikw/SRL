using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal class VisualizationModuleViewModel
    {
        public ICommand CalculatePathCommand { get; }

        public ICommand LoadSimulationCommand { get; }
        public ICommand LoadVehicleCommand { get; }
        public ICommand LoadMapCommand { get; }

        public ICommand SetVehicleRotation { get; }
        public ICommand SetStartpoint { get; }
        public ICommand SetEndpoint { get; }


        public uint CurrentTimeFrame { get; set; }
        public uint MaxTimeFrame => _maxTimeFrame;
        private const uint _maxTimeFrame = 8192 - 1;


        public Map Map { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double VehicleRotation { get; private set; }
        public Point Startpoint { get; private set; }
        public Point Endpoint { get; private set; }

        

        public VisualizationModuleViewModel()
        {
            
        }



    }
}
