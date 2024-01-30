using Chameleon.Interfaces.Bookmarks;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserSettingsDialogService : ISingletonDependency
    {
        void ShowSettingsDialog(IUserProfile userProfile, ILinkManager linkManager);
        void ShowUserAgentsDialog(IUserProfile userProfile);
        void ShowCoociesAndCacheDialog(IUserProfile userProfile);
        void ShowBookmarkDialog(IUserProfile userProfile, IProfileBookmark profileBookmark);
        void ShowBookmarkAddFolderDialog(IUserProfile userProfile);
        void ShowFolderBookmarkDialog(IUserProfile userProfile, IProfileBookmark profileBookmark);
        void ShowBookmarkFileDialog(IUserProfile userProfile, IProfileBookmark profileBookmark, IBookmarkFile bookmarkFile);
    }
}
