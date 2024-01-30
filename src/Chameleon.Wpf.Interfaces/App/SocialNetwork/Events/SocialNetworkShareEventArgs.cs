using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.SocialNetwork
{
    public class SocialNetworkShareEventArgs : EventArgs
    {
        public Uri Url { get; }
        public SocialNetworkType Type { get; }
        public IUserProfile UserProfile { get; }

        public SocialNetworkShareEventArgs(
            IUserProfile userProfile, 
            SocialNetworkType type, 
            Uri url)
        {
            Url = url;
            Type = type;
            UserProfile = userProfile;
        }
    }
}
