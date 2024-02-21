using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Chameleon.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.Users;
using Chameleon.Users.Dto;

namespace Chameleon.App.Users.PrimaryUser
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class PrimaryUserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IPrimaryUserAppService
    {
        private readonly UserAppService _userAppService;
        public PrimaryUserAppService(IRepository<User, long> repository, UserAppService userAppService)
             : base(repository)
        {
            _userAppService = userAppService;
        }

        public override Task<UserDto> CreateAsync(CreateUserDto input)
        {
            input.AddUserRole(StaticRoleNames.Tenants.PrimaryUser);
            return _userAppService.CreateAsync(input);
        }

        public override async Task<UserDto> UpdateAsync(UserDto input)
        {
            return await _userAppService.UpdateAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await _userAppService.DeleteAsync(input);
        }

        public async Task MarkGuidedTourDone()
        {
            var user = await Repository.GetAsync(AbpSession.UserId.Value);
            user.TookGuidedTour = true;
            await Repository.UpdateAsync(user);
        }
    }
}
