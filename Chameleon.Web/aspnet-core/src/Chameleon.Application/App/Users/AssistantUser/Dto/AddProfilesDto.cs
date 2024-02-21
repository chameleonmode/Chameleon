using System.Collections.Generic;

namespace Chameleon.App.Users.AssistantUser.Dto
{
    public class AddProfilesDto
    {
        public long Id { get; set; }
        public IList<int> ProfileIds { get; set; }
        public IList<int> ProfilePermissionIds { get; set; }
    }
}
