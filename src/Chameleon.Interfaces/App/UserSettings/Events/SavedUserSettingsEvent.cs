using Prism.Events;

namespace Chameleon.Interfaces.UserSettings
{
    public class SavedUserSettingsEvent
        : PubSubEvent<UserSettingsEventArgs>
    { }
}
