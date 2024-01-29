using Prism.Events;

namespace Chameleon.Interfaces.UserProfiles
{
    public class RemoveUserProfileFromFolderEvent
        : PubSubEvent<UserProfileEventArgs>
    { }
}
