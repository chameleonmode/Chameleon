using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Users.AssistantUser.Events.Base
{
    public class InviteUserOrAddProfilesEventArgs
        : EventArgs
    {
        public IList<int> ProfilePermissionIds{ get; }
        public IList<int> FolderPermissionIds { get; }

        public InviteUserOrAddProfilesEventArgs(
            IList<int> profilePermissionIds,
            IList<int> folderPermissionIds)
        {
            ProfilePermissionIds = profilePermissionIds;
            FolderPermissionIds = folderPermissionIds;
        }
    }
}
