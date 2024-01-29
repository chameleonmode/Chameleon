using Prism.Events;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public class OpenUserProfileFolderEvent
        : PubSubEvent<UserProfileFolderEventArgs>
    { }
}