using AutoMapper;

using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;

using System.Collections.Generic;

namespace Chameleon.Infrastructure.Profiles {
	public class UserProfileRepository
        : Repository<UserProfile,
            IUserProfile,
            int,
            UserProfileDto,
            CreateUserProfileDto,
            UserProfileDto,
            GetAllRequestDto
            >
        , IUserProfileRepository
    {
        private new readonly IMapper _mapper;
        private readonly IUserProfileApi _client;

        public UserProfileRepository(
            IMapper mapper,
            IUserProfileApi apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
            _client = apiClient;
            _mapper = mapper;
        }

        public IUserProfile[] GetAllByUserId(int id)
        {
            try
            {
                var r = _client.GetAllByUserId(id);
                var res = new List<IUserProfile>();

                foreach (var profile in r)
                {
                    var userProfile = new UserProfile
                    {
                        Id = profile.Id,
                        Title = profile.Title
                    };

                    res.Add(userProfile);
                }

                var rrr = res.ToArray();

                //return rrr;
                var ress = _mapper.Map<IUserProfile[]>(r);

                return ress;
            }
            catch (System.Exception ex)
            {

                var rr = ex;
            }
            
            return null;
        }

        public void MoveUserProfileToFolder(List<int> profileIds, int? foldeId)
        {
            var dto = new MoveUserProfileFromFolderDto
            {
                ProfileIds = profileIds,
                FolderId = foldeId
            };

            _client.Execute("MoveUserProfileToFolder", dto);
            foreach (var profileId in profileIds)
            {
                Entities[profileId].FolderId = foldeId;
            }
            TriggerEntityEvent<UpdateEntityEvent>();
        }

        public void SetProfileIsFavorite(int profileId, bool isFavorite)
        {
            var dto = new FavoriteProfileDto
            {
                ProfileId = profileId,
                IsFavorite = isFavorite
            };

            _client.Execute("SetProfileIsFavorite", dto);
            var entity = Entities[profileId];
            entity.IsFavourite = isFavorite;
            TriggerEntityEvent<UpdateEntityEvent>();
            OnSaved(entity);
        }

        public void SetProfileCacheLimit(int profileId, double limitCache)
        {
            var dto = new ProfileCacheLimitDto
            {
                ProfileId = profileId,
                LimitCache = limitCache
            };

            _client.Execute("SetProfileCacheLimit", dto);
            var entity = Entities[profileId];
            entity.LimitCache = limitCache;
            TriggerEntityEvent<UpdateEntityEvent>();
            OnSaved(entity);
        }

        protected override void OnSaved(IUserProfile entity)
        {
            base.OnSaved(entity);

            _eventAggregator
                .GetEvent<SavedUserProfileEvent>()
                .Publish(new UserProfileEventArgs(entity));
        }
    }
}
