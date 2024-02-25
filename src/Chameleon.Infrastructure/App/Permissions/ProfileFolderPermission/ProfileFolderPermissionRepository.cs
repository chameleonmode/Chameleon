using AutoMapper;
using Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api;
using Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Permissions.ProfileFolderPermission;


namespace Chameleon.Infrastructure.App.Permissions.ProfileFolderPermission
{
    public class ProfileFolderPermissionRepository
        : Repository<Domain.Entities.Permissions.ProfileFolderPermission.ProfileFolderPermission,
            IProfileFolderPermission,
            ProfileFolderPermissionDto
            >
        , IProfileFolderPermissionRepository
    {
        public ProfileFolderPermissionRepository(
           IMapper mapper,
           IProfileFolderPermissionApi apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
