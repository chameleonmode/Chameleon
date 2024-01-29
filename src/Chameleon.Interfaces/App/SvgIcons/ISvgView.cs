using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.SvgIcons
{
    public interface ISvgView
        : ITransientDependency
    {
        string IconName { get; set; }
    }
}
