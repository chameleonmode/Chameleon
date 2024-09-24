using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
    public interface IApplicationUser : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        bool IsAuthenticated { get; }
        string Email { get; }
        bool IsAssistant { get; }
        bool TookGuidedTour { get; }
        bool HasPemission(string permissionName);
    }
}
