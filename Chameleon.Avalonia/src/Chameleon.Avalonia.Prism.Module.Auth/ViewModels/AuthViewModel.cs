using Avalonia;
using Chameleon.Auth.Services;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Auth.Events;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime;
using System.Security.Authentication;

namespace Chameleon.Avalonia.Prism.Module.Auth.ViewModels;

public class AuthViewModel : DialogViewModelBase, IAuthViewModel
{
    private readonly IAuthService _authService;
    private readonly IApplicationSettings _settings;
    private readonly IApplicationSettingsService _settingsService;
    private readonly IEventAggregator _eventAggregator;
   // private readonly IMessageBoxService _messageBoxService;
    public AuthViewModel(
    IAuthService authService,
        IApplicationSettingsService settingsService,
        IEventAggregator eventAggregator
        )
    {
        _authService = authService;
        _settingsService = settingsService;

        _settings = _settingsService.Get();
        _userName = _settings.Login.LoginName;
        _licenceKey = _settings.Login.LicenseKey;

        AuthCommand = new DelegateCommand(SubmitAsync, CanSubmit);
        CancelCommand = new DelegateCommand(CloseDialog);

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<SubmitAsyncEvent>()
            .Subscribe(SubmitAsync);

        //_messageBoxService = messageBoxService;
    }

    private string _title = string.Empty;
    public override string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _licenceKey;
    public string LicenceKey
    {
        get => _licenceKey;
        set
        {
            ErrorMessage = string.Empty;
            SetProperty(ref _licenceKey, value.Trim(), RaiseCanExecuteChanged);
        }
    }

    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            ErrorMessage = string.Empty;
            SetProperty(ref _userName, value.Trim(), RaiseCanExecuteChanged);
        }
    }

    private bool _isSubmiting;
    public bool IsSubmiting
    {
        get => _isSubmiting;
        set
        {
            ErrorMessage = string.Empty;
            if (SetProperty(ref _isSubmiting, value, RaiseCanExecuteChanged))
            {
                RaisePropertyChanged(nameof(IsInputEnabled));
            }
        }
    }

    public bool IsInputEnabled => !IsSubmiting;

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AuthCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public IAuthResult AuthResult { get; private set; }

    private void RaiseCanExecuteChanged()
    {
        AuthCommand.RaiseCanExecuteChanged();
    }

    private async void SubmitAsync()
    {
        await Dosubmit();
        //await DoRequest();
        //Task.Run(Submit);
    }
    async Task Dosubmit()
    {
        string errorMessage = string.Empty;

        try
        {
            IsSubmiting = true;
            // store auth info to reuse next startup
            _settings.Login.Set(_userName, _licenceKey);
            await _settingsService.Save();

            if (!_licenceKey.StartsWith("KEY") &&
                !_authService.IsLicenseActive(_licenceKey))
            {
                //var modalResult = this.InvokeOnUiThread(() =>
                //{
                //    return _messageBoxService.ShowDialog(
                //        new MessageBox.Services.MessageBoxOptions
                //        {
                //            Title = "Warning",
                //            Text = "Do you want to activate another license? Current one will not be active anymore.",
                //            Icon = SystemIcons.Warning,
                //            Buttons = MessageBoxButton.OKCancel,
                //            ContentButtons = new MessageBoxContentButtonsViewModel
                //            {
                //                ContentOkButton = "Activate"
                //            }
                //        });
                //});
                //if (modalResult != ButtonResult.OK)
                //    return;
            }

            // try login with entered user name and password
            AuthResult = _authService.Login(_userName, _licenceKey);

          
            //await _authService.LoginAsync();

            // close dialog
            CloseDialog(ButtonResult.OK);

            // hiding spinner in case of successfull reconnection
            //if (_mainWindow != null)
            //    Application.Current.Dispatcher.Invoke(() => _mainWindow.HideWaitIndicator());

            return;
        }
        catch (AuthenticationException ex)
        {
            errorMessage = $"Login failed: Invalid email or licence key";
        }
        catch (WebException ex)
        {
            //ExceptionHandler.ShowException(ex);
            errorMessage = $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            errorMessage = "Error with login";
        }
        finally
        {
            IsSubmiting = false;
        }

        ErrorMessage = errorMessage;
    }
 async Task DoRequest()
{
    var url = "https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U";
    //var url = "https://github.com/explore";
    using HttpClient client = new HttpClient();
    var res = await client.GetAsync(url);
    res.EnsureSuccessStatusCode();
    var sr = await res.Content.ReadAsStringAsync();
}

    private async void Submit()
    {
        string errorMessage = string.Empty;

        try
        {
            IsSubmiting = true;
            // store auth info to reuse next startup
            _settings.Login.Set(_userName, _licenceKey);
            await _settingsService.Save();

AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

string tfsDefaultCollection = "https://api.chameleonmode.com";

string testUrl = $"{tfsDefaultCollection}/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U";

var myCache = new CredentialCache
{
    {
        new Uri(testUrl), "NTLM",
        CredentialCache.DefaultNetworkCredentials
    }
};

var httpClientHandler = new HttpClientHandler
{
    Credentials = myCache,
    AllowAutoRedirect= true,
};

var client = new HttpClient(httpClientHandler)
{
    BaseAddress = new Uri(tfsDefaultCollection)
};
httpClientHandler.PreAuthenticate = true;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
var test = client.GetAsync(testUrl).Result;

AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

            var uri = new Uri("https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U");
            var credentialsCache = new CredentialCache
                {
                    {
                        uri, "NTLM",
                        new NetworkCredential("domain\name", "pwd")
                    }
                };
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("user","pass") };          
               // handler.ClientCertificateOptions = ClientCertificateOption.Manual;
     // handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
   // handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

    // I also tried to add another certificates that was provided to https access 
    // by administrators of the site, but it still doesn't work.
    //handler.ClientCertificates.Add(new X509Certificate2(@"C:\certificates\cert.crt"));
    //handler.ClientCertificates.Add(new X509Certificate2(@"C:\certificates\cert_ca.crt"));

    var httpClient = new HttpClient(handler);
    
                    httpClient.DefaultRequestHeaders.ConnectionClose = false;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.GetAsync("https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U");
        // ^ HttpRequestException: An error occurred while sending the request.
    
            


            ServicePointManager.FindServicePoint(uri).ConnectionLeaseTimeout = 120 * 1000;  // Close connection after two minutes
    
            
System.Net.ServicePointManager.SecurityProtocol |=
    SecurityProtocolType.Tls12 | 
    SecurityProtocolType.Tls11 | 
    SecurityProtocolType.Tls; // comparable to modern browsers
    using (var handler1 = new HttpClientHandler())
{
    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
      handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

    // I also tried to add another certificates that was provided to https access 
    // by administrators of the site, but it still doesn't work.
    //handler.ClientCertificates.Add(new X509Certificate2(@"C:\certificates\cert.crt"));
    //handler.ClientCertificates.Add(new X509Certificate2(@"C:\certificates\cert_ca.crt"));

    using (var clientoo = new HttpClient(handler))
    {
        var responseo = await client.GetAsync("https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U");
        // ^ HttpRequestException: An error occurred while sending the request.
    }
}
            using (var handlero = new HttpClientHandler())
{
    handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
//ServicePointManager.DefaultConnectionLimit = int.MaxValue;
  //string url = "https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U";
  HttpClient clientooo = new HttpClient(handler);

HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U");

request.Headers.Add("accept", "text/plain");
request.Headers.Add("Authorization", "null");
//request.Headers.Add("X-XSRF-TOKEN", "CfDJ8L5_bCB2f3FMulmol3sV3xssEKmARyxOmz65a0Fqeb78cOsWCzy7GgLzMfKEJVBgZIeeQlXEY_ighgyuzcSjL3J7iKrYi25rvOTaUceJGfCTF03SiacCsc5YofNtdKqDuqBuczqvuTxsUg8h7xJQMdA");

HttpResponseMessage responseoo = await client.SendAsync(request).ConfigureAwait(false);
response.EnsureSuccessStatusCode();
string responseBody = await response.Content.ReadAsStringAsync();
}
//request.ProtocolVersion = HttpVersion.Version10;
   // var client = new HttpClient(new HttpClientHandler()
  //      {
  //          UseDefaultCredentials = true,
  //          PreAuthenticate = true,
  //          Credentials = CredentialCache.DefaultCredentials,
  //       
  //      })
  //  {
  //      Timeout = TimeSpan.FromSeconds(20),
  //      
  //  };

   // client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
    //client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en;q=0.5");
    //client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:123.0) Gecko/20100101 Firefox/123.0");
    //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
    //client.DefaultRequestHeaders.Add("Host", "api.chameleonmode.com");
    //client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
    //client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
    //client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
   // client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
   // client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");


   //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
 //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
 //client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
 //client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
 //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
//client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");

 //   using (var message = new HttpRequestMessage(HttpMethod.Get, url))
 //   {
//
//        using (var httpResponse = Task.Run(() => client.SendAsync(message)).Result)
//        {
 ///           Console.WriteLine("{0}: {1}", httpResponse.StatusCode, httpResponse.ReasonPhrase);
//    }



   //          var c = new HttpClient();

  //          var r = await c.GetAsync("http://api.chameleonmode.com/api/TokenAuth/IsLicenseActive?key=HHTQ-QJYS-ZMWX-CO5U");
  //          var res = await r.Content.ReadAsStringAsync();
            if (await Task.Run(()=>{return 
            !_licenceKey.StartsWith("KEY") &&
                !_authService.IsLicenseActive(_licenceKey);}))
            {
                //var modalResult = this.InvokeOnUiThread(() =>
                //{
                //    return _messageBoxService.ShowDialog(
                //        new MessageBox.Services.MessageBoxOptions
                //        {
                //            Title = "Warning",
                //            Text = "Do you want to activate another license? Current one will not be active anymore.",
                //            Icon = SystemIcons.Warning,
                //            Buttons = MessageBoxButton.OKCancel,
                //            ContentButtons = new MessageBoxContentButtonsViewModel
                //            {
                //                ContentOkButton = "Activate"
                //            }
                //        });
                //});
                //if (modalResult != ButtonResult.OK)
                //    return;
            }

            // try login with entered user name and password
            AuthResult = _authService.Login(_userName, _licenceKey);

          
            //await _authService.LoginAsync();

            // close dialog
            CloseDialog(ButtonResult.OK);

            // hiding spinner in case of successfull reconnection
            //if (_mainWindow != null)
            //    Application.Current.Dispatcher.Invoke(() => _mainWindow.HideWaitIndicator());

            return;
        }
        catch (AuthenticationException ex)
        {
            errorMessage = $"Login failed: Invalid email or licence key";
        }
        catch (WebException ex)
        {
            //ExceptionHandler.ShowException(ex);
            errorMessage = $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            errorMessage = "Error with login";
        }
        finally
        {
            IsSubmiting = false;
        }

        ErrorMessage = errorMessage;
    }

    private bool CanSubmit()
    {
        return !string.IsNullOrEmpty(_licenceKey)
            && !string.IsNullOrEmpty(_userName)
            && !_isSubmiting;
    }
}
