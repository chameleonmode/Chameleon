
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class SelectedUserProfileEventArgs : EventArgs
    {
        public IUserProfile UserProfile { get; }
        public bool IsSelected { get; }
        public SelectedUserProfileEventArgs(IUserProfile userProfile, bool isSelected)
        {
            UserProfile = userProfile;
            IsSelected = isSelected;
        }
    }
}
