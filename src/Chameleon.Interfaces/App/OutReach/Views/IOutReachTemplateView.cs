using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachTemplateView
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void SetOutReachTemplate(IOutReachTemplate template, IUserProfile userProfile);
    }
}
