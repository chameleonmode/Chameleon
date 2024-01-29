using Prism.Events;

namespace Chameleon.Interfaces.UserProfiles
{
    public class OpenUserBrowserEvent
        : PubSubEvent<UserProfileEventArgs>
    { }
}