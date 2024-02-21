using Chameleon.App.Services.License_Key.Dto;

namespace Chameleon.Models.TokenAuth
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }
        public string EncryptedAccessToken { get; set; }
        public long ExpireInSeconds { get; set; }
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public long? CreatorUserId { get; set; }
        public string[] Permissions { get; set; }
        public LicenseLimits LicenseLimits { get; set; }
        public bool TookGuidedTour { get; set; }
        public bool CanCreateProfiles { get; set; }
    }
}
