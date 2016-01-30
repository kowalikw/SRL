using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel.Base
{
    /// <summary>
    /// Base class for all view-models in the application.
    /// </summary>
    public abstract class ViewModel : ViewModelBase
    {
        /// <summary>
        /// Forces commands' update. <seealso cref="ICommand"/>.
        /// </summary>
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
