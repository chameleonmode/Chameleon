using Abp.Domain.Entities.Auditing;
using Chameleon.App.Entities.ShareFolders;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.App.Entities
{
    public class Folder : FullAuditedEntity
    {
        public string Title { get; set; }
        public bool IsFavorite { get; set; }
        public virtual ICollection<Profile> Profiles { get; protected set; }
        public virtual ICollection<UserFolder> UserFolders { get; protected set; }

        public Profile FindProfileOrNull(int profileId)
        {
            return Profiles.FirstOrDefault(profile => profile.Id == profileId);
        }
    }
}
