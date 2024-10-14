using Chameleon.Interfaces.App.Users.AssistantUser.Events.Base;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;

using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Assistants.Events {
	public class AddProfilesEventArgs
        : InviteUserOrAddProfilesEventArgs
    {
        public long AssistantId { get; }
        public IList<IUserProfile> Profiles { get; }
        public IList<IUserProfileFolder> Folders { get; }
        public AddProfilesEventArgs(
            long assistantId,
            IList<IUserProfile> profiles,
            IList<int> profilePermissionIds,
            IList<IUserProfileFolder> folders,
            IList<int> folderPermissionIds)
            : base(profilePermissionIds, folderPermissionIds)
        {
            AssistantId = assistantId;
            Profiles = profiles;
            Folders = folders;
        }
    }
    public class AddProfilesEvent
        : PubSubEvent<AddProfilesEventArgs>
    {
    }
}
