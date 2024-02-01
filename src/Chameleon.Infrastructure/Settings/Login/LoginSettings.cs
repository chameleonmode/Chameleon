using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.Settings
{
    public class LoginSettings : ILoginSettings
    {
        public string LoginName { get; set; } = string.Empty;
        public string LicenseKey { get; set; } = string.Empty;

        public void Set(string loginName, string licenseKey)
        {
            LoginName = loginName ?? string.Empty;
            LicenseKey = licenseKey ?? string.Empty;
        }
    }
}
