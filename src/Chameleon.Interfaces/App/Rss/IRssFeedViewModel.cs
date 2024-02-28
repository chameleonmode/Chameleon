using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Rss;

public interface IRssFeedViewModel
    : IUserProfileSetter
    , IUserProfileRequiredEntity
{
    string RSSFeedsText { get; set; }
}
