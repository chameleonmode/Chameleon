using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.SvgIcons
{
    public interface ISvgIconView
        : ITransientDependency
    {
        string IconName { get; set; }
    }
}
