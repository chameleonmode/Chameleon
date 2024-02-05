using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.UserProfiles.Api.Additional;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using Prism.Events;
using Chameleon.Core.Extensions;

namespace Chameleon.Infrastructure.UserProfiles
{
    public class UserProfileBusinessRepository
        : UserProfileItemRepository<
            UserProfileBusiness,
            IUserProfileBusiness,
            UserProfileBusinessDto,
            CreateUserProfileBusinessDto,
            UserProfileBusinessDto
            >
        , IUserProfileBusinessRepository
    {
        public UserProfileBusinessRepository(
            IMapper mapper,
            IUserProfileApiBusiness apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfileBusiness entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Businesses.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfileBusiness entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Businesses.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfileBusiness entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Businesses.AddIfMissing(entity);
        }
    }
}

