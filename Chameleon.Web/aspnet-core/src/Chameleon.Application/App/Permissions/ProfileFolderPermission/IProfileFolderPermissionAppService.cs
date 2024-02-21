using Abp.Application.Services;
using Chameleon.App.Permissions.Dto;
using System.Threading.Tasks;

namespace Chameleon.App.Permissions
{
    public interface IProfileFolderPermissionAppService 
        : IApplicationService
    {
        Task<ProfileFolderPermissionDto[]> GetAll();
    }
}
