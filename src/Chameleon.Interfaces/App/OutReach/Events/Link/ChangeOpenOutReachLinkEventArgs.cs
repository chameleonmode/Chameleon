using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.OutReach
{
    public class ChangeOpenOutReachLinkEventArgs
    {
        public ILinkManager LinkManager { get; }

        public ChangeOpenOutReachLinkEventArgs(ILinkManager linkManager)
        {
            LinkManager = linkManager;
        }
    }
}
