

using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Interfaces.UserProfiles {
	public class CreateUserProfileEvent 
        : PubSubEvent<CreateUserProfileEventArgs>
    { }

    public class OnCreatedCreateUserProfileEvent
        : PubSubEvent<ChangeProfilesInFavoriteFolderEventArgs>
    { }
}