using Prism.Events;

namespace Chameleon.Interfaces.UserProfiles
{
    public class OpenUserProfileEvent
        : PubSubEvent<UserProfileEventArgs>
    { }
}