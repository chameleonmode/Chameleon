using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UpgradePlan
{
    public interface IUpgradePlanView : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Title { get; set; }
    }
}
