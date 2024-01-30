using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.OutReach
{
    public class OutReachEventArgs : EventArgs
    {
        public IUserProfile UserProfile { get; }

        public OutReachEventArgs(
             IUserProfile userProfile)
        {
            UserProfile = userProfile;
        }
    }
}
