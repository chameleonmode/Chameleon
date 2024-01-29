using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.OutReach
{
    public interface IUserProfileOutReachRss
		: IUserProfileRequiredEntity
    {
        string RssName { get; set; }
        string RssLink { get; set; }
        string ContactName { get; set; }
        string ContactEmail { get; set; }
        string Notes { get; set; }
        OutReachRssStatus Status { get; set; }
    }
}
