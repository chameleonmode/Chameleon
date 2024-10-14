using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Interfaces.UserProfiles;

using System.Collections.Generic;
using Chameleon.Core.Extensions;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Infrastructure.OutReach.Api;
using Chameleon.Interfaces.OutReach;
using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Infrastructure.OutReach {
	public class UserProfileOutReachRepository
       : UserProfileItemRepository<
           UserProfileOutReachRss,
           IUserProfileOutReachRss,
           UserProfileOutReachRssDto,
           CreateUserProfileOutReachRssDto,
           UserProfileOutReachRssDto>
       , IUserProfileOutReachRssRepository
    {
        public UserProfileOutReachRepository(
            IMapper mapper,
            IUserProfileApiOutReachRss apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override void OnDeleted(IUserProfileOutReachRss entity)
        {
            base.OnDeleted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.OutReachRsses.Remove(item => item.Id == entity.Id);
        }

        protected override void OnInserted(IUserProfileOutReachRss entity)
        {
            base.OnInserted(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.OutReachRsses.AddIfMissing(entity);
        }

        protected override void OnGet(IUserProfileOutReachRss entity)
        {
            base.OnGet(entity);

            if (TryGetProfile(entity, out IUserProfile profile))
                profile.OutReachRsses.AddIfMissing(entity);
        }
    }
}
