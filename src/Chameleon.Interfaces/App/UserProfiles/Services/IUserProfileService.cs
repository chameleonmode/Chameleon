using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileService 
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserProfiles GetAll();
        Task<IUserProfiles> GetAllAsync();
        List<IUserProfile> GetAllByUserId(int id);
        IUserProfile Get(int Id, bool ignoreCache = false);
        Task<IUserProfile> CreateAsync(int? folderId);
        void Delete(IUserProfile userProfile);
        void Save(IUserProfile userProfile);
        void Import(IUserProfile userProfile);
        void Import(IList<IUserProfile> userProfiles);
        Task ImportAsync(IList<IUserProfile> userProfiles);
        void ShowOutOfLimitPopup();
        void MoveUserProfileToFolder(List<int> profileIds, int? folderId);
        void SetProfileIsFavorite(int profileId, bool isFavorite);
        void SetProfileCacheLimit(int profileId, double limitCache);
        IReadOnlyList<IUserProfile> Search(IUserProfileSearchRequest request);
        void Sync();
        bool IsSharedProfile(IUserProfile profile);
    }
}
