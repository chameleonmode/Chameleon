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
    public class UserProfileAddressRepository 
        : UserProfileItemRepository<
            UserProfileAddress,
            IUserProfileAddress,
            UserProfileAddressDto,
            CreateUserProfileAddressDto,
            UserProfileAddressDto
            >
        , IUserProfileAddressRepository
    {
        public UserProfileAddressRepository(
            IMapper mapper,
            IUserProfileApiAddress apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfileAddress entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Addresses.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfileAddress entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Addresses.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfileAddress entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Addresses.AddIfMissing(entity);
        }
    }
}
