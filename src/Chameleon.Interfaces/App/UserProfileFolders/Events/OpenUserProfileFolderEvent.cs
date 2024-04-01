

namespace Chameleon.Interfaces.UserProfileFolders
{
    public class OpenUserProfileFolderEvent
        : PubSubEvent<UserProfileFolderEventArgs>
    { }

    public class UpdateUserProfileFolderEvent
    : PubSubEvent<UserProfileFolderEventArgs>
    { }
}