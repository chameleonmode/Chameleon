
using Avalonia.Controls;
using Chameleon.Interfaces.Views;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class WindowDialogService : IWindowDialogService
{
    public Task ShowDialogAsync(Action<object, EventArgs>[] events)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
        
    }

    public void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize) where TViewModel : class
    {
        if (ContainerServiceHelper.Resolve<TView>() is Control view)
        {
            var viewModel = view.DataContext as TViewModel;

            initialize?.Invoke(viewModel);

            Window window = new Window() { Topmost = true, Width=480, Height=560 };
            window.Content = view;
            window.Show();
        }
    }
}
