using System;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Messenger.Default.Register<GoToPageMessage>(this, msg => SetPage(msg.ViewType));
        }

        private void SetPage(Type viewType)
        {
            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            Uri pageUri = (Uri)uriDictionary[viewType.Name];

            IInputElement target = NavigationHelper.FindFrame(NavigationHelper.FrameTop, Application.Current.MainWindow);
            NavigationCommands.GoToPage.Execute(pageUri, target);
        }
    }
}