using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.YouTube
{
    public interface IGoogleAuthenticationService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        Task<IOAuthToken> HandleRedirect(IUserProfile userProfile, string[] scopes);
    }
}
