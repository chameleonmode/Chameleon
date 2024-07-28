using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Settings;

public interface IPhoneVerificationViewModel
    : ITransientDependency
{
    bool IsCodesverifyVisible { get; set; }
    bool IsSMSPVAVisible { get; set; }
}