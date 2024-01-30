using Chameleon.Interfaces.Entities;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserUserAgent : IEntity
    {
        string Name { get; set; }
        string Value { get; }
    }
}
