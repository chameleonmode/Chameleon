using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public LoginSettings Login { get; set; } = new LoginSettings();
        ILoginSettings IApplicationSettings.Login => Login;
    }
}
