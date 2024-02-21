using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using System.Threading.Tasks;
using System.Linq;
using Chameleon.App.Entities;
using System.Collections.Generic;

namespace Chameleon.App
{
    public class UserSettingsAppService
        : AsyncCrudAppService<
            UserSettings,
            UserSettingsDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateUserSettingsDto,
            UpdateUserSettingsDto
            >
        , IUserSettingsAppService
    {
        private long? CurrentUserId => AbpSession.GetUserId();

        public UserSettingsAppService(IRepository<UserSettings> repository)
            : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public override Task<UserSettingsDto> UpdateAsync(UpdateUserSettingsDto input)
        {
            input.UserId = CurrentUserId;
            return base.UpdateAsync(input);
        }

        public override Task<UserSettingsDto> CreateAsync(CreateUserSettingsDto input)
        {
            input.UserId = CurrentUserId;
            return base.CreateAsync(input);
        }

        public override async Task<PagedResultDto<UserSettingsDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            
            result.Items = result.Items
                .Where(a => a.UserId == CurrentUserId)
                .ToList();

            if (result.Items.Count == 0)
            {
                var items = new List<UserSettingsDto>() 
                { 
                    await CreateAsync(new CreateUserSettingsDto()) 
                };
                result.Items = items;
            }

            return result;
        }
    }
}
