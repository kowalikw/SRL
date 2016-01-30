using Infralution.Localization.Wpf;
using Microsoft.Practices.ServiceLocation;
using SRL.Commons;
using SRL.Main.View.Localization;
using SRL.Main.ViewModel.Services;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// View-model class that contains non-UI logic for the settings tab.
    /// </summary>
    public class SettingsViewModel : Base.ViewModel
    {
        /// <summary>
        /// Enables and disables anti-aliasing in MonoGame controls.
        /// </summary>
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

        /// <summary>
        /// Gets and sets UI language.
        /// </summary>
        /// <remarks>Value change applies after application restart.</remarks>
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

        /// <summary>
        /// Enables and disables vehicle path drawing during simulation.
        /// </summary>
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
