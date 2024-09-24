using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Services;

public interface IClipboardService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    static readonly IClipboardService Instance;
    void SetOwner(object owner);
    Task SetTextAsync(string text);
}
