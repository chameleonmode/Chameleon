using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using Chameleon.App.Entities.Assistant;
using Chameleon.App.Entities.Permissions;
using Chameleon.App.Entities.ShareFolders;
using Chameleon.App.ShareFolders.Dto;
using Chameleon.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App.ShareFolders
{
    [AbpAuthorize]
    public class ShareFoldersAppService
        : AsyncCrudAppService<
            UserFolder,
            ShareFolderDto,
            int,
            ShareFolderGetAllRequestDto,
            CreateShareFolderDto,
            UpdateShareFolderDto>
        , IShareFoldersAppService
    {
        private readonly IRepository<Folder> _folderRepository;
        private readonly IRepository<ProfilePermission> _profileFolderPermissionRepository;
        private readonly IRepository<UserFolderPermission> _userFolderPermissionRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<ProfileAssistant, long> _profileAssistantRepository;
        private readonly IRepository<ProfileAssistantPermission, long> _profileAssistantPermissionRepository;

        public ShareFoldersAppService(
           IRepository<UserFolder> repository,
           IRepository<Folder> folderRepository,
           IRepository<ProfilePermission> profileFolderPermissionRepository,
           IRepository<UserFolderPermission> userFolderPermissionRepository,
           IRepository<Profile> profileRepository,
           IRepository<ProfileAssistant, long> profileAssistantRepository,
           IRepository<ProfileAssistantPermission, long> profileAssistantPermissionRepository
           ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;

            _folderRepository = folderRepository;
            _profileFolderPermissionRepository = profileFolderPermissionRepository;
            _userFolderPermissionRepository = userFolderPermissionRepository;
            _profileRepository = profileRepository;
            _profileAssistantRepository = profileAssistantRepository;
            _profileAssistantPermissionRepository = profileAssistantPermissionRepository;
        }

        public override async Task<PagedResultDto<ShareFolderDto>> GetAllAsync(ShareFolderGetAllRequestDto input)
        {
            var userFolders = await Repository
              .GetAllIncluding(uf => uf.UserFolderPermissions)
              .FilterByUserId(input.UserId)
              .ToListAsync();

            var response = userFolders
                .Select(uf => MapToEntityDto(uf))
                .ToList();

            return new PagedResultDto<ShareFolderDto>(response.Count, response);
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Primary)]
        public async Task<IList<ShareFolderDto>> ShareAsync(CreateShareFolderDto input)
        {
            var userFolderIds = await InsertUserFoldersAndGetIdsAsync(input.FolderIds, input.UserId);

            if(input.PermissionIds != null)
            {
                await InsertUserFolderPermissionsAsync(userFolderIds, input.PermissionIds);
            }

            var response = userFolderIds
                .Select(id => MapToEntityDto(Repository.Get(id)))
                .ToList();

            return response;
        }

        protected override ShareFolderDto MapToEntityDto(UserFolder userFolder)
        {
            var shareFolderDto = new ShareFolderDto
            {
                Id = userFolder.Id,
                FolderId = userFolder.FolderId,
                UserId = userFolder.UserId,
                FolderName = GetFolderName(userFolder.FolderId),
                FolderPermissions = GetFolderPermissions(userFolder.UserFolderPermissions)
            };

            return shareFolderDto;
        }

        private IList<ShareFolderPermissionDto> GetFolderPermissions(ICollection<UserFolderPermission> userFolderPermissions)
        {
            var folderPermissions = _profileFolderPermissionRepository
                    .GetAll()
                    .Select(pfp => new ShareFolderPermissionDto
                    {
                        PermissionId = pfp.Id,
                        PermissionName = pfp.PermissionName,
                        DisplayName = pfp.DisplayName,
                        IsGranted = userFolderPermissions != null && userFolderPermissions
                            .Select(ufp => ufp.ProfilePermissionId)
                            .Contains(pfp.Id)
                    })
                    .ToList();

            return folderPermissions;
        }

        private string GetFolderName(int folderId)
        {
            var folderName =  _folderRepository
                    .GetAll()
                    .Where(f => f.Id == folderId)
                    .Select(f => f.Title)
                    .First();

            return folderName;
        }

        private async Task InsertUserFolderPermissionsAsync(IList<int> userFolderIds, IList<int> permissionIds)
        {
            foreach (var userFolderId in userFolderIds)
            {
                await InsertUserFolderPermissionsAsync(userFolderId, permissionIds);
            }
        }

        private async Task InsertUserFolderPermissionsAsync(int userFolderId, IList<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                await _userFolderPermissionRepository.InsertAsync(new UserFolderPermission
                {
                    UserFolderId = userFolderId,
                    ProfilePermissionId = permissionId
                });
            }
        }

        private async Task<IList<int>> InsertUserFoldersAndGetIdsAsync(IList<int> folderIds, long userId)
        {
            var existingFolderIds = GetAllFolderIds(userId);

            folderIds = folderIds
                .Where(id => !existingFolderIds.Contains(id))
                .ToList();

            var userFolderIds = new List<int>();

            foreach (var folderId in folderIds)
            {
                var userFolderId = await Repository.InsertAndGetIdAsync(new UserFolder()
                {
                    UserId = userId,
                    FolderId = folderId
                });

                userFolderIds.Add(userFolderId);
            }

            return userFolderIds;
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Primary)]
        public async Task AddPermissionAsync(CreateShareFolderPermissionDto input)
        {
           var exist = _userFolderPermissionRepository
                .GetAll()
                .Where(ufp => ufp.UserFolderId == input.UserFolderId)
                .Where(ufp => ufp.ProfilePermissionId == input.PermissionId)
                .Any();

            if(exist)
            {
                return;
            }

            await _userFolderPermissionRepository.InsertAsync(new UserFolderPermission()
            {
                UserFolderId = input.UserFolderId,
                ProfilePermissionId = input.PermissionId
            });
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Primary)]
        public async Task DeletePermissionAsync(int userFolderId, int permissionId)
        {
            await _userFolderPermissionRepository.DeleteAsync(
                ufp => ufp.UserFolderId == userFolderId && ufp.ProfilePermissionId == permissionId);
        }

        public IList<int> GetAllProfileIdsFromSharedFolder(long userId)
        {
            var folderIds = GetAllFolderIds(userId);

            var response = _profileRepository
                .GetAll()
                .Where(p => p.FolderId != null)
                .Where(p => folderIds.Contains(p.FolderId.Value))
                .Select(p => p.Id)
                .ToList();

            return response;
        }

        private IList<int> GetAllFolderIds(long userId)
        {
            return Repository
                .GetAll()
                .FilterByUserId(userId)
                .Select(uf => uf.FolderId)
                .ToList();
        }

        public IList<string> GetAllProfilePermissionNames(long userId, int profileId, int? folderId)
        {
            var permissionIds = new List<int>();

            if (folderId != null)
            {
               var userFolderId = Repository
                    .GetAll()
                    .Where(uf => uf.UserId == userId && uf.FolderId == folderId.Value)
                    .Select(uf => uf.Id)
                    .FirstOrDefault();

                if(userFolderId != 0)
                {
                     var permissionIdsFromSharedFolder = _userFolderPermissionRepository
                        .GetAll()
                        .Where(ufp => ufp.UserFolderId == userFolderId)
                        .Select(ufp => ufp.ProfilePermissionId)
                        .ToList();

                    permissionIds.AddRange(permissionIdsFromSharedFolder);               
                }
            }

            var profileAssistantId = _profileAssistantRepository
                .GetAll()
                .Where(pa => pa.UserId == userId && pa.ProfileId == profileId)
                .Select(pa => pa.Id)
                .FirstOrDefault();

            if(profileAssistantId != 0)
            {
                var permissionIdsFromSharedProfile = _profileAssistantPermissionRepository
                        .GetAll()
                        .Where(pap => pap.ProfileAssistantId == profileAssistantId)
                        .Select(pap => pap.ProfilePermissionId)
                        .ToList();

                permissionIds.AddRange(permissionIdsFromSharedProfile);
            }

            permissionIds = permissionIds
                .Distinct()
                .ToList();

            var response = _profileFolderPermissionRepository
                .GetAll()
                .Where(pfp => permissionIds.Contains(pfp.Id))
                .Select(pfp => pfp.PermissionName)
                .ToList();

            return response;
        }
    }
}
