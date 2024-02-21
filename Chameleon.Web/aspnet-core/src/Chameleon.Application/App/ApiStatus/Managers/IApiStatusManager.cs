using Abp.Dependency;

namespace Chameleon.App
{
    public interface IApiStatusManager
        : ISingletonDependency
    {
        bool LoginIsFailed { get; set; }
        bool IsOld { get; }
    }
}
