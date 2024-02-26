using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChameleonChromeExtension : ChameleonChromiumExtension
    {
        public ChameleonChromeExtension(
            IApplicationEnvironment applicationEnvironment
            ) : base(applicationEnvironment, SystemBrowserType.Chrome)
        {
        }
    }
}
