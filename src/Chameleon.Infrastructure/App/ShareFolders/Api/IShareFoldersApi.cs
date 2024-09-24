using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.ShareFolders.Api.Dto;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders.Api
{
    public interface IShareFoldersApi
        : IApiLayer<ShareFolderDto, int, CreateShareFolderDto, UpdateShareFolderDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IList<ShareFolderDto> Share(CreateShareFolderDto input);
        void AddPermission(CreateShareFolderPermissionDto input);
        void DeletePermission(int userFolderId, int permissionId);
        string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId);
        void DeleteFolder(int id);
    }
}
