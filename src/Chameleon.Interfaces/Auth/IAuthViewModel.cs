using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthViewModel 
        : ISingletonDependency
    {
        IAuthResult AuthResult { get; }
    }
}
