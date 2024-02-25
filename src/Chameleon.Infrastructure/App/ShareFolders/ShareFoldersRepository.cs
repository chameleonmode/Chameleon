using AutoMapper;
using Chameleon.Domain.Entities.ShareFolders;
using Chameleon.Infrastructure.App.ShareFolders.Api;
using Chameleon.Infrastructure.App.ShareFolders.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.ShareFolders;

using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders
{
    public class ShareFoldersRepository
        : Repository<ShareFolder,
            IShareFolder,
            int,
            ShareFolderDto,
            CreateShareFolderDto,
            UpdateShareFolderDto,
            ShareFolderGetAllRequestDto
            >
        , IShareFoldersRepository
    {
        private readonly IShareFoldersApi _shareFoldersApi;

        public ShareFoldersRepository(
            IMapper mapper,
            IShareFoldersApi shareFoldersApi,
            IEventAggregator eventAggregator
            ) : base(mapper, shareFoldersApi, eventAggregator)
        {
            _shareFoldersApi = shareFoldersApi;
        }

        public override void Delete(int id)
        {
            _shareFoldersApi.DeleteFolder(id);
        }

        public void AddPermission(int userFolderId, int permissionId)
        {
            var createDto = new CreateShareFolderPermissionDto()
            {
                UserFolderId = userFolderId,
                PermissionId = permissionId
            };
            _shareFoldersApi.AddPermission(createDto);
        }

        public void DeletePermission(int userFolderId, int permissionId)
        {
            _shareFoldersApi.DeletePermission(userFolderId, permissionId);
        }

        public IList<IShareFolder> Share(long userId, IList<int> folderIds, IList<int> permissionIds)
        {
            var createDto = new CreateShareFolderDto()
            {
                UserId = userId,
                FolderIds = folderIds,
                PermissionIds = permissionIds
            };

            var dtos =  _shareFoldersApi.Share(createDto);
            var entities = _mapper.Map<IList<IShareFolder>>(dtos);

            return entities;
        }
        public string[] GetAllProfilePermissionNames(long userId, int profileId, int? folderId)
        {
            return _shareFoldersApi.GetAllProfilePermissionNames(userId, profileId, folderId);
        }
    }
}
