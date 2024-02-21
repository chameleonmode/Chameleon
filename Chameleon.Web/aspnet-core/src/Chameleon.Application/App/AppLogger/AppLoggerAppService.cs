using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using Abp.Runtime.Session;
using System.Threading.Tasks;
using Chameleon.Authorization.Users;
using System.Linq;
using System.Collections.Generic;

namespace Chameleon.App
{
    public class AppLoggerAppService
        : AsyncCrudAppService<
            AppLogger,
            AppLoggerDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateAppLoggerDto,
            UpdateAppLoggerDto
            >
        , IAppLoggerAppService
    {
        private long? CurrentUserId => AbpSession.GetUserId();
        public UserManager UserManager { get; set; }

        public AppLoggerAppService(IRepository<AppLogger> repository)
            : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public override async Task<AppLoggerDto> UpdateAsync(UpdateAppLoggerDto input)
        {
            await CheckInput(input);
            return await base.UpdateAsync(input);
        } 

        public override async Task<AppLoggerDto> CreateAsync(CreateAppLoggerDto input)
        {
            await CheckInput(input);
            return await base.CreateAsync(input);
        }

        public async Task RemoveAll()
        {
            await Repository.DeleteAsync(a => a.Id > 0);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task CheckInput(AppLoggerBaseDto input)
        {
            if (CurrentUserId == null)
            {
                return;
            }

            var user = await UserManager.FindByIdAsync(CurrentUserId.ToString());
            input.UserName = user.UserName;
            input.UserId = CurrentUserId;
        }
    }
}
