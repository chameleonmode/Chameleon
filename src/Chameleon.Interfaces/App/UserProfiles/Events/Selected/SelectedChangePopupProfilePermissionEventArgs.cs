using System;

namespace Chameleon.Interfaces.App.UserProfiles.Events.Selected
{
    public class SelectedChangePopupProfilePermissionEventArgs : EventArgs
    {
        public string PermissionName { get; }
        public bool IsChecked { get; }
        public SelectedChangePopupProfilePermissionEventArgs(string permissionName, bool isChecked)
        {
            PermissionName = permissionName;
            IsChecked = isChecked;
        }
    }
}