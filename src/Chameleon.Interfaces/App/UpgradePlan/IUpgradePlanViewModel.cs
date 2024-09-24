using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UpgradePlan
{
    public interface IUpgradePlanViewModel 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string LimitExceededText { get; set; }
    }
}
