using Chameleon.App.Services.License_Key.Dto;

namespace Chameleon.Models.TokenAuth
{
    public class LicenseAuthenticateResultModel
    {
        public string AccessToken { get; set; }
        public long ExpireInSeconds { get; set; }
        public long UserId { get; set; }
        public LicenseLimits LicenseLimits { get; set; }
        public string RefreshToken { get; set; }
    }
}
