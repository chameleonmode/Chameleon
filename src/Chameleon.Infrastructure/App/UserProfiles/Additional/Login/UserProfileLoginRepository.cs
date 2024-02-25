using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.UserProfiles.Api.Additional;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;

using Chameleon.Core.Extensions;

namespace Chameleon.Infrastructure.UserProfiles
{
    public class UserProfileLoginRepository
        : UserProfileItemRepository<
            UserProfileLogin,
            IUserProfileLogin,
            UserProfileLoginDto,
            CreateUserProfileLoginDto,
            UserProfileLoginDto>
        , IUserProfileLoginRepository
    {
        public UserProfileLoginRepository(
            IMapper mapper,
            IUserProfileApiLogin apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfileLogin entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Logins.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfileLogin entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Logins.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfileLogin entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Logins.AddIfMissing(entity);
        }
    }
}
