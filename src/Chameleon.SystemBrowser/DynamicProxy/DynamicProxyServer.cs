using Fluxzy;
using Fluxzy.Certificates;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Chameleon.SystemBrowser.Proxy
{
    public class DynamicProxyServer
    {
        private readonly WebProxy _externalProxy;
        public DynamicProxyServer(WebProxy externalProxy)
        {
            _externalProxy = externalProxy;
        }

        private Fluxzy.Proxy _proxyServer;
        //private ExplicitProxyEndPoint _explicitProxyEndPoint;
        public int Port => 0;// _explicitProxyEndPoint?.Port ?? 0;
        public string Host => "127.0.0.1";
        public string Server => $"{Host}:{Port}";

        public async void Start()
        {
            Stop();

            await using var fProxy = new Fluxzy.Proxy(FluxzySetting.CreateDefault());
            //_proxyServer = new ProxyServer();
            //_proxyServer.ConnectTimeOutSeconds = 180;
            //_proxyServer.ConnectionTimeOutSeconds = 180;
            //_proxyServer.ConnectTimeOutSeconds = 180;
            //
            //SubscribeToEvents();
            //
            //_explicitProxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 0, false);
            //_proxyServer.AddEndPoint(_explicitProxyEndPoint);
            //
            //_proxyServer.UpStreamHttpProxy = _externalProxy;
            //_proxyServer.UpStreamHttpsProxy = _externalProxy;
            //
            //EnsureRootCertificate();
            //
            //_proxyServer.Start();
        }

        private void EnsureRootCertificate()
        {
           // var сertificateManager = CertificateManager;
           // сertificateManager.RootCertificateIssuerName = "Chameleon";
           // сertificateManager.RootCertificateName = "Chameleon Root Certificate Authority";
           //
           // if(!IsCertificateTrusted())
           // {
           //     сertificateManager.RemoveTrustedRootCertificate(false);
           // }
           //
           // сertificateManager.EnsureRootCertificate(true, false, false);
        }

        public bool IsCertificateTrusted()
        {
            // try
            // {
            //     return CertificateManager.IsRootCertificateUserTrusted();
            // }
            // catch 
            // {
            //     return false;
            // }

            return true;
        }

       //private Certificate CertificateManager 
       //    => _proxyServer.CertificateManager;

      //private void OnClientConnectionCountChanged(object sender, EventArgs e)
      //{
      //    if (_proxyServer.ClientConnectionCount == 0)
      //    {
      //        //ScheduleStop();
      //    }
      //}

        private void ScheduleStop()
        {
            UnsubscribeFromEvents();
            Task.Delay(3000)
                .ContinueWith(_ => Stop());
        }

        public void Stop()
        {
           // if (_proxyServer == null)
           // {
           //     return;
           // }
           //
           // UnsubscribeFromEvents();
           // Stop(_proxyServer);
           // _proxyServer = null;
        }

        private void SubscribeToEvents()
        {
          //  _proxyServer.ClientConnectionCountChanged += OnClientConnectionCountChanged;
        }

        private void UnsubscribeFromEvents()
        {
           // _proxyServer.ClientConnectionCountChanged -= OnClientConnectionCountChanged;
        }

      //private void Stop(ProxyServer proxyServer)
      //{
      //    ExecuteAndIgnoreExceptions(proxyServer.Stop);
      //    ExecuteAndIgnoreExceptions(proxyServer.Dispose);
      //}
      
        private void ExecuteAndIgnoreExceptions(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // ignore
            }
        }
    }
}
