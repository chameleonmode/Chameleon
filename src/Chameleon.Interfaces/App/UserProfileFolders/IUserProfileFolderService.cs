using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IUserProfileFolderService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserProfileFolder Get(int folderId);
        IUserProfileFolders GetAll();
        Task<IUserProfileFolders> GetAllAsync();
        IUserProfileFolder Create(string title = null);
        void Delete(IUserProfileFolder userProfile);
        void Save(IUserProfileFolder userProfile);
        string GetTitle(int? folderId);
        void Sync();
        bool IsSharedFolder(IUserProfileFolder folder);
    }
}
