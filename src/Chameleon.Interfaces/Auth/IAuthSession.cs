using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthSession 
        : IAuthUser
        , IAuthUserToken
        , IAuthLimits
        , IAuthPermissions
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
