using System;
using System.Collections.Concurrent;
using System.Windows.Input;

namespace SRL.Main.ViewModel.Services
{
    internal class NavigationService : INavigationService
    {
        private readonly ConcurrentDictionary<string, Uri> _uris = new ConcurrentDictionary<string, Uri>();

        public void Configure(string pageKey, Uri pageUri)
        {
            if (_uris.ContainsKey(pageKey))
                _uris[pageKey] = pageUri;
            else
                _uris.TryAdd(pageKey, pageUri);
        }

        public void GoToPage(string pageKey)
        {
            NavigationCommands.GoToPage.Execute(_uris[pageKey], null);
        }
    }
}
