using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public interface IUserSettingsAppService
        : IAsyncCrudAppService<
            UserSettingsDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateUserSettingsDto,
            UpdateUserSettingsDto
            >
    {
    }
}
