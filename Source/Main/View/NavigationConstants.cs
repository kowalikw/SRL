using SRL.Main.View.Pages;

namespace SRL.Main.View
{
    internal static class NavigationConstants
    {
        private static readonly string pagesPath = "/View/Pages/";

        public static readonly string HomeViewPath = pagesPath + nameof(HomeView);
        public static readonly string MapEditorViewPath = pagesPath + nameof(MapEditorView);
        public static readonly string VehicleEditorViewPath = pagesPath + nameof(VehicleEditorView);
        public static readonly string TracingViewPath = pagesPath + nameof(TracingView);
        public static readonly string SettingsViewPath = pagesPath + nameof(SettingsView);
        public static readonly string SimulationViewPath = pagesPath + nameof(SimulationView);
    }
}
