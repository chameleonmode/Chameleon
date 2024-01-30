using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ILinkManager
    {
        string Url { get; set; }
        string NameUrl { get; set; }
        IUserProfile UserProfile { get; set; }
    }
}
