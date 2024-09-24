using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.SidePanel
{
    public interface ISidePanelView : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        bool IsOpen { get; set; }
        string MainHeader { get; set; }
        string SecondaryHeader { get; set; }
        IViewControl ViewControl { set; }
    }
}
