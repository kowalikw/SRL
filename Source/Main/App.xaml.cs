using System;
using System.Configuration;
using System.Windows;
using Infralution.Localization.Wpf;
using SRL.Main.View.Localization;
using SRL.Main;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CultureManager.UICulture = Settings.Default.Language.GetCultureInfo();
        }
    }
}
