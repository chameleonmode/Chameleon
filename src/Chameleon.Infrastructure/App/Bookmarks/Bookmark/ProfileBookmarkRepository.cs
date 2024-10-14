using AutoMapper;

using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Interfaces.Bookmarks;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;

using System;
using System.Collections.Concurrent;

namespace Chameleon.Infrastructure.Bookmarks {
	public class ProfileBookmarkRepository
    : UserProfileItemRepository<
           ProfileBookmark,
           IProfileBookmark,
           ProfileBookmarkDto,
           CreateProfileBookmarkDto,
           ProfileBookmarkDto>
       , IProfileBookmarkRepository
    {
        public ProfileBookmarkRepository(
           IMapper mapper,
           IProfileBookmarkApi apiClient,
           IEventAggregator eventAggregator,
           IUserProfileRepository profileRepository
           ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        protected override bool TryAddEntity(IProfileBookmark entity)
        {
            if (entity.BookmarkType == BookmarkType.GlobalFolder)
            {
                if (IsEntityLoadedById(entity.Id))
                {
                    return false;
                }
            }
            return base.TryAddEntity(entity);
        }

        protected override bool FilterLoadedByProfileId(IProfileBookmark entity, int profileId)
        {
            return base.FilterLoadedByProfileId(entity, profileId)
                || entity.BookmarkType == BookmarkType.GlobalFolder;
        }
    }
}
