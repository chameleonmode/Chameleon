using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class OpenUserProfileTabEventArgs : EventArgs
    {
        public UserProfileIdentityTab UserProfileIdentityTab { get; }

        public OpenUserProfileTabEventArgs(UserProfileIdentityTab userProfileIdentityTab)
        {
            UserProfileIdentityTab = userProfileIdentityTab;
        }
    }
}
