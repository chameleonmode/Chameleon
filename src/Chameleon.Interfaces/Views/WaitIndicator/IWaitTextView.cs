using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Views.WaitIndicator
{
    public interface IWaitTextView
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        bool IsIndicatorVisible { get; set; }
        string Text { get; set; }
    }
}
