
using Chameleon.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.Common.CommunityIOC;

public enum ConfigureType
{
    Single,
    Transient,
    Scoped
}
/// <summary>
/// community toolkit implementation 
/// </summary>
public class CTServiceProvider
{
    public static CTServiceProvider Instance { get; } = new();
    private CTServiceProvider()
    {
    }

    public ServiceCollection ServiceCollect { get; } = new ServiceCollection();

    /// <summary>
    /// Configures the singles for the application.
    /// </summary>
    public void ConfigureTyps(Dictionary<Type,Type> singles, ConfigureType ct)
    {
        foreach (var type in singles)
            switch (ct)
            {
                case ConfigureType.Single:
                    ServiceCollect.AddSingleton(type.Key, type.Value);
                    break;
                case ConfigureType.Transient:
                    ServiceCollect.AddTransient(type.Key, type.Value);
                    break;
                case ConfigureType.Scoped:
                    ServiceCollect.AddScoped(type.Key, type.Value);
                    break;
                default:
                    break;
            }

        Services = ServiceCollect.BuildServiceProvider();
    }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }
}
