namespace Chameleon.Interfaces;

public interface IInnerUserControl
{
    Task InvokeAsyncRelayCommand(object param = null);
}
