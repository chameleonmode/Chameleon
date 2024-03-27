using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Dashboard
{
    public interface IDashboardViewModel : ISingletonDependency
    {
        IUserProfile SelectedProfile { get; set; }
    }
}
