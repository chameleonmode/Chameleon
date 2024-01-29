using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.OutReach
{
    public class RssFeedsEventArgs : EventArgs
    {
        public IUserProfile UserProfile { get; }
        public string TextFeeds { get; }

        public RssFeedsEventArgs(
            IUserProfile userProfile,
            string textFeeds)
        {
            UserProfile = userProfile;
            TextFeeds = textFeeds;
        }
    }
}
