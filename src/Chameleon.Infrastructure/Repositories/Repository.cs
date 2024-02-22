using AutoMapper;
using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Chameleon.Infrastructure.Dto;
using Chameleon.Interfaces.Entities;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Chameleon.Infrastructure.Repositories
{
    //<,,,,,,>
    public class StaticRepositoryFactory
    {
        public static IRepository<T1, T2, T3> Create<T1, T2, T3>() where T3 : GetAllRequestDto
        {
            return new Abstraction<T1, T2, T3>();
        }
        public static IRepository<T1> CreateOne<T1>()
        {
            return new Abstraction<T1>();
        }
    }
    public abstract class Repository<
        TEntity, TEntityInterface, TPrimaryKey,
        TEntityDto, TCreateInput, TUpdateInput,
        TGetAllRequest
        >
        : IRepository<TEntityInterface, TPrimaryKey, TGetAllRequest>
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllRequest : GetAllRequestDto, new()
    {
        protected readonly IMapper _mapper;
        protected readonly IEventAggregator _eventAggregator;
        protected readonly IApiLayer<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput> _apiClient;

        protected ConcurrentDictionary<TPrimaryKey, TEntityInterface> _entities;
        protected ConcurrentDictionary<TPrimaryKey, TEntityInterface> Entities
        {
            get
            {
                if (_entities == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(GetAll)} was not previously called");
                }
                return _entities;
            }
        }

        public Repository(
            IMapper mapper,
            IApiLayer<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput> apiClient,
            IEventAggregator eventAggregator
            )
        {
            _mapper = mapper;
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
        }

        public virtual void Delete(TPrimaryKey id)
        {
            ThrowIfDefaultKey(id);
            TEntityInterface entity;
            if (!Entities.ContainsKey(id))
            {
                throw new KeyNotFoundException(id.ToString());
            }

            _apiClient.Delete(id);
            if (!Entities.TryRemove(id, out entity))
            {
                throw new InvalidOperationException();
            }
            OnDeleted(entity);
        }

        private void ThrowIfDefaultKey(TPrimaryKey id)
        {
            if (Equals(id, default(TPrimaryKey)))
            {
                throw new ArgumentException("Invalid primary key/id");
            }
        }

        protected virtual void OnDeleted(TEntityInterface entity)
        {
            TriggerDeleteEvent(entity);
        }

        protected virtual void TriggerDeleteEvent(TEntityInterface entity)
        {
            TriggerEntityEvent<DeleteEntityEvent>(entity);
        }

        public virtual TEntityInterface Get(TPrimaryKey id)
        {
            var entity = FindById(id);
            var entityDto = _apiClient.Get(id);
            _mapper.Map(entityDto, entity);
            return entity;
        }

        protected virtual TEntityInterface FindById(TPrimaryKey id)
        {
            if (Entities.TryGetValue(id, out var entity))
            {
                return entity;
            }
            throw new KeyNotFoundException(id.ToString());
        }

        public virtual TEntityInterface[] GetAll(TGetAllRequest request = null)
        {
            if (_entities != null)
            {
                return GetEntities();
            }
            _entities = new ConcurrentDictionary<TPrimaryKey, TEntityInterface>();

            var entities = GetEntities(request);
            foreach (var entity in entities)
            {
                if (!_entities.TryAdd(entity.Id, entity))
                {
                    throw new InvalidOperationException();
                }
                OnGet(entity);
            }
            return GetEntities();
        }

        public virtual TEntityInterface[] GetAll(bool ignoreCache, TGetAllRequest request = null)
        {
            if (ignoreCache)
            {
                _entities?.Clear();
                _entities = null;
            }

            if (_entities != null)
            {
                return GetEntities();
            }
            _entities = new ConcurrentDictionary<TPrimaryKey, TEntityInterface>();

            var entities = GetEntities(request);
            foreach (var entity in entities)
            {
                if (!_entities.TryAdd(entity.Id, entity))
                {
                    throw new InvalidOperationException();
                }
                OnGet(entity);
            }
            return GetEntities();
        }

        private TEntityInterface[] GetEntities()
        {
            return _entities.Values.ToArray();
        }

        protected virtual void OnGet(TEntityInterface entity)
        {
        }

        public virtual TPrimaryKey Insert(TEntityInterface entity)
        {
            var createDto = _mapper.Map<TCreateInput>(entity);
            var entityDto = _apiClient.Insert(createDto);
            _mapper.Map(entityDto, entity);

            ThrowIfDefaultKey(entity.Id);
            if (!Entities.TryAdd(entity.Id, entity))
            {
                throw new InvalidOperationException();
            }

            OnInserted(entity);
            OnSaved(entity);
            return entity.Id;
        }

        protected virtual void OnInserted(TEntityInterface entity)
        {
            TriggerInsertEvent(entity);
        }

        protected virtual void TriggerInsertEvent(TEntityInterface entity)
        {
            TriggerEntityEvent<InsertEntityEvent>(entity);
        }

        public virtual void Update(TEntityInterface entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var entityId = entity.Id;
            ThrowIfDefaultKey(entityId);

            var existingEntity = FindById(entityId);
            var existingEntityDto = _mapper.Map<TUpdateInput>(entity);

            _apiClient.Update(existingEntityDto);
            if (!Equals(entity, existingEntity))
            {
                _mapper.Map(existingEntityDto, existingEntity);
            }

            OnUpdated(existingEntity);
            OnSaved(existingEntity);
        }

        protected virtual void OnUpdated(TEntityInterface entity)
        {
            TriggerUpdateEvent(entity);
        }

        protected virtual void TriggerUpdateEvent(TEntityInterface entity)
        {
            TriggerEntityEvent<UpdateEntityEvent>(entity);
        }

        protected bool _suppressEventTrigger;
        protected virtual void TriggerEntityEvent<TEvent>(params TEntityInterface[] entities)
            where TEvent : PubSubEvent<EntityEventArgs>, new()
        {
            if (_suppressEventTrigger)
            {
                return;
            }

            _eventAggregator
               .GetEvent<TEvent>()
               .Publish(new EntityEventArgs(entities));
        }

        protected virtual void OnSaved(TEntityInterface entity)
        {
        }

        public void Upsert(UpsertItems<TEntityInterface> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            if (input.IsEmpty)
            {
                return;
            }

            _suppressEventTrigger = true;
            try
            {
                var upsertItems = input.Upserted;
                var upsertTask = Task.Run(() => Upsert(upsertItems));

                var deletedItems = input.Deleted;
                var deleteTask = Task.Run(() => Delete(deletedItems));
                Task.WaitAll(upsertTask, deleteTask);

                _suppressEventTrigger = false;
                TriggerEntityEvent<SavedEntityEvent>(upsertItems);
            }
            finally
            {
                _suppressEventTrigger = false;
            }
        }

        private void Upsert(IList<TEntityInterface> entities)
        {
            Parallel.ForEach(entities, entity =>
            {
                if (!Equals(entity.Id, default(TPrimaryKey)))
                {
                    Update(entity);
                }
                else
                {
                    Insert(entity);
                }
            });
        }

        private void Delete(IList<TEntityInterface> entities)
        {
            if (entities.Count != 0)
            {
                Parallel.ForEach(entities, entity => Delete(entity.Id));
            }
        }

        protected virtual List<TEntityInterface> GetEntities(TGetAllRequest request = null)
        {
            if (request == null)
            {
                request = new TGetAllRequest();
            }

            var dtos = _apiClient.GetAll(request);
            var entities = new List<TEntityInterface>();
            foreach (var dto in dtos)
            {
                var entity = new TEntity();
                _mapper.Map(dto, entity);
                entities.Add(entity);
            }
            return entities;
        }
    }

    public abstract class Repository<
        TEntity, TEntityInterface, TPrimaryKey,
        TEntityDto, TCreateInput
        >
        : Repository<
            TEntity, TEntityInterface, TPrimaryKey,
            TEntityDto, TCreateInput, TEntityDto,
            GetAllRequestDto>
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        public Repository(
            IMapper mapper,
            IApiLayer<TEntityDto, TPrimaryKey, TCreateInput, TEntityDto> apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }

    public abstract class Repository<
        TEntity, TEntityInterface, TPrimaryKey, TEntityDto>
        : Repository<
            TEntity, TEntityInterface, TPrimaryKey,
            TEntityDto, TEntityDto, TEntityDto, GetAllRequestDto>
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        public Repository(
            IMapper mapper,
            IApiLayer<TEntityDto, TPrimaryKey, TEntityDto, TEntityDto> apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }

    public abstract class Repository<
        TEntity, TEntityInterface, TEntityDto>
        : Repository<
            TEntity, TEntityInterface, int,
            TEntityDto, TEntityDto, TEntityDto, GetAllRequestDto>
        where TEntity : TEntityInterface, new()
        where TEntityInterface : IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        public Repository(
           IMapper mapper,
           IApiLayer<TEntityDto, int, TEntityDto, TEntityDto> apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
