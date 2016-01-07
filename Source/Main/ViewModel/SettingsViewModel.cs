using System.Globalization;
using System.Reflection;
using Infralution.Localization.Wpf;
using SRL.Commons;

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
                CultureInfo cultureInfo = value.GetCultureInfo();
                try
                {
                    CultureManager.UICulture = cultureInfo;
                }
                catch (TargetInvocationException)
                {
                    /*
                    Yes, that's an actual exception supressing catch block. 
                    And yes, its presence makes us die inside too.
                    */

                    //TODO Come up with a proper solution.
                }
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
