using Chameleon.Infrastructure.Dto;
using Chameleon.Interfaces.Api;
using System;

namespace Chameleon.Infrastructure.Api
{
    public class PagedResultRequestDto
    {
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; }
    }

    public class ApiLayer<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected readonly IApiClient _apiClient;
        protected readonly string _apiEndpoint;

        protected ApiLayer(IApiClient apiClient, string apiEndpoint)
        {
            _apiClient = apiClient;
            _apiEndpoint = $"services/app/{apiEndpoint}";
        }

        public virtual void Delete(TPrimaryKey id)
        {
            _apiClient.Delete(GetEndpointUrl($"Delete?Id={id}"));
        }

        public virtual TEntityDto Get(TPrimaryKey id)
        {
            var dto = _apiClient.Get<TEntityDto>(GetEndpointUrl($"Get?Id={id}"));
            ThrowIfInvalidId(dto);
            return dto;
        }

        public virtual TEntityDto Insert(TCreateInput input)
        {
            var dto = _apiClient.Post<TEntityDto>(GetEndpointUrl("Create"), input);
            ThrowIfInvalidId(dto);
            return dto;
        }

        public virtual void Update(TUpdateInput input)
        {
            ThrowIfInvalidId(input);
            _apiClient.Put(GetEndpointUrl("Update"), input);
        }

        public virtual TEntityDto[] GetAll(object query = null)
        {
            if (query == null)
            {
                query = new PagedResultRequestDto
                {
                    MaxResultCount = int.MaxValue
                };
            }
            return _apiClient.Get<TEntityDto[]>(GetEndpointUrl("GetAll"), query);
        }

        protected string GetEndpointUrl(string endpointName)
        {
            return $"{_apiEndpoint}/{endpointName}";
        }

        protected void ThrowIfInvalidId<TEntityPrimaryKey>(IEntityDto<TEntityPrimaryKey> entity)
        {
            if (Equals(entity.Id, default(TEntityPrimaryKey)))
            {
                throw new InvalidOperationException("Invalid id");
            }
        }
    }

    public class ApiLayer<TEntityDto, TPrimaryKey, TOperableInput>
        : ApiLayer<TEntityDto, TPrimaryKey, TOperableInput, TOperableInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TOperableInput : IEntityDto<TPrimaryKey>
    {
        protected ApiLayer(IApiClient apiClient, string apiEndpoint)
            : base(apiClient, apiEndpoint)
        {
        }
    }

    public class ApiLayer<TEntityDto, TPrimaryKey>
        : ApiLayer<TEntityDto, TPrimaryKey, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected ApiLayer(IApiClient apiClient, string apiEndpoint)
            : base(apiClient, apiEndpoint)
        {
        }
    }

    public class ApiLayer<TEntityDto>
        : ApiLayer<TEntityDto, int>
        where TEntityDto : IEntityDto<int>
    {
        protected ApiLayer(IApiClient apiClient, string apiEndpoint)
            : base(apiClient, apiEndpoint)
        {
        }
    }
}
