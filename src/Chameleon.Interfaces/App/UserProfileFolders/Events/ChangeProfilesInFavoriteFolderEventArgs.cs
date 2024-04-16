using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.App.UserProfileFolders.Events
{
    public class ChangeProfilesInFavoriteFolderEventArgs : EventArgs
    {
        public int FolderId { get; }
        public bool Navigate { get; }
        public IUserProfile? Profile { get; }

        public ChangeProfilesInFavoriteFolderEventArgs(int folderId, bool navigate = false, IUserProfile? profile = null)
        {
            FolderId = folderId;
            Navigate = navigate;
            Profile = profile;
        }
    }
}
