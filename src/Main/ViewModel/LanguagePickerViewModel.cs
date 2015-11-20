using System.Windows;
using System.Windows.Input;
using SRL.Main.View;
using SRL.Main.View.Language;

namespace SRL.Main.ViewModel
{
    internal class LanguagePickerViewModel
    {
        public ICommand PolishLanguageCommand { get; }
        public ICommand EnglishLanguageCommand { get; }

        public LanguagePickerViewModel()
        {
            PolishLanguageCommand = new RelayCommand(o =>
            {
                Window window = new MainMenuView(Language.Polish);
                window.Show();

                //TODO close LanguagePickerView window.
            });
            
            EnglishLanguageCommand = new RelayCommand(o =>
            {
                Window window = new MainMenuView(Language.English);
                window.Show();

                //TODO close LanguagePickerView window.
            });
        }
    }
}
