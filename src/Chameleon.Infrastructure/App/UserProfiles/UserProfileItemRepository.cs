using AutoMapper;
using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.Entities;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfiles;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.Profiles
{
    public class StaticUPFactory
    {
        public static UserProfileItemRepository<
        TEntity, TEntityInterface,
        TEntityDto, TCreateInput, TUpdateInput>
            Create<TEntity, TEntityInterface,TEntityDto, TCreateInput, TUpdateInput>()
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IUserProfileRequiredEntity
        where TEntityDto : IEntityDto<int>
        where TUpdateInput : IEntityDto<int>
        {
            return new UserProfileItemRepository<
        TEntity, TEntityInterface,
        TEntityDto, TCreateInput, TUpdateInput
        >();
        }
    }
    public class UserProfileItemRepository<
        TEntity, TEntityInterface,
        TEntityDto, TCreateInput, TUpdateInput
        >
        : Repository<TEntity,
            TEntityInterface,
            int,
            TEntityDto,
            TCreateInput,
            TUpdateInput,
            UserProfileGetAllRequestDto
            >
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IUserProfileRequiredEntity
        where TEntityDto : IEntityDto<int>
        where TUpdateInput : IEntityDto<int>
    {
        private readonly HashSet<int> _getAllRequested = new HashSet<int>();
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileItemRepository(
            IMapper mapper,
            IApiLayer<TEntityDto, int, TCreateInput, TUpdateInput> apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository userProfileRepository
            ) : base(mapper, apiClient, eventAggregator)
        {
            _userProfileRepository = userProfileRepository;
        }

        public UserProfileItemRepository() : base(null, null, null)
        {
        }

        public override int Insert(TEntityInterface entity)
        {
            EnsureEntitiesLoaded(entity.ProfileId);
            return base.Insert(entity);
        }

        private void EnsureEntitiesLoaded(int profileId)
        {
            if (_getAllRequested.Contains(profileId))
            {
                return;
            }

            GetAllOrRequest(
                new UserProfileGetAllRequestDto(profileId)
                );
        }

        public override TEntityInterface[] GetAll(UserProfileGetAllRequestDto request = null)
        {
            request = request ?? new UserProfileGetAllRequestDto();
            if (request.ProfileId.HasValue)
            {
                return GetAllOrRequest(request);
            }
            throw new NotImplementedException();
        }
        public override TEntityInterface[] GetAll(bool ignoreCache, UserProfileGetAllRequestDto request = null)
        {
            request = request ?? new UserProfileGetAllRequestDto();
            if (request.ProfileId.HasValue)
            {
                return GetAllOrRequest(request, ignoreCache);
            }
            throw new NotImplementedException();
        }

        private TEntityInterface[] GetAllOrRequest(UserProfileGetAllRequestDto request, bool ignoreCache = false)
        {
            if (_entities == null)
            {
                _entities = new ConcurrentDictionary<int, TEntityInterface>();
            }

            var profileId = request.ProfileId.Value;

            if (ignoreCache)
            {
                _entities.Clear();
                _getAllRequested.Remove(profileId);
            }

            if (!_getAllRequested.Contains(profileId))
            {
                GetAllByProfileId(request);
                _getAllRequested.Add(profileId);

            }
            return GetLoadedByProfileId(profileId);
        }

        private TEntityInterface[] GetLoadedByProfileId(int profileId)
        {
            return _entities
                .Where(pair => FilterLoadedByProfileId(pair.Value, profileId))
                .Select(pair => pair.Value)
                .ToArray();
        }

        protected virtual bool FilterLoadedByProfileId(TEntityInterface entity, int profileId)
        {
            return entity.ProfileId == profileId;
        }

        private void GetAllByProfileId(UserProfileGetAllRequestDto request)
        {
            var entities = GetEntities(request);
            foreach (var entity in entities)
            {
                if (TryAddEntity(entity))
                {
                    OnGet(entity);
                }
            }
        }

        protected virtual bool TryAddEntity(TEntityInterface entity)
        {
            if (_entities.TryAdd(entity.Id, entity))
            {
                return true;
            }
            throw new InvalidOperationException();
        }

        protected bool IsEntityLoadedById(int entityId)
        {
            return _entities.ContainsKey(entityId);
        }

        protected IUserProfile GetProfile(TEntityInterface entity)
        {
            return _userProfileRepository.Get(entity.ProfileId);
        }
        protected bool TryGetProfile(TEntityInterface entity, out IUserProfile profile)
        {
            try
            {
                profile = _userProfileRepository.Get(entity.ProfileId);
            }
            catch 
            {
                profile = null;
                return false;
            }
            return true;
        }
    }
}
