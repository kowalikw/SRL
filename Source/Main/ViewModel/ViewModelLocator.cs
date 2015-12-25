using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<MapEditorViewModel>();
            SimpleIoc.Default.Register<VehicleEditorViewModel>();
            SimpleIoc.Default.Register<TracingViewModel>();
            SimpleIoc.Default.Register<SimulationViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }
        public MapEditorViewModel MapEditor
        {
            get { return ServiceLocator.Current.GetInstance<MapEditorViewModel>(); }
        }
        public VehicleEditorViewModel VehicleEditor
        {
            get { return ServiceLocator.Current.GetInstance<VehicleEditorViewModel>(); }
        }
        public TracingViewModel Tracing
        {
            get { return ServiceLocator.Current.GetInstance<TracingViewModel>(); }
        }
        public SimulationViewModel Simulation
        {
            get { return ServiceLocator.Current.GetInstance<SimulationViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}