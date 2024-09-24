using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Dashboard
{
    public interface IDashboardViewModel : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserProfile SelectedProfile { get; set; }
        void BuildSearchTerms();
    }
}
