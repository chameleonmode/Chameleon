using AutoMapper;
using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Repository;
using System;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class ProfileOutReachLinkService : IProfileOutReachLinkService
    {
        private readonly IMapper _mapper;
        private readonly IProfileOutReachLinkRepository _outReachLinkRepository;
        private readonly Dictionary<int, ProfileOutReachLinks> _entities
            = new Dictionary<int, ProfileOutReachLinks>();

        public ProfileOutReachLinkService(
           IMapper mapper,
           IProfileOutReachLinkRepository outReachLinkRepository)
        {
            _mapper = mapper;
            _outReachLinkRepository = outReachLinkRepository;
        }

        public IProfileOutReachLinks GetAll(int profileId)
        {
            if (_entities.TryGetValue(profileId, out var outReachLinks))
            {
                return outReachLinks;
            }

            var request = new UserProfileGetAllRequestDto(profileId);
            var items = _outReachLinkRepository.GetAll(request);
            outReachLinks = new ProfileOutReachLinks(profileId, items);
            _entities[profileId] = outReachLinks;
            return outReachLinks;
        }

        public IProfileOutReachLink Save(IProfileOutReachLink outReachLink)
        {
            EnsureEntitiesContainsProfileId(outReachLink.ProfileId);
            var outreachLink = _mapper.Map<ProfileOutReachLink>(outReachLink);
            _outReachLinkRepository.Insert(outreachLink);
            _entities[outReachLink.ProfileId].Add(outreachLink);
            return outreachLink;
        }

        public void Delete(IProfileOutReachLink outReachLink)
        {
            _outReachLinkRepository.Delete(outReachLink.Id);
            this.InvokeOnUiThread(() => _entities[outReachLink.ProfileId].Remove(x => x.Id == outReachLink.Id));
        }

        public void Update(IProfileOutReachLink outReachLink)
        {
            var outreachLink = _mapper.Map<ProfileOutReachLink>(outReachLink);
            this.InvokeOnUiThread(() => _outReachLinkRepository.Update(outreachLink));
        }

        private void EnsureEntitiesContainsProfileId(int profileId)
        {
            if (!_entities.ContainsKey(profileId))
            {
                _entities[profileId] = new ProfileOutReachLinks(profileId, GetAll(profileId));
            }
        }

        public IProfileOutReachLinks GetAllByReminderDate(DateTime reminderDate)
        {
            var items = _outReachLinkRepository.GetAll(reminderDate);
            var outReachLinks = new ProfileOutReachLinks(items);
            return outReachLinks;
        }
    }
}
