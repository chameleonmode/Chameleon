using Chameleon.Application.Events;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Startup;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Startup
{
    public class ApplicationStartup : IApplicationStartup
    {

        public ApplicationStartup(
             IEnumerable<IApplicationEventHandlers> _)
        {
        }
    }
}
