using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Services;
using System.Collections.Generic;
using Chameleon.Core.Extensions;

namespace Chameleon.Infrastructure.App.OutReach.OutReachRss;

public class UserProfileOutReachRssesService
				: IUserProfileOutReachRssesService {
	private readonly IMapper _mapper;
	private readonly IUserProfileOutReachRssRepository _outReachRssRepository;
	private readonly Dictionary<int, UserProfileOutReachRsses> _entities
			= [];

	public UserProfileOutReachRssesService(
			IMapper mapper
			, IUserProfileOutReachRssRepository outReachRssRepository)
	{
		_mapper = mapper;
		_outReachRssRepository = outReachRssRepository;
	}
	public IUserProfileOutReachRsses GetAll(int profileId)
	{
		if (_entities.TryGetValue(profileId, out var outReachRsses)) {
			return outReachRsses;
		}

		var request = new UserProfileGetAllRequestDto(profileId);
		var items = _outReachRssRepository.GetAll(request);
		outReachRsses = new UserProfileOutReachRsses(profileId, items);
		_entities[profileId] = outReachRsses;
		return outReachRsses;
	}

	public IUserProfileOutReachRss AddOutReachRss(UserProfileOutReachRssBindable outReachRss)
	{
		EnsureEntitiesContainsProfileId(outReachRss.ProfileId);
		var outreachRss = _mapper.Map<UserProfileOutReachRss>(outReachRss);
		_outReachRssRepository.Insert(outreachRss);
		_entities[outReachRss.ProfileId].Add(outreachRss);
		return outreachRss;
	}

	public void SaveOutReachRss(UserProfileOutReachRssBindable outReachRss)
	{
		//var outreachRss = _mapper.Map<UserProfileOutReachRss>(outReachRss);
		//_dipatcherService.InvokeOnUiThread(() => _outReachRssRepository.Update(outreachRss));
		throw new NotImplementedException();
	}

	public void DeleteOutReachRss(UserProfileOutReachRssBindable outReachRss)
	{
		//var outreachRss = _mapper.Map<UserProfileOutReachRss>(outReachRss);
		//_outReachRssRepository.Delete(outReachRss.Id);
		//_dipatcherService.InvokeOnUiThread(() => _entities[outReachRss.ProfileId].Remove(x => x.Id == outreachRss.Id));
		throw new NotImplementedException();
	}

	private void EnsureEntitiesContainsProfileId(int profileId)
	{
		if (!_entities.ContainsKey(profileId)) {
			_entities[profileId] = new UserProfileOutReachRsses(profileId, GetAll(profileId));
		}
	}
}
