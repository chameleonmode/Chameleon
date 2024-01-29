using System;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public class UserProfileFolderEventArgs : EventArgs
    {
        public IUserProfileFolder UserProfileFolder { get; }

        public UserProfileFolderEventArgs(IUserProfileFolder userProfileFolder)
        {
            UserProfileFolder = userProfileFolder;
        }
    }
}
