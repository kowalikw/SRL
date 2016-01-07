using System;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel
{
    public class MainViewModel : Base.ViewModel
    {
        public MainViewModel()
        {
            Messenger.Default.Register<GoToPageMessage>(this, GoToPageHandler);
            Messenger.Default.Register<SaveFileDialogMessage>(this, SaveFileDialogHandler);
            Messenger.Default.Register<OpenFileDialogMessage>(this, OpenFileDialogHandler);
            Messenger.Default.Register<ErrorDialogMessage>(this, ErrorDialogHandler);
            Messenger.Default.Register<InfoDialogMessage>(this, InfoDialogHandler);
            //TODO Not sure if navigation belongs in the view-model. Keep it like this until a better idea comes up.
        }

        private void GoToPageHandler(GoToPageMessage msg)
        {
            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            Uri pageUri = (Uri)uriDictionary[msg.ViewType.Name];
            
            IInputElement target = NavigationHelper.FindFrame(NavigationHelper.FrameTop, Application.Current.MainWindow);
            NavigationCommands.GoToPage.Execute(pageUri, target);
        }

        private void SaveFileDialogHandler(SaveFileDialogMessage msg)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = msg.Filter;

            if (dialog.ShowDialog() == true)
                msg.FilenameCallback(dialog.FileName);
            else
                msg.FilenameCallback(null);
        }

        private void OpenFileDialogHandler(OpenFileDialogMessage msg)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = msg.Filter;

            if (dialog.ShowDialog() == true)
                msg.FilenameCallback(dialog.FileName);
            else
                msg.FilenameCallback(null);
        }

        private void ErrorDialogHandler(ErrorDialogMessage msg)
        {
            ModernDialog.ShowMessage(msg.ErrorDescription, "Error", MessageBoxButton.OK); //TODO dialog title UICulture dependant
        }

        private void InfoDialogHandler(InfoDialogMessage msg)
        {
            ModernDialog.ShowMessage(msg.Description, "Info", MessageBoxButton.OK); //TODO dialog title UICulture dependant
        }

    }
}