using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Rss
{
    public interface IUserProfileRssFeed 
        : IUserProfileRequiredEntity
    {
        string Url { get; set; }
    }
}
