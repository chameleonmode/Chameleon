using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
	[Obsolete("Added for compatibility with corrent infrastructure project until _authSession refactoed out only")]
	public interface IAuthSession 
        : IAuthUser
        , IAuthUserToken
        , IAuthLimits
        , IAuthPermissions
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
