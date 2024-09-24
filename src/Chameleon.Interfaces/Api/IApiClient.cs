using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Api
{
    public interface IApiClient : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        TResponse Get<TResponse>(string url, object query = null);
        Task<TResponse> GetAsync<TResponse>(string url, object query = null);
        TResponse Post<TResponse>(string url, object body = null);
        Task<TResponse> PostAsync<TResponse>(string url, object body = null);
        void Post(string url, object body = null);
        void Put(string url, object body = null);
        void Delete(string url);
    }
}
