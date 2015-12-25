using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight;

namespace SRL.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {

        }

        private void SetPage<T>() where T : UserControl
        {
            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            Uri pageUri = (Uri)uriDictionary[nameof(T)];

            IInputElement target = NavigationHelper.FindFrame(NavigationHelper.FrameTop, Application.Current.MainWindow);
            NavigationCommands.GoToPage.Execute(pageUri, target);
        }
    }
}