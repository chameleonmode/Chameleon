using Chameleon.Interfaces.Ioc;
using Chameleon.lib.Common.Enums;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISetPreferencesService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void SetPreferences(IWebBrowserSettings webBrowser, string browserProfileFolderPath, SystemBrowserType browserType);
    }
}
