using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Chameleon.App.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class UserDefaultSettingsAppService
        : AsyncCrudAppService<
            UserDefaultSettings,
            UserDefaultSettingsDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateUserDefaultSettingsDto,
            UpdateUserDefaultSettingsDto
            >
        , IUserDefaultSettingsAppService
    {
        private long? CurrentUserId => AbpSession.GetUserId();

        public UserDefaultSettingsAppService(IRepository<UserDefaultSettings> repository)
            : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public override Task<UserDefaultSettingsDto> UpdateAsync(UpdateUserDefaultSettingsDto input)
        {
            input.UserId = CurrentUserId;
            return base.UpdateAsync(input);
        }

        public override Task<UserDefaultSettingsDto> CreateAsync(CreateUserDefaultSettingsDto input)
        {
            input.UserId = CurrentUserId;
            return base.CreateAsync(input);
        }

        public override async Task<PagedResultDto<UserDefaultSettingsDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            result.Items = result.Items
                .Where(a => a.UserId == CurrentUserId)
                .ToList();

            return result;
        }
    }
}
