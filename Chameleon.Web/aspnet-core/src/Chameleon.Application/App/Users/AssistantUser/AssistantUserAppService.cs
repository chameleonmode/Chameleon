using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Chameleon.App.Entities;
using Chameleon.App.Entities.Assistant;
using Chameleon.App.Entities.Permissions;
using Chameleon.App.ShareFolders;
using Chameleon.App.ShareFolders.Dto;
using Chameleon.App.Users.AssistantUser.Dto;
using Chameleon.App.ValueObjects;
using Chameleon.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.Users;
using Chameleon.Users.Dto;
using Microsoft.EntityFrameworkCore;

namespace Chameleon.App.Users.AssistantUser
{
    [AbpAuthorize(PermissionNames.Pages_Users_Assistant)]
    public class AssistantUserAppService
        : AsyncCrudAppService<
            User,
            UserDto,
            long,
            PagedUserResultRequestDto,
            CreateUserAssistantDto,
            UserDto>
        , IAssistantUserAppService
    {
        private readonly UserAppService _userService;
        private readonly UserManager _userManager;
        private readonly IRepository<AssistantLicense> _assistantLicenseRepository;
        private readonly IRepository<ProfileAssistant, long> _profileAssistantRepository;
        private readonly IRepository<ProfileAssistantPermission, long> _profileAssistantPermissionRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<ProfilePermission> _profileFolderPermissionRepository;
        private readonly IShareFoldersAppService _shareFoldersAppService;

        public AssistantUserAppService(
            IRepository<User, long> repository,
            UserAppService userAppService,
            UserManager userManager,
            IRepository<AssistantLicense> assistantLicenseRepository,
            IRepository<ProfileAssistant, long> profileAssistantRepository,
            IRepository<ProfileAssistantPermission, long> profileAssistantPermissionRepository,
            IRepository<Profile> profileRepository,
            IRepository<ProfilePermission> profileFolderPermissionRepository,
            IShareFoldersAppService shareFoldersAppService
            )
             : base(repository)
        {
            _userManager = userManager;
            _userService = userAppService;
            _assistantLicenseRepository = assistantLicenseRepository;
            _profileAssistantRepository = profileAssistantRepository;
            _profileAssistantPermissionRepository = profileAssistantPermissionRepository;
            _profileRepository = profileRepository;
            _profileFolderPermissionRepository = profileFolderPermissionRepository;
            _shareFoldersAppService = shareFoldersAppService;
        }


        public override async Task<UserDto> CreateAsync(CreateUserAssistantDto input)
        {
            var primaryUser = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

            if(primaryUser.EmailAddress == input.EmailAddress || primaryUser.UserName == input.UserName)
            {
                throw new UserFriendlyException("Sorry, you can not invite yourself.");
            }
            
            if(await _userManager.FindByNameOrEmailAsync(AbpSession.TenantId.Value, input.EmailAddress) != null)
            {
                throw new UserFriendlyException($"Sorry, the assistant with email {input.EmailAddress} is already invited.");
            }

            if (await _userManager.FindByNameOrEmailAsync(AbpSession.TenantId.Value, input.UserName) != null)
            {
                throw new UserFriendlyException($"Sorry, the assistant with username {input.UserName} is already invited.");
            }

            var createUserDto = new CreateUserDto
            {
                EmailAddress = input.EmailAddress,
                Password = AssistantLicenseKey.Generate(),
                IsActive = true,
                RoleNames = new string[] { StaticRoleNames.Tenants.AssistantUser },
                Name = "",
                UserName = input.UserName,
                Surname = ""
            };
            
            var createdUserDto = await _userService.CreateAsync(createUserDto);
            var createdUser = await _userManager.GetUserByIdAsync(createdUserDto.Id);

            await _assistantLicenseRepository.InsertAsync(new AssistantLicense
            {
                LicenseKey = new AssistantLicenseKey(createUserDto.Password),
                UserId = createdUser.Id,
                PrimaryUser = primaryUser
            });

            await SetPermissionsForSeveralProfilesAsync(createdUser.Id, input.ProfileIds, input.ProfilePermissionIds);
            await _shareFoldersAppService.ShareAsync(new CreateShareFolderDto
            {
                UserId = createdUser.Id,
                FolderIds = input.FolderIds,
                PermissionIds = input.FolderPermissionIds
            });

            var response = new UserAssistantDto()
            {
                Id = createdUserDto.Id,
                UserName = createdUserDto.UserName,
                EmailAddress = createdUserDto.EmailAddress,
                Password = createUserDto.Password,
                ProfileIds = input.ProfileIds,
                FolderIds = input.FolderIds
            };

            return response;
        }

        public override async Task<PagedResultDto<UserDto>> GetAllAsync(PagedUserResultRequestDto input)
        {
            var users = await base.GetAllAsync(input);
            var assistantUsers = new List<UserAssistantDto>();

            foreach (var item in users.Items)
            {
                var user = ObjectMapper.Map<User>(item);

                if (await _userManager.IsInRoleAsync(user, StaticRoleNames.Tenants.AssistantUser))
                {
                    var assistLic = await _assistantLicenseRepository
                        .GetAll()
                        .Where(p => p.UserId == user.Id)
                        .Select(p => new { licenseKey = p.LicenseKey, canCreateProfiles = p.CanCreateProfiles })
                        .SingleAsync();

                    var assistantUser = ObjectMapper.Map<UserAssistantDto>(user);
                    assistantUser.Password = assistLic.licenseKey;
                    assistantUser.CanCreateProfiles = assistLic.canCreateProfiles;

                    assistantUsers.Add(assistantUser);
                }
            }

            return new PagedResultDto<UserDto>(assistantUsers.Count(), assistantUsers);
        }

        public override async Task<UserDto> UpdateAsync(UserDto input)
        {
            return await _userService.UpdateAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await _userService.DeleteAsync(input);
        }

        public async Task SetCanCreateProfiles(long assistantId, bool canCreateProfiles) 
        {
            var assistLicense = await _assistantLicenseRepository.FirstOrDefaultAsync(al => al.UserId == assistantId);
            
            if (assistLicense.CreatorUserId != AbpSession.UserId)
                throw new UserFriendlyException("Sorry. This is not your assistant. You can't change his permissions.");

            assistLicense.CanCreateProfiles = canCreateProfiles;
            await _assistantLicenseRepository.GetDbContext().SaveChangesAsync();
        }

        public async Task<IList<AssistantProfileDto>> GetAllAssistantProfilesByIdAsync(long assistantId)
        {
            var profileIds = await GetProfileIdsByUserIdAsync(assistantId);

            var response = await _profileRepository
                .GetAll()
                .Where(p => profileIds.Contains(p.Id))
                .Select(p => new AssistantProfileDto()
                {
                    Id = assistantId,
                    ProfileId = p.Id,
                    ProfileName = p.Title
                })
                .ToListAsync();

            return response;
        }

        public async Task DeleteAssistantProfileAsync(long assistantId, int profileId)
        {
            await _profileAssistantRepository.DeleteAsync(p => p.UserId == assistantId && p.ProfileId == profileId);
        }

        public async Task<IList<AssistantProfilePermissionDto>> GetAllProfilePermissionsAsync(long assistantId, int profileId)
        {
            var profileAssistantId = await GetProfileAssistantIdAsync(assistantId, profileId);
            var profilePermissionIds = await GetAllProfilePermissionIdsAsync(profileAssistantId);

            var response = await _profileFolderPermissionRepository
                    .GetAll()
                    .Select(p => new AssistantProfilePermissionDto()
                    {
                        Id = p.Id,
                        PermissionName = p.PermissionName,
                        DisplayName = p.DisplayName,
                        IsGranted = profilePermissionIds.Contains(p.Id),
                        ProfileAssistantId = profileAssistantId
                    })
                    .ToListAsync();

            return response;
        }

        public async Task InsertProfilePermissionAsync(AssistantProfilePermissionDto assistantProfilePermission)
        {
            var profileAssistantPermission = new ProfileAssistantPermission()
            {
                ProfileAssistantId = assistantProfilePermission.ProfileAssistantId,
                ProfilePermissionId = assistantProfilePermission.Id
            };
            await _profileAssistantPermissionRepository.InsertAsync(profileAssistantPermission);
        }

        public async Task DeleteProfilePermissionAsync(long profileAssistantId, int profilePermissionId)
        {
            await _profileAssistantPermissionRepository.DeleteAsync
                (p => p.ProfileAssistantId == profileAssistantId && p.ProfilePermissionId == profilePermissionId);
        }

        public async Task ShareUserProfileAsync(CreateAssistantProfileDto input)
        {
            var permissionIds = await GetAllPermissionIds(input.PermissionNames);

            foreach (var assistantUserId in input.AssistantUserIds)
            {
                var profileAssistant = new ProfileAssistant()
                {
                    ProfileId = input.ProfileId,
                    UserId = assistantUserId
                };

                var profileAssistantId = await _profileAssistantRepository.InsertAndGetIdAsync(profileAssistant);

                foreach (var permissionId in permissionIds)
                {
                    var profileAssistantPermission = new ProfileAssistantPermission()
                    {
                        ProfileAssistantId = profileAssistantId,
                        ProfilePermissionId = permissionId
                    };

                    await _profileAssistantPermissionRepository.InsertAsync(profileAssistantPermission);
                }
            }
        }

        public async Task AddProfilesAsync(AddProfilesDto input)
        {
            await SetPermissionsForSeveralProfilesAsync(input.Id, input.ProfileIds, input.ProfilePermissionIds);
        }

        private async Task<IList<int>> GetAllPermissionIds(IList<string> permissionNames)
        {
            return await _profileFolderPermissionRepository
                            .GetAll()
                            .Where(p => permissionNames.Contains(p.PermissionName))
                            .Select(p => p.Id)
                            .ToListAsync();
        }

        private async Task<IList<int>> GetProfileIdsByUserIdAsync(long userId)
        {
            return await _profileAssistantRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Select(p => p.ProfileId)
                .ToListAsync();
        }

        private async Task<IList<int>> GetAllProfilePermissionIdsAsync(long profileAssistantId)
        {
            return await _profileAssistantPermissionRepository
                            .GetAll()
                            .Where(p => p.ProfileAssistantId == profileAssistantId)
                            .Select(p => p.ProfilePermissionId)
                            .ToListAsync();
        }

        private async Task<long> GetProfileAssistantIdAsync(long assistantId, int profileId)
        {
            return await _profileAssistantRepository
                            .GetAll()
                            .Where(p => p.UserId == assistantId && p.ProfileId == profileId)
                            .Select(p => p.Id)
                            .FirstAsync();
        }

        private async Task<long> InsertAssistantProfileAndGetIdAsync(int profileId, long assistantId)
        {
            return await _profileAssistantRepository.InsertAndGetIdAsync(new ProfileAssistant()
            {
                ProfileId = profileId,
                UserId = assistantId
            });
        }

        private async Task<IList<long>> InsertAssistantProfilesAndGetIdsAsync(IList<int> profileIds, long assistantId)
        {
            var profileAssistantIds = new List<long>();

            foreach (var profileId in profileIds)
            {
                var profileAssistantId = await InsertAssistantProfileAndGetIdAsync(profileId, assistantId);

                profileAssistantIds.Add(profileAssistantId);
            }

            return profileAssistantIds;
        }

        private async Task InsertProfileAssistantPermissionsAsync(IList<long> profileAssistantIds, IList<int> permissionIds)
        {
            foreach (var profileAssistantId in profileAssistantIds)
            {
                await InsertProfileAssistantPermissionsAsync(profileAssistantId, permissionIds);
            }
        }

        private async Task InsertProfileAssistantPermissionsAsync(long profileAssistantId, IList<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                await InsertProfileAssistantPermissionAsync(profileAssistantId, permissionId);
            }
        }

        private async Task InsertProfileAssistantPermissionAsync(long profileAssistantId, int permissionId)
        {
            await _profileAssistantPermissionRepository.InsertAsync(new ProfileAssistantPermission()
            {
                ProfileAssistantId = profileAssistantId,
                ProfilePermissionId = permissionId
            });
        }

        private async Task SetPermissionsForSeveralProfilesAsync(long assistantId, IList<int> profileIds, IList<int> permissionIds)
        {
            var profileAssistantIds = await InsertAssistantProfilesAndGetIdsAsync(profileIds, assistantId);

            if(permissionIds != null)
            {
                await InsertProfileAssistantPermissionsAsync(profileAssistantIds, permissionIds);
            }
        }
    }
}
