using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.OutReach
{
    public class OutReachRssEventArgs : EventArgs
    {
        public IUserProfileOutReachRss UserProfileOutReachRss { get; }
        public IUserProfile UserProfile { get; }

        public OutReachRssEventArgs(
            IUserProfileOutReachRss userProfileOutReachRss
            , IUserProfile userProfile)
        {
            UserProfileOutReachRss = userProfileOutReachRss;
            UserProfile = userProfile;
        }
    }
}
