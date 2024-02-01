using Chameleon.Interfaces.App.Assistants;
using System.Collections.Generic;

namespace Chameleon.Domain.Entities
{
    public class UserAssistant
        : IUserAssistant
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool CanCreateProfiles { get; set; }
        public IList<int> ProfileIds { get; set; }
        public IList<int> ProfilePermissionIds { get; set; }
        public IList<int> FolderIds { get; set; }
        public IList<int> FolderPermissionIds { get; set; }
    }
}
