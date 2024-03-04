using Chameleon.Interfaces.Auth;

namespace Chameleon.Common.Helpers;

public class CurrentApplicationUser
{
    public static CurrentApplicationUser Current { get; } = new CurrentApplicationUser();

    private IApplicationUser? _applicationUser;

    public void SetCurrentUser(IApplicationUser user)
    {
        _applicationUser = user;
    }

    public IApplicationUser? GetCurrentUser()
    {
        return _applicationUser;
    }
}