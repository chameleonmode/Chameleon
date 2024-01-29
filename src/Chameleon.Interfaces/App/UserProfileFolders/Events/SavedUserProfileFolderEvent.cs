using Prism.Events;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public class SavedUserProfileFolderEvent
        : PubSubEvent<UserProfileFolderEventArgs>
    { }
}