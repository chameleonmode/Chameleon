using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class UserProfileEventArgs : EventArgs
    {
        public string? Url { get; }
        public IUserProfile UserProfile { get; }

        public UserProfileEventArgs(IUserProfile userProfile, string? url = null)
        {
            Url = url;
            UserProfile = userProfile;
        }
    }
}
