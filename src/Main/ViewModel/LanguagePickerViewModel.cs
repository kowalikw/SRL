using System.Windows;
using System.Windows.Input;
using SRL.Main.Model;
using SRL.Main.View;

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
                Window mainWindow = new MainMenuView(Language.Polish);
                mainWindow.Show();

                //TODO close LanguagePickerView window.
            });
            
            EnglishLanguageCommand = new RelayCommand(o =>
            {
                Window mainWindow = new MainMenuView(Language.English);
                mainWindow.Show();

                //TODO close LanguagePickerView window.
            });
        }
    }
}
