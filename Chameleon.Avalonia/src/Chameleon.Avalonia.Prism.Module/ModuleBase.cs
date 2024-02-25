using Prism.Ioc;
using Prism.Modularity;

namespace Chameleon.Avalonia.Prism.Module;

public abstract class ModuleBase : IModule
{
    public virtual void OnInitialized(IContainerProvider containerProvider)
    {
        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
    }

    public virtual void RegisterTypes(IContainerRegistry containerRegistry)
    {

    }
}
