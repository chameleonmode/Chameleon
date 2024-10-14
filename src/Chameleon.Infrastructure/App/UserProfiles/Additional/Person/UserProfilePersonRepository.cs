using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.UserProfiles.Api.Additional;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;

using Chameleon.Core.Extensions;
using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Infrastructure.UserProfiles {
	public class UserProfilePersonRepository
        : UserProfileItemRepository<
            UserProfilePerson,
            IUserProfilePerson,
            UserProfilePersonDto,
            CreateUserProfilePersonDto,
            UserProfilePersonDto>
        , IUserProfilePersonRepository
    {
        public UserProfilePersonRepository(
            IMapper mapper,
            IUserProfileApiPerson apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfilePerson entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Persons.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfilePerson entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Persons.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfilePerson entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.Persons.AddIfMissing(entity);
        }
    }
}
