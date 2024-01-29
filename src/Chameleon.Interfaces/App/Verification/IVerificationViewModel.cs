using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Verification
{
    public interface IVerificationViewModel
        : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
