using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public LoginSettings Login { get; set; } = new LoginSettings();
        public SettingsSettings Settings { get; set; } = new SettingsSettings();
        ILoginSettings IApplicationSettings.Login => Login;

        ISettingsSettings IApplicationSettings.Settings => Settings;
    }
}
