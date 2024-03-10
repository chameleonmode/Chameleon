using Chameleon.Interfaces.UserProfiles;
using Prism.Mvvm;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.ProxySettings;

public class ProxySettingsBindable
       : BindableBase
       , IProxySettings
       , IDataErrorInfo
{
    //private readonly TcpIpPortRule _tcpIpPortRule;
    public ProxySettingsBindable()
    {
        //TODO DI usage need changing mapping rule
        //_tcpIpPortRule = new TcpIpPortRule();
    }

    private string _host;
    public string Host
    {
        get => _host;
        set => SetProperty(ref _host, value);
    }

    public string HostForRequest { get => HostConverter.GetHostForRequest(Host); }

    private int _port;
    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    private string _userName;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    private string _password;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool CanUse => !string.IsNullOrWhiteSpace(Host);
    public bool HasUserName => !string.IsNullOrWhiteSpace(UserName);
    public string Server { get; }

    public string Error { get; private set; }
    public string this[string columnName]
    {
        get
        {
            string error = null;
            // TODO: add validatiom - uncoment below
            //switch (columnName)
            //{
            //    case nameof(Port):
            //        var result = _tcpIpPortRule.Validate(Port, CultureInfo.InvariantCulture);
            //        if(result!= null && result.ErrorContent != null)
            //        {
            //            error = result.ErrorContent.ToString();
            //        }
            //        break;
            //    default:
            //        break;
            //}
            Error = error;
            return error;
        }
    }
}
