using GalaSoft.MvvmLight.Ioc;
using Infralution.Localization.Wpf;
using Microsoft.Practices.ServiceLocation;
using SRL.Commons;
using SRL.Main.View.Localization;
using SRL.Main.ViewModel.Services;

namespace SRL.Main.ViewModel
{
    public class SettingsViewModel : Base.ViewModel
    {
        public bool AntialiasingEnabled
        {
            get
            {
                return Settings.Default.AntialiasingEnabled;
            }
            set
            {
                Settings.Default.AntialiasingEnabled = value;
                Settings.Default.Save();
            }
        }

        public Language Language
        {
            get
            {
                return Settings.Default.Language;
            }
            set
            {
                Settings.Default.Language = value;
                Settings.Default.Save();

                if (CultureManager.UICulture.GetLanguage() != value)
                {
                    ServiceLocator.Current.GetInstance<IDialogService>().ShowMessageDialog(
                        Dialogs.ResourceManager.GetString("langChangeRestartTitle"),
                        Dialogs.ResourceManager.GetString("langChangeRestartMsg"),
                        null);
                }
            }
        }

        public bool ShowPath
        {
            get
            {
                return Settings.Default.ShowPath;
            }
            set
            {
                Settings.Default.ShowPath = value;
                Settings.Default.Save();
            }
        }
    }
}
