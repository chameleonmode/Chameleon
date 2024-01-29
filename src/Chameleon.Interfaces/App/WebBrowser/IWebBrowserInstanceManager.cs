using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserInstanceManager : ISingletonDependency
    {
        IWebBrowserInstance Create(IUserProfile userProfile);
        IList<IWebBrowserInstance> Get(IUserProfile userProfile);
        IList<IWebBrowserInstance> GetOrCreate(IUserProfile userProfile);
    }
}
