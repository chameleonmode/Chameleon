using Abp.Domain.Repositories;
using Chameleon.App.Entities.Permissions;
using Chameleon.App.Permissions.Dto;
using System.Threading.Tasks;

namespace Chameleon.App.Permissions
{
    public class ProfileFolderPermissionAppService
        : ChameleonAppServiceBase
        , IProfileFolderPermissionAppService
    {
        private readonly IRepository<ProfilePermission> _repository;

        public ProfileFolderPermissionAppService
            (IRepository<ProfilePermission> repository)
        {
            _repository = repository;
        }

        public async Task<ProfileFolderPermissionDto[]> GetAll()
        {
            var entities  = await _repository.GetAllListAsync();
            var dtos = ObjectMapper.Map<ProfileFolderPermissionDto[]>(entities);
            return dtos;
        }
    }
}
