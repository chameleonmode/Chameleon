using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UpgradePlan
{
    public interface IUpgradePlanViewModel 
        : ITransientDependency
    {
        string LimitExceededText { get; set; }
    }
}
