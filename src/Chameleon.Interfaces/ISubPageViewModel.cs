namespace Chameleon.Interfaces;

public interface ISubPageViewModel : IHaveInitialize
{
    Task InitAsync(object? param);
    Task OnNavigatedToAsync(object? param);
}
