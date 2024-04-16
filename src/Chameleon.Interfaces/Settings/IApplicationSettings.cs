using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IApplicationSettings
    {
        ILoginSettings Login { get; }
        ISettingsSettings Settings { get; }
    }
}
