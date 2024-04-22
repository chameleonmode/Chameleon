

using Chameleon.Interfaces.App.UserProfileFolders.Events;

namespace Chameleon.Interfaces.UserProfiles
{
    public class CreateUserProfileEvent 
        : PubSubEvent<CreateUserProfileEventArgs>
    { }

    public class OnCreatedCreateUserProfileEvent
        : PubSubEvent<ChangeProfilesInFavoriteFolderEventArgs>
    { }
}