using Abp.Application.Services;
using Chameleon.App.Users.AssistantUser.Dto;
using Chameleon.Users.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.App.Users.AssistantUser
{
    public interface IAssistantUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserAssistantDto, UserDto>
    {
        Task<IList<AssistantProfileDto>> GetAllAssistantProfilesByIdAsync(long assistantId);
        Task DeleteAssistantProfileAsync(long assistantId, int profileId);
        Task<IList<AssistantProfilePermissionDto>> GetAllProfilePermissionsAsync(long assistantId, int profileId);
        Task InsertProfilePermissionAsync(AssistantProfilePermissionDto assistantProfilePermission);
        Task DeleteProfilePermissionAsync(long profileAssistantId, int profilePermissionId);
        Task ShareUserProfileAsync(CreateAssistantProfileDto createAssistantProfileDtos);
        Task AddProfilesAsync(AddProfilesDto userAssistantDto);
    }
}
