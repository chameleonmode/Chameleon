using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachTemplateView
        : ISingletonDependency
    {
        void SetOutReachTemplate(IOutReachTemplate template, IUserProfile userProfile);
    }
}
