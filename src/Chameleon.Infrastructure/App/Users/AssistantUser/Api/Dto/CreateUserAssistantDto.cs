using System.Collections.Generic;

namespace Chameleon.Infrastructure.Users
{
    public class CreateUserAssistantDto
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public IList<int> ProfileIds { get; set; }
        public IList<int> ProfilePermissionIds { get; set; }
        public IList<int> FolderIds { get; set; }
        public IList<int> FolderPermissionIds { get; set; }
    }
}
