namespace Chameleon.Interfaces;

public interface IPageViewModel
{
    Task InvokeAsyncRelayCommand(object param = null);
}
