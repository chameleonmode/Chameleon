
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class SelectedUserProxySettingEventArgs : EventArgs
    {
        public bool IsSelected { get; }
        public bool OpenChangeProxies { get; }
        public SelectedUserProxySettingEventArgs(bool isSelected, bool openChangeProxies = false)
        {
            IsSelected = isSelected;
            OpenChangeProxies = openChangeProxies;
        }
    }
}
