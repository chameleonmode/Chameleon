
using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Assistants.Events
{
    public class SavedUserAssistantEventArgs 
        : EventArgs
    {
        public long Id { get; }
        public string AssistantName { get; }
        public string AssistantEmail { get; }
        public string Password { get; }
        public IList<int> ProfileIds { get; }
        public IList<int> FolderIds { get; }
        public IList<int> ProfilePermissionids{ get; }

        public SavedUserAssistantEventArgs(
            long id,
            string assistantName,
            string assistantEmail,
            string password,
            IList<int> profileIds,
            IList<int> folderIds,
            IList<int> profilePermissionids)
        {
            Id = id;
            AssistantName = assistantName;
            AssistantEmail = assistantEmail;
            Password = password;
            ProfileIds = profileIds;
            FolderIds = folderIds;
            ProfilePermissionids = profilePermissionids;
        }
    }

    public class SavedUserAssistantEvent 
        : PubSubEvent<SavedUserAssistantEventArgs>
    {

    }
}
