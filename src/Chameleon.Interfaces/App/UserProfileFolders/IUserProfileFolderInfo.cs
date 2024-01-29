using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IUserProfileFolderInfo : IEntity
    {
        string Title { get; set; }
        bool IsFavorite { get; set; }
        int ProfilesCount { get; set; }
        long? CreatorUserId { get; set; }
    }
}
