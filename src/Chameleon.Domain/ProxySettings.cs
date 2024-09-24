using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Domain.Entities;
public class ProxySettings : IProxySettings
{
    private string _host = string.Empty;
    public string Host
    {
        get => _host;
        set => _host = value?.Trim() ?? string.Empty;
    }

    public string HostForRequest { get => HostConverter.GetHostForRequest(Host); }

    public const int DefaultPort = 80;
    private int _port = DefaultPort;
    public int Port
    {
        get => _port;
        set
        {
            if (value < 0 || value >= 65535)
            {
                value = 0;
               // throw new ArgumentOutOfRangeException();
            }
            _port = value;
        }
    }

    private string _userName = string.Empty;
    public string UserName
    {
        get => _userName;
        set => _userName = value?.Trim() ?? string.Empty; 
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => _password = value?.Trim() ?? string.Empty;
    }

    public bool CanUse => Host.Length > 0;
    public bool HasUserName => UserName.Length > 0;
    public string Server => CanUse ? $"{HostForRequest}:{Port}" : string.Empty;

    public string ServerForRequest => CanUse ? $"http://{Server}" : string.Empty;
}
