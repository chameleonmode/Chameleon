using Chameleon.Interfaces.Settings;
using System;

namespace Chameleon.Interfaces.UserSettings
{
    public class UserSettingsEventArgs : EventArgs
    {
        public IUserSetting UserSetting { get; }

        public UserSettingsEventArgs(IUserSetting userSetting)
        {
            UserSetting = userSetting;
        }
    }
}
