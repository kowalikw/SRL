namespace SRL.Main.ViewModel.Services
{
    /// <summary>
    /// Contains methods, properties, and events to support page navigation.
    /// </summary>
    internal interface INavigationService
    {
        /// <summary>
        /// Navigates to a page by string key.
        /// </summary>
        /// <param name="pageKey">Key.</param>
        void GoToPage(string pageKey);
    }
}
