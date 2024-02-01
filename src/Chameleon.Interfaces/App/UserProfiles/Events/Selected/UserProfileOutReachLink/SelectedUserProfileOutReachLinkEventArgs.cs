using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class SelectedUserProfileOutReachLinkEventArgs : EventArgs
    {
        public bool IsSelected { get; }
        public SelectedUserProfileOutReachLinkEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }

    public class SelectedAllUserProfileOutReachLinkEventArgs : EventArgs
    {
        public bool IsSelected { get; }
        public SelectedAllUserProfileOutReachLinkEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}
