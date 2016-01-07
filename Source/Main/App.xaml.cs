using System.Windows;
using Infralution.Localization.Wpf;
using SRL.Commons;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Language UILanguage { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            UILanguage = Settings.Default.Language;
            CultureManager.UICulture = UILanguage.GetCultureInfo();
        }
    }
}
