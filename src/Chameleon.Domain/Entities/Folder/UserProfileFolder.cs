using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Domain.Entities
{
    public class UserProfileFolder : IUserProfileFolder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsFavorite { get; set; }
        public int ProfilesCount { get; set; }
        public long? CreatorUserId { get; set; }
        public bool Navigated { get; set; }
    }
}
