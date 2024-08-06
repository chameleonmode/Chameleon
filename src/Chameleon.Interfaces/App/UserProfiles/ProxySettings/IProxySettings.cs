namespace Chameleon.Interfaces.UserProfiles
{
    public interface IProxySettings
    {
        string Host { get; set; }
        string HostForRequest { get; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        
        bool CanUse { get; }
        bool HasUserName { get; }
        string Server { get; }
        string ServerForRequest { get; }
    }
}
