using Chameleon.Interfaces.App.Users.AssistantUser.Events.Base;

using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Assistants.Events
{
    public class InviteUserAssistantEventArgs
        : InviteUserOrAddProfilesEventArgs
    {
        public string AssistantName { get; }
        public string AssistantEmail { get; }
        public IList<int> ProfileIds { get; }
        public IList<int> FolderIds { get; }

        public InviteUserAssistantEventArgs(
            string assistantName, 
            string assistantEmail, 
            IList<int> profileIds,
            IList<int> profilePermissionIds,
            IList<int> folderIds,
            IList<int> folderPermissionIds)
            : base(profilePermissionIds, folderPermissionIds)
        {
            AssistantName = assistantName;
            AssistantEmail = assistantEmail;
            ProfileIds = profileIds;
            FolderIds = folderIds;
        }
    }
    public class InviteUserAssistantEvent 
        : PubSubEvent<InviteUserAssistantEventArgs>
    {
    }
}
