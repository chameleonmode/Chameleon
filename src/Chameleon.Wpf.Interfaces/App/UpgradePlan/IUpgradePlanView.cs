using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UpgradePlan
{
    public interface IUpgradePlanView : ITransientDependency
    {
        string Title { get; set; }
    }
}
