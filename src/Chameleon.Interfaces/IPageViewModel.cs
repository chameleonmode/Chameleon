namespace Chameleon.Interfaces;

public interface IPageViewModel : IHaveInitialize
{
    string Title { get; set; }
    Task OnNavigatedToAsync(object? param);
}
