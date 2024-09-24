using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Assistants
{
    public interface IUnshareItemPopupViewModel 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Text { get; set; }
    }
}
