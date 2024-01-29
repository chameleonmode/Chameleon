using Chameleon.Interfaces.Settings;
using System;

namespace Chameleon.Interfaces.UserSettings
{
    public class UserDefaultSettingsEventArgs : EventArgs
    {
        public IUserDefaultSetting UserDefaultSetting { get; }

        public UserDefaultSettingsEventArgs(IUserDefaultSetting userDefaultSetting)
        {
            UserDefaultSetting = userDefaultSetting;
        }
    }
}
