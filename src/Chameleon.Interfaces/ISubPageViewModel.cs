namespace Chameleon.Interfaces;

public interface ISubPageViewModel : IHaveInitialize
{
    string Title { get; set; }
    Task InitAsync(object? param);
    Task OnNavigatedToAsync(object? param);
}
