using Abp.Domain.Entities;
using Chameleon.App.Entities.Assistant;
using Chameleon.Authorization.Users;
using System.Collections.Generic;

namespace Chameleon.App.Entities
{
    public class ProfileAssistant
        : Entity<long>,
          IMustHaveProfile,
          IMustHaveUser
    {
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<ProfileAssistantPermission> ProfileAssistantPermissions { get; set; }
    }
}
