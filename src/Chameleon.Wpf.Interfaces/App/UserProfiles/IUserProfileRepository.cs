using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileRepository
        : IRepository<IUserProfile, int, GetAllRequestDto>
    {
        void MoveUserProfileToFolder(List<int> profileIds, int? folderId);
        void SetProfileIsFavorite(int profileId, bool isFavorite);
        void SetProfileCacheLimit(int profileId, double limitCache);
        IUserProfile[] GetAllByUserId(int id);
    }
}
