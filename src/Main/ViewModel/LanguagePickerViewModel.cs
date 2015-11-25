using System.Windows;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Main.View;
using SRL.Main.View.Language;

namespace SRL.Main.ViewModel
{
    internal class LanguagePickerViewModel : CloseableViewModel
    {
        public ICommand PolishLanguageCommand { get; }
        public ICommand EnglishLanguageCommand { get; }

        public LanguagePickerViewModel()
        {
            PolishLanguageCommand = new RelayCommand(o =>
            {
                Window window = new MainMenuView(Language.Polish);
                window.Show();

                OnClosingRequest();
            });
            
            EnglishLanguageCommand = new RelayCommand(o =>
            {
                Window window = new MainMenuView(Language.English);
                window.Show();

                OnClosingRequest();
            });
        }
    }
}
