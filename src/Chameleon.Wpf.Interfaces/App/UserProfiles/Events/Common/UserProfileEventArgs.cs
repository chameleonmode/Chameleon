using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class UserProfileEventArgs : EventArgs
    {
        public Uri Url { get; }
        public IUserProfile UserProfile { get; }

        public UserProfileEventArgs(IUserProfile userProfile, Uri url = null)
        {
            Url = url;
            UserProfile = userProfile;
        }
    }
}
