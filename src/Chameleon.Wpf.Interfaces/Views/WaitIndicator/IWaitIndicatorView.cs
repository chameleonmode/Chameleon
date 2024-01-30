using Chameleon.Interfaces.Ioc;
using Microsoft.Maui.Controls;

namespace Chameleon.Interfaces.Views.WaitIndicator
{
    public interface IWaitIndicatorView
        : ITransientDependency
    {
        bool IsIndicatorVisible { get; set; }
        void AttachTo(ContentView element);
        void Detach();
    }
}
