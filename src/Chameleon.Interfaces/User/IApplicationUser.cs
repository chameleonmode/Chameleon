using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
    public interface IApplicationUser : ISingletonDependency
    {
        bool IsAuthenticated { get; }
        string Email { get; }
        bool IsAssistant { get; }
        bool TookGuidedTour { get; }
        bool HasPemission(string permissionName);
    }
}
