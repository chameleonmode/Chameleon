using Chameleon.Interfaces.Auth;

namespace Chameleon.Auth.Api.Response
{
    public class AuthRefreshTokenResponse : IAuthRefreshTokenResponse
    {
        public string NewAccessToken { get; set; }
        public string NewRefreshToken { get; set;  }
        public long ExpireInSeconds { get; set;  }
    }
}
