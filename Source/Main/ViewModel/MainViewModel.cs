using System;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using SRL.Main.Messages;
using SRL.Main.View.Dialogs;

namespace SRL.Main.ViewModel
{
    internal class MainViewModel : Base.ViewModel
    {
        private IDialogService _dialogService;

        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Messenger.Default.Register<GoToPageMessage>(this, GoToPageMessageHandler);
            Messenger.Default.Register<ShowDialogMessage>(this, ShowDialogMessageHandler);

            //TODO Not sure if navigation belongs in the view-model. Keep it like this until a better idea comes up.
        }

        private void GoToPageMessageHandler(GoToPageMessage msg)
        {
            var uriDictionary = (ResourceDictionary)Application.Current.Resources["UriDictionary"];
            Uri pageUri = (Uri)uriDictionary[msg.ViewType.Name];

            IInputElement target = NavigationHelper.FindFrame(NavigationHelper.FrameTop, Application.Current.MainWindow);
            NavigationCommands.GoToPage.Execute(pageUri, target);
        }

        private void ShowDialogMessageHandler(ShowDialogMessage msg)
        {
            switch (msg.Args.DialogType)
            {
                case DialogArgs.Type.OpenFileDialog:
                    {
                        var args = (OpenFileDialogArgs)msg.Args;
                        string filename = null;
                        _dialogService.ShowOpenFileDialog(args.Filter, out filename, result =>
                        {
                            args.CloseCallback(result, filename);
                        });
                        break;
                    }
                case DialogArgs.Type.SaveFileDialog:
                    {
                        var args = (SaveFileDialogArgs)msg.Args;
                        string filename = null;
                        _dialogService.ShowSaveFileDialog(args.Filter, out filename, result =>
                        {
                            args.CloseCallback(result, filename);
                        });
                        break;
                    }
                case DialogArgs.Type.OptionsDialog:
                    {
                        var args = (OptionsDialogArgs)msg.Args;
                        _dialogService.ShowOptionsDialog(args.Options, args.CloseCallback);
                        break;
                    }
                case DialogArgs.Type.MessageDialog:
                    {
                        var args = (MessageDialogArgs)msg.Args;
                        _dialogService.ShowMessageDialog(args.Title, args.Description, args.CloseCallback);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}