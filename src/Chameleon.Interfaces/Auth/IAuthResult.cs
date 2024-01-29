namespace Chameleon.Interfaces.Auth
{
    public interface IAuthResult
        : IAuthUser
        , IAuthUserToken
        , IAuthPermissions
        , IAuthLimits
    {
    }
}
