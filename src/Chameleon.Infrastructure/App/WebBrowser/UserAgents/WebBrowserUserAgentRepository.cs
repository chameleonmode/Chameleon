using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.WebBrowsers
{
    public class WebBrowserUserAgentRepository 
        : IWebBrowserUserAgentRepository
    {
        private readonly IMapper _mapper;
        private readonly IWebBrowserUserAgentApi _apiClient;
        
        public WebBrowserUserAgentRepository(
            IWebBrowserUserAgentApi apiClient,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _apiClient = apiClient;
        }

        private List<IWebBrowserUserAgent> _items;
        private List<IWebBrowserUserAgent> Items
        {
            get
            {
                if (_items != null)
                {
                    return _items;
                }

                var userAgetsDtos = _apiClient.GetAll();
                var userAgents = _mapper.Map<WebBrowserUserAgent[]>(userAgetsDtos);
                _items = new List<IWebBrowserUserAgent>(userAgents);
                return _items;
            }
        }

        public IWebBrowserUserAgent[] GetAll(GetAllRequestDto request)
        {
            return Items.ToArray();
        }

        public IWebBrowserUserAgent Get(int id)
        {
            var userAgent = Items
                .FirstOrDefault(item => item.Id == id);
            if (userAgent == null)
            {
                throw new KeyNotFoundException(id.ToString());
            }
            return userAgent;
        }

        public void Delete(int id)
        {
            var userAgent = Items
                .FirstOrDefault(item => item.Id == id);

            if(userAgent == null)
            {
                throw new KeyNotFoundException(id.ToString());
            }

            _apiClient.Delete(id);
            Items.Remove(userAgent);
        }

        public void Update(IWebBrowserUserAgent browserAgent)
        {
            var userAgent = Items
                .FirstOrDefault(item => item.Id == browserAgent.Id);

            if(userAgent.Value != browserAgent.Value)
            {
                var userAgentDto = _mapper.Map<IWebBrowserUserAgent, WebBrowserUserAgentDto>(browserAgent);
                _apiClient.Update(userAgentDto);
            }
        }

        public int Insert(IWebBrowserUserAgent entity)
        {
            var userAgentDto = _mapper.Map<IWebBrowserUserAgent, CreateWebBrowserUserAgentDto>(entity);

            var resposneDto = _apiClient.Insert(userAgentDto);
            if (resposneDto.Id <= 0)
            {
                throw new InvalidOperationException();
            }

            entity.Id = resposneDto.Id;
            Items.Add(entity);
            return entity.Id;
        }

        public void Upsert(UpsertItems<IWebBrowserUserAgent> entities)
        {
            throw new NotImplementedException();
        }

        public IWebBrowserUserAgent[] GetAll(bool ignoreCache, GetAllRequestDto request = null)
        {
            throw new NotImplementedException();
        }
    }
}
