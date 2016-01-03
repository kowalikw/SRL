using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight;
using Infralution.Localization.Wpf;
using SRL.Main.View.Localization;

namespace SRL.Main.ViewModel
{
    public class SettingsViewModel : ViewModelBase
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
                CultureInfo cultureInfo = value.GetCultureInfo();
             //   CultureManager.UICulture = cultureInfo; //TODO solve this issue
                Settings.Default.Save();
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
