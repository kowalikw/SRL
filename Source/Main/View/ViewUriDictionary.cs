using System;
using System.Collections.Generic;
using SRL.Main.View.Pages;

namespace SRL.Main.View
{
    internal static class ViewUriDictionary
    {
        //TODO

        private static readonly string pagesPath = "/View/Pages/";

        private static readonly Dictionary<Type, string> _dictionary = new Dictionary<Type, string>
        {
            {typeof(HomeView), pagesPath + nameof(HomeView)},
            {typeof(MapEditorView), pagesPath + nameof(MapEditorView)},
            {typeof(SettingsView), pagesPath + nameof(SettingsView)},
            {typeof(SimulationView), pagesPath + nameof(SimulationView)},
            {typeof(TracingView), pagesPath + nameof(TracingView)},
            {typeof(VehicleEditorView), pagesPath + nameof(VehicleEditorView)},
        };


    }
}
