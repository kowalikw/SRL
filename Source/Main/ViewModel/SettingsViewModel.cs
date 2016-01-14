using System.Globalization;
using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight.Messaging;
using Infralution.Localization.Wpf;
using SRL.Commons;
using SRL.Main.Messages;
using SRL.Main.View.Dialogs;
using SRL.Main.View.Localization;

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
                    var args = new MessageDialogArgs();
                    args.Title = Dialogs.ResourceManager.GetString("langChangeRestartTitle");
                    args.Description = Dialogs.ResourceManager.GetString("langChangeRestartMsg");
                    Messenger.Default.Send(new ShowDialogMessage(args));
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
