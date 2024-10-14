using AutoMapper;

using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.Prospector.Api;
using Chameleon.Infrastructure.Prospector.Api.Dto;
using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Infrastructure.Prospector {
	public class UserProfileProspectorBlogsOfInterestRepository
       : UserProfileItemRepository<
           UserProfileProspectorBlogsOfInterest,
           IUserProfileProspectorBlogsOfInterest,
           UserProfileProspectorBlogsOfInterestDto,
           CreateUserProfileProspectorBlogsOfInterestDto,
           UserProfileProspectorBlogsOfInterestDto>
       , IUserProfileProspectorBlogsOfInterestRepository
    {
        public UserProfileProspectorBlogsOfInterestRepository(
            IMapper mapper,
            IUserProfileApiProspector apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfileProspectorBlogsOfInterest entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.ProspectorBlogsOfInterest.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfileProspectorBlogsOfInterest entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.ProspectorBlogsOfInterest.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfileProspectorBlogsOfInterest entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.ProspectorBlogsOfInterest.AddIfMissing(entity);
        }
    }
}
