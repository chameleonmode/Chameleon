using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.OutReach
{
    public class OutReachLinkEventArgs
    {
        public IProfileOutReachLink ProfileOutReachLink { get; }
        public IUserProfile UserProfile { get; }
        public bool IsDisable { get; }

        public bool Edit { get; }

        public OutReachLinkEventArgs(
            IProfileOutReachLink profileOutReachLink,
            IUserProfile userProfile,
            bool isDisable = false)
        {
            ProfileOutReachLink = profileOutReachLink;
            UserProfile = userProfile;
            IsDisable = isDisable;
        }
    }
}
