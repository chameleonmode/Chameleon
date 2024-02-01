using Chameleon.Infrastructure.Dto;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Users
{
    public class UserAssistantDto
        : IEntityDto<long>
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public bool CanCreateProfiles { get; set; }
        public string[] RoleNames { get; set; }
        public string Password { get; set; }
        public IList<int> ProfileIds { get; set; }
        public IList<int> ProfilePermissionIds { get; set; }
    }
}
