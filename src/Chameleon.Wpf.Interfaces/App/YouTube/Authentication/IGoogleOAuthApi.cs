using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IGoogleOAuthApi
    {
        IOAuthToken ExchangeCodeForAccessToken();

        string AuthorizationRequestUri { get; }
        string State { get; }
        string RedirectUri { get; }
        string CodeVerifier { get; }
        string[] Scopes { get; }
        string Code { get;  set; }
        IUserProfile UserProfile { get; }
    }
}
