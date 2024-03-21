namespace Chameleon.Interfaces;

public interface ISubPageViewModel : IInnerUserControl
{
    Task InitAsync();
    Task InitAsync(object? param = null);
}
