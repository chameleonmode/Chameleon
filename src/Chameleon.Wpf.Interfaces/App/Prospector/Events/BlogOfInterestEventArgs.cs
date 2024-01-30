using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.Prospector
{
    public class BlogOfInterestEventArgs : EventArgs
    {
        public IUserProfileProspectorBlogsOfInterest UserProfileProspectorBlogsOfInterest { get; }
        public IUserProfile UserProfile { get; }
        public string Title { get; set; }

        public BlogOfInterestEventArgs(
            IUserProfileProspectorBlogsOfInterest userProfileProspectorBlogsOfInterest
            , IUserProfile userProfile)
        {
            UserProfileProspectorBlogsOfInterest = userProfileProspectorBlogsOfInterest;
            UserProfile = userProfile;
        }
    }
}
