using Chameleon.Interfaces.UserProfiles;
using System.Net;


namespace Chameleon.SystemBrowser.Proxy
{
    public class DynamicProxyServerFactory
    {
        public static DynamicProxyServer Create(IProxySettings proxy)
        {
            var hostName = HostConverter.GetHostForRequest(proxy.Host);
            // Create a new proxy instance 
           // await using var fProxy = new Fluxzy.Proxy(FluxzySetting.CreateDefault());

            // Proxy run will returns the endpoints that the proxy is listening on
            // var endPoints = fProxy.Run();
            //var fs = FluxzySetting.CreateDefault();
            //var externalProxy = new Proxy(fs);
            //var externalProxy = new ExternalProxy
            //{
            //    HostName = hostName,
            //    Port = proxy.Port,
            //    UserName = proxy.UserName,
            //    Password = proxy.Password
            //};
            //var p = new WebProxy(hostName, proxy.Port)
            //{
            //     Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
            //};

            var proxyServer = new DynamicProxyServer(null);
            //proxyServer.Start();
            return proxyServer;
        }
    }
}
