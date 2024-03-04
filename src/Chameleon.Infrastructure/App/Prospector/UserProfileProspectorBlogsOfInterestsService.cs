using AutoMapper;
using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Prospector;

public class UserProfileProspectorBlogsOfInterestsService
     : IUserProfileProspectorBlogsOfInterestsService
{
    private readonly IMapper _mapper;
    private readonly IUserProfileProspectorBlogsOfInterestRepository _prospectorBlogsOfInterestRepository;
    private readonly Dictionary<int, UserProfileProspectorBlogsOfInterests> _entities
        = new Dictionary<int, UserProfileProspectorBlogsOfInterests>();

    public UserProfileProspectorBlogsOfInterestsService(
        IMapper mapper
        , IUserProfileProspectorBlogsOfInterestRepository prospectorBlogsOfInterestRepository)
    {
        _mapper = mapper;
        _prospectorBlogsOfInterestRepository = prospectorBlogsOfInterestRepository;
    }
    public IUserProfileProspectorBlogsOfInterests GetAll(int profileId)
    {
        if (_entities.TryGetValue(profileId, out var prospectorBlogsOfInterests))
        {
            return prospectorBlogsOfInterests;
        }

        var request = new UserProfileGetAllRequestDto(profileId);
        var items = _prospectorBlogsOfInterestRepository.GetAll(request);
        prospectorBlogsOfInterests = new UserProfileProspectorBlogsOfInterests(profileId, items);
        _entities[profileId] = prospectorBlogsOfInterests;
        return prospectorBlogsOfInterests;
    }

    public IUserProfileProspectorBlogsOfInterest AddProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest prospectorBlogsOfInterest)
    {
        EnsureEntitiesContainsProfileId(prospectorBlogsOfInterest.ProfileId);
        var blogsOfInterest = _mapper.Map<UserProfileProspectorBlogsOfInterest>(prospectorBlogsOfInterest);
        _prospectorBlogsOfInterestRepository.Insert(blogsOfInterest);
        _entities[blogsOfInterest.ProfileId].Add(blogsOfInterest);
        return blogsOfInterest;
    }

    public void SaveProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest prospectorBlogsOfInterest)
    {
        var blogsOfInterest = _mapper.Map<UserProfileProspectorBlogsOfInterest>(prospectorBlogsOfInterest);
        this.InvokeOnUiThread(() => _prospectorBlogsOfInterestRepository.Update(blogsOfInterest));
    }

    public void DeleteProspectorBlogsOfInterest(IUserProfileProspectorBlogsOfInterest prospectorBlogsOfInterest)
    {
        var blogsOfInterest = _mapper.Map<UserProfileProspectorBlogsOfInterest>(prospectorBlogsOfInterest);
        _prospectorBlogsOfInterestRepository.Delete(blogsOfInterest.Id);
        this.InvokeOnUiThread(() => _entities[blogsOfInterest.ProfileId].Remove(x => x.Id == blogsOfInterest.Id));
    }

    private void EnsureEntitiesContainsProfileId(int profileId)
    {
        if (!_entities.ContainsKey(profileId))
        {
            _entities[profileId] = new UserProfileProspectorBlogsOfInterests(profileId, GetAll(profileId));
        }
    }
}
