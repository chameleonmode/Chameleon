using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Chameleon.App.Entities;
using System.Collections.Generic;

namespace Chameleon.App
{
    public interface IUserDefaultSettingsAppService
        : IAsyncCrudAppService<
            UserDefaultSettingsDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateUserDefaultSettingsDto,
            UpdateUserDefaultSettingsDto
            >
    {
    }
}
