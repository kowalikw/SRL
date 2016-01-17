using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel.Base
{
    public abstract class ViewModel : ViewModelBase
    {
        protected void RaiseRequerySuggested()
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        protected ViewModel()
        {
            Messenger.Default.Register<CleanupMessage>(this, msg => Cleanup());
        }
    }
}
