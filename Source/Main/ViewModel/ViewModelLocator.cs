using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using SRL.Commons.Model.Base;
using SRL.Main.Messages;
using SRL.Main.View.Pages;
using SRL.Main.ViewModel.Services;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
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

            SimpleIoc.Default.Register<INavigationService>(NavigationServiceFactory);
            SimpleIoc.Default.Register<IDialogService>(DialogServiceFactory);

            var algorithm = new Algorithm.Algorithm();
            SimpleIoc.Default.Register<IAlgorithm>(() => algorithm, algorithm.Key);
            SimpleIoc.Default.Register<IAlgorithm>(() => algorithm); // default algorithm
        }

        /// <summary>
        /// Gets the registered <see cref="MainViewModel"/> instance.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
        /// <summary>
        /// Gets the registered <see cref="SettingsViewModel"/> instance.
        /// </summary>
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }
        /// <summary>
        /// Gets the registered <see cref="MapEditorViewModel"/> instance.
        /// </summary>
        public MapEditorViewModel MapEditor
        {
            get { return ServiceLocator.Current.GetInstance<MapEditorViewModel>(); }
        }
        /// <summary>
        /// Gets the registered <see cref="VehicleEditorViewModel"/> instance.
        /// </summary>
        public VehicleEditorViewModel VehicleEditor
        {
            get { return ServiceLocator.Current.GetInstance<VehicleEditorViewModel>(); }
        }
        /// <summary>
        /// Gets the registered <see cref="TracingViewModel"/> instance.
        /// </summary>
        public TracingViewModel Tracing
        {
            get { return ServiceLocator.Current.GetInstance<TracingViewModel>(); }
        }
        /// <summary>
        /// Gets the registered <see cref="SimulationViewModel"/> instance.
        /// </summary>
        public SimulationViewModel Simulation
        {
            get { return ServiceLocator.Current.GetInstance<SimulationViewModel>(); }
        }

        private INavigationService NavigationServiceFactory()
        {
            var nav = new NavigationService();

            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            
            nav.Configure(nameof(HomeView), (Uri)uriDictionary[nameof(HomeView)]);
            nav.Configure(nameof(MapEditorView), (Uri)uriDictionary[nameof(MapEditorView)]);
            nav.Configure(nameof(SettingsView), (Uri)uriDictionary[nameof(SettingsView)]);
            nav.Configure(nameof(SimulationView), (Uri)uriDictionary[nameof(SimulationView)]);
            nav.Configure(nameof(TracingView), (Uri)uriDictionary[nameof(TracingView)]);
            nav.Configure(nameof(VehicleEditorView), (Uri)uriDictionary[nameof(VehicleEditorView)]);

            return nav;
        }

        private IDialogService DialogServiceFactory()
        {
            var srv = new DialogService(Application.Current.MainWindow);
            return srv;
        }

        /// <summary>
        /// Cleans up registered view-models and resets the service container.
        /// </summary>
        public static void Cleanup()
        {
            Messenger.Default.Send(new CleanupMessage());

            SimpleIoc.Default.Reset();
        }
    }
}