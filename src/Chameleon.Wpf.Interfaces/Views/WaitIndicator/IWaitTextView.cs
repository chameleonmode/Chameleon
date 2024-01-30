using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Views.WaitIndicator
{
    public interface IWaitTextView
        : ITransientDependency
    {
        bool IsIndicatorVisible { get; set; }
        string Text { get; set; }
    }
}
