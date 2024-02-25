using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class ProfileOutReachLinkRepository
        : UserProfileItemRepository<
           ProfileOutReachLink,
           IProfileOutReachLink,
           ProfileOutReachLinkDto,
           CreateProfileOutReachLinkDto,
           ProfileOutReachLinkDto>
       , IProfileOutReachLinkRepository
    {
        public ProfileOutReachLinkRepository(
           IMapper mapper,
           IProfileOutReachLinkApi apiClient,
           IEventAggregator eventAggregator,
           IUserProfileRepository profileRepository
           ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }

        public List<IProfileOutReachLink> GetAll(DateTime reminderDate)
        {
            var entities = GetEntities()
                .Where(a => a.ReminderDatetime != null)
                .Where(a => a.ReminderDatetime.Value.Date == reminderDate)
                .ToList();

            return entities;
        }
    }
}
