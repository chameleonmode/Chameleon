namespace Chameleon.Interfaces.Auth
{
    public interface IAuthRefreshTokenResponse
    {
        string NewAccessToken { get; }
        string NewRefreshToken { get; }
        long ExpireInSeconds { get; }
    }
}
