using System.Globalization;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using Infralution.Localization.Wpf;
using SRL.Commons;
using SRL.Main.Messages;

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

                InfoDialogMessage msg = new InfoDialogMessage();
                msg.Description = "Application must be restarted to change the language.";
                Messenger.Default.Send(msg);
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
