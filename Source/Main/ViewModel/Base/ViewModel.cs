using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace SRL.Main.ViewModel.Base
{
    public abstract class ViewModel : ViewModelBase
    {
        protected void RaiseRequerySuggested()
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }
    }
}
