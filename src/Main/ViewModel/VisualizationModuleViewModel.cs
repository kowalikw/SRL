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
        ICommand CalculatePathCommand { get; }

        ICommand LoadSimulationCommand { get; }
        ICommand LoadVehicleCommand { get; }
        ICommand LoadMapCommand { get; }

        ICommand SetVehicleRotation { get; }
        ICommand SetStartpoint { get; }
        ICommand SetEndpoint { get; }

        



        public Map Map { get; set; }
        public Vehicle Vehicle { get; set; }
        
        

        public VisualizationModuleViewModel()
        {

        }



    }
}
