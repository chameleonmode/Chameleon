using Chameleon.Interfaces.Ioc;
using System.Windows.Controls;

namespace Chameleon.Interfaces.Views.WaitIndicator
{
    public interface IWaitIndicatorView
        : ITransientDependency
    {
        bool IsIndicatorVisible { get; set; }
        void AttachTo(ContentControl element);
        void Detach();
    }
}
