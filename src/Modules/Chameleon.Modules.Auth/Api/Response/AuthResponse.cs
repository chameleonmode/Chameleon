namespace Chameleon.Auth.Api
{
    public class AuthResponse : IAuthResponse
    {
        public string AccessToken { get; set; }
        public string EncryptedAccessToken { get; set; }
        public long ExpireInSeconds { get; set; }
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public long? CreatorUserId { get; set; }
        public string[] Permissions { get; set; }
        public Limits LicenseLimits { get; set; }
        public bool IsValid => !string.IsNullOrWhiteSpace(AccessToken);
        public bool TookGuidedTour { get; set; }
        public bool CanCreateProfiles { get; set; }
    }
}
