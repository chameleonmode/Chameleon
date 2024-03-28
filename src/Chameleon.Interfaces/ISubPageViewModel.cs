namespace Chameleon.Interfaces;

public interface ISubPageViewModel : IPageViewModel
{
    Task InitAsync(object? param);
    Task OnNavigatedToAsync(object? param);
}
