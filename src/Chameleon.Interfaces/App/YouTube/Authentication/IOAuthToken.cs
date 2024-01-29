using System;

namespace Chameleon.Interfaces.YouTube
{
    public interface IOAuthToken
    {
        string AccessToken { get; set; }
        string TokenType { get; set; }
        int ExpiresIn { get; set; }
        string RefreshToken { get; set; }
        string[] Scopes { get; set; }
        DateTime ExpirationDate { get; set; }
    }
}
