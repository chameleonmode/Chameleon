using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class NativeApiClient
    {
        private readonly IAuthSession _session;
        protected readonly IApplicationConfiguration _configuration;
        public NativeApiClient(
            IAuthSession session,
            IApplicationConfiguration configuration
            )
        {
            _session = session;
            _configuration = configuration;
        }

        public virtual TResponse Get<TResponse>(string url, object query = null)
        {
            //_toaster.ShowInformation($"get-{url}");
            var r = CreateGetRequest()
                .ForUrl(url)
                .WithQuery(query)
                .Send()
                .GetResult<TResponse>();
            //_toaster.ShowSuccess($"get-{url}");
            return r;
        }
        public virtual async Task<TResponse> GetAsync<TResponse>(string url, object query = null)
        {
            return await Task.Run(()=> { return Get<TResponse>(url, query); });
        }

        protected virtual ApiPostRequest CreatePostRequest() => 
            new ApiPostRequest(_session, _configuration);

        protected virtual ApiGetRequest CreateGetRequest() => 
            new ApiGetRequest(_session, _configuration);

        public virtual TResponse Post<TResponse>(string url, object body = null)
        {
            //_toaster.ShowInformation($"post-{url}");
            var result = CreatePostRequest()
                .ForUrl(url)
                .WithBody(body)
                .Send()
                .GetResult<TResponse>();
            //_toaster.ShowSuccess($"post-{url}");
            return result;
        }
        public virtual async Task<TResponse> PostAsync<TResponse>(string url, object query = null)
        {
            return await Task.Run(() => { return Post<TResponse>(url, query); });
        }

        public virtual void Post(string url, object body = null)
        {
            new ApiPostRequest(_session, _configuration)
                .ForUrl(url)
                .WithBody(body)
                .Send()
                .GetResult();
        }

        public virtual void Put(string url, object body = null)
        {
            //_toaster.ShowInformation($"put-{url}");
            new ApiPutRequest(_session, _configuration)
                .ForUrl(url)
                .WithBody(body)
                .Send()
                .GetResult();
            //_toaster.ShowSuccess($"put-{url}");
        }

        public virtual void Delete(string url)
        {
            //_toaster.ShowInformation($"delete-{url}");
            new ApiDeleteRequest(_session, _configuration)
                .ForUrl(url)
                .Send()
                .GetResult();
            //_toaster.ShowSuccess($"delete-{url}");
        }
    }
}
