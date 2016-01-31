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
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            CultureManager.UICulture = Settings.Default.Language.GetCultureInfo();
        }
    }
}
