using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISetPreferencesService : ISingletonDependency
    {
        void SetPreferences(IWebBrowserSettings webBrowser, string browserProfileFolderPath, SystemBrowserType browserType);
    }
}
