
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class SelectedUserDefaultSettingEventArgs : EventArgs
    {
        public bool IsSelected { get; }
        public SelectedUserDefaultSettingEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}
