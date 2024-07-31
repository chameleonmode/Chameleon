namespace Chameleon.Auth.Api
{
    public interface IAuthResponse
    {
        string AccessToken { get; }
        string EncryptedAccessToken { get; }
        long ExpireInSeconds { get; }
        string RefreshToken { get; }
        long UserId { get; }
        long? CreatorUserId { get; }
        string[] Permissions { get; }
        Limits LicenseLimits { get; }
        bool TookGuidedTour { get; }
        bool CanCreateProfiles { get; set; }
    }
}
