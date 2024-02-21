using Abp.Application.Services;
using Chameleon.App.ShareFolders.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.App.ShareFolders
{
    public interface IShareFoldersAppService 
        : IAsyncCrudAppService<
            ShareFolderDto,
            int,
            ShareFolderGetAllRequestDto,
            CreateShareFolderDto,
            UpdateShareFolderDto
            >
    {
        Task<IList<ShareFolderDto>> ShareAsync(CreateShareFolderDto input);
        Task AddPermissionAsync(CreateShareFolderPermissionDto input);
        Task DeletePermissionAsync(int userFolderId, int permissionId);
        IList<int> GetAllProfileIdsFromSharedFolder(long userId);
        IList<string> GetAllProfilePermissionNames(long userId, int profileId, int? folderId);
    }
}
