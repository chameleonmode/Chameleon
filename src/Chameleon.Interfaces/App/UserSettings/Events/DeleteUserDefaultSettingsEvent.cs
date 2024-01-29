using Prism.Events;

namespace Chameleon.Interfaces.UserSettings
{
    public class DeleteUserDefaultSettingsEvent
        : PubSubEvent<UserDefaultSettingsEventArgs>
    {
    }
}
