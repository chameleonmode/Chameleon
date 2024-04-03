using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Services;

public interface IClipboardService : ISingletonDependency
{
    static readonly IClipboardService Instance;
    void SetOwner(object owner);
    Task SetTextAsync(string text);
}
