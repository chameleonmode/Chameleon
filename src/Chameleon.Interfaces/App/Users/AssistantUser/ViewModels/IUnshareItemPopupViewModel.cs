using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Assistants
{
    public interface IUnshareItemPopupViewModel 
        : ITransientDependency
    {
        string Text { get; set; }
    }
}
