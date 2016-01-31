using System;
using System.Collections.Concurrent;
using System.Windows.Input;

namespace SRL.Main.ViewModel.Services
{
    /// <summary>
    /// Simple, ModernUI-compatible implementation of <see cref="INavigationService"/> interface.
    /// </summary>
    internal class NavigationService : INavigationService
    {
        private readonly ConcurrentDictionary<string, Uri> _uris = new ConcurrentDictionary<string, Uri>();

        /// <summary>
        /// Links string key with a page <see cref="Uri"/>.
        /// </summary>
        /// <param name="pageKey">String key.</param>
        /// <param name="pageUri"><see cref="Uri"/> value.</param>
        public void Configure(string pageKey, Uri pageUri)
        {
            if (_uris.ContainsKey(pageKey))
                _uris[pageKey] = pageUri;
            else
                _uris.TryAdd(pageKey, pageUri);
        }

        /// <inheritdoc />
        public void GoToPage(string pageKey)
        {
            NavigationCommands.GoToPage.Execute(_uris[pageKey], null);
        }
    }
}
