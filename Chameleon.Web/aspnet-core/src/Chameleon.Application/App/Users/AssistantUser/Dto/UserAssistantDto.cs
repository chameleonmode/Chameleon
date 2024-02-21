using Chameleon.Users.Dto;
using System.Collections.Generic;

namespace Chameleon.App.Users.AssistantUser.Dto
{
    public class UserAssistantDto 
        : UserDto
    {
        public string Password { get; set; }
        public bool CanCreateProfiles { get; set; }
        public IList<int> ProfileIds { get; set; }
        public IList<int> FolderIds { get; set; }
    }
}
