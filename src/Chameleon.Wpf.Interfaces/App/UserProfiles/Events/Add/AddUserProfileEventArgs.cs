using Chameleon.Interfaces.UserProfileFolders;
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class AddUserProfileEventArgs : EventArgs
    {
        public IUserProfile UserProfile { get; }
        public IUserProfileFolder UserProfileFolder { get; }
        public AddUserProfileEventArgs(IUserProfile userProfile, IUserProfileFolder userProfileFolder) 
        {
            UserProfile = userProfile;
            UserProfileFolder = userProfileFolder;
        }
    }
}
