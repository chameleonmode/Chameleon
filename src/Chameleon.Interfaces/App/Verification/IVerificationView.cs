using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Verification
{
    public interface IVerificationView
        : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
