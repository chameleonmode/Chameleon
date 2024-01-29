using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class InitializeUserProfileTabEventArgs : EventArgs
    {
        public int ProfileId { get; set; }
        public UserProfileViewTab UserProfileViewTab { get; set; }
    }
}
