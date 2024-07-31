using Chameleon.Interfaces.Auth;

namespace Chameleon.Auth.Core
{
    public class AuthSession : IAuthSession
    {
        public long UserId { get; set; }
        public long? CreatorUserId { get; set; }
        public string UserName { get; set; }
        public string AuthToken { get; set; }
        public bool HasAuthToken => !string.IsNullOrEmpty(AuthToken);
        public long ExpireInSeconds { get; set; }
        public string EncryptedAccessToken { get; set; }
        public string AuthRefreshToken { get; set; }
        public string[] Permissions { get; set; }
        public ILimits Limits { get; set; }
        public bool TookGuidedTour { get; set; }
        public bool CanCreateProfiles { get; set; }
    }
}
