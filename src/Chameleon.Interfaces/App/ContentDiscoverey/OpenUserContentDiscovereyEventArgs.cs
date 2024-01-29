using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.App.ContentDiscoverey
{

    public class OpenUserContentDiscovereyEventArgs : EventArgs
    {
        public IUserProfile UserProfile { get; }

        public OpenUserContentDiscovereyEventArgs(IUserProfile userProfile)
        {
            UserProfile = userProfile;
        }
    }
}
