using Prism.Events;

namespace Chameleon.Interfaces.UserSettings
{
    public class SavedUserDefaultSettingsEvent
        : PubSubEvent<UserDefaultSettingsEventArgs>
    {}
}
