using System;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using SRL.Main.Messages;
using SRL.Main.View.Dialogs;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ModernWindow
    {
        private IDialogService _dialogService;

        public MainView()
        {
            InitializeComponent();
            _dialogService = new DialogService(this);

            Messenger.Default.Register<GoToPageMessage>(this, GoToPageMessageHandler);
            Messenger.Default.Register<ShowDialogMessage>(this, ShowDialogMessageHandler);
        }

        private void GoToPageMessageHandler(GoToPageMessage msg)
        {
            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            Uri pageUri = (Uri)uriDictionary[msg.ViewType.Name];

            IInputElement target = NavigationHelper.FindFrame(NavigationHelper.FrameTop, this);
            NavigationCommands.GoToPage.Execute(pageUri, target);
        }

        private void ShowDialogMessageHandler(ShowDialogMessage msg)
        {
            _dialogService.ShowDialog(msg.Args);
        }
    }
}
