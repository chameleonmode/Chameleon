using Chameleon.Infrastructure.App.ShareFolders.Api.Dto;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders
{
    public interface IShareFoldersRepository
        : IRepository<IShareFolder, int, ShareFolderGetAllRequestDto>
    {
        IList<IShareFolder> Share(long userId, IList<int> folderIds, IList<int> permissionIds);
        void AddPermission(int userFolderId, int permissionId);
        void DeletePermission(int userFolderId, int permissionId);
        string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId);
    }
}
