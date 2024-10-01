namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISetPreferencesService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void SetPreferences(IWebBrowserSettings webBrowser, string browserProfileFolderPath, lib.Common.Constants.Enums.SystemBrowserType browserType);
    }
}
