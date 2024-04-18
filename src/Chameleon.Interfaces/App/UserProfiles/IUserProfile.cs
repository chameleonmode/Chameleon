using Chameleon.Interfaces.UserProfiles.Additional;
using Chameleon.Interfaces.WebBrowser;
using System.Collections.Generic;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.YouTube;
using Chameleon.Interfaces.WordPress;
using Chameleon.Interfaces.App;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfile : IUserProfileInfo  , INavigateFromSearch
    {
        IProxySettings Proxy { get; set; }
        IWebBrowserSettings WebBrowser { get; set; }
        IYouTubeSettings YoutubeSettings { get; set; }
        IWordPressSettings WordPressSettings { get; set; }
         
        IList<IUserProfileBusiness> Businesses { get; }
        IList<IUserProfileLogin> Logins { get; }
        IList<IUserProfilePerson> Persons { get; }
        IList<IUserProfileAddress> Addresses { get; }
        IList<IUserProfileOutReachRss> OutReachRsses { get; }
        IList<IUserProfileProspectorBlogsOfInterest> ProspectorBlogsOfInterest { get; }

        bool HasPermission(string permissionName);
    }    
}
