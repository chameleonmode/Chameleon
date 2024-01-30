namespace Chameleon.Interfaces.Auth
{
    public interface IAuthUserToken
    {
        string AuthToken { get; set; }
        bool HasAuthToken { get; }
        long ExpireInSeconds { get; set; }
        string EncryptedAccessToken { get; set; }
        string AuthRefreshToken { get; set; }
    }
}
