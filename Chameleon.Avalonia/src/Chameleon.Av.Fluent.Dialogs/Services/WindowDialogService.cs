
namespace Chameleon.Av.Fluent.Dialogs.Services;

public class WindowDialogService : IWindowDialogService
{
    public Task ShowDialogAsync(Action<object, EventArgs>[] events)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
        
    }
}
